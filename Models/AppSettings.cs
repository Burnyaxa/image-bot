using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json;
using Newtonsoft.Json.Linq;
using System.IO;

namespace image_bot.Models
{
    public static class AppSettings
    {
        static AppSettings()
        {
            using (StreamReader r = new StreamReader(@"Properties\botSettings.json"))
            { 
                string json = r.ReadToEnd();
                Url = JObject.Parse(json)["url"].ToString();
                Name = JObject.Parse(json)["name"].ToString();
                Key = JObject.Parse(json)["key"].ToString();
                SQLServerConnection = JObject.Parse(json)["sqlserverConnection"].ToString();
                CloudName = JObject.Parse(json)["cloudinary_name"].ToString();
                CloudKey = JObject.Parse(json)["cloudinary_api_key"].ToString();
                CloudSecret = JObject.Parse(json)["cloudinary_api_secret"].ToString();
            }
        }
        public static string Url { get; }
        public static string Name { get; }
        public static string Key { get; }
        public static string SQLServerConnection { get; }
        public static string CloudName { get; }
        public static string CloudKey { get; }
        public static string CloudSecret { get; }
    }
}
