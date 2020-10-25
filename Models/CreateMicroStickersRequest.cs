using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace image_bot.Models
{
    public enum MicroStickersStatus
    {
        AwaitingName,
        AwaitingSticker
    }
    public class CreateMicroStickersRequest
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public BotUser User { get; set; }
        /// <summary>
        /// has default value inicial in UsersState
        /// </summary>
        public MicroStickersStatus Status { get; set; }
        public string Name { get; set; }
        }
}
