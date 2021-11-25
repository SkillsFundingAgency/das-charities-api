using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;

namespace SFA.DAS.Charities.Domain
{
    public class PerformanceLogger : IDisposable
    {
        private readonly Stopwatch _sw;
        private readonly string _description;
        private readonly double _warningThreshold;
        private readonly ILogger _logger;

        public PerformanceLogger(string description, ILogger logger, long warningThresholdInSeconds = 30)
        {
            _description = description;
            _warningThreshold = TimeSpan.FromSeconds(warningThresholdInSeconds).TotalMilliseconds;
            _logger = logger;
            _sw = new Stopwatch();
            _sw.Start();
            logger.LogInformation("Started {performanceTarget}", description);
        }

        public long GetElapsedTimeInMilliseconds() => _sw.ElapsedMilliseconds;

        public void Stop() => _sw.Stop();

        public void Dispose()
        {
            if (!_sw.IsRunning) return; 

            _sw.Stop();

            var logLevel = _sw.ElapsedMilliseconds > _warningThreshold ? LogLevel.Warning : LogLevel.Information;

            _logger.Log(logLevel, "{performanceTarget} took {sw.ElapsedMilliseconds} ms to complete", _description, _sw.ElapsedMilliseconds);
        }
    }
}
