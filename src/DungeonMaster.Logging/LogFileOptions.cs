using Microsoft.Extensions.Logging;
using System;

namespace DungeonMaster
{
    public sealed class LogFileOptions
    {
        public LogLevel MinimumLogLevel { get; set; } = LogLevel.Information;

        public string FilePath { get; set; }

        public bool IsFlightRecord { get; set; }

        public bool UseSystemClockForTimestamps { get; set; }

        internal void Validate()
        {
            if (string.IsNullOrWhiteSpace(FilePath))
            {
                throw new InvalidOperationException("File paths cannot be null or whitespace and must be a valid file path.");
            }
        }
    }
}
