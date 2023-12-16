using IISLogLoader.Config;
using IISLogLoader.Console.Repositories;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IISLogLoader.Console
{
    public class LogLoaderBackgroundWorker : BackgroundService
    {
        private readonly ILogger<LogLoaderBackgroundWorker> _logger;
        private readonly ILogLoaderWorker _logLoaderWorker;
        private readonly IUserConfigRepository _userConfigRepo;

        public LogLoaderBackgroundWorker(ILogger<LogLoaderBackgroundWorker> logger, ILogLoaderWorker logLoaderWorker, IUserConfigRepository userConfigRepo)
        {
            _logger = logger;
            _logLoaderWorker = logLoaderWorker;
            _userConfigRepo = userConfigRepo;
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            UserConfig userConfig = _userConfigRepo.Load();
            while (!cancellationToken.IsCancellationRequested)
            {
                _logger.LogDebug("Running log loader worker");
                await _logLoaderWorker.ExecuteAsync(userConfig);
                _logger.LogDebug($"Invoking task delay of {userConfig.PollIntervalMinutes} minutes");
                await Task.Delay(userConfig.PollIntervalMinutes * 1000 * 60, cancellationToken);
            }
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Background worker service started.");
            base.StartAsync(cancellationToken);
            return Task.CompletedTask;
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            base.StopAsync(cancellationToken);
            _logger.LogInformation("Background worker ervice stopped.");
            return Task.CompletedTask;
        }
    }
}
