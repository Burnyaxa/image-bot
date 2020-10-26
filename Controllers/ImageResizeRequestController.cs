using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using image_bot.Models;
using Microsoft.EntityFrameworkCore;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
namespace image_bot.Controllers
{
    [Route("api/image-resize-requests")]
    [ApiController]
    public class ImageResizeRequestController : ControllerBase
    {
        public UsersState db;
        public Cloudinary cloudinary;
        public Account account;
        public ImageResizeRequestController(UsersState context)
        {
            db = context;
            account = new Account(AppSettings.CloudName, AppSettings.Cloudkey, AppSettings.CloudSecret);
            cloudinary = new Cloudinary(account);
        }

        [HttpPost]
        public async Task<IActionResult> Post(int userId)
        {
            BotUser user = new BotUser() { Id = userId };
            if (db.ImageResizeRequests.Any(b => b.UserId == user.Id)) return BadRequest("Cannot create an existing request.");
            ImageResizeRequest imageResizeRequest = new ImageResizeRequest() { UserId = user.Id };
            db.ImageResizeRequests.Add(imageResizeRequest);
            await db.SaveChangesAsync();
            string uri = String.Format(AppSettings.Url, "api/image-resize-requests/") + user.Id.ToString();
            return Created(uri, imageResizeRequest);
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
            ImageResizeRequest imageResizeRequest = db.ImageResizeRequests.Include(i => i.User).Where(i => i.UserId == user.Id).FirstOrDefault();
            if (imageResizeRequest == null)
            {
                return NotFound();
            }
            return new OkObjectResult(imageResizeRequest);
        }

        [Route("{userId}")]
        [HttpPut]
        public async Task<IActionResult> Put(int userId, [FromBody] ImageResizeRequest request)
        {
            if (db.ImageResizeRequests.Any(b => b.Id == request.Id))
            {
                db.Update(request);
                await db.SaveChangesAsync();
                return Ok();
            }
            db.ImageResizeRequests.Add(request);
            await db.SaveChangesAsync();
            string uri = String.Format(AppSettings.Url, "api/image-resize-requests/") + request.User.Id.ToString();
            return Created(uri, request);
        }

        [Route("{userId}")]
        [HttpDelete]
        public async Task<IActionResult> Delete(int userId)
        {
            ImageResizeRequest request = db.ImageResizeRequests.Where(i => i.UserId == userId).FirstOrDefault();
            if (request == null) return NotFound();
            db.ImageResizeRequests.Remove(request);
            await db.SaveChangesAsync();
            return NoContent();
        }

        [Route("create-request")]
        [HttpPost]
        public async Task<ActionResult> CreateRequest(long chatId)
        {
            BotUser botUser = db.BotUsers.Where(b => b.ChatId == chatId).First();
            botUser.CurentCommand = BotCommand.Resize;
            db.BotUsers.Update(botUser);
            ImageResizeRequest request = new ImageResizeRequest() { UserId = botUser.Id };
            if (db.ImageResizeRequests.Any(i => i.UserId == botUser.Id))
            {
                return BadRequest();
            }
            db.ImageResizeRequests.Add(request);
            await db.SaveChangesAsync();
            return Ok();
        }

        [Route("set-parameters")]
        [HttpPost]
        public async Task<IActionResult> SetParameters(long chatId, int height, int width)
        {
            BotUser user = db.BotUsers.Where(b => b.ChatId == chatId).First();
            ImageResizeRequest request = db.ImageResizeRequests.Include(u => u.User).Where(u => u.UserId == user.Id).First();
            request.Height = height;
            request.Width = width;
            request.Status = ImageResizeStatus.AwaitingImage;
            db.ImageResizeRequests.Update(request);
            await db.SaveChangesAsync();
            return Ok();
        }

        [Route("resize")]
        [HttpGet]
        public async Task<IActionResult> Resize(long chatId, string url)
        {
            BotUser user = db.BotUsers.Where(b => b.ChatId == chatId).First();
            ImageResizeRequest request = db.ImageResizeRequests.Include(u => u.User).Where(u => u.UserId == user.Id).First();
            ImageUploadParams uploadParams = new ImageUploadParams()
            {
                File = new FileDescription(url),
                Transformation = new Transformation().Width(request.Width).Height(request.Height).Crop("scale")
            };
            var uploadResult = await cloudinary.UploadAsync(uploadParams);
            return new OkObjectResult(uploadResult.Url);
        }

        [Route("delete-request")]
        [HttpDelete]
        public async Task<IActionResult> DeleteRequest(long chatId)
        {
            BotUser user = db.BotUsers.Where(b => b.ChatId == chatId).First();
            ImageResizeRequest request = db.ImageResizeRequests.Where(i => i.UserId == user.Id).First();
            if(!db.ImageResizeRequests.Any(r => r.Id == request.Id))
            {
                return BadRequest();
            }
            db.ImageResizeRequests.Remove(request);
            user.CurentCommand = BotCommand.Start;
            db.BotUsers.Update(user);
            await db.SaveChangesAsync();
            return Ok();
        }
    }
}
