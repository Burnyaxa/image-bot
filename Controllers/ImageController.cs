using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using image_bot.Models;
using CloudinaryDotNet.Actions;
using CloudinaryDotNet;
using Swashbuckle.AspNetCore.Annotations;
using System.Net;

namespace image_bot.Controllers
{
    [Route("api/1.0.0/image")]
    [ApiController]
    [Produces("application/json")]
    public class ImageController : ControllerBase
    {
        public Cloudinary cloudinary;
        public Account account;

        public ImageController()
        {
            account = new Account(AppSettings.CloudName, AppSettings.CloudKey, AppSettings.CloudSecret);
            cloudinary = new Cloudinary(account);
        }

        /// <summary>
        /// Resizes the image and uploads it to the cloud storage
        /// </summary>
        /// <param name="image"></param>
        /// <response code="201">Returns the newly created image</response>
        /// <response code="400">If the client puts invalid data into the request</response>
        [Route("resize")]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(ImageUploadResult))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Resize([FromBody] ImageToResize image)
        {

            ImageUploadParams uploadParams = new ImageUploadParams()
            {
                File = new FileDescription(image.Url),
                Transformation = new Transformation().Width(image.Width).Height(image.Height).Crop("scale")
            };
            var uploadResult = await cloudinary.UploadAsync(uploadParams);
            return Created(uploadResult.Url, uploadResult);
        }
    }
}
