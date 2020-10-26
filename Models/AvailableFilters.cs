using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace image_bot.Models
{
    public enum AvailableFilters
    {
        negate, 
        auto_brightness, 
        brightness_hsb, 
        sepia, 
        grayscale, 
        blackwhite, 
        colorize,
        hue,
        assist_colorblind, 
        simulate_colorblind,
        tint,
        contrast, 
        auto_contrast,  
        auto_color
    }
}
