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
    [Route("api/1.0.0/image-resize-requests")]
    [ApiController]
    [Produces("application/json")]
    public class ImageResizeRequestController : ControllerBase
    {
        public UsersState db;
        public Cloudinary cloudinary;
        public Account account;
        public ImageResizeRequestController(UsersState context)
        {
            db = context;
            account = new Account(AppSettings.CloudName, AppSettings.CloudKey, AppSettings.CloudSecret);
            cloudinary = new Cloudinary(account);
        }

        /// <summary>
        /// Create a new Image Resize Request
        /// </summary>
        /// <param name="userId"></param>
        /// <response code="201">Returns the newly created image resize request</response>
        /// <response code="400">If the client puts invalid data into the request</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(ImageResizeRequest))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Post(int userId)
        {
            BotUser user = new BotUser() { Id = userId };
            if (db.ImageResizeRequests.Any(b => b.UserId == user.Id)) return BadRequest("Cannot create an existing request.");
            ImageResizeRequest imageResizeRequest = new ImageResizeRequest() { UserId = user.Id };
            db.ImageResizeRequests.Add(imageResizeRequest);
            await db.SaveChangesAsync();
            string uri = String.Format(AppSettings.Url, "api/1.0.0/image-resize-requests/") + user.Id.ToString();
            return Created(uri, imageResizeRequest);
        }

        /// <summary>
        /// Get an Image Resize Request by UserId
        /// </summary>
        /// <param name="userId"></param>
        /// <response code="200">Returns the image resize request</response>
        /// <response code="404">If the client puts non-existing userId into the request</response>
        [Route("{userId}")]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ImageResizeRequest))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
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

        /// <summary>
        /// Create or Update an Image Resize Request by UserId
        /// </summary>
        /// <param name="userId"></param>
        /// <response code="200">Updates the image resize request</response>
        /// <response code="201">Returns a new Image Resize Request</response>
        /// <response code="409">If update is impossible</response>"
        [Route("{userId}")]
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(ImageResizeRequest))]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Put(int userId, [FromBody] ImageResizeRequest request)
        {
            if (db.ImageResizeRequests.Any(b => b.UserId == userId))
            {
                db.Entry(request).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                await db.SaveChangesAsync();
                return Ok();
            }
            db.ImageResizeRequests.Add(request);
            await db.SaveChangesAsync();
            string uri = String.Format(AppSettings.Url, "api/1.0.0/image-resize-requests/") + request.User.Id.ToString();
            return Created(uri, request);
        }

        /// <summary>
        /// Delete Image Resize Request by UserId
        /// </summary>
        /// <param name="userId"></param>
        /// <response code="204">Deletes the image resize request</response>
        /// <response code="404">If the client puts non-existing userId into the request</response>
        [Route("{userId}")]
        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int userId)
        {
            ImageResizeRequest request = db.ImageResizeRequests.Where(i => i.UserId == userId).FirstOrDefault();
            if (request == null) return NotFound();
            db.ImageResizeRequests.Remove(request);
            await db.SaveChangesAsync();
            return NoContent();
        }
    }
}
