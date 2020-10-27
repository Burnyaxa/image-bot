using System;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using System.Net.Http;
using Newtonsoft.Json;
using System.Text;
using System.Collections.Generic;
using Microsoft.AspNetCore.WebUtilities;

namespace image_bot.Models.Commands
{
    public class StartCommand : Command
    {
        public override string Name => @"/start";

        public override bool Contains(Message message)
        {
            if (message == null || message.Type != Telegram.Bot.Types.Enums.MessageType.Text)
                return false;

            return message.Text.Contains(this.Name);
        }

        public override async Task Execute(Message message, TelegramBotClient botClient)
        {
            var chatId = message.Chat.Id;
            string baseUrl = string.Format(AppSettings.Url, "api/users");
            var query = new Dictionary<string, string>
            {
                ["chatId"] = chatId.ToString()
            };
            HttpClient client = new HttpClient();
            HttpResponseMessage res = await client.PostAsync(QueryHelpers.AddQueryString(baseUrl, query), null);
            await botClient.SendTextMessageAsync(chatId, "Hello and welcome to image-bot. Type '/' to see available commands.", parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown);
        }
    }
}
