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
        public static string Url { get; } = JObject.Parse("Properties/botSettings.json")["url"].ToString();
        public static string Name { get; } = "image-bot";
        public static string Key { get; } = JObject.Parse("Properties/botSettings.json")["key"].ToString();
    }
}
