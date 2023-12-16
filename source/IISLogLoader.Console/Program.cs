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
        // LogLoaderBackgroundWorker will run as a service
        logger.LogInformation("IISLogReader run started in service mode");
        host.Run();
    }
    else
    {
        // we've run as a console - just execute the LogLoaderWorker once
        logger.LogInformation("IISLogReader run started in console mode");

        LogLoaderConsoleWorker worker = host.Services.GetService<LogLoaderConsoleWorker>()!;
        worker.Run().GetAwaiter().GetResult();
    }
}
catch (Exception ex)
{
    logger!.LogCritical(ex, ex.Message);
    throw;
}

if (Debugger.IsAttached)
{
    Console.WriteLine();
    Console.WriteLine("Execution complete - hit enter to close the program.");
    Console.ReadLine();
}
Environment.Exit(0);