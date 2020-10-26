using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using System.Net.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Newtonsoft.Json;
using Telegram.Bot.Types.ReplyMarkups;


namespace image_bot.Models.Commands
{
    public class ApplyFilterCommand : Command
    {
        public override string Name => @"/filter";

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

            // if there's no current request for this user
            // create new filter request for this user
            var response = await client.GetAsync(QueryHelpers.AddQueryString(url, query));
            if (!response.IsSuccessStatusCode)
            {
                url = string.Format(AppSettings.Url, "api/filter/create-request");
                await client.PostAsync(QueryHelpers.AddQueryString(url, query), null);
                var rkm = createKeyboard();
             
                await botClient.SendTextMessageAsync(chatId, "Please input your preferred filter.", parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown, false, false, 0, rkm);
                return;
            }
            string result = await response.Content.ReadAsStringAsync();
            ApplyFilterStus status = JsonConvert.DeserializeObject<ApplyFilterStus>(result);
            switch (status)
            {
                case ApplyFilterStus.AwaitingFilterSelect:
                    url = string.Format(AppSettings.Url, "api/filter/choose");
                    var data = new Dictionary<string, string>()
                    {
                        ["chatId"] = chatId.ToString(),
                        ["requestedFilter"] = message.Text
                    };
                    await client.PostAsync(QueryHelpers.AddQueryString(url, data), null);
                    await botClient.SendTextMessageAsync(chatId, "Good. Now send me your image.", replyMarkup: new ReplyKeyboardRemove(), parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown);

                    break;
                case ApplyFilterStus.AwaitingImage:
                    var file = await botClient.GetFileAsync(message.Photo.LastOrDefault()?.FileId);
                    string baseUrl = string.Format("https://api.telegram.org/file/bot{0}/{1}", AppSettings.Key, file.FilePath);
                    url = string.Format(AppSettings.Url, "api/filter/apply");
                    query = new Dictionary<string, string>
                    {
                        ["chatId"] = chatId.ToString(),
                        ["url"] = baseUrl
                    };
                    response = await client.PostAsync(QueryHelpers.AddQueryString(url, query), null);
                    result = await response.Content.ReadAsStringAsync();
                    string imageUrl = JsonConvert.DeserializeObject<string>(result);
                    await botClient.SendPhotoAsync(chatId, imageUrl);
                    url = string.Format(AppSettings.Url, "api/filter/delete-request");
                    query = new Dictionary<string, string>
                    {
                        ["chatId"] = chatId.ToString(),
                    };
                    await client.DeleteAsync(QueryHelpers.AddQueryString(url, query));
                    break;
            }
        }
        
        private ReplyKeyboardMarkup createKeyboard()
        {
            var rkm = new ReplyKeyboardMarkup();
            rkm.OneTimeKeyboard = true;
            KeyboardButton[] kewboardRow = new KeyboardButton[3];
            var rows = new List<KeyboardButton[]>();
            var cols = new List<KeyboardButton>();
            AvailableFilters[] filters = (AvailableFilters[])Enum.GetValues(typeof(AvailableFilters));


            rows.Add(new KeyboardButton[] { new KeyboardButton("view gallery") });
            for (int i = 0; i < filters.Length; i++)
            {
                cols.Add(new KeyboardButton(filters[i].ToString()));
                if (i % 3 == 0 && i != 0)
                {
                    rows.Add(cols.ToArray());
                    cols = new List<KeyboardButton>();
                }
            }
            rkm.Keyboard = rows.ToArray();
            
            return rkm;
        }
    }
    
}
