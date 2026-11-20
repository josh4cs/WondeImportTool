using WondeImportTool.Tools;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

internal class Program
{
    public static async Task Main(string[] args)
    {
        using IHost host = Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((ctx, cfg) =>
            {
                cfg.AddJsonFile("appsettings.json", optional: true)
                   .AddEnvironmentVariables();
            })
            .ConfigureServices((ctx, services) =>
            {
                var config = ctx.Configuration;
                var apiKey = config["Wonde:API_Key"] ?? throw new InvalidOperationException("Wonde:ApiKey missing");
                var conn = config.GetConnectionString("Wonde") ?? throw new InvalidOperationException("Connection string 'Wonde' missing");
                var logPath = config["Log:Path"];

                services.AddSingleton(new WondeImportService(apiKey, conn, logPath));
            })
            .Build();

        var svc = host.Services.GetRequiredService<WondeImportService>();
        var config = host.Services.GetRequiredService<IConfiguration>();
        var cts = new CancellationTokenSource();

        Console.CancelKeyPress += (s, e) =>
        {
            e.Cancel = true;
            cts.Cancel();
        };

        string? requestId = null;

        try
        {
            var schoolIds = config.GetSection("Wonde:SchoolIds")
                .GetChildren()
                .Select(section => section.Value)
                .Where(value => !string.IsNullOrWhiteSpace(value))
                .Select(value => value!)
                .ToArray();

            if (schoolIds.Length == 0)
            {
                throw new InvalidOperationException("Wonde:SchoolIds must contain at least one school ID");
            }

            if (!DateTime.TryParse(
                    config["Wonde:FallbackStartDate"],
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal,
                    out var fallbackStartDate))
            {
                throw new InvalidOperationException("Wonde:FallbackStartDate is missing or invalid");
            }

            DateTime? universalUpdatedAfterDate = null;
            var universalUpdatedAfterValue = config["Wonde:UpdatedAfterDate"];
            if (!string.IsNullOrWhiteSpace(universalUpdatedAfterValue))
            {
                if (!DateTime.TryParse(
                        universalUpdatedAfterValue,
                        CultureInfo.InvariantCulture,
                        DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal,
                        out var parsedUniversalUpdatedAfterDate))
                {
                    throw new InvalidOperationException("Wonde:UpdatedAfterDate is invalid");
                }

                universalUpdatedAfterDate = parsedUniversalUpdatedAfterDate;
            }

            var pageSize = GetPositiveConfigurationValue(config["Wonde:AchievementPageSize"], "Wonde:AchievementPageSize");
            var batchSize = GetPositiveConfigurationValue(config["Wonde:AchievementBatchSize"], "Wonde:AchievementBatchSize");
            var attendancePageSize = GetPositiveConfigurationValue(config["Wonde:AttendancePageSize"], "Wonde:AttendancePageSize");
            var attendanceBatchSize = GetPositiveConfigurationValue(
                config["Wonde:AttendanceBatchSize"] ?? config["Wonde:AchievementBatchSize"],
                "Wonde:AttendanceBatchSize");

            var exclusionsPageSize = GetPositiveConfigurationValue(
                config["Wonde:ExclusionsPageSize"] ?? config["Wonde:AchievementPageSize"],
                "Wonde:ExclusionsPageSize");
            var exclusionsBatchSize = GetPositiveConfigurationValue(
                config["Wonde:ExclusionsBatchSize"] ?? config["Wonde:AchievementBatchSize"],
                "Wonde:ExclusionsBatchSize");

            var groupsPageSize = GetPositiveConfigurationValue(
                config["Wonde:GroupsPageSize"] ?? config["Wonde:AchievementPageSize"],
                "Wonde:GroupsPageSize");
            var groupsBatchSize = GetPositiveConfigurationValue(
                config["Wonde:GroupsBatchSize"] ?? config["Wonde:AchievementBatchSize"],
                "Wonde:GroupsBatchSize");

            var employeePageSize = GetPositiveConfigurationValue(
                config["Wonde:EmployeePageSize"] ?? config["Wonde:AchievementPageSize"],
                "Wonde:EmployeePageSize");
            var employeeBatchSize = GetPositiveConfigurationValue(
                config["Wonde:EmployeeBatchSize"] ?? config["Wonde:AchievementBatchSize"],
                "Wonde:EmployeeBatchSize");

            var classesPageSize = GetPositiveConfigurationValue(
                config["Wonde:ClassesPageSize"] ?? config["Wonde:AchievementPageSize"],
                "Wonde:ClassesPageSize");
            var classesBatchSize = GetPositiveConfigurationValue(
                config["Wonde:ClassesBatchSize"] ?? config["Wonde:AchievementBatchSize"],
                "Wonde:ClassesBatchSize");

            var subjectsPageSize = GetPositiveConfigurationValue(
                config["Wonde:SubjectsPageSize"] ?? config["Wonde:AchievementPageSize"],
                "Wonde:SubjectsPageSize");
            var subjectsBatchSize = GetPositiveConfigurationValue(
                config["Wonde:SubjectsBatchSize"] ?? config["Wonde:AchievementBatchSize"],
                "Wonde:SubjectsBatchSize");

            var academicYear = config["Wonde:StudentAcademicYear"];
            if (string.IsNullOrWhiteSpace(academicYear))
            {
                throw new InvalidOperationException("Wonde:StudentAcademicYear is missing");
            }

            var studentPageSize = GetPositiveConfigurationValue(config["Wonde:StudentPageSize"], "Wonde:StudentPageSize");
            var studentBatchSize = GetPositiveConfigurationValue(config["Wonde:StudentBatchSize"], "Wonde:StudentBatchSize");


            // Generate a unique request ID for this import session
            requestId = Guid.NewGuid().ToString();


            await svc.LogTextAsync("INFO", $"Import Started:", cts.Token).ConfigureAwait(false);

            Boolean.TryParse(config["Wonde:ImportSchools"], out var importSchoolsFlag);
            if (importSchoolsFlag)
            {
                await svc.LogTextAsync("INFO", $"Schools import started:", cts.Token).ConfigureAwait(false);
                Console.WriteLine("Schools import started.");
                await svc.GetSchoolsAsync(requestId, cts.Token);
                Console.WriteLine("Schools import completed.");
            }

            Boolean.TryParse(config["Wonde:ImportAchievement"], out var importAchievementFlag);
            if (importAchievementFlag)
            {
                await svc.LogTextAsync("INFO", $"Achievements import started:", cts.Token).ConfigureAwait(false);
                Console.WriteLine("Achievements import started.");
                await svc.GetAchievementsAsync(schoolIds, universalUpdatedAfterDate, fallbackStartDate, pageSize, batchSize, requestId, cts.Token);
                Console.WriteLine("Achievements import completed.");
            }

            Boolean.TryParse(config["Wonde:ImportBehaviour"], out var importBehaviourFlag);
            if (importBehaviourFlag)
            {
                await svc.LogTextAsync("INFO", $"Behaviours import started:", cts.Token).ConfigureAwait(false);
                Console.WriteLine("Behaviours import started.");
                await svc.GetBehavioursAsync(schoolIds, universalUpdatedAfterDate, fallbackStartDate, pageSize, batchSize, requestId, cts.Token);
                Console.WriteLine("Behaviours import completed.");
            }
            Boolean.TryParse(config["Wonde:ImportStudents"], out var importStudentsFlag);
            if (importStudentsFlag)
            {
                await svc.LogTextAsync("INFO", $"Student import started:", cts.Token).ConfigureAwait(false);
                Console.WriteLine("Students import started.");
                await svc.GetStudentsAsync(schoolIds, academicYear, studentPageSize, studentBatchSize, universalUpdatedAfterDate, fallbackStartDate, requestId, cts.Token);
                Console.WriteLine("Students import completed.");
            }

            Boolean.TryParse(config["Wonde:ImportStudentLeavers"], out var importStudentLeaversFlag);
            if (importStudentLeaversFlag)
            {
                await svc.LogTextAsync("INFO", $"Student leavers import started:", cts.Token).ConfigureAwait(false);
                Console.WriteLine("Student leavers import started.");
                await svc.GetStudentLeaversAsync(schoolIds, academicYear, studentPageSize, studentBatchSize, universalUpdatedAfterDate, fallbackStartDate, requestId, cts.Token);
                Console.WriteLine("Student leavers import completed.");
            }

            Boolean.TryParse(config["Wonde:ImportExclusions"], out var importExclusionsFlag);
            if (importExclusionsFlag)
            {
                await svc.LogTextAsync("INFO", $"Exclusions import started:", cts.Token).ConfigureAwait(false);
                Console.WriteLine("Exclusions import started.");
                await svc.GetExclusionsAsync(schoolIds, universalUpdatedAfterDate, fallbackStartDate, exclusionsPageSize, exclusionsBatchSize, requestId, cts.Token);
                Console.WriteLine("Exclusions import completed.");
            }

            Boolean.TryParse(config["Wonde:ImportGroups"], out var importGroupsFlag);
            if (importGroupsFlag)
            {
                await svc.LogTextAsync("INFO", $"Groups import started:", cts.Token).ConfigureAwait(false);
                Console.WriteLine("Groups import started.");
                await svc.GetGroupsAsync(schoolIds, universalUpdatedAfterDate, fallbackStartDate, groupsPageSize, groupsBatchSize, requestId, cts.Token);
                Console.WriteLine("Groups import completed.");
            }

            Boolean.TryParse(config["Wonde:ImportClasses"], out var importClassesFlag);
            if (importClassesFlag)
            {
                await svc.LogTextAsync("INFO", $"Classes import started:", cts.Token).ConfigureAwait(false);
                Console.WriteLine("Classes import started.");
                await svc.GetClassesAsync(schoolIds, universalUpdatedAfterDate, fallbackStartDate, classesPageSize, classesBatchSize, requestId, cts.Token);
                Console.WriteLine("Classes import completed.");
            }

            Boolean.TryParse(config["Wonde:ImportSubjects"], out var importSubjectsFlag);
            if (importSubjectsFlag)
            {
                await svc.LogTextAsync("INFO", $"Subjects import started:", cts.Token).ConfigureAwait(false);
                Console.WriteLine("Subjects import started.");
                await svc.GetSubjectsAsync(schoolIds, universalUpdatedAfterDate, fallbackStartDate, subjectsPageSize, subjectsBatchSize, requestId, cts.Token);
                Console.WriteLine("Subjects import completed.");
            }

            Boolean.TryParse(config["Wonde:ImportEmployees"], out var importEmployeesFlag);
            if (importEmployeesFlag)
            {
                await svc.LogTextAsync("INFO", $"Employees import started:", cts.Token).ConfigureAwait(false);
                Console.WriteLine("Employees import started.");
                await svc.GetEmployeesAsync(schoolIds, universalUpdatedAfterDate, fallbackStartDate, employeePageSize, employeeBatchSize, requestId, cts.Token);
                Console.WriteLine("Employees import completed.");
            }

            Boolean.TryParse(config["Wonde:ImportAttendance"], out var importAttendanceFlag);
            if (importAttendanceFlag)
            {
                await svc.LogTextAsync("INFO", $"Attendance import started:", cts.Token).ConfigureAwait(false);
                Console.WriteLine("Attendance import started.");
                await svc.GetAttendanceSessionsAsync(schoolIds, universalUpdatedAfterDate, fallbackStartDate, attendancePageSize, attendanceBatchSize, requestId, cts.Token);
                Console.WriteLine("Attendance import completed.");
            }
            await svc.LogTextAsync("INFO", $"Import Finished:", cts.Token).ConfigureAwait(false);
        }
        catch (OperationCanceledException)
        {
            Console.WriteLine("Cancelled.");
            await AppendDailyLogAsync(config["Log:Path"], "WARN", $"Import run cancelled. request_id={requestId ?? "N/A"}").ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Unhandled import error: {ex.Message}");
            await AppendDailyLogAsync(config["Log:Path"], "ERROR", $"Unhandled import error. request_id={requestId ?? "N/A"}. {ex}").ConfigureAwait(false);
        }
    }

