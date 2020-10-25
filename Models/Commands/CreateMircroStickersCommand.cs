using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace image_bot.Models.Commands
{
    public class CreateMircroStickersCommand : Command
    {
        public override string Name => @"/micro-stickers";

        public override bool Contains(Message message)
        {
            if (message == null || message.Type != Telegram.Bot.Types.Enums.MessageType.Text)
                return false;

            return message.Text.Contains(this.Name);
        }

        public override Task Execute(Message message, TelegramBotClient client)
        {
            throw new NotImplementedException();
        }
    }
}
