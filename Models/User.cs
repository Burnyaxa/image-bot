using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace image_bot.Models
{
    public enum Command
    {
        Resize,
        ApplyFilter,
        CreateMicroSticker
    }
    public class User
    {
        public int Id { get; set; }
        
        public int ChatId { get; set; }

        public Command CurentCommand { get; set; }

    }
}
