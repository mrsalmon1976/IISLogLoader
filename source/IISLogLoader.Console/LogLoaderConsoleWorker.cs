using IISLogLoader.Config;
using IISLogLoader.Console.Repositories;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IISLogLoader.Console
{
    public class LogLoaderConsoleWorker
    {
        private readonly ILogger<LogLoaderConsoleWorker> _logger;
        private readonly IUserConfigRepository _userConfigRepo;
        private readonly ILogLoaderWorker _logLoaderWorker;

        public LogLoaderConsoleWorker(ILogger<LogLoaderConsoleWorker> logger, IUserConfigRepository userConfigRepo, ILogLoaderWorker logLoaderWorker) 
        {
            _logger = logger;
            _userConfigRepo = userConfigRepo;
            _logLoaderWorker = logLoaderWorker;
        }
        
        public async Task Run()
        {
            UserConfig userConfig = _userConfigRepo.Load();
            await _logLoaderWorker.ExecuteAsync(userConfig);
            _logger.LogInformation("IISLogReader run complete");
        }
    }
}
