#nullable disable

using Azure.Core;
using DeviceDetectorNET;
using UAParser;
using System.Web;

namespace AdminPanel.Interface
{
    public interface IAuditLog
    {
        public string GetDeviceType(HttpContext context);
        public string GetOperatingSystem(HttpContext context);
        public string GetBrowserName(HttpContext context);
        public string GetBrowserVersion(HttpContext context);
        public string GetIpAddress(HttpContext context);
    }

    public class AuditLog : IAuditLog
    {
        public string GetDeviceType(HttpContext context) 
        {
            string userAgent = context.Request.Headers["User-Agent"];
            var device = new DeviceDetector(userAgent);
            device.Parse();

            return device.GetDeviceName();
        }

        public string GetOperatingSystem(HttpContext context) 
        {
            string userAgent = context.Request.Headers["User-Agent"];
            var device = new DeviceDetector(userAgent);
            device.Parse();

            return device.GetOs().ToString();
        }

        [Obsolete]
        public string GetBrowserName(HttpContext context)
        {
            string userAgent = context.Request.Headers["User-Agent"];
            var parser = Parser.GetDefault();
            ClientInfo clientInfo = parser.Parse(userAgent);

            return clientInfo?.UserAgent.Family;
        }

        [Obsolete]
        public string GetBrowserVersion(HttpContext context)
        {
            string userAgent = context.Request.Headers["User-Agent"];
            var parser = Parser.GetDefault();
            ClientInfo clientInfo = parser.Parse(userAgent);

            return clientInfo?.UserAgent.Major;
        }

        public string GetIpAddress(HttpContext context)
        {
            return context.Connection.RemoteIpAddress.ToString();
        }
    }
}
