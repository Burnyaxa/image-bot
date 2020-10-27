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
    [Route("api/1.0.0/sticker")]
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

        /// <summary>
        /// Resizes the sticker image and uploads it to the cloud storage
        /// </summary>
        /// <param name="image"></param>
        /// <response code="201">Returns the newly created sticker image</response>
        /// <response code="400">If the client puts invalid data into the request</response>
        [Route("resize")]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(ImageUploadResult))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
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
