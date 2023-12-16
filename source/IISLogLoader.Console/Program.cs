using IISLogLoader.Config;
using IISLogLoader.Console;
using IISLogLoader.Console.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

string[] arguments = Environment.GetCommandLineArgs();

bool runAsService = (arguments.Where(x => x.ToLower().EndsWith("runasservice")).Count() > 0);
// runAsService = true;

IHost host = BootStrapper.CreateHost(runAsService, arguments);
var logger = host.Services.GetService<ILogger<Program>>()!;


try
{
    if (runAsService)
    {
        host.Run();
    }
    else
    {
        logger.LogInformation("IISLogReader run started in console mode");
        IUserConfigRepository userConfigRepo = host.Services.GetService<IUserConfigRepository>()!;
        ILogLoaderWorker logLoaderWorker = host.Services.GetService<ILogLoaderWorker>()!;
        await logLoaderWorker.ExecuteAsync(userConfigRepo.Load());
        logger.LogInformation("IISLogReader run complete");
        if (Debugger.IsAttached)
        {
            Console.WriteLine();
            Console.WriteLine("Execution complete - hit enter to close the program.");
            Console.ReadLine();
        }
    }
}
catch (Exception ex)
{
    logger!.LogCritical(ex, ex.Message);
    throw;
}

Environment.Exit(0);