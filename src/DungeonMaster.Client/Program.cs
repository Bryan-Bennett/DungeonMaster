using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Unity;

namespace DungeonMaster
{
    partial class Program : IDisposable
    {
        public bool Running { get; private set; }

        private UnityContainer Container { get; } = new UnityContainer();

        private ILogger<Program> Log { get; set; }

        private Program()
        {
        }

        public async Task RunAsync()
        {
            if (Running)
            {
                throw new InvalidOperationException("The program is already running!");
            }
            Running = true;
            Configure(Container);
            var bot = Container.Resolve<DungeonMasterBot>();
            await bot.RunAsync();
        }

        public void Dispose()
        {
            Container.Dispose();
        }

        #region Main
        static void Main(string[] args)
        {
            using (var program = new Program())
            {
                program.RunAsync().GetAwaiter().GetResult();
            }
        }
        #endregion
    }
}
