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
using Microsoft.AspNetCore.Http;

namespace image_bot.Controllers
{
    [Route("api/1.0.0/filter")]
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

        /// <summary>
        /// Filters image and uploads it to the cloud storage
        /// </summary>
        /// <param name="image"></param>
        /// <response code="201">Returns the newly created image with the filter applied</response>
        /// <response code="400">If the client puts invalid data into the request</response>
        [Route("apply")]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(ImageUploadResult))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ApplyFilter([FromBody] ImageToFilter image)
        {
            // upload image to cloud
            ImageUploadParams uploadParams = new ImageUploadParams()
            {
                File = new FileDescription(image.Url),
                Transformation = new Transformation()
                    .Effect("art:" + image.Filter.ToString())
            };
            // image uploaded
            // uploadResult has url
            var uploadResult = await cloudinary.UploadAsync(uploadParams);
            return Created(uploadResult.Url, uploadResult);
        }
    }
}
