using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IISLogLoader.Console.Config
{
    public class AppConfig
    {
        public virtual string UserConfigFilePath
        {
            get
            {
                return Path.Combine(AppUtils.ApplicationBaseFolder, "UserSettings.json");
            }
        }
    }
}
