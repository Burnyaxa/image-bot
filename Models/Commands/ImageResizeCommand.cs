using Microsoft.AspNetCore.WebUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace image_bot.Models.Commands
{
    public class ImageResizeCommand : Command
    {
        public override string Name => @"/resize";

        public override bool Contains(Message message)
        {
            if (message == null || message.Type != Telegram.Bot.Types.Enums.MessageType.Text)
                return false;

            return message.Text.Contains(this.Name);
        }

        public override async Task Execute(Message message, TelegramBotClient botClient)
        {
            var chatId = message.Chat.Id;
            HttpClient client = new HttpClient();
            string url = string.Format(AppSettings.Url, "api/user/get-status");
            var query = new Dictionary<string, string>
            {
                ["chatId"] = chatId.ToString()
            };

            var response = await client.GetAsync(QueryHelpers.AddQueryString(url, query));
            response.Content.Headers
        }
    }
}
