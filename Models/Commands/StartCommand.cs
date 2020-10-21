using System;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using System.Net.Http;
using Newtonsoft.Json;
using System.Text;

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
            User user = new User() { ChatId = chatId };
            string baseUrl = string.Format(AppSettings.Url, "api/user/create");
            HttpClient client = new HttpClient();
            HttpResponseMessage res = await client.PostAsync(baseUrl, new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json"));
            HttpContent content = res.Content;
            
            await botClient.SendTextMessageAsync(chatId, "Hello and welcome to image-bot. Type '/' to see available commands.", parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown);
        }
    }
}
