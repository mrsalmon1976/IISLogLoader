using IISLogLoader.Common;
using IISLogLoader.Common.Data;
using IISLogLoader.Console.Factories;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IISLogLoader.Console.Tests.Factories
{
    [TestFixture]
    public class LogDbContextFactoryTests
    {
        [Test]
        public void GetDbContext_SqlServer_ReturnsValidDbContext()
        {
            // execute
            ILogDbContextFactory factory = new LogDbContextFactory();
            SqlServerLogDbContext dbContext = factory.GetDbContext(LogLoaderDbType.SqlServer, String.Empty, String.Empty) as SqlServerLogDbContext;            
            
            // assert
            Assert.That(dbContext, Is.Not.Null);
        }

        [Test]
        public void GetDbContext_AllTypes_ReturnsValidDbContext()
        {
            // execute
            ILogDbContextFactory factory = new LogDbContextFactory();
            foreach (string dbType in LogLoaderDbType.AllTypes)
            {
                ILogDbContext dbContext = factory.GetDbContext(dbType, String.Empty, String.Empty);
                Assert.That(dbContext, Is.Not.Null);    
            }
        }

        [Test]
        public void GetDbContext_UnsupportedDbType_ThrowsException()
        {
            // setup
            ILogDbContextFactory factory = new LogDbContextFactory();
            TestDelegate testDelegate = new TestDelegate(() => factory.GetDbContext("TEST", String.Empty, String.Empty));

            // assert
            Assert.That(testDelegate, Throws.Exception.TypeOf<NotSupportedException>());

        }

    }
}
