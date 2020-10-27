using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using image_bot.Models;
using Microsoft.AspNetCore.Mvc;
//using Telegram.Bot.Types;
using CloudinaryDotNet.Actions;
using CloudinaryDotNet;
using Microsoft.EntityFrameworkCore;

namespace image_bot.Controllers
{
    [Route("api/filter")]
    [ApiController]
    [Produces("application/json")]
    public class FilterController : Controller
    {
        public Cloudinary cloudinary;
        public Account account;
        public FilterController()
        {
            account = new Account(AppSettings.CloudName, AppSettings.CloudKey, AppSettings.CloudSecret);
            cloudinary = new Cloudinary(account);
        }

        [Route("apply")]
        [HttpPost]
        public async Task<IActionResult> ApplyFilter([FromBody] ImageToFilter image)
        {
            // upload image to cloud
            ImageUploadParams uploadParams = new ImageUploadParams()
            {
                File = new FileDescription(image.Url),
                Transformation = new Transformation()
                    .Effect(image.Filter.ToString())
            };
            // image uploaded
            // uploadResult has url
            var uploadResult = await cloudinary.UploadAsync(uploadParams);
            return Created(uploadResult.Url, uploadResult);
        }
    }
}
