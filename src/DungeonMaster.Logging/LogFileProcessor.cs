using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Threading;

namespace DungeonMaster
{
    internal class LogFileProcessor : IDisposable
    {
        private const int _maxQueuedMessages = 1024;

        private readonly object _fileLock = new object();

        private readonly BlockingCollection<LogFileEntry> _entries = new BlockingCollection<LogFileEntry>(_maxQueuedMessages);

        private readonly Thread _myThread;

        private FileStream _stream;

        private StreamWriter _writer;

        public bool Disposed { get; private set; }

        private LogFileOptions Options { get; }

        private LogLevel HighestLogLevelLogged { get; set; } = LogLevel.Trace;

        private bool DeleteLogFileOnDispose
        {
            get
            {
                if (Options.IsFlightRecord && HighestLogLevelLogged < LogLevel.Warning)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public LogFileProcessor(LogFileOptions options)
        {
            Options = options ?? throw new ArgumentNullException(nameof(options));
            var filePath = Options.FilePath;
            var dir = Path.GetDirectoryName(filePath);
            Directory.CreateDirectory(dir);

            _stream = File.OpenWrite(Options.FilePath);
            _stream.Seek(0, SeekOrigin.End);
            _writer = new StreamWriter(_stream);
            _myThread = new Thread(ProcessLogQueue)
            {
                IsBackground = true,
                Name = "DungeonMaster Log File Processor"
            };
            _myThread.Start();
        }

        public void Log<TState>(string category, LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {

            if (string.IsNullOrWhiteSpace(category))
            {
                throw new InvalidOperationException("Categories for log file entries cannot be null or contain only whitespace characters.");
            }

            if (!IsEnabled(logLevel))
            {
                return;
            }

            if (formatter == null)
            {
                return;
            }

            if (logLevel > HighestLogLevelLogged)
            {
                HighestLogLevelLogged = logLevel;
            }

            var timestamp = (Options.UseSystemClockForTimestamps) ? DateTime.Now : DateTime.UtcNow;
            string msg = formatter(state, exception);
            if (string.IsNullOrWhiteSpace(msg))
            {
                return;
            }

            var entry = new LogFileEntry(timestamp, category, logLevel, eventId, msg);
            if (!_entries.IsAddingCompleted)
            {
                try
                {
                    _entries.Add(entry);
                }
                catch (InvalidOperationException) { }
            }
            else
            {
                CommitEntry(entry);
            }
        }

        public void Dispose()
        {
            if (!Disposed)
            {
                lock (_fileLock)
                {
                    if (!Disposed)
                    {
                        _stream.Flush();
                        _stream.Dispose();
                        _stream = null;
                        _writer = null;

                        _entries.CompleteAdding();
                        try
                        {
                            _myThread.Join(1500);
                        }
                        catch (ThreadStateException) { }

                        if (DeleteLogFileOnDispose)
                        {
                            File.Delete(Options.FilePath);
                        }
                        Disposed = true;
                    }
                }
            }
        }

        private void ProcessLogQueue()
        {
            try
            {
                foreach (var entry in _entries.GetConsumingEnumerable())
                {
                    CommitEntry(entry);
                }
            }
            catch
            {
                try
                {
                    _entries.CompleteAdding();
                }
                catch { }
            }
        }

        private void CommitEntry(LogFileEntry entry)
        {
            if (!Disposed)
            {
                lock (_fileLock)
                {
                    if (!Disposed)
                    {
                        string timestamp = entry.Timestamp.ToString("s");
                        string logLevelText = GetLogLevelText(entry.LogLevel);
                        var split = entry.Message.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
                        string msg = string.Join($"{Environment.NewLine}\t", split);
                        msg = $"{timestamp} -> {logLevelText} {entry.Category}[{entry.EventId}]:{Environment.NewLine}\t{msg}";
                        _writer.WriteLine(msg);
                        _writer.Flush();
                    }
                }
            }
        }

        private bool IsEnabled(LogLevel logLevel)
        {
            return (logLevel >= Options.MinimumLogLevel && logLevel != LogLevel.None);
        }

        private string GetLogLevelText(LogLevel level)
        {
            switch (level)
            {
                case LogLevel.Trace:
                    return "[trce]";
                case LogLevel.Debug:
                    return "[dbug]";
                case LogLevel.Information:
                    return "[info]";
                case LogLevel.Warning:
                    return "[warn]";
                case LogLevel.Error:
                    return "[fail]";
                case LogLevel.Critical:
                    return "[crit]";
                default:
                    return "[none]";
            }
        }

        private class LogFileEntry
        {
            public DateTime Timestamp { get; }

            public LogLevel LogLevel { get; }

            public EventId EventId { get; }

            public string Message { get; }

            public string Category { get; }

            public LogFileEntry(DateTime timestamp, string category, LogLevel logLevel, EventId eventId, string message)
            {
                Timestamp = timestamp;
                Category = category;
                LogLevel = logLevel;
                EventId = eventId;
                Message = message;
            }
        }
    }
}
