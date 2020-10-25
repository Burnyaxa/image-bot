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
                    url = string.Format(AppSettings.Url, "api/micro-sticker/name");
                    var data = new Dictionary<string, string>()
                    {
                        ["chatId"] = chatId.ToString(),
                        ["name"] = name
                    };
                    await client.PostAsync(QueryHelpers.AddQueryString(url, data), null);
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
                        url = string.Format(AppSettings.Url, "api/micro-sticker/create-sticker");
                        query = new Dictionary<string, string>
                        {
                            ["url"] = baseUrl
                        };
                       
                        response = await client.GetAsync(QueryHelpers.AddQueryString(url, query));
                        result = await response.Content.ReadAsStringAsync();

                        stickerImages.Add(JsonConvert.DeserializeObject<string>(result));
                        emojis.Add(sticker.Emoji);
                    }

                    url = string.Format(AppSettings.Url, "api/micro-sticker/name");
                    query = new Dictionary<string, string>
                    {
                        ["chatId"] = chatId.ToString()
                    };

                    response = await client.GetAsync(QueryHelpers.AddQueryString(url, query));
                    result = await response.Content.ReadAsStringAsync();
                    int userId = message.From.Id;
                    string stickerPackName = JsonConvert.DeserializeObject<string>(result);
                    string shortStickerPackName = stickerPackName.ToLower().Replace(' ', '_') + "_by_imagebot";
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

                    url = string.Format(AppSettings.Url, "api/micro-sticker/delete-request");
                    query = new Dictionary<string, string>
                    {
                        ["chatId"] = chatId.ToString()
                    };

                    response = await client.DeleteAsync(QueryHelpers.AddQueryString(url, query));

                    break;
            }
        }
    }
}