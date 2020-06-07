using System;
using System.Text.Json;

namespace focusify.Models
{
    public class AppSettings
    {
        private static readonly string settingsFile = System.Web.HttpContext.Current.Server.MapPath(@"\Resources\settings.json");

        public static AppSettings Get()
        {
            string configJson = System.IO.File.ReadAllText(settingsFile);

            return JsonSerializer.Deserialize<AppSettings>(configJson);
        }

        public string Focused { get; set; }
        public string NotFocused { get; set; }
    }
}