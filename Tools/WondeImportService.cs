using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using System.IO;
using System.Net.Http;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;

namespace WondeImportTool.Tools
{
    public class WondeImportService
    {
        private readonly string _apiKey;
        private readonly string _connectionString;
        private readonly string _logsDirectory;

        public WondeImportService(string apiKey, string connectionString, string? logsDirectory = null)
        {
            _apiKey = apiKey ?? throw new ArgumentNullException(nameof(apiKey));
            _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
            _logsDirectory = string.IsNullOrWhiteSpace(logsDirectory)
                ? Path.Combine(AppContext.BaseDirectory, "logs")
                : logsDirectory;
        }

        private async Task LogImportEventAsync(
            string? schoolId,
            string? requestId,
            string? tableName,
            string? instance,
            string? url,
            DateTime? startTime,
            DateTime? endTime,
            int? rowsProcessed,
            CancellationToken cancellationToken = default)
        {
            try
            {
                await using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync(cancellationToken).ConfigureAwait(false);

                await using var command = new SqlCommand(
                    @"INSERT INTO [dbo].[IMPORT_LOG]
                    ([school_id], [request_id], [table_name], [instance], [url], [start_time], [end_time], [rows_processed])
                    VALUES
                    (@school_id, @request_id, @table_name, @instance, @url, @start_time, @end_time, @rows_processed);",
                    connection);

                command.Parameters.Add(new SqlParameter("@school_id", SqlDbType.VarChar, 50) { Value = string.IsNullOrWhiteSpace(schoolId) ? "SYSTEM" : schoolId });
                command.Parameters.Add(new SqlParameter("@request_id", SqlDbType.VarChar, 50) { Value = (object?)requestId ?? DBNull.Value });
                command.Parameters.Add(new SqlParameter("@table_name", SqlDbType.VarChar, 50) { Value = (object?)tableName ?? DBNull.Value });
                command.Parameters.Add(new SqlParameter("@instance", SqlDbType.VarChar, 50) { Value = (object?)instance ?? DBNull.Value });
                command.Parameters.Add(new SqlParameter("@url", SqlDbType.NVarChar) { Value = (object?)url ?? DBNull.Value });
                command.Parameters.Add(new SqlParameter("@start_time", SqlDbType.DateTime) { Value = (object?)startTime ?? DBNull.Value });
                command.Parameters.Add(new SqlParameter("@end_time", SqlDbType.DateTime) { Value = (object?)endTime ?? DBNull.Value });
                command.Parameters.Add(new SqlParameter("@rows_processed", SqlDbType.Int) { Value = (object?)rowsProcessed ?? DBNull.Value });

                await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Failed to write IMPORT_LOG entry: {ex.Message}");
            }
        }

        private async Task<DateTime> ResolveUpdatedAfterDateAsync(
            SqlConnection connection,
            string schoolId,
            string tableName,
            DateTime? universalUpdatedAfterDate,
            DateTime fallbackStartDate,
            CancellationToken cancellationToken = default)
        {
            if (universalUpdatedAfterDate.HasValue)
            {
                return universalUpdatedAfterDate.Value.Date;
            }

            var latestSectionFinishEndTime = await GetLatestSectionFinishEndTimeAsync(
                connection,
                schoolId,
                tableName,
                cancellationToken).ConfigureAwait(false);

            if (latestSectionFinishEndTime.HasValue)
            {
                return latestSectionFinishEndTime.Value.Date.AddDays(-1);
            }

            return fallbackStartDate.Date;
        }

        private static async Task<DateTime?> GetLatestSectionFinishEndTimeAsync(
            SqlConnection connection,
            string schoolId,
            string tableName,
            CancellationToken cancellationToken = default)
        {
            await using var command = new SqlCommand(
                @"SELECT TOP (1) [end_time]
                  FROM [dbo].[IMPORT_LOG]
                  WHERE [school_id] = @school_id
                    AND [table_name] = @table_name
                    AND [instance] = 'section finish'
                    AND [end_time] IS NOT NULL
                  ORDER BY [end_time] DESC;",
                connection);

            command.Parameters.Add(new SqlParameter("@school_id", SqlDbType.VarChar, 50) { Value = schoolId });
            command.Parameters.Add(new SqlParameter("@table_name", SqlDbType.VarChar, 50) { Value = tableName });

            var result = await command.ExecuteScalarAsync(cancellationToken).ConfigureAwait(false);
            return result is null || result is DBNull ? null : (DateTime?)result;
        }

        public async Task LogTextAsync(string level, string message, CancellationToken cancellationToken = default)
        {
            try
            {
                Directory.CreateDirectory(_logsDirectory);

                var logFilePath = Path.Combine(_logsDirectory, $"{DateTime.UtcNow:yyyy-MM-dd}.txt");
                var line = $"{DateTime.UtcNow:O} [{level}] {message}{Environment.NewLine}";
                await File.AppendAllTextAsync(logFilePath, line, cancellationToken).ConfigureAwait(false);
            }
            catch
            {
                // Logging should not break imports.
                Console.WriteLine($"Failed to write log text: {message}");
            }
        }

        public async Task GetSchoolsAsync(string requestId, CancellationToken cancellationToken = default)
        {
            var sectionStartTime = DateTime.UtcNow;
            await LogImportEventAsync("SYSTEM", requestId, "schools", "section start", null, sectionStartTime, sectionStartTime, 0, cancellationToken).ConfigureAwait(false);

            using var client = new HttpClient();
            using var request = new HttpRequestMessage(HttpMethod.Get, "https://api.wonde.com/v1.0/schools");
            request.Headers.Add("Authorization", _apiKey);

            var requestStartTime = DateTime.UtcNow;
            await LogImportEventAsync("SYSTEM", requestId, "schools", "request", request.RequestUri?.ToString(), requestStartTime, requestStartTime, 0, cancellationToken).ConfigureAwait(false);

            using var response = await client.SendAsync(request, cancellationToken).ConfigureAwait(false);
            var responseEndTime = DateTime.UtcNow;
            await LogImportEventAsync("SYSTEM", requestId, "schools", "response", request.RequestUri?.ToString(), requestStartTime, responseEndTime, 0, cancellationToken).ConfigureAwait(false);
            if (!response.IsSuccessStatusCode)
            {
                await LogImportEventAsync("SYSTEM", requestId, "schools", "api failure", request.RequestUri?.ToString(), requestStartTime, responseEndTime, 0, cancellationToken).ConfigureAwait(false);
                await LogTextAsync("ERROR", $"Schools API call failed: {(int)response.StatusCode} {response.ReasonPhrase}", cancellationToken).ConfigureAwait(false);
                return;
            }

            var json = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
            var wondeSchool = WondeModels.WondeSchool.FromJson(json);

            foreach (var sch in wondeSchool.data)
            {
                cancellationToken.ThrowIfCancellationRequested();

                try
                {
                    await using var con = new SqlConnection(_connectionString);
                    await con.OpenAsync(cancellationToken).ConfigureAwait(false);

                    await using var cmd = new SqlCommand("school_details_Create", con)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    cmd.Parameters.Add(new SqlParameter("@id", SqlDbType.NVarChar, 50) { Value = (object)sch.id ?? DBNull.Value });
                    cmd.Parameters.Add(new SqlParameter("@name", SqlDbType.NVarChar, 50) { Value = (object)sch.name ?? "" });
                    cmd.Parameters.Add(new SqlParameter("@establishment_number", SqlDbType.Int) { Value = (object)sch.establishment_number ?? DBNull.Value });
                    cmd.Parameters.Add(new SqlParameter("@urn", SqlDbType.Int) { Value = (object)sch.urn ?? DBNull.Value });
                    cmd.Parameters.Add(new SqlParameter("@phase_of_education", SqlDbType.NVarChar, 50) { Value = (object)sch.phase_of_education ?? "" });
                    cmd.Parameters.Add(new SqlParameter("@la_code", SqlDbType.Int) { Value = (object)sch.la_code ?? DBNull.Value });
                    cmd.Parameters.Add(new SqlParameter("@timezone", SqlDbType.NVarChar, 50) { Value = (object)sch.timezone ?? "" });
                    cmd.Parameters.Add(new SqlParameter("@mis", SqlDbType.NVarChar, 50) { Value = (object)sch.mis ?? "" });

                    var addr = sch.address ?? new WondeModels.WondeSchool_Address();
                    cmd.Parameters.Add(new SqlParameter("@address_line_1", SqlDbType.NVarChar, 50) { Value = (object)addr.address_line_1 ?? "" });
                    cmd.Parameters.Add(new SqlParameter("@address_line_2", SqlDbType.NVarChar, 50) { Value = (object)addr.address_line_2 ?? "" });
                    cmd.Parameters.Add(new SqlParameter("@address_town", SqlDbType.NVarChar, 50) { Value = (object)addr.address_town ?? "" });
                    cmd.Parameters.Add(new SqlParameter("@address_postcode", SqlDbType.NVarChar, 50) { Value = (object)addr.address_postcode ?? "" });

                    var ext = sch.extended ?? new WondeModels.WondeSchool_Extended();
                    cmd.Parameters.Add(new SqlParameter("@allows_writeback", SqlDbType.Bit) { Value = (object)ext.allows_writeback ?? false });
                    cmd.Parameters.Add(new SqlParameter("@has_timetables", SqlDbType.Bit) { Value = (object)ext.has_timetables ?? false });
                    cmd.Parameters.Add(new SqlParameter("@has_lesson_attendance", SqlDbType.Bit) { Value = (object)ext.has_lesson_attendance ?? false });

                    await cmd.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine($"Error importing school {sch.id}: {ex.Message}");
                    // consider logging to file/telemetry and/or continuing
                }
            }

            var sectionEndTime = DateTime.UtcNow;
            await LogImportEventAsync("SYSTEM", requestId, "schools", "section finish", null, sectionStartTime, sectionEndTime, 0, cancellationToken).ConfigureAwait(false);
        }

        public async Task GetStudentsAsync(
            IReadOnlyCollection<string> schoolIds,
            string academicYear,
            int pageSize,
            int batchSize,
            DateTime? universalUpdatedAfterDate,
            DateTime fallbackStartDate,
            string requestId,
            CancellationToken cancellationToken = default)
        {
            ValidateStudentImportArguments(schoolIds, academicYear, pageSize, batchSize);

            var sectionStartTime = DateTime.UtcNow;
            await LogImportEventAsync("SYSTEM", requestId, "students", "section start", null, sectionStartTime, sectionStartTime, 0, cancellationToken).ConfigureAwait(false);

            using var client = new HttpClient();
            await using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync(cancellationToken).ConfigureAwait(false);
            await TruncateStudentStagingAsync(connection, cancellationToken).ConfigureAwait(false);

            var studentRows = CreateStudentImportTable();
            var groupRows = CreateGroupStudentTable();

            foreach (var schoolId in schoolIds)
            {
                try
                {
                    var updatedAfterDate = await ResolveUpdatedAfterDateAsync(
                        connection,
                        schoolId,
                        "students",
                        universalUpdatedAfterDate,
                        fallbackStartDate,
                        cancellationToken).ConfigureAwait(false);

                    var pageUri = BuildStudentsUri(schoolId, pageSize);
                    while (pageUri is not null)
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        using var request = new HttpRequestMessage(HttpMethod.Get, pageUri);
                        request.Headers.Add("Authorization", _apiKey);

                        var requestStartTime = DateTime.UtcNow;
                        await LogImportEventAsync(schoolId, requestId, "students", "request", request.RequestUri?.ToString(), requestStartTime, requestStartTime, 0, cancellationToken).ConfigureAwait(false);

                        using var response = await client.SendAsync(request, cancellationToken).ConfigureAwait(false);
                        var responseEndTime = DateTime.UtcNow;
                        await LogImportEventAsync(schoolId, requestId, "students", "response", request.RequestUri?.ToString(), requestStartTime, responseEndTime, 0, cancellationToken).ConfigureAwait(false);
                        if (!response.IsSuccessStatusCode)
                        {
                            await LogImportEventAsync(schoolId, requestId, "students", "api failure", request.RequestUri?.ToString(), requestStartTime, responseEndTime, 0, cancellationToken).ConfigureAwait(false);
                            await LogTextAsync("ERROR", $"Students API call failed for school {schoolId}: {(int)response.StatusCode} {response.ReasonPhrase}. URL: {request.RequestUri}", cancellationToken).ConfigureAwait(false);
                            break;
                        }

                        var json = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
                        var page = WondeModels.WondeStudent.FromJson(json)
                            ?? throw new InvalidOperationException($"Wonde returned an empty student response for school {schoolId}.");

                        AddStudentRows(schoolId, academicYear, page.data, studentRows, groupRows);
                        if (studentRows.Rows.Count >= batchSize)
                        {
                            var rowsProcessed = studentRows.Rows.Count;
                            await WriteStudentBatchAsync(connection, studentRows, groupRows, cancellationToken).ConfigureAwait(false);
                            var batchEndTime = DateTime.UtcNow;
                            await LogImportEventAsync(schoolId, requestId, "students", "insert batch complete", request.RequestUri?.ToString(), requestStartTime, batchEndTime, rowsProcessed, cancellationToken).ConfigureAwait(false);
                        }

                        pageUri = GetNextPageUri(page.meta?.pagination, schoolId, "student");
                    }
                }
                catch (Exception ex) when (ex is not OperationCanceledException)
                {
                    var errorTime = DateTime.UtcNow;
                    await LogImportEventAsync(schoolId, requestId, "students", "error", null, errorTime, errorTime, 0, cancellationToken).ConfigureAwait(false);
                    await LogTextAsync("ERROR", $"Students import failed for school {schoolId}: {ex.Message}", cancellationToken).ConfigureAwait(false);
                    continue;
                }
            }



            var finalStudentRows = studentRows.Rows.Count;
            if (finalStudentRows > 0)
            {
                var finalBatchStartTime = DateTime.UtcNow;
                await WriteStudentBatchAsync(connection, studentRows, groupRows, cancellationToken).ConfigureAwait(false);
                var finalBatchEndTime = DateTime.UtcNow;
                await LogImportEventAsync("SYSTEM", requestId, "students", "insert batch complete", null, finalBatchStartTime, finalBatchEndTime, finalStudentRows, cancellationToken).ConfigureAwait(false);
            }
            await MergeStudentStagingAsync(connection, cancellationToken).ConfigureAwait(false);

            var sectionEndTime = DateTime.UtcNow;
            await LogImportEventAsync("SYSTEM", requestId, "students", "section finish", null, sectionStartTime, sectionEndTime, 0, cancellationToken).ConfigureAwait(false);
        }

