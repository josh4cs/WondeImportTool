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

        public WondeImportService(string apiKey, string connectionString)
        {
            _apiKey = apiKey ?? throw new ArgumentNullException(nameof(apiKey));
            _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
        }

        public async Task GetSchoolsAsync(CancellationToken cancellationToken = default)
        {
            using var client = new HttpClient();
            using var request = new HttpRequestMessage(HttpMethod.Get, "https://api.wonde.com/v1.0/schools");
            request.Headers.Add("Authorization", _apiKey);

            using var response = await client.SendAsync(request, cancellationToken).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();

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
        }

        public async Task GetStudentsAsync(
            IReadOnlyCollection<string> schoolIds,
            string academicYear,
            int pageSize,
            int batchSize,
            CancellationToken cancellationToken = default)
        {
            ValidateStudentImportArguments(schoolIds, academicYear, pageSize, batchSize);

            using var client = new HttpClient();
            await using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync(cancellationToken).ConfigureAwait(false);
            await TruncateStudentStagingAsync(connection, cancellationToken).ConfigureAwait(false);

            var studentRows = CreateStudentImportTable();
            var groupRows = CreateGroupStudentTable();

            foreach (var schoolId in schoolIds)
            {
                var pageUri = BuildStudentsUri(schoolId, pageSize);
                while (pageUri is not null)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    using var request = new HttpRequestMessage(HttpMethod.Get, pageUri);
                    request.Headers.Add("Authorization", _apiKey);
                    using var response = await client.SendAsync(request, cancellationToken).ConfigureAwait(false);
                    response.EnsureSuccessStatusCode();

                    var json = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
                    var page = WondeModels.WondeStudent.FromJson(json)
                        ?? throw new InvalidOperationException($"Wonde returned an empty student response for school {schoolId}.");

                    AddStudentRows(schoolId, academicYear, page.data, studentRows, groupRows);
                    if (studentRows.Rows.Count >= batchSize)
                    {
                        await WriteStudentBatchAsync(connection, studentRows, groupRows, cancellationToken).ConfigureAwait(false);
                    }

                    pageUri = GetNextPageUri(page.meta?.pagination, schoolId, "student");
                }
            }

            await WriteStudentBatchAsync(connection, studentRows, groupRows, cancellationToken).ConfigureAwait(false);
            await MergeStudentStagingAsync(connection, cancellationToken).ConfigureAwait(false);
        }

        public async Task GetStudentLeaversAsync(
            IReadOnlyCollection<string> schoolIds,
            string academicYear,
            int pageSize,
            int batchSize,
            CancellationToken cancellationToken = default)
        {
            ValidateStudentImportArguments(schoolIds, academicYear, pageSize, batchSize);

            using var client = new HttpClient();
            await using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync(cancellationToken).ConfigureAwait(false);
            await TruncateLeaverStagingAsync(connection, cancellationToken).ConfigureAwait(false);

            var leaverRows = CreateStudentImportTable();
            var groupRows = CreateGroupStudentTable();
            var cdbsRows = CreateLeaverCdbsTable();

            foreach (var schoolId in schoolIds)
            {
                var pageUri = BuildStudentLeaversUri(schoolId, pageSize);
                while (pageUri is not null)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    using var request = new HttpRequestMessage(HttpMethod.Get, pageUri);
                    request.Headers.Add("Authorization", _apiKey);
                    using var response = await client.SendAsync(request, cancellationToken).ConfigureAwait(false);
                    response.EnsureSuccessStatusCode();

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
                        await WriteLeaverBatchAsync(connection, leaverRows, groupRows, cdbsRows, cancellationToken).ConfigureAwait(false);
                    }

                    pageUri = GetNextPageUri(page.meta?.pagination, schoolId, "student-leaver");
                }
            }

            await WriteLeaverBatchAsync(connection, leaverRows, groupRows, cdbsRows, cancellationToken).ConfigureAwait(false);
            await MergeLeaverStagingAsync(connection, cancellationToken).ConfigureAwait(false);
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
            DateTime achievementStartDate,
            int pageSize,
            int batchSize,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(schoolIds);

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

                var pageUri = BuildAchievementsUri(schoolId, achievementStartDate, pageSize);
                var pageNumber = 0;

                while (pageUri is not null)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    using var request = new HttpRequestMessage(HttpMethod.Get, pageUri);
                    request.Headers.Add("Authorization", _apiKey);

                    using var response = await client.SendAsync(request, cancellationToken).ConfigureAwait(false);
                    response.EnsureSuccessStatusCode();

                    var json = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
                    var page = WondeModels.WondeAchievement.FromJson(json)
                        ?? throw new InvalidOperationException($"Wonde returned an empty achievement response for school {schoolId}.");

                    pageNumber++;
                    Console.WriteLine($"School {schoolId}: received achievement page {pageNumber} containing {page.data?.Count ?? 0} records.");
                    AddAchievementRows(schoolId, page.data, achievementRows, studentRows);

                    if (achievementRows.Rows.Count >= batchSize)
                    {
                        await WriteAchievementBatchAsync(connection, achievementRows, studentRows, cancellationToken).ConfigureAwait(false);
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

            await WriteAchievementBatchAsync(connection, achievementRows, studentRows, cancellationToken).ConfigureAwait(false);
            await MergeAchievementStagingAsync(connection, cancellationToken).ConfigureAwait(false);
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
            DateTime behaviourStartDate,
            int pageSize,
            int batchSize,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(schoolIds);

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

                var pageUri = BuildBehavioursUri(schoolId, behaviourStartDate, pageSize);
                var pageNumber = 0;

                while (pageUri is not null)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    using var request = new HttpRequestMessage(HttpMethod.Get, pageUri);
                    request.Headers.Add("Authorization", _apiKey);

                    using var response = await client.SendAsync(request, cancellationToken).ConfigureAwait(false);
                    response.EnsureSuccessStatusCode();

                    var json = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
                    var page = WondeModels.WondeBehaviour.FromJson(json)
                        ?? throw new InvalidOperationException($"Wonde returned an empty behaviour response for school {schoolId}.");

                    pageNumber++;
                    Console.WriteLine($"School {schoolId}: received behaviour page {pageNumber} containing {page.data?.Count ?? 0} records.");
                    AddBehaviourRows(schoolId, page.data, behaviourRows, studentRows);

                    if (behaviourRows.Rows.Count >= batchSize)
                    {
                        await WriteBehaviourBatchAsync(connection, behaviourRows, studentRows, cancellationToken).ConfigureAwait(false);
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

            await WriteBehaviourBatchAsync(connection, behaviourRows, studentRows, cancellationToken).ConfigureAwait(false);
            await MergeBehaviourStagingAsync(connection, cancellationToken).ConfigureAwait(false);
        }

        private static Uri BuildBehavioursUri(string schoolId, DateTime behaviourStartDate, int pageSize)
        {
            var query = string.Join(
                "&",
                $"per_page={pageSize.ToString(CultureInfo.InvariantCulture)}",
                "include=students",
                $"incident_date_after={Uri.EscapeDataString(behaviourStartDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture))}");

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
                $"achievement_date_after={Uri.EscapeDataString(achievementStartDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture))}");

            return new Uri($"https://api.wonde.com/v1.0/schools/{Uri.EscapeDataString(schoolId)}/achievements?{query}");
        }
    }
}