using IISLogLoader.Common.IO;
using IISLogLoader.Config;
using IISLogLoader.Console.Models;
using IISLogLoader.Console.Repositories;
using IISLogLoader.Test.Common;
using NSubstitute;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace IISLogLoader.Console.Tests.Repositories
{
    [TestFixture]
    public class UserConfigRepositoryTests
    {

        private static string DefaultBaseFolder = AppContext.BaseDirectory;


        [Test]
        public void Load_FileExists_LoadsFromFile()
        {
            // setup
            string filePath = Path.Combine(DefaultBaseFolder, UserConfigRepository.DefaultFileName);
            UserConfig userConfig = new SubstituteBuilder<UserConfig>().WithRandomProperties().Build();
            string userConfigJson = JsonSerializer.Serialize(userConfig);

            IFileWrapper fileWrapper = new SubstituteBuilder<IFileWrapper>().Build();
            fileWrapper.Exists(filePath).Returns(true);
            fileWrapper.ReadAllText(filePath).Returns(userConfigJson);

            // execute
            IUserConfigRepository userConfigRepo = CreateUserConfigRepository(fileWrapper: fileWrapper);
            UserConfig result = userConfigRepo.Load();

            //assert
            fileWrapper.Received(1).Exists(filePath);
            fileWrapper.Received(1).ReadAllText(filePath);
            Assert.That(result.PollIntervalMinutes, Is.EqualTo(userConfig.PollIntervalMinutes));
        }

        [Test]
        public void Load_FileNotExists_LoadsDefaultUserConfig()
        {
            // setup
            IFileWrapper fileWrapper = new SubstituteBuilder<IFileWrapper>().Build();
            fileWrapper.Exists(Arg.Any<string>()).Returns(false);

            // execute
            IUserConfigRepository userConfigRepo = CreateUserConfigRepository(fileWrapper: fileWrapper);
            UserConfig result = userConfigRepo.Load();

            //assert
            fileWrapper.Received(1).Exists(Arg.Any<String>());
            fileWrapper.DidNotReceive().ReadAllText(Arg.Any<String>());
            Assert.That(result.PollIntervalMinutes, Is.EqualTo(UserConfig.DefaultPollIntervalMinutes));
        }

        private UserConfigRepository CreateUserConfigRepository(string? appBaseFolder = null, IFileWrapper? fileWrapper = null)
        {
            appBaseFolder ??= DefaultBaseFolder;
            fileWrapper ??= new SubstituteBuilder<IFileWrapper>().Build();
            return new UserConfigRepository(appBaseFolder, fileWrapper);
        }


    }
}
