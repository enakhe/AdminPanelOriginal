#nullable disable

using Azure.Core;
using DeviceDetectorNET;
using UAParser;
using System.Web;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using IPinfo;
using IPinfo.Models;

namespace AdminPanel.Interface
{
    public interface IAuditLog
    {
        public string GetDeviceType(HttpContext context);
        public string GetOperatingSystem(HttpContext context);
        public string GetBrowserName(HttpContext context);
        public string GetBrowserVersion(HttpContext context);
        public string GetIpAddress(HttpContext context);
        public Task<string> GetContinent();
        public Task<string> GetCountryName();
        public Task<string> GetCountry();
        public Task<string> GetCity();
        public Task<string> GetState();

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

        public async Task<string> GetContinent()
        {
            string token = "37b3a341039091";
            IPinfoClient client = new IPinfoClient.Builder()
              .AccessToken(token)
              .Build();

            string ip = "102.211.164.2";

            IPResponse ipResponse = await client.IPApi.GetDetailsAsync(ip);

            return ipResponse.Continent.Name;
        }

        public async Task<string> GetCountry()
        {
            string token = "37b3a341039091";
            IPinfoClient client = new IPinfoClient.Builder()
              .AccessToken(token)
              .Build();

            string ip = "102.211.164.2";

            IPResponse ipResponse = await client.IPApi.GetDetailsAsync(ip);

            return ipResponse.Country;
        }

        public async Task<string> GetCountryName()
        {
            string token = "37b3a341039091";
            IPinfoClient client = new IPinfoClient.Builder()
              .AccessToken(token)
              .Build();

            string ip = "102.211.164.2";

            IPResponse ipResponse = await client.IPApi.GetDetailsAsync(ip);

            return ipResponse.CountryName;
        }

        public async Task<string> GetCity()
        {
            string token = "37b3a341039091";
            IPinfoClient client = new IPinfoClient.Builder()
              .AccessToken(token)
              .Build();

            string ip = "102.211.164.2";

            IPResponse ipResponse = await client.IPApi.GetDetailsAsync(ip);

            return ipResponse.City;
        }

        public async Task<string> GetState()
        {
            string token = "37b3a341039091";
            IPinfoClient client = new IPinfoClient.Builder()
              .AccessToken(token)
              .Build();

            string ip = "102.211.164.2";

            IPResponse ipResponse = await client.IPApi.GetDetailsAsync(ip);

            return ipResponse.Region;
        }
    }
}
