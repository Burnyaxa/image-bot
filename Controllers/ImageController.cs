using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using image_bot.Models;
using CloudinaryDotNet.Actions;
using CloudinaryDotNet;

namespace image_bot.Controllers
{
    [Route("api/image")]
    [ApiController]
    public class ImageController : ControllerBase
    {
        public Cloudinary cloudinary;
        public Account account;

        public ImageController()
        {
            account = new Account(AppSettings.CloudName, AppSettings.Cloudkey, AppSettings.CloudSecret);
            cloudinary = new Cloudinary(account);
        }

        [Route("resize")]
        [HttpPost]
        public async Task<IActionResult> Resize([FromBody] ImageToResize image)
        {

            ImageUploadParams uploadParams = new ImageUploadParams()
            {
                File = new FileDescription(image.Url),
                Transformation = new Transformation().Width(image.Width).Height(image.Height).Crop("scale")
            };
            var uploadResult = await cloudinary.UploadAsync(uploadParams);
            return Created(uploadResult.Url, uploadResult);
            return new OkObjectResult(uploadResult.Url);
        }
    }
}
