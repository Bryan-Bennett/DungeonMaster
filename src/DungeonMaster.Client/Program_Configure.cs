using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using Unity;

namespace DungeonMaster
{
    partial class Program
    {
        private void Configure(IUnityContainer container)
        {
            var loggers = container.AddLogging();
            loggers.AddLogFile(x =>
            {
                x.FilePath = @"logs\log.txt";
#if DEBUG
                x.MinimumLogLevel = LogLevel.Debug;
#endif
            });
            AddFlightRecord(loggers);
            Log = loggers.CreateLogger<Program>();
            InitializeLog();

            var discordClient = new DiscordSocketClient(new DiscordSocketConfig
            {
#if DEBUG
                LogLevel = Discord.LogSeverity.Debug,
#else
                LogLevel = Discord.LogSeverity.Info,
#endif
            });

            var commands = new CommandService(new CommandServiceConfig
            {
                CaseSensitiveCommands = false,
                DefaultRunMode = RunMode.Async,
#if DEBUG
                LogLevel = Discord.LogSeverity.Debug,
#else
                LogLevel = Discord.LogSeverity.Info,
#endif    
            });

            _ = commands.AddModulesAsync(Assembly.GetEntryAssembly(), container.ToServiceProvider());
            var tokenPath = Path.Combine(Assembly.GetEntryAssembly().GetAssemblyDirectory(), "token.txt");
            var fvi = FileVersionInfo.GetVersionInfo(Assembly.GetEntryAssembly().GetAssemblyPath());
            var version = fvi.ProductVersion;

            var settings = new DungeonMasterSettings()
            {
                Token = DiscordToken.FromFile(tokenPath),
                GameName = $"DungeonMaster v{version}"
            };

            container.RegisterInstance(settings);
            container.RegisterInstance(discordClient);
            container.RegisterInstance(commands);
        }

        [Conditional("DEBUG")]
        private void AddFlightRecord(ILoggerFactory loggers)
        {
            loggers.AddLogFile(x =>
            {
                x.FilePath = @"logs\flightrecord.txt";
                x.IsFlightRecord = true;
            });
        }

        private void InitializeLog()
        {
            var fvi = FileVersionInfo.GetVersionInfo(Assembly.GetEntryAssembly().GetAssemblyPath());
            Log.LogInformation($"DungeonMaster v{fvi.ProductVersion}");
        }
    }
}
