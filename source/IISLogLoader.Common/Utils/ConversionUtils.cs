using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IISLogLoader.Common.Utils
{
    public class ConversionUtils
    {
        public static int? TryGetInt(string val)
        {
            int result;
            if (Int32.TryParse(val, out result))
            {
                return result;
            }
            return null;
        }

        public static long? TryGetLong(string val)
        {
            long result;
            if (Int64.TryParse(val, out result)) 
            {
                return result;
            }
            return null;
        }

        public static long? TryGetLong(int? val)
        {
            if (val == null)
            {
                return null;
            }
            return Convert.ToInt64(val);
        }

    }
}
