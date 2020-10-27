using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using image_bot.Models;
namespace image_bot.Controllers
{
    [Route("api/sticker")]
    [ApiController]
    public class StickerController : ControllerBase
    {
        public Cloudinary cloudinary;
        public Account account;

        public StickerController()
        {
            account = new Account(AppSettings.CloudName, AppSettings.CloudKey, AppSettings.CloudSecret);
            cloudinary = new Cloudinary(account);
        }

        [Route("resize")]
        [HttpPost]
        public async Task<IActionResult> Resize([FromBody] StickerToResize sticker)
        {

            ImageUploadParams uploadParams = new ImageUploadParams()
            {
                File = new FileDescription(sticker.Url),
                Transformation = new Transformation().Width(sticker.Width).Height(sticker.Height).Crop("pad").Gravity("west").FetchFormat("png")
            };
            var uploadResult = await cloudinary.UploadAsync(uploadParams);
            return Created(uploadResult.Url, uploadResult);
        }
    }
}
