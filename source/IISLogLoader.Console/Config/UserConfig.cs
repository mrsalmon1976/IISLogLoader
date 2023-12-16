using IISLogLoader.Console.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace IISLogLoader.Config
{
    public class UserConfig
    {
        public const int DefaultPollIntervalMinutes = 60;

        public int PollIntervalMinutes { get; set; } = DefaultPollIntervalMinutes;

        public LogStorageInfo[]? LogStores { get; set; }

    }
}
