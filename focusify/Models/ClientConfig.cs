using System;
using System.Text.Json;

namespace focusify.Models
{
    public class ClientConfig
    {
        private static string configFile = System.Web.HttpContext.Current.Server.MapPath(@"\Resources\client.json");

        public static ClientConfig Get()
        {
            string configJson = System.IO.File.ReadAllText(configFile);

            Console.WriteLine(configJson);

            return JsonSerializer.Deserialize<ClientConfig>(configJson);
        }

        public string id { get; set; }
        public string secret { get; set; }
    }
}