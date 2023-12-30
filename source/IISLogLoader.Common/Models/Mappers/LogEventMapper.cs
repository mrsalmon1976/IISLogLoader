using IISLogLoader.Common.Utils;
using IISLogParser;
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

        public static LogEvent MapFromIISLogEvent(string filePath, IISLogEvent iisEvent)
        {
            LogEvent logEvent = new LogEvent()
            {
                FilePath = filePath,
                DateTime = iisEvent.DateTimeEvent,
                ClientIpAddress = iisEvent.cIp,
                BytesReceived = ConversionUtils.TryGetLong(iisEvent.csBytes),
                Cookie = iisEvent.csCookie,
                Host = iisEvent.csHost,
                Method = iisEvent.csMethod,
                Referrer = iisEvent.csReferer,
                UriQuery = iisEvent.csUriQuery,
                UriStem = iisEvent.csUriStem,
                UserAgent = iisEvent.csUserAgent,
                UserName = iisEvent.csUsername,
                ProtocolVersion = iisEvent.csVersion,
                ServerName = iisEvent.sComputername,
                ServerIpAddress = iisEvent.sIp,
                ServerPort = iisEvent.sPort,
                SiteName = iisEvent.sSitename,
                BytesSent = ConversionUtils.TryGetLong(iisEvent.scBytes),
                StatusCode = iisEvent.scStatus,
                ProtocolSubstatus = (iisEvent.scSubstatus == null ? "" : iisEvent.scWin32Status.ToString()),
                WindowsStatusCode = (iisEvent.scWin32Status == null ? "" : iisEvent.scWin32Status.ToString()),
                TimeTaken = iisEvent.timeTaken
            };

            return logEvent;
        }

        public static IEnumerable<LogEvent> MapFromW3CEvents(string filePath, IEnumerable<W3CEvent> iisEvents)
        {
            return iisEvents.Select(x => MapFromW3CEvent(filePath, x));
        }

        public static IEnumerable<LogEvent> MapFromIISLogEvents(string filePath, IEnumerable<IISLogEvent> iisEvents)
        {
            return iisEvents.Select(x => MapFromIISLogEvent(filePath, x));
        }


    }
}
