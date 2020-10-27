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
    [Route("api/1.0.0/apply-filter-requests")]
    [ApiController]
    [Produces("application/json")]
    public class ApplyFilterRequestController : Controller
    {
        public UsersState db;
        public Cloudinary cloudinary;
        public Account account;
        public ApplyFilterRequestController(UsersState context)
        {
            db = context;
            account = new Account(AppSettings.CloudName, AppSettings.CloudKey, AppSettings.CloudSecret);
            cloudinary = new Cloudinary(account);
        }

        /// <summary>
        /// Creates a new ApplyFilterrequest
        /// </summary>
        /// <param name="userId">Id of the user that made the request.</param>
        /// <response code="201">Returns the newly created ApplyFilterRequest</response>
        /// <response code="400">If the client puts invalid data into the request</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(ImageUploadResult))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Post(int userId)
        {
            BotUser user = new BotUser() { Id = userId };
            if (db.ApplyFilterRequests.Any(b => b.UserId == user.Id)) return BadRequest("Cannot create an existing request.");
            ApplyFilterRequest applyFilterRequest = new ApplyFilterRequest() { UserId = user.Id };
            db.ApplyFilterRequests.Add(applyFilterRequest);
            await db.SaveChangesAsync();
            string uri = String.Format(AppSettings.Url, "api/1.0.0/apply-filter-requests/") + user.Id.ToString();
            return Created(uri, applyFilterRequest);
        }

        /// <summary>
        /// Get an Apply Filter Request by UserId
        /// </summary>
        /// <param name="userId"></param>
        /// <response code="200">Returns the ApplyFilter Request</response>
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
            ApplyFilterRequest applyFilterRequest = db.ApplyFilterRequests.Include(i => i.User).Where(i => i.UserId == user.Id).FirstOrDefault();
            if (applyFilterRequest == null)
            {
                return NotFound();
            }
            return new OkObjectResult(applyFilterRequest);
        }

        /// <summary>
        /// Create or Update an Image Resize Request by UserId
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="request">A new version of request to put</param>
        /// <response code="200">Updates the Apply Filter Request</response>
        /// <response code="201">Returns a new Apply Filter Request</response>
        /// <response code="409">If update is impossible</response>"
        [Route("{userId}")]
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(ImageResizeRequest))]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Put(int userId, [FromBody] ApplyFilterRequest request)
        {
            if (db.ApplyFilterRequests.Any(b => b.UserId == userId))
            {
                db.Update(request);
                await db.SaveChangesAsync();
                return Ok();
            }
            db.ApplyFilterRequests.Add(request);
            await db.SaveChangesAsync();
            string uri = String.Format(AppSettings.Url, "api/1.0.0/apply-filter-requests/") + request.User.Id.ToString();
            return Created(uri, request);
        }

        /// <summary>
        /// Delete Apply Filter Request by UserId
        /// </summary>
        /// <param name="userId"></param>
        /// <response code="204">Deletes the Apply Filter Request</response>
        /// <response code="404">If the client puts non-existing userId into the request</response>
        [Route("{userId}")]
        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int userId)
        {
            ApplyFilterRequest request = db.ApplyFilterRequests.Where(i => i.UserId == userId).FirstOrDefault();
            if (request == null) return NotFound();
            db.ApplyFilterRequests.Remove(request);
            await db.SaveChangesAsync();
            return NoContent();
        }

    }
}
