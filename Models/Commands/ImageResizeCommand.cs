using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.WebUtilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

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
            long chatId = message.Chat.Id;
            BotUser user;
            ImageResizeRequest request;
            string url, result;
            Dictionary<string, string> query;
            HttpResponseMessage response;
            HttpClient client = new HttpClient();
            ImageToResize image;

            url = string.Format(AppSettings.Url, "api/users/") + chatId.ToString();
            response = await client.GetAsync(url);
            result = await response.Content.ReadAsStringAsync();
            user = JsonConvert.DeserializeObject<BotUser>(result);

            url = string.Format(AppSettings.Url, "api/users/") + chatId.ToString() + "/requests";

            response = await client.GetAsync(url);
            if (!response.IsSuccessStatusCode)
            {
                url = string.Format(AppSettings.Url, "api/image-resize-requests");
                query = new Dictionary<string, string>()
                {
                    ["userId"] = user.Id.ToString()
                };
                await client.PostAsync(QueryHelpers.AddQueryString(url, query), null);

                user.CurentCommand = BotCommand.Resize;

                url = string.Format(AppSettings.Url, "api/users/") + chatId.ToString();
                await client.PutAsync(url, new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json"));

                await botClient.SendTextMessageAsync(chatId, "Please input parameters of the future image in format heightxwidth.", parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown);
                return;
            }

            result = await response.Content.ReadAsStringAsync();
            request = JsonConvert.DeserializeObject<ImageResizeRequest>(result);
            switch (request.Status)
            {
                case ImageResizeStatus.AwaitingSize:
                    string height = message.Text.Split(':')[0];
                    string width = message.Text.Split(':')[1];
                    request.Height = Convert.ToInt32(height);
                    request.Width = Convert.ToInt32(width);
                    request.Status = ImageResizeStatus.AwaitingImage;
                    url = string.Format(AppSettings.Url, "api/image-resize-requests/") + user.Id;
                    //var data = new Dictionary<string, string>()
                    //{
                    //    ["chatId"] = chatId.ToString(),
                    //    ["height"] = height,
                    //    ["width"] = width 
                    //};
                    await client.PutAsync(url, new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json"));
                    await botClient.SendTextMessageAsync(chatId, "Good. Now send me your image.", parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown);
                    break;
                case ImageResizeStatus.AwaitingImage:
                    var file = await botClient.GetFileAsync(message.Photo.LastOrDefault()?.FileId);
                    string baseUrl = string.Format("https://api.telegram.org/file/bot{0}/{1}", AppSettings.Key, file.FilePath);
                    url = string.Format(AppSettings.Url, "api/image/resize");
                    image = new ImageToResize()
                    {
                        Url = baseUrl,
                        Width = (int)request.Width,
                        Height = (int)request.Height
                    };
                    //query = new Dictionary<string, string>
                    //{
                    //    ["chatId"] = chatId.ToString(),
                    //    ["url"] = baseUrl
                    //};
                    response = await client.PostAsync(url, new StringContent(JsonConvert.SerializeObject(image), Encoding.UTF8, "application/json"));
                    result = await response.Content.ReadAsStringAsync();
                    ImageUploadResult imageUrl = JsonConvert.DeserializeObject<ImageUploadResult>(result);
                    await botClient.SendPhotoAsync(chatId, imageUrl.Url.ToString());
                    url = string.Format(AppSettings.Url, "api/image-resize-requests/") + user.Id;
                    //query = new Dictionary<string, string>
                    //{
                    //    ["chatId"] = chatId.ToString(),
                    //};
                    await client.DeleteAsync(url);
                    user.CurentCommand = BotCommand.Start;

                    url = string.Format(AppSettings.Url, "api/users/") + chatId.ToString();
                    await client.PutAsync(url, new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json"));
                    break;                       
            }
        }
    }
}
