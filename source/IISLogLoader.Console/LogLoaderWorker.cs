using IISLogLoader.Config;
using IISLogLoader.Console.Models;
using IISLogLoader.Console.Services;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IISLogLoader.Console
{
    public interface ILogLoaderWorker
    {
        Task ExecuteAsync(UserConfig userConfig);
    }
    internal class LogLoaderWorker : ILogLoaderWorker
    {
        private readonly ILogger<LogLoaderWorker> _logger;
        private readonly ILogOrchestrationService _logOrchestrationService;

        public LogLoaderWorker(ILogger<LogLoaderWorker> logger, ILogOrchestrationService logOrchestrationService)
        {
            _logger = logger;
            _logOrchestrationService = logOrchestrationService;
        }

        public async Task ExecuteAsync(UserConfig userConfig)
        {
            _logger.LogDebug("Log loader worker service execution start");

            foreach (var logStore in userConfig.LogStores ?? Enumerable.Empty<LogStorageInfo>())
            {
                await _logOrchestrationService.ProcessLogs(logStore);
            }

            _logger.LogDebug("Log loader worker service execution end");
        }

    }
}
