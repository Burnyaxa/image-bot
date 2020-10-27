using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using image_bot.Models;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.EntityFrameworkCore;

namespace image_bot.Controllers
{
    [Route("api/micro-sticker-requests")]
    [ApiController]
    public class MicroStickerRequestController : ControllerBase
    {
        public UsersState db;

        private const int _stickerWidth = 512;
        private const int _stickerHeight = 100;
        public MicroStickerRequestController(UsersState context)
        {
            db = context;
        }


        [HttpPost]
        public async Task<IActionResult> Post(int userId)
        {
            BotUser user = new BotUser() { Id = userId };
            if (db.CreateMicroStickersRequests.Any(b => b.UserId == user.Id)) return BadRequest("Cannot create an existing request.");
            CreateMicroStickersRequest request = new CreateMicroStickersRequest() { UserId = user.Id };
            db.CreateMicroStickersRequests.Add(request);
            await db.SaveChangesAsync();
            string uri = String.Format(AppSettings.Url, "api/micro-sticker-requests/") + user.Id.ToString();
            return Created(uri, request);
        }


        [Route("{userId}")]
        [HttpGet]
        public IActionResult GetByUserId(int userId)
        {
            BotUser user = db.BotUsers.Where(b => b.Id == userId).FirstOrDefault();
            if (user == null)
            {
                return NotFound();
            }
            CreateMicroStickersRequest request = db.CreateMicroStickersRequests.Include(i => i.User).Where(i => i.UserId == user.Id).FirstOrDefault();
            if (request == null)
            {
                return NotFound();
            }
            return new OkObjectResult(request);
        }

        [Route("{userId}")]
        [HttpPut]
        public async Task<IActionResult> Put(int userId, [FromBody] CreateMicroStickersRequest request)
        {
            if (db.CreateMicroStickersRequests.Any(b => b.UserId == userId))
            {
                db.Entry(request).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                await db.SaveChangesAsync();
                return Ok();
            }
            db.CreateMicroStickersRequests.Add(request);
            await db.SaveChangesAsync();
            string uri = String.Format(AppSettings.Url, "api/micro-sticker-requests/") + request.User.Id.ToString();
            return Created(uri, request);
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

        [Route("name")]
        [HttpPost]
        public async Task<IActionResult> Name(long chatId, string name)
        {
            BotUser user = db.BotUsers.Where(b => b.ChatId == chatId).First();
            CreateMicroStickersRequest request = db.CreateMicroStickersRequests.Include(u => u.User).Where(u => u.UserId == user.Id).First();
            request.Name = name;
            request.Status = MicroStickersStatus.AwaitingSticker;
            db.CreateMicroStickersRequests.Update(request);
            await db.SaveChangesAsync();
            return Ok();
        }

        [Route("name")]
        [HttpGet]
        public IActionResult Name(long chatId)
        {
            BotUser user = db.BotUsers.Where(b => b.ChatId == chatId).First();
            CreateMicroStickersRequest request = db.CreateMicroStickersRequests.Include(u => u.User).Where(u => u.UserId == user.Id).First();
            return new OkObjectResult(request.Name);
        }

        [Route("create-sticker")]
        [HttpGet]
        public async Task<IActionResult> CreateSticker(string url)
        {
            ImageUploadParams uploadParams = new ImageUploadParams()
            {
                File = new FileDescription(url),
                Transformation = new Transformation().Width(_stickerWidth).Height(_stickerHeight).Crop("pad").Gravity("west").FetchFormat("png")
            };
            var uploadResult = await cloudinary.UploadAsync(uploadParams);
            return new OkObjectResult(uploadResult.Url);
        }

        [Route("delete-request")]
        [HttpDelete]
        public async Task<IActionResult> DeleteRequest(long chatId)
        {
            BotUser user = db.BotUsers.Where(b => b.ChatId == chatId).First();
            CreateMicroStickersRequest request = db.CreateMicroStickersRequests.Where(i => i.UserId == user.Id).First();
            if (!db.CreateMicroStickersRequests.Any(r => r.Id == request.Id))
            {
                return BadRequest();
            }
            db.CreateMicroStickersRequests.Remove(request);
            user.CurentCommand = BotCommand.Start;
            db.BotUsers.Update(user);
            await db.SaveChangesAsync();
            return Ok();
        }
    }
}
