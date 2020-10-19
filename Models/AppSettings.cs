﻿using System;
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
            }
        }
        public static string Url { get; }
        public static string Name { get; }
        public static string Key { get; }
    }
}