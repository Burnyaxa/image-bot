using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace image_bot.Models
{
    public class ImageToFilter
    {
        public string Url { get; set; } 
        public AvailableFilters Filter { get; set; }
    }
}