    private static async Task AppendDailyLogAsync(string? configuredLogsDirectory, string level, string message)
    {
        try
        {
            var logsDirectory = string.IsNullOrWhiteSpace(configuredLogsDirectory)
                ? Path.Combine(AppContext.BaseDirectory, "logs")
                : configuredLogsDirectory;
            Directory.CreateDirectory(logsDirectory);

            var logFilePath = Path.Combine(logsDirectory, $"{DateTime.UtcNow:yyyy-MM-dd}.txt");
            var line = $"{DateTime.UtcNow:O} [{level}] {message}{Environment.NewLine}";
            await File.AppendAllTextAsync(logFilePath, line).ConfigureAwait(false);
        }
        catch
        {
            // Logging should not terminate the app.
        }
    }

    private static int GetPositiveConfigurationValue(string? value, string key)
    {
        if (!int.TryParse(value, out var result) || result <= 0)
        {
            throw new InvalidOperationException($"{key} must be a positive integer");
        }

        return result;
    }

    private static int GetOptionalNonNegativeConfigurationValue(string? value, string key, int defaultValue)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return defaultValue;
        }

        if (!int.TryParse(value, out var result) || result < 0)
        {
            throw new InvalidOperationException($"{key} must be a non-negative integer");
        }

        return result;
    }
}