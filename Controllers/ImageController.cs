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
    [Route("api/image")]
    [ApiController]
    public class ImageController : ControllerBase
    {
        public UsersState db;
        public Cloudinary cloudinary;
        public Account account;
        public ImageController(UsersState context)
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
            botUser.CurentCommand = BotCommand.Resize;
            db.BotUsers.Update(botUser);
            ImageResizeRequest request = new ImageResizeRequest() { UserId = botUser.Id };
            db.ImageResizeRequests.Add(request);
            await db.SaveChangesAsync();
            return Ok();
        }

        [Route("set-parameters")]
        [HttpPost]
        public async Task<IActionResult> SetParameters(BotUser user, int height, int width)
        {
            ImageResizeRequest request = db.ImageResizeRequests.Include(u => u.User).Where(u => u.UserId == user.Id).First();
            request.Height = height;
            request.Width = width;
            db.ImageResizeRequests.Update(request);
            await db.SaveChangesAsync();
            return Ok();
        }

        [Route("resize")]
        [HttpGet]
        public async Task<IActionResult> Resize(BotUser user, string url)
        {
            ImageResizeRequest request = db.ImageResizeRequests.Include(u => u.User).Where(u => u.UserId == user.Id).First();
            ImageUploadParams uploadParams = new ImageUploadParams()
            {
                File = new FileDescription(url),
                EagerTransforms = new List<Transformation>()
                {
                    new Transformation().Width(request.Width).Height(request.Height)
                }

            };
            var uploadResult = await cloudinary.UploadAsync(uploadParams);
            return new OkObjectResult(uploadResult.Url);
        }

        [Route("delete-request")]
        [HttpDelete]
        public async Task<IActionResult> DeleteRequest(ImageResizeRequest request)
        {
            if(!db.ImageResizeRequests.Any(r => r.Id == request.Id))
            {
                return BadRequest();
            }
            db.ImageResizeRequests.Remove(request);
            await db.SaveChangesAsync();
            return Ok();
        }
    }
}
