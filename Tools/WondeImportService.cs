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
                achievementRow["points"] = ToSqlInt32(achievement.points, "achievement points");
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
                    studentRow["points"] = ToSqlInt32(achievement.points, "achievement points");
                    studentRow["points_meta"] = student.meta is null
                        ? DBNull.Value
                        : ToSqlInt32(student.meta.points, "student points metadata");
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