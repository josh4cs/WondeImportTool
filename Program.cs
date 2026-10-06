using WondeImportTool.Tools;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Globalization;
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

                services.AddSingleton(new WondeImportService(apiKey, conn));
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

        try
        {
            var schoolIds = config.GetSection("Wonde:AchievementSchoolIds")
                .GetChildren()
                .Select(section => section.Value)
                .Where(value => !string.IsNullOrWhiteSpace(value))
                .Select(value => value!)
                .ToArray();

            if (schoolIds.Length == 0)
            {
                throw new InvalidOperationException("Wonde:AchievementSchoolIds must contain at least one school ID");
            }

            if (!DateTime.TryParse(
                    config["Wonde:AchievementStartDate"],
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal,
                    out var achievementStartDate))
            {
                throw new InvalidOperationException("Wonde:AchievementStartDate is missing or invalid");
            }

            var pageSize = GetPositiveConfigurationValue(config["Wonde:AchievementPageSize"], "Wonde:AchievementPageSize");
            var batchSize = GetPositiveConfigurationValue(config["Wonde:AchievementBatchSize"], "Wonde:AchievementBatchSize");

            Console.WriteLine("Achievements import started.");
            await svc.GetAchievementsAsync(schoolIds, achievementStartDate, pageSize, batchSize, cts.Token);
            Console.WriteLine("Achievements import completed.");
        }
        catch (OperationCanceledException)
        {
            Console.WriteLine("Cancelled.");
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
}