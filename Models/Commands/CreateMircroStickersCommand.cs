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

namespace image_bot.Models.Commands
{
    public class CreateMircroStickersCommand : Command
    {
        private const int _stickerHeight = 100;
        private const int _strickerWidth = 512;
        public override string Name => @"/micro-stickers";

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
            CreateMicroStickersRequest request;
            string url, result;
            Dictionary<string, string> query;
            HttpResponseMessage response;
            HttpClient client = new HttpClient();
            StickerToResize stickerToResize;

            client = new HttpClient();
            url = string.Format(AppSettings.Url, "api/users/") + chatId.ToString();
            response = await client.GetAsync(url);
            result = await response.Content.ReadAsStringAsync();
            user = JsonConvert.DeserializeObject<BotUser>(result);

            url = string.Format(AppSettings.Url, "api/users/") + chatId.ToString() + "/requests";

            response = await client.GetAsync(url);
            if (!response.IsSuccessStatusCode)
            {
                url = string.Format(AppSettings.Url, "api/micro-sticker-requests");
                query = new Dictionary<string, string>()
                {
                    ["userId"] = user.Id.ToString()
                };
                await client.PostAsync(QueryHelpers.AddQueryString(url, query), null);

                user.CurentCommand = BotCommand.CreateMicroStickers;

                url = string.Format(AppSettings.Url, "api/users/") + chatId.ToString();
                await client.PutAsync(url, new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json"));

                await botClient.SendTextMessageAsync(chatId, "Please input name of the future sticker pack.", parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown);
                return;
            }

            result = await response.Content.ReadAsStringAsync();
            request = JsonConvert.DeserializeObject<CreateMicroStickersRequest>(result);

            switch (request.Status)
            {
                case MicroStickersStatus.AwaitingName:
                    string name = message.Text;
                    request.Name = name;
                    request.Status = MicroStickersStatus.AwaitingSticker;
                    url = string.Format(AppSettings.Url, "api/micro-sticker-requests/") + user.Id;
                    await client.PutAsync(url, new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json"));

                    await botClient.SendTextMessageAsync(chatId, "Good. Now send me a sticker.", parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown);
                    break;
                case MicroStickersStatus.AwaitingSticker:
                    Sticker updateSticker = message.Sticker;
                    StickerSet stickerSet = await botClient.GetStickerSetAsync(updateSticker.SetName);
                    List<string> stickerImages = new List<string>();
                    List<string> emojis = new List<string>();
                    foreach (var sticker in stickerSet.Stickers) {
                        var file = await botClient.GetFileAsync(sticker.FileId);
                        string baseUrl = string.Format("https://api.telegram.org/file/bot{0}/{1}", AppSettings.Key, file.FilePath);
                        url = string.Format(AppSettings.Url, "api/sticker/resize");
                        stickerToResize = new StickerToResize()
                        {
                            Url = baseUrl,
                            Width = _strickerWidth,
                            Height = _stickerHeight
                        };

                        response = await client.PostAsync(url, new StringContent(JsonConvert.SerializeObject(stickerToResize), Encoding.UTF8, "application/json"));
                        result = await response.Content.ReadAsStringAsync();
                        ImageUploadResult imageUrl = JsonConvert.DeserializeObject<ImageUploadResult>(result);

                        stickerImages.Add(imageUrl.Url.ToString());
                        emojis.Add(sticker.Emoji);
                    }

                    int userId = message.From.Id;
                    string stickerPackName = request.Name;
                    string shortStickerPackName = stickerPackName.ToLower().Replace(' ', '_') + "_by_burnyaxa_bot";
                    await botClient.CreateNewStickerSetAsync(userId, shortStickerPackName, stickerPackName, stickerImages.First(), emojis.First());
                    stickerImages.RemoveAt(0);
                    emojis.RemoveAt(0);
                    if (stickerImages.Count() != 0 && emojis.Count() != 0) {
                        var stickerData = stickerImages.Zip(emojis, (s, e) => new { Image = s, Emoji = e });
                        foreach(var element in stickerData)
                        {
                            await botClient.AddStickerToSetAsync(userId, shortStickerPackName, element.Image, element.Emoji);
                        }
                    }
                    StickerSet newStickers = await botClient.GetStickerSetAsync(shortStickerPackName);
                    await botClient.SendTextMessageAsync(chatId, "Enjoy your micro-stickers :)", parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown);
                    await botClient.SendStickerAsync(chatId, newStickers.Stickers.First().FileId);

                    url = string.Format(AppSettings.Url, "api/micro-sticker-requests/") + user.Id;

                    await client.DeleteAsync(url);
                    user.CurentCommand = BotCommand.Start;

                    url = string.Format(AppSettings.Url, "api/users/") + chatId.ToString();
                    await client.PutAsync(url, new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json"));

                    break;
            }
        }
    }
}