using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using System.Net.Http;
using Microsoft.AspNetCore.Mvc;

namespace image_bot.Models.Commands
{
    public class ApplyFilterCommand : Command
    {
        public override string Name => throw new NotImplementedException();

        public override bool Contains(Message message)
        {
            if (message == null || message.Type != Telegram.Bot.Types.Enums.MessageType.Text)
                return false;

            return message.Text.Contains(this.Name);
        }

        public override async Task Execute(Message message, TelegramBotClient botClient)
        {
            var chatId = message.Chat.Id;
            string baseUrl = string.Format(AppSettings.Url, "api/user/create");
            HttpClient client = new HttpClient();
            HttpResponseMessage res = await client.PostAsync(baseUrl, new StringContent(JsonConvert.SerializeObject(chatId), Encoding.UTF8, "application/json"));
            HttpContent content = res.Content;

            IActionResult result = ResponseMessage
            content.    
        }
    }
}
