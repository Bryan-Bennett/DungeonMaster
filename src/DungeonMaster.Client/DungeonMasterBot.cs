using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DungeonMaster
{
    class DungeonMasterBot
    {
        private ILogger<DungeonMasterBot> Log { get; }

        private ILogger DiscordLogger { get; }

        private DiscordSocketClient DiscordClient { get; }

        private CommandService Commands { get; }

        private DungeonMasterSettings Settings { get; }

        public DungeonMasterBot(DungeonMasterSettings settings, ILoggerFactory loggers, DiscordSocketClient discordClient, CommandService commands)
        {
            Settings = settings;
            Log = loggers.CreateLogger<DungeonMasterBot>();
            DiscordLogger = loggers.CreateLogger(discordClient.GetType().FullName);
            DiscordClient = discordClient;
            Commands = commands;
            DiscordClient.Ready += Client_Ready;
            DiscordClient.MessageReceived += Client_MessageReceived;
            DiscordClient.Log += Client_Log;
        }

        public async Task RunAsync()
        {
            await DiscordClient.LoginAsync(TokenType.Bot, Settings.Token);
            await DiscordClient.StartAsync();
            await Task.Run(() =>
            {
                Console.WriteLine("Press any key to exit...");
                Console.ReadKey(true);
            });
        }

        private async Task Client_Ready()
        {
            await DiscordClient.SetGameAsync(Settings.GameName);
        }

        private async Task Client_MessageReceived(SocketMessage message)
        {
            //TODO
        }

        private async Task Client_Log(LogMessage arg)
        {
            await Task.Run(() =>
            {
                var logLevel = arg.Severity.ToLogLevel();
                var msg = arg.Message;
                var source = arg.Source;
                var ex = arg.Exception;
                msg = $"[{source}] {msg}";
                DiscordLogger.Log(logLevel, new EventId(0), msg, ex);
            });
        }
    }
}
