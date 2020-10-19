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
        public static string Url { get; } = "";
        public static string Name { get; } = "image-bot";
        public static string Key { get; } = "";
        //public static string Key { get; } = JObject.Parse("botSettings.json")["key"].ToString();
    }
}