        public async Task GetStudentLeaversAsync(
            IReadOnlyCollection<string> schoolIds,
            string academicYear,
            int pageSize,
            int batchSize,
            DateTime? universalUpdatedAfterDate,
            DateTime fallbackStartDate,
            string requestId,
            CancellationToken cancellationToken = default)
        {
            ValidateStudentImportArguments(schoolIds, academicYear, pageSize, batchSize);

            var sectionStartTime = DateTime.UtcNow;
            await LogImportEventAsync("SYSTEM", requestId, "student_leavers", "section start", null, sectionStartTime, sectionStartTime, 0, cancellationToken).ConfigureAwait(false);

            using var client = new HttpClient();
            await using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync(cancellationToken).ConfigureAwait(false);
            await TruncateLeaverStagingAsync(connection, cancellationToken).ConfigureAwait(false);

            var leaverRows = CreateStudentImportTable();
            var groupRows = CreateGroupStudentTable();
            var cdbsRows = CreateLeaverCdbsTable();

            foreach (var schoolId in schoolIds)
            {
                try
                {
                    var updatedAfterDate = await ResolveUpdatedAfterDateAsync(
                        connection,
                        schoolId,
                        "student_leavers",
                        universalUpdatedAfterDate,
                        fallbackStartDate,
                        cancellationToken).ConfigureAwait(false);

                    var pageUri = BuildStudentLeaversUri(schoolId, pageSize);
                    while (pageUri is not null)
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        using var request = new HttpRequestMessage(HttpMethod.Get, pageUri);
                        request.Headers.Add("Authorization", _apiKey);

                        var requestStartTime = DateTime.UtcNow;
                        await LogImportEventAsync(schoolId, requestId, "student_leavers", "request", request.RequestUri?.ToString(), requestStartTime, requestStartTime, 0, cancellationToken).ConfigureAwait(false);

                        using var response = await client.SendAsync(request, cancellationToken).ConfigureAwait(false);
                        var responseEndTime = DateTime.UtcNow;
                        await LogImportEventAsync(schoolId, requestId, "student_leavers", "response", request.RequestUri?.ToString(), requestStartTime, responseEndTime, 0, cancellationToken).ConfigureAwait(false);
                        if (!response.IsSuccessStatusCode)
                        {
                            await LogImportEventAsync(schoolId, requestId, "student_leavers", "api failure", request.RequestUri?.ToString(), requestStartTime, responseEndTime, 0, cancellationToken).ConfigureAwait(false);
                            await LogTextAsync("ERROR", $"Student leavers API call failed for school {schoolId}: {(int)response.StatusCode} {response.ReasonPhrase}. URL: {request.RequestUri}", cancellationToken).ConfigureAwait(false);
                            break;
                        }

                        var json = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
                        var page = WondeModels.WondeStudent.FromJson(json)
                            ?? throw new InvalidOperationException($"Wonde returned an empty student-leaver response for school {schoolId}.");

                        AddStudentRows(schoolId, academicYear, page.data, leaverRows, groupRows);
                        if (page.data is not null)
                        {
                            foreach (var student in page.data)
                            {
                                var cdbsRow = cdbsRows.NewRow();
                                cdbsRow["school_id"] = schoolId;
                                cdbsRow["student_id"] = ToDbValue(student.id);
                                cdbsRow["N00595"] = ToDbValue(student.cbds?.data?.N00595);
                                cdbsRows.Rows.Add(cdbsRow);
                            }
                        }

                        if (leaverRows.Rows.Count >= batchSize)
                        {
                            var rowsProcessed = leaverRows.Rows.Count;
                            await WriteLeaverBatchAsync(connection, leaverRows, groupRows, cdbsRows, cancellationToken).ConfigureAwait(false);
                            var batchEndTime = DateTime.UtcNow;
                            await LogImportEventAsync(schoolId, requestId, "student_leavers", "insert batch complete", request.RequestUri?.ToString(), requestStartTime, batchEndTime, rowsProcessed, cancellationToken).ConfigureAwait(false);
                        }

                        pageUri = GetNextPageUri(page.meta?.pagination, schoolId, "student-leaver");
                    }
                }
                catch (Exception ex) when (ex is not OperationCanceledException)
                {
                    var errorTime = DateTime.UtcNow;
                    await LogImportEventAsync(schoolId, requestId, "student_leavers", "error", null, errorTime, errorTime, 0, cancellationToken).ConfigureAwait(false);
                    await LogTextAsync("ERROR", $"Student leavers import failed for school {schoolId}: {ex.Message}", cancellationToken).ConfigureAwait(false);
                    continue;
                }
            }



            var finalLeaverRows = leaverRows.Rows.Count;
            if (finalLeaverRows > 0)
            {
                var finalBatchStartTime = DateTime.UtcNow;
                await WriteLeaverBatchAsync(connection, leaverRows, groupRows, cdbsRows, cancellationToken).ConfigureAwait(false);
                var finalBatchEndTime = DateTime.UtcNow;
                await LogImportEventAsync("SYSTEM", requestId, "student_leavers", "insert batch complete", null, finalBatchStartTime, finalBatchEndTime, finalLeaverRows, cancellationToken).ConfigureAwait(false);
            }
            await MergeLeaverStagingAsync(connection, cancellationToken).ConfigureAwait(false);

