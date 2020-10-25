using Microsoft.AspNetCore.WebUtilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
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
            if (!response.IsSuccessStatusCode)
            {
                url = string.Format(AppSettings.Url, "api/micro-sticker/create-request");
                await client.PostAsync(QueryHelpers.AddQueryString(url, query), null);
                await botClient.SendTextMessageAsync(chatId, "Please input name of the future sticker pack.", parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown);
                return;
            }

            string result = await response.Content.ReadAsStringAsync();
            MicroStickersStatus status = JsonConvert.DeserializeObject<MicroStickersStatus>(result);

            switch (status)
            {
                case MicroStickersStatus.AwaitingName:
                    string name = message.Text;
                    url = string.Format(AppSettings.Url, "api/micro-sticker/set-name");
                    var data = new Dictionary<string, string>()
                    {
                        ["chatId"] = chatId.ToString(),
                        ["name"] = name
                    };
                    await client.PostAsync(QueryHelpers.AddQueryString(url, data), null);
                    await botClient.SendTextMessageAsync(chatId, "Good. Now send me a sticker.", parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown);
                    break;
            }
        }
    }
}