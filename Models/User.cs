using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace image_bot.Models
{
    /// <summary>
    /// The command of the bot that user is currently in
    /// </summary>
    public enum BotCommand
    {
        Start,
        Resize,
        ApplyFilter,
        CreateMicroSticker
    }
    /// <summary>
    /// Info of current bot user
    /// Used to keep track of his state
    /// </summary>
    public class User
    {
        public int Id { get; set; }
        
        public int ChatId { get; set; }

        /// <summary>
        /// Command that is currently used
        /// !!!!! has a default value !!!!!
        /// !!!!! which is start      !!!!!
        /// </summary>
        public BotCommand CurentCommand { get; set; }

        public List<ApplyFilterRequest> ApplyFilterRequests { get; set; }

    }
}
