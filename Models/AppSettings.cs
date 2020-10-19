using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json;
using Newtonsoft.Json.Linq;

namespace image_bot.Models
{
    public class AppSettings
    {
        //public static string Url { get; } = JObject.Parse("botSettings.json")["url"].ToString();
        public static string Url { get; } = "https://e00530a975c7.ngrok.io/{0}";
        public static string Name { get; } = "image-bot";
        public static string Key { get; } = "1321181687:AAEXE9VNGEmwnoOy8zGkZz-tu6ZX-yAn17k";
        //public static string Key { get; } = JObject.Parse("botSettings.json")["key"].ToString();
    }
}
