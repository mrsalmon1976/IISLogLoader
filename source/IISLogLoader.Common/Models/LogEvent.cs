using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;

namespace IISLogLoader.Common.Models
{
    public class LogEvent
    {

        public string? FilePath { get; set; }

        /// <summary>
        /// The date/time the event was logged (date + time)
        /// </summary>
        public DateTime DateTime { get; set; }

        /// <summary>
        /// The IP address of the client that made the request (c-ip)
        /// </summary>
        public string? ClientIpAddress { get; set; }

        /// <summary>
        /// The number of bytes the server received (cs-bytes)
        /// </summary>
        public long? BytesReceived { get; set; }

        /// <summary>
        /// The content of the cookie sent or received if any (cs(Cookie))
        /// </summary>
        public string? Cookie { get; set; }

        /// <summary>
        /// The host header name, if any (cs_host).
        /// </summary>
        public string? Host { get; set; }

        /// <summary>
        /// The requested action, for example, a GET method (cs-method).
        /// </summary>
        public string? Method { get; set; }

        /// <summary>
        /// The site that the user last visited. This site provided a link to the current site (cs(Referrer)).
        /// </summary>
        public string? Referrer { get; set; }

        /// <summary>
        /// The query, if any that the client was trying to perform. A Universal Resource Identifier (URI) query is necessary 
        /// only for dynamic pages (cs-uri-query).
        /// </summary>
        public string? UriQuery { get; set; }

        /// <summary>
        /// The target of the action, for example, Default.htm (cs-uri-stem).
        /// </summary>
        public string? UriStem { get; set; }

        /// <summary>
        /// The browser type that the client used (cs(User-Agent)).
        /// </summary>
        public string? UserAgent { get; set; }

        /// <summary>
        /// The name of the authenticated user who accessed your server.Anonymous users are indicated by 
        /// a hyphen (cs-username).
        /// </summary>
        public string? UserName { get; set; }

        /// <summary>
        /// The protocol version —HTTP or FTP —that the client used (cs-version).
        /// </summary>
        public string? ProtocolVersion { get; set; }

        /// <summary>
        /// The name of the server on which the log file entry was generated (s-computername).
        /// </summary>
        public string? ServerName { get; set; }

        /// <summary>
        /// The IP address of the server on which the log file entry was generated (s-ip)
        /// </summary>
        public string? ServerIpAddress { get; set; }

        /// <summary>
        /// The server port number that is configured for the service. (s-port).
        /// </summary>
        public int? ServerPort { get; set; }

        /// <summary>
        /// The Internet service name and instance number that was running on the client. (s-sitename)
        /// </summary>
        public string? SiteName { get; set; }

        /// <summary>
        /// The number of bytes that the server sent (sc-bytes).
        /// </summary>
        public long? BytesSent { get; set; }

        /// <summary>
        /// The HTTP status code (sc-status).
        /// </summary>
        public int? StatusCode { get; set; }

        /// <summary>
        /// The sub status error code (sc-substatus)
        /// </summary>
        public string? ProtocolSubstatus { get; set; }

        /// <summary>
        /// The Windows status code. (sc-win32-status)
        /// </summary>
        public string? WindowsStatusCode { get; set; }

        /// <summary>
        /// The length of time that the action took, in milliseconds.
        /// </summary>
        public int? TimeTaken { get; set; }

        public int GetHashValue()
        {
            return new 
            {
                DateTime,
                ClientIpAddress,
                BytesReceived,
                Cookie,
                Host,
                Method,
                Referrer,
                UriQuery,
                UriStem,
                UserAgent,
                UserName,
                ProtocolVersion,
                ServerName,
                ServerIpAddress,
                ServerPort,
                SiteName,
                BytesSent,
                StatusCode,
                ProtocolSubstatus,
                WindowsStatusCode,
                TimeTaken
            }.GetHashCode();
        }
    }
}
