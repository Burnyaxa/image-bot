using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace image_bot.Models
{
    public class StickerToResize
    {
        public string Url { get; set; }
        public int Height { get; set; }
        public int Width { get; set; }
    }
}
