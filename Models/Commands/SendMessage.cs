using Telegram.Bot.Types.ReplyMarkups;

namespace image_bot.Models.Commands
{
    internal class SendMessage
    {
        private object id;
        private string v;

        public SendMessage(object id, string v)
        {
            this.id = id;
            this.v = v;
        }

        public ReplyKeyboardRemove ReplyMarkup { get; set; }
    }
}