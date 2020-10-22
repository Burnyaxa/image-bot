using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace image_bot.Models
{
    public enum ApplyFilterStus
    {
        AwaitingImage,
        AwaitingFilterSelect
    }
    public class ApplyFilterRequest
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public BotUser User { get; set; }
        /// <summary>
        /// has default value inicial in UsersState
        /// </summary>
        public ApplyFilterStus Status { get; set; }
        public AvailableFilters? ChosenFilter { get; set; }
    }
}
