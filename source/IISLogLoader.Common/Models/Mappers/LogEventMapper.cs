using IISLogLoader.Common.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tx.Windows;

namespace IISLogLoader.Common.Models.Mappers
{
    public class LogEventMapper
    {
        public static LogEvent MapFromW3CEvent(string filePath, W3CEvent iisEvent) 
        {
            LogEvent logEvent = new LogEvent()
            {
                FilePath = filePath,
                DateTime = iisEvent.dateTime,
                ClientIpAddress = iisEvent.c_ip,
                BytesReceived = ConversionUtils.TryGetLong(iisEvent.cs_bytes),
                Cookie = iisEvent.cs_Cookie,
                Host = iisEvent.cs_host,
                Method = iisEvent.cs_method,
                Referrer = iisEvent.cs_Referer,
                UriQuery = iisEvent.cs_uri_query,
                UriStem = iisEvent.cs_uri_stem,
                UserAgent = iisEvent.cs_User_Agent,
                UserName = iisEvent.cs_username,
                ProtocolVersion = iisEvent.cs_version,
                ServerName = iisEvent.s_computername,
                ServerIpAddress = iisEvent.s_ip,
                ServerPort = ConversionUtils.TryGetInt(iisEvent.s_port),
                SiteName = iisEvent.s_sitename,
                BytesSent = ConversionUtils.TryGetLong(iisEvent.sc_bytes),
                StatusCode = ConversionUtils.TryGetInt(iisEvent.sc_status),
                ProtocolSubstatus = iisEvent.sc_substatus,
                WindowsStatusCode = iisEvent.sc_win32_status,
                TimeTaken = ConversionUtils.TryGetInt(iisEvent.time_taken)
            };
            
            return logEvent;
        }

        public static IEnumerable<LogEvent> MapFromW3CEvents(string filePath, IEnumerable<W3CEvent> iisEvents)
        {
            return iisEvents.Select(x => MapFromW3CEvent(filePath, x));
        }

       
    }
}
