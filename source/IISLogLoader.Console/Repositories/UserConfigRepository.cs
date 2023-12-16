using IISLogLoader.Common.IO;
using IISLogLoader.Config;
using IISLogLoader.Console.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace IISLogLoader.Console.Repositories
{
    public interface IUserConfigRepository
    {
        UserConfig Load();
    }

    public class UserConfigRepository : IUserConfigRepository
    {
        private readonly string _userConfigFilePath;
        private readonly IFileWrapper _fileWrapper;

        public const string DefaultFileName = "config.json";

        public UserConfigRepository(string appBaseFolder, IFileWrapper fileWrapper)
        {
            _userConfigFilePath = Path.Combine(appBaseFolder, DefaultFileName);
            _fileWrapper = fileWrapper;
        }

        public UserConfig Load()
        {
            UserConfig userConfig = new UserConfig();
            if (_fileWrapper.Exists(_userConfigFilePath))
            {
                string json = _fileWrapper.ReadAllText(_userConfigFilePath);
                userConfig = JsonSerializer.Deserialize<UserConfig>(json)!;
            }
            return userConfig;
        }

    }
}
