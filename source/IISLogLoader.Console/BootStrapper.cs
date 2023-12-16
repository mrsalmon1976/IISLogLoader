using IISLogLoader.Common.IO;
using IISLogLoader.Config;
using IISLogLoader.Console.Config;
using IISLogLoader.Console.Factories;
using IISLogLoader.Console.Repositories;
using IISLogLoader.Console.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;

namespace IISLogLoader.Console
{
    public class BootStrapper
    {
        public static IHost CreateHost(bool runAsService, string[]? args)
        {
            IHostBuilder builder = Host.CreateDefaultBuilder(args)
                .ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                    logging.AddNLog(new NLogProviderOptions { RemoveLoggerFactoryFilter = true } );
                })
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddTransient<ILogLoaderWorker, LogLoaderWorker>();

                    // config
                    services.AddSingleton<AppConfig>();
                    services.AddSingleton<UserConfig>();

                    // utility classes
                    services.AddSingleton<IFileWrapper, FileWrapper>();
                    services.AddSingleton<IDirectoryWrapper, DirectoryWrapper>();

                    // factories
                    services.AddSingleton<ILogDbContextFactory, LogDbContextFactory>();

                    // repositories
                    services.AddTransient<IJournalRepository>((s) => new JournalRepository(AppUtils.ApplicationBaseFolder, new FileWrapper(), new DirectoryWrapper())); 
                    services.AddTransient<IUserConfigRepository>((s) => new UserConfigRepository(AppUtils.ApplicationBaseFolder, new FileWrapper())); 

                    // services
                    services.AddTransient<IFileLoadService, FileLoadService>();
                    services.AddTransient<IFileImportService, FileImportService>();
                    services.AddTransient<ILogOrchestrationService, LogOrchestrationService>();


                    if (runAsService)
                    {
                        services.AddHostedService<LogLoaderBackgroundWorker>();
                    }
                    else
                    {
                        services.AddTransient<LogLoaderConsoleWorker>();
                    }
                });

            return builder.Build();
        }
    }
}
