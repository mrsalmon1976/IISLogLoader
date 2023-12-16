using IISLogLoader.Console;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NUnit.Framework;

namespace IISLogLoader.Console.Tests
{
    [TestFixture]
    public class BootStrapperTests
    {
        [Test]
        public void CreateHost_RunAsServiceTrue_Verify()
        {
            // setup

            // execute
            IHost host = BootStrapper.CreateHost(true, null);
            ILogLoaderWorker logLoaderWorker = host.Services.GetService<ILogLoaderWorker>()!;

            // assert
            Assert.That(logLoaderWorker, Is.Not.Null); 
        }

        [Test]
        public void CreateHostBuilder_RunAsServiceTrue_Verify()
        {
            // setup

            // execute
            IHost host = BootStrapper.CreateHost(false, null);
            ILogLoaderWorker logLoaderWorker = host.Services.GetService<ILogLoaderWorker>()!;

            // assert
            Assert.That(logLoaderWorker, Is.Not.Null);
        }

    }
}