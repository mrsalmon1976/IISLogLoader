using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace IISLogLoader.Test.Common.TestAssets
{
    public class TestAssets
    {
        public static string LogFile {  get { return "IISLogLoader.Test.Common.TestAssets.LogFile.log"; } }

        public static string InvalidLogFile { get { return "IISLogLoader.Test.Common.TestAssets.InvalidLogFile.log"; } }

        public static string ReadResource(string resourceName)
        {
            Assembly? assembly = Assembly.GetAssembly(typeof(TestAssets));
            string? result = null;
            if (assembly != null)
            {

                Stream? resourceStream = assembly?.GetManifestResourceStream(resourceName);
                if (resourceStream != null)
                {
                    using (resourceStream)
                    {
                        using (StreamReader reader = new StreamReader(resourceStream))
                        {
                            result = reader.ReadToEnd();
                        }
                    }
                }
            }

            if (result == null)
            {
                throw new ArgumentException($"Resource {resourceName} does not exist in the current assembly");
            }
            return result;
        }
    }
}
