using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace image_bot.Models
{
    public enum ImageResizeStatus
    {
        None,
        AwaitingImage,
        AwaitingSize
    }
    public class ImageResizeRequest
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public BotUser User { get; set; }
        /// <summary>
        /// has default value inicial in UsersState
        /// </summary>
        public ImageResizeStatus Status { get; set; }
        public int Height { get; set; }
        public int Width { get; set; }
    }
}
