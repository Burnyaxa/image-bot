using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using image_bot.Models;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;

namespace image_bot.Controllers
{
    [Route("api/micro-sticker")]
    [ApiController]
    public class MicroStickerController : ControllerBase
    {
        public UsersState db;
        public Cloudinary cloudinary;
        public Account account;

        public MicroStickerController(UsersState context)
        {
            db = context;
            account = new Account(AppSettings.CloudName, AppSettings.CloudKey, AppSettings.CloudSecret);
            cloudinary = new Cloudinary(account);
        }

        [Route("create-request")]
        [HttpPost]
        public async Task<ActionResult> CreateRequest(long chatId)
        {
            BotUser botUser = db.BotUsers.Where(b => b.ChatId == chatId).First();
            botUser.CurentCommand = BotCommand.CreateMicroStickers;
            db.BotUsers.Update(botUser);
            CreateMicroStickersRequest request = new CreateMicroStickersRequest() { UserId = botUser.Id };
            if (db.ImageResizeRequests.Any(i => i.UserId == botUser.Id))
            {
                return BadRequest();
            }
            db.CreateMicroStickersRequests.Add(request);
            await db.SaveChangesAsync();
            return Ok();
        }



    }
}