            var sectionEndTime = DateTime.UtcNow;
            await LogImportEventAsync("SYSTEM", requestId, "student_leavers", "section finish", null, sectionStartTime, sectionEndTime, 0, cancellationToken).ConfigureAwait(false);
        }

        private static Uri BuildStudentLeaversUri(string schoolId, int pageSize) =>
            new($"https://api.wonde.com/v1.0/schools/{Uri.EscapeDataString(schoolId)}/students-leaver?per_page={pageSize.ToString(CultureInfo.InvariantCulture)}&include=groups,education_details,extended_details,cbds");

        private static DataTable CreateLeaverCdbsTable()
        {
            var table = new DataTable();
            table.Columns.Add("school_id", typeof(string));
            table.Columns.Add("student_id", typeof(string));
            table.Columns.Add("N00595", typeof(string));
            return table;
        }

        private static async Task TruncateLeaverStagingAsync(SqlConnection connection, CancellationToken cancellationToken)
        {
            await using var command = new SqlCommand(
                "TRUNCATE TABLE [dbo].[student_leaver_insert]; TRUNCATE TABLE [dbo].[student_leaver_cdbs_insert]; TRUNCATE TABLE [dbo].[groups_students_insert];", connection);
            command.CommandTimeout = 120;
            await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
        }

        private static async Task WriteLeaverBatchAsync(
            SqlConnection connection,
            DataTable leaverRows,
            DataTable groupRows,
            DataTable cdbsRows,
            CancellationToken cancellationToken)
        {
            if (leaverRows.Rows.Count == 0) return;

            using (var leaverCopy = new SqlBulkCopy(connection) { DestinationTableName = "[dbo].[student_leaver_insert]", BatchSize = leaverRows.Rows.Count, BulkCopyTimeout = 600 })
            {
                AddColumnMappings(leaverCopy, leaverRows);
                await leaverCopy.WriteToServerAsync(leaverRows, cancellationToken).ConfigureAwait(false);
            }

            if (groupRows.Rows.Count > 0)
            {
                using var groupCopy = new SqlBulkCopy(connection) { DestinationTableName = "[dbo].[groups_students_insert]", BatchSize = groupRows.Rows.Count, BulkCopyTimeout = 600 };
                AddColumnMappings(groupCopy, groupRows);
                await groupCopy.WriteToServerAsync(groupRows, cancellationToken).ConfigureAwait(false);
            }

            if (cdbsRows.Rows.Count > 0)
            {
                using var cdbsCopy = new SqlBulkCopy(connection) { DestinationTableName = "[dbo].[student_leaver_cdbs_insert]", BatchSize = cdbsRows.Rows.Count, BulkCopyTimeout = 600 };
                AddColumnMappings(cdbsCopy, cdbsRows);
                await cdbsCopy.WriteToServerAsync(cdbsRows, cancellationToken).ConfigureAwait(false);
            }

            leaverRows.Clear(); groupRows.Clear(); cdbsRows.Clear();
        }

        private static async Task MergeLeaverStagingAsync(SqlConnection connection, CancellationToken cancellationToken)
        {
            var script = await File.ReadAllTextAsync(Path.Combine(AppContext.BaseDirectory, "files", "StudentLeaverMerge.sql"), cancellationToken).ConfigureAwait(false);
            await using var command = new SqlCommand(script, connection) { CommandTimeout = 600 };
            await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
        }

        private static void ValidateStudentImportArguments(
            IReadOnlyCollection<string> schoolIds,
            string academicYear,
            int pageSize,
            int batchSize)
        {
            ArgumentNullException.ThrowIfNull(schoolIds);
            if (schoolIds.Count == 0) throw new ArgumentException("At least one school ID is required.", nameof(schoolIds));
            if (string.IsNullOrWhiteSpace(academicYear)) throw new ArgumentException("Academic year is required.", nameof(academicYear));
            if (academicYear.Length > 50) throw new ArgumentException("Academic year cannot exceed 50 characters.", nameof(academicYear));
            if (pageSize <= 0) throw new ArgumentOutOfRangeException(nameof(pageSize));
            if (batchSize <= 0) throw new ArgumentOutOfRangeException(nameof(batchSize));
        }

        private static Uri BuildStudentsUri(string schoolId, int pageSize) =>
            new($"https://api.wonde.com/v1.0/schools/{Uri.EscapeDataString(schoolId)}/students?per_page={pageSize.ToString(CultureInfo.InvariantCulture)}&include=groups,education_details,extended_details");

        private static Uri? GetNextPageUri(WondeModels.WondeStudent_Pagination? pagination, string schoolId, string endpointName)
        {
            if (pagination is null) throw new InvalidOperationException($"Wonde response for school {schoolId} did not include {endpointName} pagination metadata.");
            if (!pagination.more) return null;
            return pagination.next ?? throw new InvalidOperationException($"Wonde response for school {schoolId} indicated more {endpointName} pages but did not provide a next URL.");
        }

        private static async Task TruncateStudentStagingAsync(SqlConnection connection, CancellationToken cancellationToken)
        {
            await using var command = new SqlCommand(
                "TRUNCATE TABLE [dbo].[students_insert]; TRUNCATE TABLE [dbo].[groups_students_insert];", connection);
            command.CommandTimeout = 120;
            await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
        }

        private static DataTable CreateStudentImportTable()
        {
            var table = new DataTable();
            foreach (var column in new[] { "school_id", "academic_year", "id", "upi", "mis_id", "initials", "title", "surname", "forename", "middle_names", "legal_surname", "legal_forename", "gender", "gender_identity" }) table.Columns.Add(column, typeof(string));
            table.Columns.Add("date_of_birth", typeof(DateTime));
            foreach (var column in new[] { "restored_at", "created_at", "updated_at" }) table.Columns.Add(column, typeof(DateTime));
            foreach (var column in new[] { "admission_number", "upn", "local_upn", "former_upn", "learner_number", "current_nc_year", "part_time", "part_time_rate" }) table.Columns.Add(column, typeof(string));
            foreach (var column in new[] { "admission_date", "leaving_date" }) table.Columns.Add(column, typeof(DateTime));
            foreach (var column in new[] { "next_of_kin", "religion", "ethnicity", "ethnicity_code", "nationality", "birth_place", "custody_alert", "custody_details", "dietary_needs", "paramedical_support", "sen_status", "first_language", "marital_status" }) table.Columns.Add(column, typeof(string));
            foreach (var column in new[] { "in_lea_care", "ever_in_care", "free_school_meals", "free_school_meals_6", "english_as_additional_language" }) table.Columns.Add(column, typeof(bool));
            foreach (var column in new[] { "english_as_additional_language_status", "premium_pupil_notes", "enrolment_status" }) table.Columns.Add(column, typeof(string));
            foreach (var column in new[] { "premium_pupil_indicator", "premium_pupil_eligible", "service_children_indicator" }) table.Columns.Add(column, typeof(bool));
            foreach (var column in new[] { "enrolment_start_date", "enrolment_end_date" }) table.Columns.Add(column, typeof(DateTime));
            table.Columns.Add("gifted_and_talented_status", typeof(bool));
            foreach (var column in new[] { "national_insurance_number", "application_status", "boarding_status", "permanent_resident", "in_care_details", "general_notes", "leaver_destination", "fsm_review_date", "youth_support_services_agreement", "responsible_care_authority", "home_language" }) table.Columns.Add(column, typeof(string));
            foreach (var column in new[] { "child_protection_plan", "child_in_need" }) table.Columns.Add(column, typeof(bool));
            table.Columns.Add("imported_at", typeof(DateTime));
            return table;
        }

        private static DataTable CreateGroupStudentTable()
        {
            var table = new DataTable();
            table.Columns.Add("school_id", typeof(string));
            table.Columns.Add("student_id", typeof(string));
            table.Columns.Add("group_id", typeof(string));
            table.Columns.Add("start_date", typeof(DateTime));
            table.Columns.Add("end_date", typeof(DateTime));
            table.Columns.Add("imported_at", typeof(DateTime));
            return table;
        }

        private static void AddStudentRows(
            string schoolId,
            string academicYear,
            IReadOnlyCollection<WondeModels.WondeStudent_obj>? students,
            DataTable studentRows,
            DataTable groupRows)
        {
            if (students is null) return;
            foreach (var student in students)
            {
                var education = student.education_details?.data;
                var extended = student.extended_details?.data;
                var row = studentRows.NewRow();
                row["school_id"] = schoolId;
                row["academic_year"] = academicYear;
                row["id"] = ToDbValue(NullableString(student.id, "student ID", 50));
                row["upi"] = ToDbValue(NullableString(student.upi, "upi", 50));
                row["mis_id"] = ToDbValue(NullableString(student.mis_id, "mis ID", 50));
                row["initials"] = ToDbValue(NullableString(student.initials, "initials", 50));
                row["title"] = ToDbValue(student.title); row["surname"] = ToDbValue(student.surname); row["forename"] = ToDbValue(student.forename); row["middle_names"] = ToDbValue(student.middle_names); row["legal_surname"] = ToDbValue(student.legal_surname); row["legal_forename"] = ToDbValue(student.legal_forename);
                row["gender"] = ToDbValue(student.gender); row["gender_identity"] = ToDbValue(student.gender_identity);
                row["date_of_birth"] = ToDbValue(ToSqlDateTime(student.date_of_birth)?.Date); row["restored_at"] = ToDbValue(ToSqlDateTime(student.restored_at)); row["created_at"] = ToDbValue(ToSqlDateTime(student.created_at)); row["updated_at"] = ToDbValue(ToSqlDateTime(student.updated_at));
                row["admission_number"] = ToDbValue(education?.admission_number); row["upn"] = ToDbValue(education?.upn); row["local_upn"] = ToDbValue(education?.local_upn); row["former_upn"] = ToDbValue(education?.former_upn); row["learner_number"] = ToDbValue(education?.learner_number); row["current_nc_year"] = ToDbValue(education?.current_nc_year); row["part_time"] = ToDbValue(education?.part_time?.ToString()); row["part_time_rate"] = ToDbValue(education?.part_time_rate);
                row["admission_date"] = ToDbValue(ToSqlDateTime(education?.admission_date)?.Date); row["leaving_date"] = ToDbValue(ToSqlDateTime(education?.leaving_date)?.Date);
                SetExtendedStudentValues(row, extended);
                row["imported_at"] = DateTime.UtcNow;
                studentRows.Rows.Add(row);

                if (student.groups?.data is null) continue;
                foreach (var group in student.groups.data)
                {
                    var groupRow = groupRows.NewRow();
                    groupRow["school_id"] = schoolId; groupRow["student_id"] = ToDbValue(student.id); groupRow["group_id"] = ToDbValue(group.id);
                    groupRow["start_date"] = ToDbValue(ToSqlDateTime(group.meta?.start_date)); groupRow["end_date"] = ToDbValue(ToSqlDateTime(group.meta?.end_date)); groupRow["imported_at"] = DateTime.UtcNow;
                    groupRows.Rows.Add(groupRow);
                }
            }
        }

        private static void SetExtendedStudentValues(DataRow row, WondeModels.WondeStudent_ExtendedDetailsData? value)
        {
            var strings = new (string, string?)[ ] { ("next_of_kin", value?.next_of_kin), ("religion", value?.religion), ("ethnicity", value?.ethnicity), ("ethnicity_code", value?.ethnicity_code), ("nationality", value?.nationality), ("birth_place", value?.birth_place), ("custody_alert", value?.custody_alert), ("custody_details", value?.custody_details), ("dietary_needs", value?.dietary_needs), ("paramedical_support", value?.paramedical_support), ("sen_status", value?.sen_status), ("first_language", value?.first_language), ("marital_status", value?.marital_status), ("english_as_additional_language_status", value?.english_as_additional_language_status), ("premium_pupil_notes", value?.premium_pupil_notes), ("enrolment_status", value?.enrolment_status), ("national_insurance_number", value?.national_insurance_number), ("application_status", value?.application_status), ("boarding_status", value?.boarding_status), ("permanent_resident", value?.permanent_resident), ("in_care_details", value?.in_care_details), ("general_notes", value?.general_notes), ("leaver_destination", value?.leaver_destination), ("fsm_review_date", value?.fsm_review_date), ("youth_support_services_agreement", value?.youth_support_services_agreement), ("responsible_care_authority", value?.responsible_care_authority), ("home_language", value?.home_language) };
            foreach (var (name, item) in strings) row[name] = ToDbValue(item);
            foreach (var (name, item) in new (string, bool?)[ ] { ("in_lea_care", value?.in_lea_care), ("ever_in_care", value?.ever_in_care), ("free_school_meals", value?.free_school_meals), ("free_school_meals_6", value?.free_school_meals_6), ("english_as_additional_language", value?.english_as_additional_language), ("premium_pupil_indicator", value?.premium_pupil_indicator), ("premium_pupil_eligible", value?.premium_pupil_eligible), ("service_children_indicator", value?.service_children_indicator), ("gifted_and_talented_status", value?.gifted_and_talented_status), ("child_protection_plan", value?.child_protection_plan), ("child_in_need", value?.child_in_need) }) row[name] = ToDbValue(item);
            row["enrolment_start_date"] = ToDbValue(ToSqlDateTime(value?.enrolment_start_date)?.Date); row["enrolment_end_date"] = ToDbValue(ToSqlDateTime(value?.enrolment_end_date)?.Date);
        }

        private static async Task WriteStudentBatchAsync(SqlConnection connection, DataTable studentRows, DataTable groupRows, CancellationToken cancellationToken)
        {
            if (studentRows.Rows.Count == 0) return;
            using var studentCopy = new SqlBulkCopy(connection) { DestinationTableName = "[dbo].[students_insert]", BatchSize = studentRows.Rows.Count, BulkCopyTimeout = 600 };
            AddColumnMappings(studentCopy, studentRows);
            await studentCopy.WriteToServerAsync(studentRows, cancellationToken).ConfigureAwait(false);
            if (groupRows.Rows.Count > 0)
            {
                using var groupCopy = new SqlBulkCopy(connection) { DestinationTableName = "[dbo].[groups_students_insert]", BatchSize = groupRows.Rows.Count, BulkCopyTimeout = 600 };
                AddColumnMappings(groupCopy, groupRows);
                await groupCopy.WriteToServerAsync(groupRows, cancellationToken).ConfigureAwait(false);
            }
            studentRows.Clear(); groupRows.Clear();
        }

        private static async Task MergeStudentStagingAsync(SqlConnection connection, CancellationToken cancellationToken)
        {
            var script = await File.ReadAllTextAsync(Path.Combine(AppContext.BaseDirectory, "files", "StudentMerge.sql"), cancellationToken).ConfigureAwait(false);
            await using var command = new SqlCommand(script, connection) { CommandTimeout = 600 };
            await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
        }

        public async Task GetAchievementsAsync(
            IReadOnlyCollection<string> schoolIds,
            DateTime? universalUpdatedAfterDate,
            DateTime fallbackStartDate,
            int pageSize,
            int batchSize,
            string requestId,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(schoolIds);

            var sectionStartTime = DateTime.UtcNow;
            await LogImportEventAsync("SYSTEM", requestId, "achievements", "section start", null, sectionStartTime, sectionStartTime, 0, cancellationToken).ConfigureAwait(false);

            if (schoolIds.Count == 0)
            {
                throw new ArgumentException("At least one school ID is required.", nameof(schoolIds));
            }

            if (pageSize <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(pageSize));
            }

            if (batchSize <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(batchSize));
            }

            using var client = new HttpClient();
            await using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync(cancellationToken).ConfigureAwait(false);
            await AcquireAchievementImportLockAsync(connection, cancellationToken).ConfigureAwait(false);
            await TruncateAchievementStagingAsync(connection, cancellationToken).ConfigureAwait(false);

            var achievementRows = CreateAchievementTable();
            var studentRows = CreateStudentTable();

            foreach (var schoolId in schoolIds)
            {
                if (string.IsNullOrWhiteSpace(schoolId))
                {
                    throw new ArgumentException("School IDs cannot be empty.", nameof(schoolIds));
                }

                try
                {
                    var updatedAfterDate = await ResolveUpdatedAfterDateAsync(
                        connection,
                        schoolId,
                        "achievements",
                        universalUpdatedAfterDate,
                        fallbackStartDate,
                        cancellationToken).ConfigureAwait(false);

                    var pageUri = BuildAchievementsUri(schoolId, updatedAfterDate, pageSize);
                    var pageNumber = 0;

                    while (pageUri is not null)
                    {
                        cancellationToken.ThrowIfCancellationRequested();

                        using var request = new HttpRequestMessage(HttpMethod.Get, pageUri);
                        request.Headers.Add("Authorization", _apiKey);

                        var requestStartTime = DateTime.UtcNow;
                        await LogImportEventAsync(schoolId, requestId, "achievements", "request", request.RequestUri?.ToString(), requestStartTime, requestStartTime, 0, cancellationToken).ConfigureAwait(false);

                        using var response = await client.SendAsync(request, cancellationToken).ConfigureAwait(false);
                        var responseEndTime = DateTime.UtcNow;
                        await LogImportEventAsync(schoolId, requestId, "achievements", "response", request.RequestUri?.ToString(), requestStartTime, responseEndTime, 0, cancellationToken).ConfigureAwait(false);
                        if (!response.IsSuccessStatusCode)
                        {
                            await LogImportEventAsync(schoolId, requestId, "achievements", "api failure", request.RequestUri?.ToString(), requestStartTime, responseEndTime, 0, cancellationToken).ConfigureAwait(false);
                            await LogTextAsync("ERROR", $"Achievements API call failed for school {schoolId}: {(int)response.StatusCode} {response.ReasonPhrase}. URL: {request.RequestUri}", cancellationToken).ConfigureAwait(false);
                            break;
                        }

                        var json = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
                        var page = WondeModels.WondeAchievement.FromJson(json)
                            ?? throw new InvalidOperationException($"Wonde returned an empty achievement response for school {schoolId}.");

                        pageNumber++;
                        Console.WriteLine($"School {schoolId}: received achievement page {pageNumber} containing {page.data?.Count ?? 0} records.");
                        AddAchievementRows(schoolId, page.data, achievementRows, studentRows);

                        if (achievementRows.Rows.Count >= batchSize)
                        {
                            var rowsProcessed = achievementRows.Rows.Count;
                            await WriteAchievementBatchAsync(connection, achievementRows, studentRows, cancellationToken).ConfigureAwait(false);
                            var batchEndTime = DateTime.UtcNow;
                            await LogImportEventAsync(schoolId, requestId, "achievements", "insert batch complete", request.RequestUri?.ToString(), requestStartTime, batchEndTime, rowsProcessed, cancellationToken).ConfigureAwait(false);
                        }

                        if (page.meta?.pagination is null)
                        {
                            throw new InvalidOperationException($"Wonde response for school {schoolId} did not include pagination metadata.");
                        }

                        if (!page.meta.pagination.more)
                        {
                            pageUri = null;
                        }
                        else if (page.meta.pagination.next is null)
                        {
                            throw new InvalidOperationException($"Wonde response for school {schoolId} indicated more pages but did not provide a next URL.");
                        }
                        else
                        {
                            pageUri = page.meta.pagination.next;
                        }
                    }
                }
                catch (Exception ex) when (ex is not OperationCanceledException)
                {
                    var errorTime = DateTime.UtcNow;
                    await LogImportEventAsync(schoolId, requestId, "achievements", "error", null, errorTime, errorTime, 0, cancellationToken).ConfigureAwait(false);
                    await LogTextAsync("ERROR", $"Achievements import failed for school {schoolId}: {ex.Message}", cancellationToken).ConfigureAwait(false);
                    continue;
                }
            }

            var finalAchievementRows = achievementRows.Rows.Count;
            if (finalAchievementRows > 0)
            {
                var finalBatchStartTime = DateTime.UtcNow;
                await WriteAchievementBatchAsync(connection, achievementRows, studentRows, cancellationToken).ConfigureAwait(false);
                var finalBatchEndTime = DateTime.UtcNow;
                await LogImportEventAsync("SYSTEM", requestId, "achievements", "insert batch complete", null, finalBatchStartTime, finalBatchEndTime, finalAchievementRows, cancellationToken).ConfigureAwait(false);
            }
            await MergeAchievementStagingAsync(connection, cancellationToken).ConfigureAwait(false);

            var sectionEndTime = DateTime.UtcNow;
            await LogImportEventAsync("SYSTEM", requestId, "achievements", "section finish", null, sectionStartTime, sectionEndTime, 0, cancellationToken).ConfigureAwait(false);
        }

        private static async Task AcquireAchievementImportLockAsync(SqlConnection connection, CancellationToken cancellationToken)
        {
            await using var command = new SqlCommand(
                "DECLARE @result int; EXEC @result = sp_getapplock @Resource = N'WondeImportTool.AchievementImport', @LockMode = N'Exclusive', @LockOwner = N'Session', @LockTimeout = 0; SELECT @result;",
                connection);

            var result = Convert.ToInt32(await command.ExecuteScalarAsync(cancellationToken).ConfigureAwait(false), CultureInfo.InvariantCulture);
            if (result < 0)
            {
                throw new InvalidOperationException($"Another achievement import is already running (sp_getapplock returned {result}).");
            }
        }

        private static async Task TruncateAchievementStagingAsync(SqlConnection connection, CancellationToken cancellationToken)
        {
            await using var command = new SqlCommand(
                "TRUNCATE TABLE [dbo].[achievements_students_insert]; TRUNCATE TABLE [dbo].[achievements_table_insert];",
                connection);
            command.CommandTimeout = 120;
            await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
        }

        private static async Task WriteAchievementBatchAsync(
            SqlConnection connection,
            DataTable achievementRows,
            DataTable studentRows,
            CancellationToken cancellationToken)
        {
            if (achievementRows.Rows.Count == 0)
            {
                return;
            }

            var achievementCount = achievementRows.Rows.Count;
            var studentCount = studentRows.Rows.Count;

            using (var achievementCopy = new SqlBulkCopy(connection))
            {
                achievementCopy.DestinationTableName = "[dbo].[achievements_table_insert]";
                achievementCopy.BatchSize = achievementCount;
                achievementCopy.BulkCopyTimeout = 600;
                AddColumnMappings(achievementCopy, achievementRows);
                await achievementCopy.WriteToServerAsync(achievementRows, cancellationToken).ConfigureAwait(false);
            }

            if (studentCount > 0)
            {
                using var studentCopy = new SqlBulkCopy(connection);
                studentCopy.DestinationTableName = "[dbo].[achievements_students_insert]";
                studentCopy.BatchSize = studentCount;
                studentCopy.BulkCopyTimeout = 600;
                AddColumnMappings(studentCopy, studentRows);
                await studentCopy.WriteToServerAsync(studentRows, cancellationToken).ConfigureAwait(false);
            }

            achievementRows.Clear();
            studentRows.Clear();
            Console.WriteLine($"Staged {achievementCount} achievements and {studentCount} student rows.");
        }

        private static async Task MergeAchievementStagingAsync(SqlConnection connection, CancellationToken cancellationToken)
        {
            var scriptPath = Path.Combine(AppContext.BaseDirectory, "files", "AchievementMerge.sql");
            var script = await File.ReadAllTextAsync(scriptPath, cancellationToken).ConfigureAwait(false);

            await using var command = new SqlCommand(script, connection)
            {
                CommandTimeout = 600
            };
            await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
            Console.WriteLine("Achievement staging merge completed.");
        }

        public async Task GetBehavioursAsync(
            IReadOnlyCollection<string> schoolIds,
            DateTime? universalUpdatedAfterDate,
            DateTime fallbackStartDate,
            int pageSize,
            int batchSize,
            string requestId,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(schoolIds);

            var sectionStartTime = DateTime.UtcNow;
            await LogImportEventAsync("SYSTEM", requestId, "behaviours", "section start", null, sectionStartTime, sectionStartTime, 0, cancellationToken).ConfigureAwait(false);

            if (schoolIds.Count == 0)
            {
                throw new ArgumentException("At least one school ID is required.", nameof(schoolIds));
            }

            if (pageSize <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(pageSize));
            }

            if (batchSize <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(batchSize));
            }

            using var client = new HttpClient();
            await using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync(cancellationToken).ConfigureAwait(false);
            await AcquireBehaviourImportLockAsync(connection, cancellationToken).ConfigureAwait(false);
            await TruncateBehaviourStagingAsync(connection, cancellationToken).ConfigureAwait(false);

            var behaviourRows = CreateBehaviourTable();
            var studentRows = CreateBehaviourStudentTable();

            foreach (var schoolId in schoolIds)
            {
                if (string.IsNullOrWhiteSpace(schoolId))
                {
                    throw new ArgumentException("School IDs cannot be empty.", nameof(schoolIds));
                }

                try
                {
                    var updatedAfterDate = await ResolveUpdatedAfterDateAsync(
                        connection,
                        schoolId,
                        "behaviours",
                        universalUpdatedAfterDate,
                        fallbackStartDate,
                        cancellationToken).ConfigureAwait(false);

                    var pageUri = BuildBehavioursUri(schoolId, updatedAfterDate, pageSize);
                    var pageNumber = 0;

                    while (pageUri is not null)
                    {
                        cancellationToken.ThrowIfCancellationRequested();

                        using var request = new HttpRequestMessage(HttpMethod.Get, pageUri);
                        request.Headers.Add("Authorization", _apiKey);

                        var requestStartTime = DateTime.UtcNow;
                        await LogImportEventAsync(schoolId, requestId, "behaviours", "request", request.RequestUri?.ToString(), requestStartTime, requestStartTime, 0, cancellationToken).ConfigureAwait(false);

                        using var response = await client.SendAsync(request, cancellationToken).ConfigureAwait(false);
                        var responseEndTime = DateTime.UtcNow;
                        await LogImportEventAsync(schoolId, requestId, "behaviours", "response", request.RequestUri?.ToString(), requestStartTime, responseEndTime, 0, cancellationToken).ConfigureAwait(false);
                        if (!response.IsSuccessStatusCode)
                        {
                            await LogImportEventAsync(schoolId, requestId, "behaviours", "api failure", request.RequestUri?.ToString(), requestStartTime, responseEndTime, 0, cancellationToken).ConfigureAwait(false);
                            await LogTextAsync("ERROR", $"Behaviours API call failed for school {schoolId}: {(int)response.StatusCode} {response.ReasonPhrase}. URL: {request.RequestUri}", cancellationToken).ConfigureAwait(false);
                            break;
                        }

                        var json = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
                        var page = WondeModels.WondeBehaviour.FromJson(json)
                            ?? throw new InvalidOperationException($"Wonde returned an empty behaviour response for school {schoolId}.");

                        pageNumber++;
                        Console.WriteLine($"School {schoolId}: received behaviour page {pageNumber} containing {page.data?.Count ?? 0} records.");
                        AddBehaviourRows(schoolId, page.data, behaviourRows, studentRows);

                        if (behaviourRows.Rows.Count >= batchSize)
                        {
                            var rowsProcessed = behaviourRows.Rows.Count;
                            await WriteBehaviourBatchAsync(connection, behaviourRows, studentRows, cancellationToken).ConfigureAwait(false);
                            var batchEndTime = DateTime.UtcNow;
                            await LogImportEventAsync(schoolId, requestId, "behaviours", "insert batch complete", request.RequestUri?.ToString(), requestStartTime, batchEndTime, rowsProcessed, cancellationToken).ConfigureAwait(false);
                        }

                        if (page.meta?.pagination is null)
                        {
                            throw new InvalidOperationException($"Wonde response for school {schoolId} did not include behaviour pagination metadata.");
                        }

                        if (!page.meta.pagination.more)
                        {
                            pageUri = null;
                        }
                        else if (page.meta.pagination.next is null)
                        {
                            throw new InvalidOperationException($"Wonde response for school {schoolId} indicated more behaviour pages but did not provide a next URL.");
                        }
                        else
                        {
                            pageUri = page.meta.pagination.next;
                        }
                    }
                }
                catch (Exception ex) when (ex is not OperationCanceledException)
                {
                    var errorTime = DateTime.UtcNow;
                    await LogImportEventAsync(schoolId, requestId, "behaviours", "error", null, errorTime, errorTime, 0, cancellationToken).ConfigureAwait(false);
                    await LogTextAsync("ERROR", $"Behaviours import failed for school {schoolId}: {ex.Message}", cancellationToken).ConfigureAwait(false);
                    continue;
                }
            }

            var finalBehaviourRows = behaviourRows.Rows.Count;
            if (finalBehaviourRows > 0)
            {
                var finalBatchStartTime = DateTime.UtcNow;
                await WriteBehaviourBatchAsync(connection, behaviourRows, studentRows, cancellationToken).ConfigureAwait(false);
                var finalBatchEndTime = DateTime.UtcNow;
                await LogImportEventAsync("SYSTEM", requestId, "behaviours", "insert batch complete", null, finalBatchStartTime, finalBatchEndTime, finalBehaviourRows, cancellationToken).ConfigureAwait(false);
            }
            await MergeBehaviourStagingAsync(connection, cancellationToken).ConfigureAwait(false);

            var sectionEndTime = DateTime.UtcNow;
            await LogImportEventAsync("SYSTEM", requestId, "behaviours", "section finish", null, sectionStartTime, sectionEndTime, 0, cancellationToken).ConfigureAwait(false);
        }

        private static Uri BuildBehavioursUri(string schoolId, DateTime behaviourStartDate, int pageSize)
        {
            var query = string.Join(
                "&",
                $"per_page={pageSize.ToString(CultureInfo.InvariantCulture)}",
                "include=students",
                $"updated_after={Uri.EscapeDataString(behaviourStartDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture))}");

            return new Uri($"https://api.wonde.com/v1.0/schools/{Uri.EscapeDataString(schoolId)}/behaviours?{query}");
        }

        private static DataTable CreateBehaviourTable()
        {
            var table = new DataTable();
            table.Columns.Add("school_id", typeof(string));
            table.Columns.Add("id", typeof(string));
            table.Columns.Add("behaviour_type", typeof(string));
            table.Columns.Add("location", typeof(string));
            table.Columns.Add("subject", typeof(string));
            table.Columns.Add("class", typeof(string));
            table.Columns.Add("status", typeof(string));
            table.Columns.Add("action", typeof(string));
            table.Columns.Add("comment", typeof(string));
            table.Columns.Add("parents_notified", typeof(string));
            table.Columns.Add("points", typeof(int));
            table.Columns.Add("incident_date", typeof(DateTime));
            table.Columns.Add("action_date", typeof(DateTime));
            table.Columns.Add("created_on", typeof(DateTime));
            table.Columns.Add("recorded_on", typeof(DateTime));
            table.Columns.Add("updated_on", typeof(DateTime));
            table.Columns.Add("imported_on", typeof(DateTime));
            return table;
        }

        private static DataTable CreateBehaviourStudentTable()
        {
            var table = new DataTable();
            table.Columns.Add("school_id", typeof(string));
            table.Columns.Add("id", typeof(string));
            table.Columns.Add("student_id", typeof(string));
            table.Columns.Add("points", typeof(int));
            table.Columns.Add("points_meta", typeof(int));
            return table;
        }

        private static void AddBehaviourRows(
            string schoolId,
            IReadOnlyCollection<WondeModels.WondeBehaviourDatum>? behaviours,
            DataTable behaviourRows,
            DataTable studentRows)
        {
            if (behaviours is null)
            {
                return;
            }

            foreach (var behaviour in behaviours)
            {
                var behaviourId = RequiredString(behaviour.id, "behaviour id", 50);
                var behaviourRow = behaviourRows.NewRow();
                behaviourRow["school_id"] = RequiredString(schoolId, "school ID", 50);
                behaviourRow["id"] = behaviourId;
                behaviourRow["behaviour_type"] = NullableString(behaviour.type, "behaviour type", 100);
                behaviourRow["location"] = NullableString(behaviour.location, "behaviour location", 100);
                behaviourRow["subject"] = NullableString(behaviour.subject, "behaviour subject", 100);
                behaviourRow["class"] = NullableString(behaviour.@class, "behaviour class", 100);
                behaviourRow["status"] = NullableString(behaviour.status, "behaviour status", 100);
                behaviourRow["action"] = NullableString(ToStringValue(behaviour.action), "behaviour action", 100);
                behaviourRow["comment"] = NullableString(behaviour.comment, "behaviour comment", int.MaxValue);
                behaviourRow["parents_notified"] = NullableString(ToStringValue(behaviour.parents_notified), "parents notified", 100) ?? (object)DBNull.Value;
                behaviourRow["points"] = ToSqlInt32(behaviour.points ?? 0, "behaviour points");
                behaviourRow["incident_date"] = ToDbValue(ToSqlDateTime(behaviour.incident_date));
                behaviourRow["action_date"] = ToDbValue(ToSqlDateTime(behaviour.action_date));
                behaviourRow["created_on"] = ToDbValue(ToSqlDateTime(behaviour.created_at));
                behaviourRow["recorded_on"] = ToDbValue(ToSqlDateTime(behaviour.recorded_date));
                behaviourRow["updated_on"] = ToDbValue(ToSqlDateTime(behaviour.updated_at));
                behaviourRow["imported_on"] = DateTime.UtcNow;
                behaviourRows.Rows.Add(behaviourRow);

                if (behaviour.students?.data is null)
                {
                    continue;
                }

                foreach (var student in behaviour.students.data)
                {
                    var studentRow = studentRows.NewRow();
                    studentRow["school_id"] = RequiredString(schoolId, "school ID", 50);
                    studentRow["id"] = behaviourId;
                    studentRow["student_id"] = NullableString(student.id, "student ID", 50);
                    studentRow["points"] = ToSqlInt32(behaviour.points ?? 0, "behaviour points");
                    studentRow["points_meta"] = student.meta is null
                        ? DBNull.Value
                        : ToSqlInt32(student.meta.points ?? 0, "behaviour student points metadata");
                    studentRows.Rows.Add(studentRow);
                }
            }
        }

        private static async Task AcquireBehaviourImportLockAsync(SqlConnection connection, CancellationToken cancellationToken)
        {
            await using var command = new SqlCommand(
                "DECLARE @result int; EXEC @result = sp_getapplock @Resource = N'WondeImportTool.BehaviourImport', @LockMode = N'Exclusive', @LockOwner = N'Session', @LockTimeout = 0; SELECT @result;",
                connection);

            var result = Convert.ToInt32(await command.ExecuteScalarAsync(cancellationToken).ConfigureAwait(false), CultureInfo.InvariantCulture);
            if (result < 0)
            {
                throw new InvalidOperationException($"Another behaviour import is already running (sp_getapplock returned {result}).");
            }
        }

        private static async Task TruncateBehaviourStagingAsync(SqlConnection connection, CancellationToken cancellationToken)
        {
            await using var command = new SqlCommand(
                "TRUNCATE TABLE [dbo].[behaviours_students_insert]; TRUNCATE TABLE [dbo].[behaviours_table_insert];",
                connection);
            command.CommandTimeout = 120;
            await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
        }

        private static async Task WriteBehaviourBatchAsync(
            SqlConnection connection,
            DataTable behaviourRows,
            DataTable studentRows,
            CancellationToken cancellationToken)
        {
            if (behaviourRows.Rows.Count == 0)
            {
                return;
            }

            var behaviourCount = behaviourRows.Rows.Count;
            var studentCount = studentRows.Rows.Count;

            using (var behaviourCopy = new SqlBulkCopy(connection))
            {
                behaviourCopy.DestinationTableName = "[dbo].[behaviours_table_insert]";
                behaviourCopy.BatchSize = behaviourCount;
                behaviourCopy.BulkCopyTimeout = 600;
                AddColumnMappings(behaviourCopy, behaviourRows);
                await behaviourCopy.WriteToServerAsync(behaviourRows, cancellationToken).ConfigureAwait(false);
            }

            if (studentCount > 0)
            {
                using var studentCopy = new SqlBulkCopy(connection);
                studentCopy.DestinationTableName = "[dbo].[behaviours_students_insert]";
                studentCopy.BatchSize = studentCount;
                studentCopy.BulkCopyTimeout = 600;
                AddColumnMappings(studentCopy, studentRows);
                await studentCopy.WriteToServerAsync(studentRows, cancellationToken).ConfigureAwait(false);
            }

            behaviourRows.Clear();
            studentRows.Clear();
            Console.WriteLine($"Staged {behaviourCount} behaviours and {studentCount} behaviour student rows.");
        }

        private static async Task MergeBehaviourStagingAsync(SqlConnection connection, CancellationToken cancellationToken)
        {
            var scriptPath = Path.Combine(AppContext.BaseDirectory, "files", "BehaviourMerge.sql");
            var script = await File.ReadAllTextAsync(scriptPath, cancellationToken).ConfigureAwait(false);

            await using var command = new SqlCommand(script, connection)
            {
                CommandTimeout = 600
            };
            await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
            Console.WriteLine("Behaviour staging merge completed.");
        }

        public async Task GetAttendanceSessionsAsync(
            IReadOnlyCollection<string> schoolIds,
            DateTime? universalUpdatedAfterDate,
            DateTime fallbackStartDate,
            int pageSize,
            int batchSize,
            string requestId,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(schoolIds);

            var sectionStartTime = DateTime.UtcNow;
            await LogImportEventAsync("SYSTEM", requestId, "attendance_session", "section start", null, sectionStartTime, sectionStartTime, 0, cancellationToken).ConfigureAwait(false);

            if (schoolIds.Count == 0)
            {
                throw new ArgumentException("At least one school ID is required.", nameof(schoolIds));
            }

            if (pageSize <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(pageSize));
            }

            if (batchSize <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(batchSize));
            }

            using var client = new HttpClient();
            await using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync(cancellationToken).ConfigureAwait(false);
            await AcquireAttendanceImportLockAsync(connection, cancellationToken).ConfigureAwait(false);
            await TruncateAttendanceStagingAsync(connection, cancellationToken).ConfigureAwait(false);

            var attendanceRows = CreateAttendanceSessionTable();

            foreach (var schoolId in schoolIds)
            {
                if (string.IsNullOrWhiteSpace(schoolId))
                {
                    throw new ArgumentException("School IDs cannot be empty.", nameof(schoolIds));
                }

                try
                {
                    var updatedAfterDate = await ResolveUpdatedAfterDateAsync(
                        connection,
                        schoolId,
                        "attendance_session",
                        universalUpdatedAfterDate,
                        fallbackStartDate,
                        cancellationToken).ConfigureAwait(false);

                    var pageUri = BuildAttendanceSessionsUri(schoolId, updatedAfterDate, pageSize);
                    var pageNumber = 0;

                    while (pageUri is not null)
                    {
                        cancellationToken.ThrowIfCancellationRequested();

                        using var request = new HttpRequestMessage(HttpMethod.Get, pageUri);
                        request.Headers.Add("Authorization", _apiKey);

                        var requestStartTime = DateTime.UtcNow;
                        await LogImportEventAsync(schoolId, requestId, "attendance_session", "request", request.RequestUri?.ToString(), requestStartTime, requestStartTime, 0, cancellationToken).ConfigureAwait(false);

                        using var response = await client.SendAsync(request, cancellationToken).ConfigureAwait(false);
                        var responseEndTime = DateTime.UtcNow;
                        await LogImportEventAsync(schoolId, requestId, "attendance_session", "response", request.RequestUri?.ToString(), requestStartTime, responseEndTime, 0, cancellationToken).ConfigureAwait(false);
                        if (!response.IsSuccessStatusCode)
                        {
                            await LogImportEventAsync(schoolId, requestId, "attendance_session", "api failure", request.RequestUri?.ToString(), requestStartTime, responseEndTime, 0, cancellationToken).ConfigureAwait(false);
                            await LogTextAsync("ERROR", $"Attendance API call failed for school {schoolId}: {(int)response.StatusCode} {response.ReasonPhrase}. URL: {request.RequestUri}", cancellationToken).ConfigureAwait(false);
                            break;
                        }

                        var json = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
                        var page = WondeModels.WondeAttendanceSession.FromJson(json)
                            ?? throw new InvalidOperationException($"Wonde returned an empty attendance response for school {schoolId}.");

                        pageNumber++;
                        Console.WriteLine($"School {schoolId}: received attendance page {pageNumber} containing {page.data?.Count ?? 0} records.");
                        AddAttendanceSessionRows(schoolId, page.data, attendanceRows);

                        if (attendanceRows.Rows.Count >= batchSize)
                        {
                            var rowsProcessed = attendanceRows.Rows.Count;
                            await WriteAttendanceBatchAsync(connection, attendanceRows, cancellationToken).ConfigureAwait(false);
                            var batchEndTime = DateTime.UtcNow;
                            await LogImportEventAsync(schoolId, requestId, "attendance_session", "insert batch complete", request.RequestUri?.ToString(), requestStartTime, batchEndTime, rowsProcessed, cancellationToken).ConfigureAwait(false);
                        }

                        if (page.meta?.pagination is null)
                        {
                            throw new InvalidOperationException($"Wonde response for school {schoolId} did not include attendance pagination metadata.");
                        }

                        if (!page.meta.pagination.more)
                        {
                            pageUri = null;
                        }
                        else if (page.meta.pagination.next is null)
                        {
                            throw new InvalidOperationException($"Wonde response for school {schoolId} indicated more attendance pages but did not provide a next URL.");
                        }
                        else
                        {
                            pageUri = page.meta.pagination.next;
                        }
                    }
                }
                catch (Exception ex) when (ex is not OperationCanceledException)
                {
                    var errorTime = DateTime.UtcNow;
                    await LogImportEventAsync(schoolId, requestId, "attendance_session", "error", null, errorTime, errorTime, 0, cancellationToken).ConfigureAwait(false);
                    await LogTextAsync("ERROR", $"Attendance import failed for school {schoolId}: {ex.Message}", cancellationToken).ConfigureAwait(false);
                    continue;
                }
            }

            var finalAttendanceRows = attendanceRows.Rows.Count;
            if (finalAttendanceRows > 0)
            {
                var finalBatchStartTime = DateTime.UtcNow;
                await WriteAttendanceBatchAsync(connection, attendanceRows, cancellationToken).ConfigureAwait(false);
                var finalBatchEndTime = DateTime.UtcNow;
                await LogImportEventAsync("SYSTEM", requestId, "attendance_session", "insert batch complete", null, finalBatchStartTime, finalBatchEndTime, finalAttendanceRows, cancellationToken).ConfigureAwait(false);
            }

            await MergeAttendanceStagingAsync(connection, cancellationToken).ConfigureAwait(false);

            var sectionEndTime = DateTime.UtcNow;
            await LogImportEventAsync("SYSTEM", requestId, "attendance_session", "section finish", null, sectionStartTime, sectionEndTime, 0, cancellationToken).ConfigureAwait(false);
        }

        private static Uri BuildAttendanceSessionsUri(string schoolId, DateTime attendanceStartDate, int pageSize)
        {
            var query = string.Join(
                "&",
                $"per_page={pageSize.ToString(CultureInfo.InvariantCulture)}",
                $"updated_after={Uri.EscapeDataString(attendanceStartDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture))}");

            return new Uri($"https://api.wonde.com/v1.0/schools/{Uri.EscapeDataString(schoolId)}/attendance/session?{query}");
        }

        private static DataTable CreateAttendanceSessionTable()
        {
            var table = new DataTable();
            table.Columns.Add("school_id", typeof(string));
            table.Columns.Add("id", typeof(string));
            table.Columns.Add("date", typeof(DateTime));
            table.Columns.Add("comment", typeof(string));
            table.Columns.Add("session", typeof(string));
            table.Columns.Add("employee_id", typeof(string));
            table.Columns.Add("attendance_code_id", typeof(string));
            table.Columns.Add("student_id", typeof(string));
            table.Columns.Add("imported_at", typeof(DateTime));
            return table;
        }

        private static void AddAttendanceSessionRows(
            string schoolId,
            IReadOnlyCollection<WondeModels.WondeAttendanceSession_Datum>? attendanceSessions,
            DataTable attendanceRows)
        {
            if (attendanceSessions is null)
            {
                return;
            }

            foreach (var attendance in attendanceSessions)
            {
                var row = attendanceRows.NewRow();
                row["school_id"] = RequiredString(schoolId, "school ID", 50);
                row["id"] = RequiredString(attendance.id, "attendance id", 50);
                row["date"] = ToDbValue(ToSqlDateTime(attendance.date)?.Date);
                row["comment"] = NullableString(attendance.comment, "attendance comment", int.MaxValue);
                row["session"] = NullableString(attendance.session, "attendance session", 2);
                row["employee_id"] = NullableString(attendance.employee, "attendance employee ID", 50);
                row["attendance_code_id"] = NullableString(attendance.attendance_code, "attendance code ID", 50);
                row["student_id"] = NullableString(attendance.student, "attendance student ID", 50);
                row["imported_at"] = DateTime.UtcNow;
                attendanceRows.Rows.Add(row);
            }
        }

        private static async Task AcquireAttendanceImportLockAsync(SqlConnection connection, CancellationToken cancellationToken)
        {
            await using var command = new SqlCommand(
                "DECLARE @result int; EXEC @result = sp_getapplock @Resource = N'WondeImportTool.AttendanceImport', @LockMode = N'Exclusive', @LockOwner = N'Session', @LockTimeout = 0; SELECT @result;",
                connection);

            var result = Convert.ToInt32(await command.ExecuteScalarAsync(cancellationToken).ConfigureAwait(false), CultureInfo.InvariantCulture);
            if (result < 0)
            {
                throw new InvalidOperationException($"Another attendance import is already running (sp_getapplock returned {result}).");
            }
        }

        private static async Task TruncateAttendanceStagingAsync(SqlConnection connection, CancellationToken cancellationToken)
        {
            await using var command = new SqlCommand(
                "TRUNCATE TABLE [dbo].[attendance_session_insert];",
                connection);
            command.CommandTimeout = 120;
            await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
        }

        private static async Task WriteAttendanceBatchAsync(
            SqlConnection connection,
            DataTable attendanceRows,
            CancellationToken cancellationToken)
        {
            if (attendanceRows.Rows.Count == 0)
            {
                return;
            }

            var attendanceCount = attendanceRows.Rows.Count;
            using var attendanceCopy = new SqlBulkCopy(connection);
            attendanceCopy.DestinationTableName = "[dbo].[attendance_session_insert]";
            attendanceCopy.BatchSize = attendanceCount;
            attendanceCopy.BulkCopyTimeout = 600;
            AddColumnMappings(attendanceCopy, attendanceRows);
            await attendanceCopy.WriteToServerAsync(attendanceRows, cancellationToken).ConfigureAwait(false);

            attendanceRows.Clear();
            Console.WriteLine($"Staged {attendanceCount} attendance rows.");
        }

        private static async Task MergeAttendanceStagingAsync(SqlConnection connection, CancellationToken cancellationToken)
        {
            var scriptPath = Path.Combine(AppContext.BaseDirectory, "files", "AttendanceMerge.sql");
            var script = await File.ReadAllTextAsync(scriptPath, cancellationToken).ConfigureAwait(false);

            await using var command = new SqlCommand(script, connection)
            {
                CommandTimeout = 600
            };
            await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
            Console.WriteLine("Attendance staging merge completed.");
        }

        public async Task GetExclusionsAsync(
            IReadOnlyCollection<string> schoolIds,
            DateTime? universalUpdatedAfterDate,
            DateTime fallbackStartDate,
            int pageSize,
            int batchSize,
            string requestId,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(schoolIds);

            var sectionStartTime = DateTime.UtcNow;
            await LogImportEventAsync("SYSTEM", requestId, "exclusion_table", "section start", null, sectionStartTime, sectionStartTime, 0, cancellationToken).ConfigureAwait(false);

            if (schoolIds.Count == 0)
            {
                throw new ArgumentException("At least one school ID is required.", nameof(schoolIds));
            }

            if (pageSize <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(pageSize));
            }

            if (batchSize <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(batchSize));
            }

            using var client = new HttpClient();
            await using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync(cancellationToken).ConfigureAwait(false);
            await AcquireExclusionImportLockAsync(connection, cancellationToken).ConfigureAwait(false);
            await TruncateExclusionStagingAsync(connection, cancellationToken).ConfigureAwait(false);

            var exclusionRows = CreateExclusionTable();
            var exclusionStudentRows = CreateExclusionStudentTable();

            foreach (var schoolId in schoolIds)
            {
                if (string.IsNullOrWhiteSpace(schoolId))
                {
                    throw new ArgumentException("School IDs cannot be empty.", nameof(schoolIds));
                }

                try
                {
                    var updatedAfterDate = await ResolveUpdatedAfterDateAsync(
                        connection,
                        schoolId,
                        "exclusion_table",
                        universalUpdatedAfterDate,
                        fallbackStartDate,
                        cancellationToken).ConfigureAwait(false);

                    var pageUri = BuildExclusionsUri(schoolId, updatedAfterDate, pageSize);
                    var pageNumber = 0;

                    while (pageUri is not null)
                    {
                        cancellationToken.ThrowIfCancellationRequested();

                        using var request = new HttpRequestMessage(HttpMethod.Get, pageUri);
                        request.Headers.Add("Authorization", _apiKey);

                        var requestStartTime = DateTime.UtcNow;
                        await LogImportEventAsync(schoolId, requestId, "exclusion_table", "request", request.RequestUri?.ToString(), requestStartTime, requestStartTime, 0, cancellationToken).ConfigureAwait(false);

                        using var response = await client.SendAsync(request, cancellationToken).ConfigureAwait(false);
                        var responseEndTime = DateTime.UtcNow;
                        await LogImportEventAsync(schoolId, requestId, "exclusion_table", "response", request.RequestUri?.ToString(), requestStartTime, responseEndTime, 0, cancellationToken).ConfigureAwait(false);
                        if (!response.IsSuccessStatusCode)
                        {
                            await LogImportEventAsync(schoolId, requestId, "exclusion_table", "api failure", request.RequestUri?.ToString(), requestStartTime, responseEndTime, 0, cancellationToken).ConfigureAwait(false);
                            await LogTextAsync("ERROR", $"Exclusions API call failed for school {schoolId}: {(int)response.StatusCode} {response.ReasonPhrase}. URL: {request.RequestUri}", cancellationToken).ConfigureAwait(false);
                            break;
                        }

                        var json = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
                        var page = WondeModels.WondeExclusion.FromJson(json)
                            ?? throw new InvalidOperationException($"Wonde returned an empty exclusions response for school {schoolId}.");

                        pageNumber++;
                        Console.WriteLine($"School {schoolId}: received exclusions page {pageNumber} containing {page.data?.Count ?? 0} records.");
                        AddExclusionRows(schoolId, page.data, exclusionRows, exclusionStudentRows);

                        if (exclusionRows.Rows.Count >= batchSize)
                        {
                            var rowsProcessed = exclusionRows.Rows.Count;
                            await WriteExclusionBatchAsync(connection, exclusionRows, exclusionStudentRows, cancellationToken).ConfigureAwait(false);
                            var batchEndTime = DateTime.UtcNow;
                            await LogImportEventAsync(schoolId, requestId, "exclusion_table", "insert batch complete", request.RequestUri?.ToString(), requestStartTime, batchEndTime, rowsProcessed, cancellationToken).ConfigureAwait(false);
                        }

                        if (page.meta?.pagination is null)
                        {
                            throw new InvalidOperationException($"Wonde response for school {schoolId} did not include exclusion pagination metadata.");
                        }

                        if (!page.meta.pagination.more)
                        {
                            pageUri = null;
                        }
                        else if (page.meta.pagination.next is null)
                        {
                            throw new InvalidOperationException($"Wonde response for school {schoolId} indicated more exclusion pages but did not provide a next URL.");
                        }
                        else
                        {
                            pageUri = page.meta.pagination.next;
                        }
                    }
                }
                catch (Exception ex) when (ex is not OperationCanceledException)
                {
                    var errorTime = DateTime.UtcNow;
                    await LogImportEventAsync(schoolId, requestId, "exclusion_table", "error", null, errorTime, errorTime, 0, cancellationToken).ConfigureAwait(false);
                    await LogTextAsync("ERROR", $"Exclusions import failed for school {schoolId}: {ex.Message}", cancellationToken).ConfigureAwait(false);
                    continue;
                }
            }

            var finalExclusionRows = exclusionRows.Rows.Count;
            if (finalExclusionRows > 0)
            {
                var finalBatchStartTime = DateTime.UtcNow;
                await WriteExclusionBatchAsync(connection, exclusionRows, exclusionStudentRows, cancellationToken).ConfigureAwait(false);
                var finalBatchEndTime = DateTime.UtcNow;
                await LogImportEventAsync("SYSTEM", requestId, "exclusion_table", "insert batch complete", null, finalBatchStartTime, finalBatchEndTime, finalExclusionRows, cancellationToken).ConfigureAwait(false);
            }

            await MergeExclusionStagingAsync(connection, cancellationToken).ConfigureAwait(false);

            var sectionEndTime = DateTime.UtcNow;
            await LogImportEventAsync("SYSTEM", requestId, "exclusion_table", "section finish", null, sectionStartTime, sectionEndTime, 0, cancellationToken).ConfigureAwait(false);
        }

        private static Uri BuildExclusionsUri(string schoolId, DateTime exclusionStartDate, int pageSize)
        {
            var query = string.Join(
                "&",
                $"per_page={pageSize.ToString(CultureInfo.InvariantCulture)}",
                "include=student,student-leaver",
                $"updated_after={Uri.EscapeDataString(exclusionStartDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture))}");

            return new Uri($"https://api.wonde.com/v1.0/schools/{Uri.EscapeDataString(schoolId)}/exclusions?{query}");
        }

        private static DataTable CreateExclusionTable()
        {
            var table = new DataTable();
            table.Columns.Add("school_id", typeof(string));
            table.Columns.Add("id", typeof(string));
            table.Columns.Add("type", typeof(string));
            table.Columns.Add("type_code", typeof(string));
            table.Columns.Add("reason", typeof(string));
            table.Columns.Add("reason_code", typeof(string));
            table.Columns.Add("start_date", typeof(DateTime));
            table.Columns.Add("start_session", typeof(string));
            table.Columns.Add("end_date", typeof(DateTime));
            table.Columns.Add("end_session", typeof(string));
            table.Columns.Add("sessions", typeof(string));
            table.Columns.Add("days", typeof(string));
            table.Columns.Add("academic_year", typeof(string));
            table.Columns.Add("term", typeof(string));
            table.Columns.Add("comments", typeof(string));
            table.Columns.Add("agencies_involved", typeof(string));
            table.Columns.Add("discipline_committee_date", typeof(DateTime));
            table.Columns.Add("discipline_committee_result", typeof(string));
            table.Columns.Add("discipline_committee_reinstatement_date", typeof(DateTime));
            table.Columns.Add("discipline_committee_representation_made", typeof(bool));
            table.Columns.Add("appeal_received", typeof(bool));
            table.Columns.Add("appeal_result", typeof(string));
            table.Columns.Add("appeal_result_date", typeof(DateTime));
            table.Columns.Add("appeal_reinstatement_date", typeof(DateTime));
            table.Columns.Add("created_at", typeof(DateTime));
            table.Columns.Add("updated_at", typeof(DateTime));
            table.Columns.Add("imported_at", typeof(DateTime));
            return table;
        }

        private static DataTable CreateExclusionStudentTable()
        {
            var table = new DataTable();
            table.Columns.Add("school_id", typeof(string));
            table.Columns.Add("id", typeof(string));
            table.Columns.Add("student_id", typeof(string));
            return table;
        }

        private static void AddExclusionRows(
            string schoolId,
            IReadOnlyCollection<WondeModels.Datum>? exclusions,
            DataTable exclusionRows,
            DataTable exclusionStudentRows)
        {
            if (exclusions is null)
            {
                return;
            }

            foreach (var exclusion in exclusions)
            {
                var exclusionId = RequiredString(exclusion.id, "exclusion id", 50);
                var row = exclusionRows.NewRow();
                row["school_id"] = RequiredString(schoolId, "school ID", 50);
                row["id"] = exclusionId;
                row["type"] = NullableString(exclusion.type, "exclusion type", 50);
                row["type_code"] = NullableString(exclusion.type_code, "exclusion type code", 50);
                row["reason"] = NullableString(exclusion.reason, "exclusion reason", 255);
                row["reason_code"] = NullableString(exclusion.reason_code, "exclusion reason code", 255);
                row["start_date"] = ToDbValue(ToSqlDateTime(exclusion.start_date));
                row["start_session"] = NullableString(exclusion.start_session, "exclusion start session", 50);
                row["end_date"] = ToDbValue(ToSqlDateTime(exclusion.end_date));
                row["end_session"] = NullableString(exclusion.end_session, "exclusion end session", 50);
                row["sessions"] = NullableString(exclusion.sessions, "exclusion sessions", 50);
                row["days"] = NullableString(exclusion.days, "exclusion days", 50);
                row["academic_year"] = NullableString(exclusion.academic_year, "exclusion academic year", 50);
                row["term"] = NullableString(exclusion.term, "exclusion term", 50);
                row["comments"] = NullableString(exclusion.comments, "exclusion comments", int.MaxValue);
                row["agencies_involved"] = NullableString(exclusion.agencies_involved, "exclusion agencies involved", 255);
                row["discipline_committee_date"] = ToDbValue(ToSqlDateTime(exclusion.discipline_committee_date));
                row["discipline_committee_result"] = NullableString(exclusion.discipline_committee_result, "exclusion discipline committee result", 255);
                row["discipline_committee_reinstatement_date"] = ToDbValue(ToSqlDateTime(exclusion.discipline_committee_reinstatement_date));
                row["discipline_committee_representation_made"] = ToDbValue(exclusion.discipline_committee_representation_made);
                row["appeal_received"] = ToDbValue(exclusion.appeal_received);
                row["appeal_result"] = NullableString(exclusion.appeal_result, "exclusion appeal result", 255);
                row["appeal_result_date"] = ToDbValue(ToSqlDateTime(exclusion.appeal_result_date));
                row["appeal_reinstatement_date"] = ToDbValue(ToSqlDateTime(exclusion.appeal_reinstatement_date));
                row["created_at"] = ToDbValue(ToSqlDateTime(exclusion.created_at));
                row["updated_at"] = ToDbValue(ToSqlDateTime(exclusion.updated_at));
                row["imported_at"] = DateTime.UtcNow;
                exclusionRows.Rows.Add(row);

                var studentId = exclusion.student?.data?.id ?? exclusion.studentleaver?.data?.id;
                if (!string.IsNullOrWhiteSpace(studentId))
                {
                    var studentRow = exclusionStudentRows.NewRow();
                    studentRow["school_id"] = RequiredString(schoolId, "school ID", 50);
                    studentRow["id"] = exclusionId;
                    studentRow["student_id"] = NullableString(studentId, "exclusion student ID", 50);
                    exclusionStudentRows.Rows.Add(studentRow);
                }
            }
        }

        private static async Task AcquireExclusionImportLockAsync(SqlConnection connection, CancellationToken cancellationToken)
        {
            await using var command = new SqlCommand(
                "DECLARE @result int; EXEC @result = sp_getapplock @Resource = N'WondeImportTool.ExclusionImport', @LockMode = N'Exclusive', @LockOwner = N'Session', @LockTimeout = 0; SELECT @result;",
                connection);

            var result = Convert.ToInt32(await command.ExecuteScalarAsync(cancellationToken).ConfigureAwait(false), CultureInfo.InvariantCulture);
            if (result < 0)
            {
                throw new InvalidOperationException($"Another exclusion import is already running (sp_getapplock returned {result}).");
            }
        }

        private static async Task TruncateExclusionStagingAsync(SqlConnection connection, CancellationToken cancellationToken)
        {
            await using var command = new SqlCommand(
                "TRUNCATE TABLE [dbo].[exclusion_students_insert]; TRUNCATE TABLE [dbo].[exclusion_table_insert];",
                connection);
            command.CommandTimeout = 120;
            await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
        }

        private static async Task WriteExclusionBatchAsync(
            SqlConnection connection,
            DataTable exclusionRows,
            DataTable exclusionStudentRows,
            CancellationToken cancellationToken)
        {
            if (exclusionRows.Rows.Count == 0)
            {
                return;
            }

            var exclusionCount = exclusionRows.Rows.Count;
            var exclusionStudentCount = exclusionStudentRows.Rows.Count;

            using (var exclusionCopy = new SqlBulkCopy(connection))
            {
                exclusionCopy.DestinationTableName = "[dbo].[exclusion_table_insert]";
                exclusionCopy.BatchSize = exclusionCount;
                exclusionCopy.BulkCopyTimeout = 600;
                AddColumnMappings(exclusionCopy, exclusionRows);
                await exclusionCopy.WriteToServerAsync(exclusionRows, cancellationToken).ConfigureAwait(false);
            }

            if (exclusionStudentCount > 0)
            {
                using var exclusionStudentCopy = new SqlBulkCopy(connection);
                exclusionStudentCopy.DestinationTableName = "[dbo].[exclusion_students_insert]";
                exclusionStudentCopy.BatchSize = exclusionStudentCount;
                exclusionStudentCopy.BulkCopyTimeout = 600;
                AddColumnMappings(exclusionStudentCopy, exclusionStudentRows);
                await exclusionStudentCopy.WriteToServerAsync(exclusionStudentRows, cancellationToken).ConfigureAwait(false);
            }

            exclusionRows.Clear();
            exclusionStudentRows.Clear();
            Console.WriteLine($"Staged {exclusionCount} exclusions and {exclusionStudentCount} exclusion student rows.");
        }

        private static async Task MergeExclusionStagingAsync(SqlConnection connection, CancellationToken cancellationToken)
        {
            var scriptPath = Path.Combine(AppContext.BaseDirectory, "files", "ExclusionMerge.sql");
            var script = await File.ReadAllTextAsync(scriptPath, cancellationToken).ConfigureAwait(false);

            await using var command = new SqlCommand(script, connection)
            {
                CommandTimeout = 600
            };
            await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
            Console.WriteLine("Exclusion staging merge completed.");
        }

        public async Task GetGroupsAsync(
            IReadOnlyCollection<string> schoolIds,
            DateTime? universalUpdatedAfterDate,
            DateTime fallbackStartDate,
            int pageSize,
            int batchSize,
            string requestId,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(schoolIds);

            var sectionStartTime = DateTime.UtcNow;
            await LogImportEventAsync("SYSTEM", requestId, "groups", "section start", null, sectionStartTime, sectionStartTime, 0, cancellationToken).ConfigureAwait(false);

            if (schoolIds.Count == 0)
            {
                throw new ArgumentException("At least one school ID is required.", nameof(schoolIds));
            }

            if (pageSize <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(pageSize));
            }

            if (batchSize <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(batchSize));
            }

            using var client = new HttpClient();
            await using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync(cancellationToken).ConfigureAwait(false);
            await AcquireGroupsImportLockAsync(connection, cancellationToken).ConfigureAwait(false);
            await TruncateGroupsStagingAsync(connection, cancellationToken).ConfigureAwait(false);

            var groupRows = CreateGroupsTable();
            var groupStudentRows = CreateGroupsStudentsTable();
            var groupEmployeeRows = CreateGroupsEmployeesTable();

            foreach (var schoolId in schoolIds)
            {
                if (string.IsNullOrWhiteSpace(schoolId))
                {
                    throw new ArgumentException("School IDs cannot be empty.", nameof(schoolIds));
                }

                try
                {
                    var updatedAfterDate = await ResolveUpdatedAfterDateAsync(
                        connection,
                        schoolId,
                        "groups",
                        universalUpdatedAfterDate,
                        fallbackStartDate,
                        cancellationToken).ConfigureAwait(false);

                    var pageUri = BuildGroupsUri(schoolId, updatedAfterDate, pageSize);
                    var pageNumber = 0;

                    while (pageUri is not null)
                    {
                        cancellationToken.ThrowIfCancellationRequested();

                        using var request = new HttpRequestMessage(HttpMethod.Get, pageUri);
                        request.Headers.Add("Authorization", _apiKey);

                        var requestStartTime = DateTime.UtcNow;
                        await LogImportEventAsync(schoolId, requestId, "groups", "request", request.RequestUri?.ToString(), requestStartTime, requestStartTime, 0, cancellationToken).ConfigureAwait(false);

                        using var response = await client.SendAsync(request, cancellationToken).ConfigureAwait(false);
                        var responseEndTime = DateTime.UtcNow;
                        await LogImportEventAsync(schoolId, requestId, "groups", "response", request.RequestUri?.ToString(), requestStartTime, responseEndTime, 0, cancellationToken).ConfigureAwait(false);
                        if (!response.IsSuccessStatusCode)
                        {
                            await LogImportEventAsync(schoolId, requestId, "groups", "api failure", request.RequestUri?.ToString(), requestStartTime, responseEndTime, 0, cancellationToken).ConfigureAwait(false);
                            await LogTextAsync("ERROR", $"Groups API call failed for school {schoolId}: {(int)response.StatusCode} {response.ReasonPhrase}. URL: {request.RequestUri}", cancellationToken).ConfigureAwait(false);
                            break;
                        }

                        var json = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
                        var page = WondeModels.WondeGroups.FromJson(json)
                            ?? throw new InvalidOperationException($"Wonde returned an empty groups response for school {schoolId}.");

                        pageNumber++;
                        Console.WriteLine($"School {schoolId}: received groups page {pageNumber} containing {page.data?.Count ?? 0} records.");
                        AddGroupsRows(schoolId, page.data, groupRows, groupStudentRows, groupEmployeeRows);

                        if (groupRows.Rows.Count >= batchSize)
                        {
                            var rowsProcessed = groupRows.Rows.Count;
                            await WriteGroupsBatchAsync(connection, groupRows, groupStudentRows, groupEmployeeRows, cancellationToken).ConfigureAwait(false);
                            var batchEndTime = DateTime.UtcNow;
                            await LogImportEventAsync(schoolId, requestId, "groups", "insert batch complete", request.RequestUri?.ToString(), requestStartTime, batchEndTime, rowsProcessed, cancellationToken).ConfigureAwait(false);
                        }

                        if (page.meta?.pagination is null)
                        {
                            throw new InvalidOperationException($"Wonde response for school {schoolId} did not include groups pagination metadata.");
                        }

                        if (!page.meta.pagination.more)
                        {
                            pageUri = null;
                        }
                        else if (page.meta.pagination.next is null)
                        {
                            throw new InvalidOperationException($"Wonde response for school {schoolId} indicated more groups pages but did not provide a next URL.");
                        }
                        else
                        {
                            pageUri = page.meta.pagination.next;
                        }
                    }
                }
                catch (Exception ex) when (ex is not OperationCanceledException)
                {
                    var errorTime = DateTime.UtcNow;
                    await LogImportEventAsync(schoolId, requestId, "groups", "error", null, errorTime, errorTime, 0, cancellationToken).ConfigureAwait(false);
                    await LogTextAsync("ERROR", $"Groups import failed for school {schoolId}: {ex.Message}", cancellationToken).ConfigureAwait(false);
                    continue;
                }
            }

            var finalGroupRows = groupRows.Rows.Count;
            if (finalGroupRows > 0)
            {
                var finalBatchStartTime = DateTime.UtcNow;
                await WriteGroupsBatchAsync(connection, groupRows, groupStudentRows, groupEmployeeRows, cancellationToken).ConfigureAwait(false);
                var finalBatchEndTime = DateTime.UtcNow;
                await LogImportEventAsync("SYSTEM", requestId, "groups", "insert batch complete", null, finalBatchStartTime, finalBatchEndTime, finalGroupRows, cancellationToken).ConfigureAwait(false);
            }

            await MergeGroupsStagingAsync(connection, cancellationToken).ConfigureAwait(false);

            var sectionEndTime = DateTime.UtcNow;
            await LogImportEventAsync("SYSTEM", requestId, "groups", "section finish", null, sectionStartTime, sectionEndTime, 0, cancellationToken).ConfigureAwait(false);
        }

        private static Uri BuildGroupsUri(string schoolId, DateTime groupsStartDate, int pageSize)
        {
            var query = string.Join(
                "&",
                $"per_page={pageSize.ToString(CultureInfo.InvariantCulture)}",
                "include=students,employees",
                $"updated_after={Uri.EscapeDataString(groupsStartDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture))}");

            return new Uri($"https://api.wonde.com/v1.0/schools/{Uri.EscapeDataString(schoolId)}/groups?{query}");
        }

        private static DataTable CreateGroupsTable()
        {
            var table = new DataTable();
            table.Columns.Add("school_id", typeof(string));
            table.Columns.Add("id", typeof(string));
            table.Columns.Add("mis_id", typeof(string));
            table.Columns.Add("name", typeof(string));
            table.Columns.Add("code", typeof(string));
            table.Columns.Add("type", typeof(string));
            table.Columns.Add("description", typeof(string));
            table.Columns.Add("notes", typeof(string));
            table.Columns.Add("division", typeof(string));
            table.Columns.Add("created_at", typeof(DateTime));
            table.Columns.Add("updated_at", typeof(DateTime));
            table.Columns.Add("imported_at", typeof(DateTime));
            return table;
        }

        private static DataTable CreateGroupsStudentsTable()
        {
            var table = new DataTable();
            table.Columns.Add("school_id", typeof(string));
            table.Columns.Add("student_id", typeof(string));
            table.Columns.Add("group_id", typeof(string));
            table.Columns.Add("start_date", typeof(DateTime));
            table.Columns.Add("end_date", typeof(DateTime));
            table.Columns.Add("imported_at", typeof(DateTime));
            return table;
        }

        private static DataTable CreateGroupsEmployeesTable()
        {
            var table = new DataTable();
            table.Columns.Add("school_id", typeof(string));
            table.Columns.Add("employee_id", typeof(string));
            table.Columns.Add("group_id", typeof(string));
            table.Columns.Add("role", typeof(string));
            return table;
        }

        private static void AddGroupsRows(
            string schoolId,
            IReadOnlyCollection<WondeModels.WondeGroupsDatum>? groups,
            DataTable groupRows,
            DataTable groupStudentRows,
            DataTable groupEmployeeRows)
        {
            if (groups is null)
            {
                return;
            }

            foreach (var group in groups)
            {
                var groupId = RequiredString(group.id, "group id", 50);
                var row = groupRows.NewRow();
                row["school_id"] = RequiredString(schoolId, "school ID", 50);
                row["id"] = groupId;
                row["mis_id"] = NullableString(group.mis_id, "group mis id", 50);
                row["name"] = NullableString(group.name, "group name", 100);
                row["code"] = NullableString(group.code, "group code", 100);
                row["type"] = NullableString(group.type, "group type", 100);
                row["description"] = NullableString(group.description, "group description", int.MaxValue);
                row["notes"] = NullableString(group.notes, "group notes", int.MaxValue);
                row["division"] = NullableString(group.division, "group division", 100);
                row["created_at"] = ToDbValue(ToSqlDateTime(group.created_at));
                row["updated_at"] = ToDbValue(ToSqlDateTime(group.updated_at));
                row["imported_at"] = DateTime.UtcNow;
                groupRows.Rows.Add(row);

                if (group.students?.data is not null)
                {
                    foreach (var student in group.students.data)
                    {
                        var studentRow = groupStudentRows.NewRow();
                        studentRow["school_id"] = RequiredString(schoolId, "school ID", 50);
                        studentRow["student_id"] = NullableString(student.id, "group student ID", 50);
                        studentRow["group_id"] = groupId;
                        studentRow["start_date"] = ToDbValue(ToSqlDateTime(student.meta?.start_date));
                        studentRow["end_date"] = ToDbValue(ToSqlDateTime(student.meta?.end_date));
                        studentRow["imported_at"] = DateTime.UtcNow;
                        groupStudentRows.Rows.Add(studentRow);
                    }
                }

                if (group.employees?.data is not null)
                {
                    foreach (var employee in group.employees.data)
                    {
                        var employeeRow = groupEmployeeRows.NewRow();
                        employeeRow["school_id"] = RequiredString(schoolId, "school ID", 50);
                        employeeRow["employee_id"] = NullableString(employee.id, "group employee ID", 50);
                        employeeRow["group_id"] = groupId;
                        employeeRow["role"] = NullableString(employee.meta?.role, "group employee role", 50);
                        groupEmployeeRows.Rows.Add(employeeRow);
                    }
                }
            }
        }

        private static async Task AcquireGroupsImportLockAsync(SqlConnection connection, CancellationToken cancellationToken)
        {
            await using var command = new SqlCommand(
                "DECLARE @result int; EXEC @result = sp_getapplock @Resource = N'WondeImportTool.GroupsImport', @LockMode = N'Exclusive', @LockOwner = N'Session', @LockTimeout = 0; SELECT @result;",
                connection);

            var result = Convert.ToInt32(await command.ExecuteScalarAsync(cancellationToken).ConfigureAwait(false), CultureInfo.InvariantCulture);
            if (result < 0)
            {
                throw new InvalidOperationException($"Another groups import is already running (sp_getapplock returned {result}).");
            }
        }

        private static async Task TruncateGroupsStagingAsync(SqlConnection connection, CancellationToken cancellationToken)
        {
            await using var command = new SqlCommand(
                "TRUNCATE TABLE [dbo].[groups_employees_insert]; TRUNCATE TABLE [dbo].[groups_students_insert]; TRUNCATE TABLE [dbo].[groups_insert];",
                connection);
            command.CommandTimeout = 120;
            await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
        }

        private static async Task WriteGroupsBatchAsync(
            SqlConnection connection,
            DataTable groupRows,
            DataTable groupStudentRows,
            DataTable groupEmployeeRows,
            CancellationToken cancellationToken)
        {
            if (groupRows.Rows.Count == 0)
            {
                return;
            }

            var groupCount = groupRows.Rows.Count;
            var groupStudentCount = groupStudentRows.Rows.Count;
            var groupEmployeeCount = groupEmployeeRows.Rows.Count;

            using (var groupCopy = new SqlBulkCopy(connection))
            {
                groupCopy.DestinationTableName = "[dbo].[groups_insert]";
                groupCopy.BatchSize = groupCount;
                groupCopy.BulkCopyTimeout = 600;
                AddColumnMappings(groupCopy, groupRows);
                await groupCopy.WriteToServerAsync(groupRows, cancellationToken).ConfigureAwait(false);
            }

            if (groupStudentCount > 0)
            {
                using var groupStudentCopy = new SqlBulkCopy(connection);
                groupStudentCopy.DestinationTableName = "[dbo].[groups_students_insert]";
                groupStudentCopy.BatchSize = groupStudentCount;
                groupStudentCopy.BulkCopyTimeout = 600;
                AddColumnMappings(groupStudentCopy, groupStudentRows);
                await groupStudentCopy.WriteToServerAsync(groupStudentRows, cancellationToken).ConfigureAwait(false);
            }

            if (groupEmployeeCount > 0)
            {
                using var groupEmployeeCopy = new SqlBulkCopy(connection);
                groupEmployeeCopy.DestinationTableName = "[dbo].[groups_employees_insert]";
                groupEmployeeCopy.BatchSize = groupEmployeeCount;
                groupEmployeeCopy.BulkCopyTimeout = 600;
                AddColumnMappings(groupEmployeeCopy, groupEmployeeRows);
                await groupEmployeeCopy.WriteToServerAsync(groupEmployeeRows, cancellationToken).ConfigureAwait(false);
            }

            groupRows.Clear();
            groupStudentRows.Clear();
            groupEmployeeRows.Clear();
            Console.WriteLine($"Staged {groupCount} groups, {groupStudentCount} group-student rows and {groupEmployeeCount} group-employee rows.");
        }

        private static async Task MergeGroupsStagingAsync(SqlConnection connection, CancellationToken cancellationToken)
        {
            var scriptPath = Path.Combine(AppContext.BaseDirectory, "files", "GroupsMerge.sql");
            var script = await File.ReadAllTextAsync(scriptPath, cancellationToken).ConfigureAwait(false);

            await using var command = new SqlCommand(script, connection)
            {
                CommandTimeout = 600
            };
            await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
            Console.WriteLine("Groups staging merge completed.");
        }

        public async Task GetEmployeesAsync(
            IReadOnlyCollection<string> schoolIds,
            DateTime? universalUpdatedAfterDate,
            DateTime fallbackStartDate,
            int pageSize,
            int batchSize,
            string requestId,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(schoolIds);

            var sectionStartTime = DateTime.UtcNow;
            await LogImportEventAsync("SYSTEM", requestId, "employees", "section start", null, sectionStartTime, sectionStartTime, 0, cancellationToken).ConfigureAwait(false);

            if (schoolIds.Count == 0)
            {
                throw new ArgumentException("At least one school ID is required.", nameof(schoolIds));
            }

            if (pageSize <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(pageSize));
            }

            if (batchSize <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(batchSize));
            }

            using var client = new HttpClient();
            await using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync(cancellationToken).ConfigureAwait(false);
            await AcquireEmployeesImportLockAsync(connection, cancellationToken).ConfigureAwait(false);
            await TruncateEmployeeStagingAsync(connection, cancellationToken).ConfigureAwait(false);

            var employeeRows = CreateEmployeesTable();
            var employmentRows = CreateEmploymentDetailsTable();
            var emailRows = CreateEmployeeEmailTable();

            foreach (var schoolId in schoolIds)
            {
                if (string.IsNullOrWhiteSpace(schoolId))
                {
                    throw new ArgumentException("School IDs cannot be empty.", nameof(schoolIds));
                }

                try
                {
                    var updatedAfterDate = await ResolveUpdatedAfterDateAsync(
                        connection,
                        schoolId,
                        "employees",
                        universalUpdatedAfterDate,
                        fallbackStartDate,
                        cancellationToken).ConfigureAwait(false);

                    var pageUri = BuildEmployeesUri(schoolId, updatedAfterDate, pageSize);
                    var pageNumber = 0;

                    while (pageUri is not null)
                    {
                        cancellationToken.ThrowIfCancellationRequested();

                        using var request = new HttpRequestMessage(HttpMethod.Get, pageUri);
                        request.Headers.Add("Authorization", _apiKey);

                        var requestStartTime = DateTime.UtcNow;
                        await LogImportEventAsync(schoolId, requestId, "employees", "request", request.RequestUri?.ToString(), requestStartTime, requestStartTime, 0, cancellationToken).ConfigureAwait(false);

                        using var response = await client.SendAsync(request, cancellationToken).ConfigureAwait(false);
                        var responseEndTime = DateTime.UtcNow;
                        await LogImportEventAsync(schoolId, requestId, "employees", "response", request.RequestUri?.ToString(), requestStartTime, responseEndTime, 0, cancellationToken).ConfigureAwait(false);
                        if (!response.IsSuccessStatusCode)
                        {
                            await LogImportEventAsync(schoolId, requestId, "employees", "api failure", request.RequestUri?.ToString(), requestStartTime, responseEndTime, 0, cancellationToken).ConfigureAwait(false);
                            await LogTextAsync("ERROR", $"Employees API call failed for school {schoolId}: {(int)response.StatusCode} {response.ReasonPhrase}. URL: {request.RequestUri}", cancellationToken).ConfigureAwait(false);
                            break;
                        }

                        var json = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
                        var page = WondeModels.WondeEmployee.FromJson(json)
                            ?? throw new InvalidOperationException($"Wonde returned an empty employees response for school {schoolId}.");

                        pageNumber++;
                        Console.WriteLine($"School {schoolId}: received employees page {pageNumber} containing {page.data?.Count ?? 0} records.");
                        AddEmployeeRows(schoolId, page.data, employeeRows, employmentRows, emailRows);

                        if (employeeRows.Rows.Count >= batchSize)
                        {
                            var rowsProcessed = employeeRows.Rows.Count;
                            await WriteEmployeeBatchAsync(connection, employeeRows, employmentRows, emailRows, cancellationToken).ConfigureAwait(false);
                            var batchEndTime = DateTime.UtcNow;
                            await LogImportEventAsync(schoolId, requestId, "employees", "insert batch complete", request.RequestUri?.ToString(), requestStartTime, batchEndTime, rowsProcessed, cancellationToken).ConfigureAwait(false);
                        }

                        if (page.meta?.pagination is null)
                        {
                            throw new InvalidOperationException($"Wonde response for school {schoolId} did not include employee pagination metadata.");
                        }

                        if (!page.meta.pagination.more)
                        {
                            pageUri = null;
                        }
                        else if (page.meta.pagination.next is null)
                        {
                            throw new InvalidOperationException($"Wonde response for school {schoolId} indicated more employee pages but did not provide a next URL.");
                        }
                        else
                        {
                            pageUri = page.meta.pagination.next;
                        }
                    }
                }
                catch (Exception ex) when (ex is not OperationCanceledException)
                {
                    var errorTime = DateTime.UtcNow;
                    await LogImportEventAsync(schoolId, requestId, "employees", "error", null, errorTime, errorTime, 0, cancellationToken).ConfigureAwait(false);
                    await LogTextAsync("ERROR", $"Employees import failed for school {schoolId}: {ex.Message}", cancellationToken).ConfigureAwait(false);
                    continue;
                }
            }

            var finalEmployeeRows = employeeRows.Rows.Count;
            if (finalEmployeeRows > 0)
            {
                var finalBatchStartTime = DateTime.UtcNow;
                await WriteEmployeeBatchAsync(connection, employeeRows, employmentRows, emailRows, cancellationToken).ConfigureAwait(false);
                var finalBatchEndTime = DateTime.UtcNow;
                await LogImportEventAsync("SYSTEM", requestId, "employees", "insert batch complete", null, finalBatchStartTime, finalBatchEndTime, finalEmployeeRows, cancellationToken).ConfigureAwait(false);
            }

            await MergeEmployeeStagingAsync(connection, cancellationToken).ConfigureAwait(false);

            var sectionEndTime = DateTime.UtcNow;
            await LogImportEventAsync("SYSTEM", requestId, "employees", "section finish", null, sectionStartTime, sectionEndTime, 0, cancellationToken).ConfigureAwait(false);
        }

        private static Uri BuildEmployeesUri(string schoolId, DateTime updatedAfterDate, int pageSize)
        {
            var query = string.Join(
                "&",
                $"per_page={pageSize.ToString(CultureInfo.InvariantCulture)}",
                "include=employment_details,contact_details",
                $"updated_after={Uri.EscapeDataString(updatedAfterDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture))}");

            return new Uri($"https://api.wonde.com/v1.0/schools/{Uri.EscapeDataString(schoolId)}/employees?{query}");
        }

        private static DataTable CreateEmployeesTable()
        {
            var table = new DataTable();
            table.Columns.Add("school_id", typeof(string));
            table.Columns.Add("id", typeof(string));
            table.Columns.Add("upi", typeof(string));
            table.Columns.Add("mis_id", typeof(string));
            table.Columns.Add("initials", typeof(string));
            table.Columns.Add("title", typeof(string));
            table.Columns.Add("surname", typeof(string));
            table.Columns.Add("forename", typeof(string));
            table.Columns.Add("middle_names", typeof(string));
            table.Columns.Add("legal_surname", typeof(string));
            table.Columns.Add("legal_forename", typeof(string));
            table.Columns.Add("gender", typeof(string));
            table.Columns.Add("date_of_birth", typeof(DateTime));
            table.Columns.Add("created_at", typeof(DateTime));
            table.Columns.Add("updated_at", typeof(DateTime));
            table.Columns.Add("imported_at", typeof(DateTime));
            return table;
        }

        private static DataTable CreateEmploymentDetailsTable()
        {
            var table = new DataTable();
            table.Columns.Add("school_id", typeof(string));
            table.Columns.Add("employee_id", typeof(string));
            table.Columns.Add("teaching_staff", typeof(bool));
            table.Columns.Add("current", typeof(bool));
            table.Columns.Add("teacher_number", typeof(string));
            table.Columns.Add("teacher_category", typeof(string));
            table.Columns.Add("staff_code", typeof(string));
            table.Columns.Add("primary_location", typeof(string));
            table.Columns.Add("employment_start_date", typeof(DateTime));
            table.Columns.Add("employment_end_date", typeof(DateTime));
            table.Columns.Add("local_authority_start_date", typeof(DateTime));
            table.Columns.Add("employee_payroll_number", typeof(string));
            table.Columns.Add("role_text", typeof(string));
            table.Columns.Add("qualified_teacher_status", typeof(string));
            table.Columns.Add("qualified_teacher_status_route", typeof(string));
            table.Columns.Add("higher_level_teaching_assistant_status", typeof(string));
            return table;
        }

        private static DataTable CreateEmployeeEmailTable()
        {
            var table = new DataTable();
            table.Columns.Add("school_id", typeof(string));
            table.Columns.Add("employee_id", typeof(string));
            table.Columns.Add("email", typeof(string));
            table.Columns.Add("primary", typeof(string));
            table.Columns.Add("home", typeof(string));
            table.Columns.Add("work", typeof(string));
            return table;
        }

        private static void AddEmployeeRows(
            string schoolId,
            IReadOnlyCollection<WondeModels.WondeEmployee_Datum>? employees,
            DataTable employeeRows,
            DataTable employmentRows,
            DataTable emailRows)
        {
            if (employees is null)
            {
                return;
            }

            foreach (var employee in employees)
            {
                var employeeId = RequiredString(employee.id, "employee id", 50);

                var employeeRow = employeeRows.NewRow();
                employeeRow["school_id"] = RequiredString(schoolId, "school ID", 50);
                employeeRow["id"] = employeeId;
                employeeRow["upi"] = NullableString(employee.upi, "employee upi", 50);
                employeeRow["mis_id"] = NullableString(employee.mis_id, "employee mis id", 50);
                employeeRow["initials"] = NullableString(employee.initials, "employee initials", int.MaxValue);
                employeeRow["title"] = NullableString(employee.title, "employee title", int.MaxValue);
                employeeRow["surname"] = NullableString(employee.surname, "employee surname", int.MaxValue);
                employeeRow["forename"] = NullableString(employee.forename, "employee forename", int.MaxValue);
                employeeRow["middle_names"] = NullableString(employee.middle_names, "employee middle names", int.MaxValue);
                employeeRow["legal_surname"] = NullableString(employee.legal_surname, "employee legal surname", int.MaxValue);
                employeeRow["legal_forename"] = NullableString(employee.legal_forename, "employee legal forename", int.MaxValue);
                employeeRow["gender"] = NullableString(employee.gender, "employee gender", 50);
                employeeRow["date_of_birth"] = ToDbValue(ToSqlDateTime(employee.date_of_birth)?.Date);
                employeeRow["created_at"] = ToDbValue(ToSqlDateTime(employee.created_at));
                employeeRow["updated_at"] = ToDbValue(ToSqlDateTime(employee.updated_at));
                employeeRow["imported_at"] = DateTime.UtcNow;
                employeeRows.Rows.Add(employeeRow);

                var employment = employee.employment_details?.data;
                if (employment is not null)
                {
                    var employmentRow = employmentRows.NewRow();
                    employmentRow["school_id"] = RequiredString(schoolId, "school ID", 50);
                    employmentRow["employee_id"] = employeeId;
                    employmentRow["teaching_staff"] = ToDbValue(employment.teaching_staff);
                    employmentRow["current"] = ToDbValue(employment.current);
                    employmentRow["teacher_number"] = NullableString(employment.teacher_number, "teacher number", 255);
                    employmentRow["teacher_category"] = NullableString(employment.teacher_category, "teacher category", 255);
                    employmentRow["staff_code"] = NullableString(employment.staff_code, "staff code", 255);
                    employmentRow["primary_location"] = NullableString(employment.primary_location, "primary location", 255);
                    employmentRow["employment_start_date"] = ToDbValue(ToSqlDateTime(employment.employment_start_date)?.Date);
                    employmentRow["employment_end_date"] = ToDbValue(ToSqlDateTime(employment.employment_end_date)?.Date);
                    employmentRow["local_authority_start_date"] = ToDbValue(ToSqlDateTime(employment.local_authority_start_date)?.Date);
                    employmentRow["employee_payroll_number"] = NullableString(employment.employee_payroll_number, "employee payroll number", 255);
                    employmentRow["role_text"] = NullableString(employment.role_text, "role text", 255);
                    employmentRow["qualified_teacher_status"] = NullableString(employment.qualified_teacher_status, "qualified teacher status", 255);
                    employmentRow["qualified_teacher_status_route"] = NullableString(employment.qualified_teacher_status_route, "qualified teacher status route", 255);
                    employmentRow["higher_level_teaching_assistant_status"] = NullableString(employment.higher_level_teaching_assistant_status, "higher level teaching assistant status", 255);
                    employmentRows.Rows.Add(employmentRow);
                }

                var emails = employee.contact_details?.data?.emails;
                if (emails is not null)
                {
                    var emailRow = emailRows.NewRow();
                    emailRow["school_id"] = RequiredString(schoolId, "school ID", 50);
                    emailRow["employee_id"] = employeeId;
                    emailRow["email"] = NullableString(emails.email, "employee email", 255);
                    emailRow["primary"] = NullableString(emails.primary, "employee primary email", 255);
                    emailRow["home"] = NullableString(emails.home, "employee home email", 255);
                    emailRow["work"] = NullableString(emails.work, "employee work email", 255);
                    emailRows.Rows.Add(emailRow);
                }
            }
        }

        private static async Task AcquireEmployeesImportLockAsync(SqlConnection connection, CancellationToken cancellationToken)
        {
            await using var command = new SqlCommand(
                "DECLARE @result int; EXEC @result = sp_getapplock @Resource = N'WondeImportTool.EmployeeImport', @LockMode = N'Exclusive', @LockOwner = N'Session', @LockTimeout = 0; SELECT @result;",
                connection);

            var result = Convert.ToInt32(await command.ExecuteScalarAsync(cancellationToken).ConfigureAwait(false), CultureInfo.InvariantCulture);
            if (result < 0)
            {
                throw new InvalidOperationException($"Another employee import is already running (sp_getapplock returned {result}).");
            }
        }

        private static async Task TruncateEmployeeStagingAsync(SqlConnection connection, CancellationToken cancellationToken)
        {
            await using var command = new SqlCommand(
                "TRUNCATE TABLE [dbo].[employee_email_insert]; TRUNCATE TABLE [dbo].[employment_details_insert]; TRUNCATE TABLE [dbo].[employees_insert];",
                connection);
            command.CommandTimeout = 120;
            await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
        }

        private static async Task WriteEmployeeBatchAsync(
            SqlConnection connection,
            DataTable employeeRows,
            DataTable employmentRows,
            DataTable emailRows,
            CancellationToken cancellationToken)
        {
            if (employeeRows.Rows.Count == 0)
            {
                return;
            }

            var employeeCount = employeeRows.Rows.Count;
            var employmentCount = employmentRows.Rows.Count;
            var emailCount = emailRows.Rows.Count;

            using (var employeeCopy = new SqlBulkCopy(connection))
            {
                employeeCopy.DestinationTableName = "[dbo].[employees_insert]";
                employeeCopy.BatchSize = employeeCount;
                employeeCopy.BulkCopyTimeout = 600;
                AddColumnMappings(employeeCopy, employeeRows);
                await employeeCopy.WriteToServerAsync(employeeRows, cancellationToken).ConfigureAwait(false);
            }

            if (employmentCount > 0)
            {
                using var employmentCopy = new SqlBulkCopy(connection);
                employmentCopy.DestinationTableName = "[dbo].[employment_details_insert]";
                employmentCopy.BatchSize = employmentCount;
                employmentCopy.BulkCopyTimeout = 600;
                AddColumnMappings(employmentCopy, employmentRows);
                await employmentCopy.WriteToServerAsync(employmentRows, cancellationToken).ConfigureAwait(false);
            }

            if (emailCount > 0)
            {
                using var emailCopy = new SqlBulkCopy(connection);
                emailCopy.DestinationTableName = "[dbo].[employee_email_insert]";
                emailCopy.BatchSize = emailCount;
                emailCopy.BulkCopyTimeout = 600;
                AddColumnMappings(emailCopy, emailRows);
                await emailCopy.WriteToServerAsync(emailRows, cancellationToken).ConfigureAwait(false);
            }

            employeeRows.Clear();
            employmentRows.Clear();
            emailRows.Clear();
            Console.WriteLine($"Staged {employeeCount} employees, {employmentCount} employment rows and {emailCount} employee email rows.");
        }

        private static async Task MergeEmployeeStagingAsync(SqlConnection connection, CancellationToken cancellationToken)
        {
            var scriptPath = Path.Combine(AppContext.BaseDirectory, "files", "EmployeeMerge.sql");
            var script = await File.ReadAllTextAsync(scriptPath, cancellationToken).ConfigureAwait(false);

            await using var command = new SqlCommand(script, connection)
            {
                CommandTimeout = 600
            };
            await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
            Console.WriteLine("Employee staging merge completed.");
        }

        private static void AddColumnMappings(SqlBulkCopy bulkCopy, DataTable table)
        {
            foreach (DataColumn column in table.Columns)
            {
                bulkCopy.ColumnMappings.Add(column.ColumnName, column.ColumnName);
            }
        }

        private static DataTable CreateAchievementTable()
        {
            var table = new DataTable();
            table.Columns.Add("school_id", typeof(string));
            table.Columns.Add("id", typeof(string));
            table.Columns.Add("achievement_type", typeof(string));
            table.Columns.Add("action", typeof(string));
            table.Columns.Add("subject", typeof(string));
            table.Columns.Add("class", typeof(string));
            table.Columns.Add("points", typeof(int));
            table.Columns.Add("comments", typeof(string));
            table.Columns.Add("parents_notified", typeof(bool));
            table.Columns.Add("achievement_date", typeof(DateTime));
            table.Columns.Add("action_date", typeof(DateTime));
            table.Columns.Add("recorded_date", typeof(DateTime));
            table.Columns.Add("created_at", typeof(DateTime));
            table.Columns.Add("updated_at", typeof(DateTime));
            table.Columns.Add("imported_at", typeof(DateTime));
            return table;
        }

        private static DataTable CreateStudentTable()
        {
            var table = new DataTable();
            table.Columns.Add("school_id", typeof(string));
            table.Columns.Add("id", typeof(string));
            table.Columns.Add("student_id", typeof(string));
            table.Columns.Add("points", typeof(int));
            table.Columns.Add("points_meta", typeof(int));
            return table;
        }

        private static void AddAchievementRows(
            string schoolId,
            IReadOnlyCollection<WondeModels.WondeAchievementDatum>? achievements,
            DataTable achievementRows,
            DataTable studentRows)
        {
            if (achievements is null)
            {
                return;
            }

            foreach (var achievement in achievements)
            {
                var achievementId = RequiredString(achievement.id, "achievement id", 50);
                var achievementRow = achievementRows.NewRow();
                achievementRow["school_id"] = RequiredString(schoolId, "school ID", 50);
                achievementRow["id"] = achievementId;
                achievementRow["achievement_type"] = NullableString(achievement.type, "achievement type", 100);
                achievementRow["action"] = NullableString(ToStringValue(achievement.action), "action", 100);
                achievementRow["subject"] = NullableString(achievement.subject, "subject", 100);
                achievementRow["class"] = NullableString(achievement.@class, "class", 100);
                achievementRow["points"] = ToSqlInt32(achievement.points ?? 0, "achievement points");
                achievementRow["comments"] = NullableString(achievement.comment, "comments", int.MaxValue);
                achievementRow["parents_notified"] = ToDbValue(ToNullableBoolean(achievement.parents_notified));
                achievementRow["achievement_date"] = ToDbValue(ToSqlDateTime(achievement.achievement_date));
                achievementRow["action_date"] = ToDbValue(ToSqlDateTime(achievement.action_date));
                achievementRow["recorded_date"] = ToDbValue(ToSqlDateTime(achievement.recorded_date));
                achievementRow["created_at"] = ToDbValue(ToSqlDateTime(achievement.created_at));
                achievementRow["updated_at"] = ToDbValue(ToSqlDateTime(achievement.updated_at));
                achievementRow["imported_at"] = DateTime.UtcNow;
                achievementRows.Rows.Add(achievementRow);

                if (achievement.students?.data is null)
                {
                    continue;
                }

                foreach (var student in achievement.students.data)
                {
                    var studentRow = studentRows.NewRow();
                    studentRow["school_id"] = RequiredString(schoolId, "school ID", 50);
                    studentRow["id"] = achievementId;
                    studentRow["student_id"] = NullableString(student.id, "student ID", 50);
                    studentRow["points"] = ToSqlInt32(achievement.points ?? 0, "achievement points");
                    studentRow["points_meta"] = student.meta is null
                        ? DBNull.Value
                        : ToSqlInt32(student.meta.points ?? 0, "student points metadata");
                    studentRows.Rows.Add(studentRow);
                }
            }
        }

        private static string? ToStringValue(object? value) => value?.ToString();

        private static object ToDbValue(object? value) => value ?? DBNull.Value;

        private static string RequiredString(string? value, string fieldName, int maxLength)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new InvalidOperationException($"{fieldName} is required.");
            }

            return ValidateLength(value, fieldName, maxLength);
        }

        private static string? NullableString(string? value, string fieldName, int maxLength)
        {
            return value is null ? null : ValidateLength(value, fieldName, maxLength);
        }

        private static string ValidateLength(string value, string fieldName, int maxLength)
        {
            if (value.Length > maxLength)
            {
                throw new InvalidOperationException($"{fieldName} exceeds the SQL column length of {maxLength}.");
            }

            return value;
        }

        private static int ToSqlInt32(long value, string fieldName)
        {
            if (value is < int.MinValue or > int.MaxValue)
            {
                throw new InvalidOperationException($"{fieldName} is outside the SQL int range.");
            }

            return (int)value;
        }

        private static DateTime? ToSqlDateTime(WondeModels.AchievementDate? value)
        {
            return value?.date.UtcDateTime;
        }

        private static DateTime? ToSqlDateTime(WondeModels.CreatedAt? value)
        {
            return value?.date.UtcDateTime;
        }

        private static DateTime? ToSqlDateTime(WondeModels.WondeStudent_CreatedAt? value)
        {
            return value?.date.ToUniversalTime();
        }

        private static DateTime? ToSqlDateTime(WondeModels.WondeAttendanceSession_Date? value)
        {
            return value?.date.ToUniversalTime();
        }

        private static DateTime? ToSqlDateTime(WondeModels.WondeExclusion_CreatedAt? value)
        {
            return value?.date.ToUniversalTime();
        }

        private static DateTime? ToSqlDateTime(WondeModels.WondeGroups_Date? value)
        {
            return value?.date.ToUniversalTime();
        }

        private static DateTime? ToSqlDateTime(WondeModels.WondeEmployee_DateObj? value)
        {
            return value?.date.ToUniversalTime();
        }

        private static bool? ToNullableBoolean(object? value)
        {
            if (value is null)
            {
                return null;
            }

            if (value is bool booleanValue)
            {
                return booleanValue;
            }

            return bool.TryParse(value.ToString(), out var parsed) ? parsed : throw new InvalidOperationException("parents_notified is not a valid Boolean value.");
        }

        private static Uri BuildAchievementsUri(string schoolId, DateTime achievementStartDate, int pageSize)
        {
            var query = string.Join(
                "&",
                $"per_page={pageSize.ToString(CultureInfo.InvariantCulture)}",
                "include=students",
                $"updated_after={Uri.EscapeDataString(achievementStartDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture))}");

            return new Uri($"https://api.wonde.com/v1.0/schools/{Uri.EscapeDataString(schoolId)}/achievements?{query}");
        }
    }
}