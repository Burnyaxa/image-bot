using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using image_bot.Models;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.EntityFrameworkCore;

namespace image_bot.Controllers
{
    [Route("api/micro-sticker-requests")]
    [ApiController]
    
    public class MicroStickerRequestController : ControllerBase
    {
        public UsersState db;

        public MicroStickerRequestController(UsersState context)
        {
            db = context;
        }


        /// <summary>
        /// Create a new Micro Sticker Request
        /// </summary>
        /// <param name="userId"></param>
        /// <response code="201">Returns the newly created micro sticker request</response>
        /// <response code="400">If the client puts invalid data into the request</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(CreateMicroStickersRequest))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Post(int userId)
        {
            BotUser user = new BotUser() { Id = userId };
            if (db.CreateMicroStickersRequests.Any(b => b.UserId == user.Id)) return BadRequest("Cannot create an existing request.");
            CreateMicroStickersRequest request = new CreateMicroStickersRequest() { UserId = user.Id };
            db.CreateMicroStickersRequests.Add(request);
            await db.SaveChangesAsync();
            string uri = String.Format(AppSettings.Url, "api/micro-sticker-requests/") + user.Id.ToString();
            return Created(uri, request);
        }


        /// <summary>
        /// Get an Micro Sticker Request by UserId
        /// </summary>
        /// <param name="userId"></param>
        /// <response code="200">Returns the micro sticker request</response>
        /// <response code="404">If the client puts non-existing userId into the request</response>
        [Route("{userId}")]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CreateMicroStickersRequest))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetByUserId(int userId)
        {
            BotUser user = db.BotUsers.Where(b => b.Id == userId).FirstOrDefault();
            if (user == null)
            {
                return NotFound();
            }
            CreateMicroStickersRequest request = db.CreateMicroStickersRequests.Include(i => i.User).Where(i => i.UserId == user.Id).FirstOrDefault();
            if (request == null)
            {
                return NotFound();
            }
            return new OkObjectResult(request);
        }


        /// <summary>
        /// Create or Update a Micro Sticker Request by UserId
        /// </summary>
        /// <param name="userId"></param>
        /// <response code="200">Updates the micro sticker request</response>
        /// <response code="201">Returns a new micro sticker Request</response>
        /// <response code="409">If update is impossible</response>"
        [Route("{userId}")]
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(CreateMicroStickersRequest))]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Put(int userId, [FromBody] CreateMicroStickersRequest request)
        {
            if (db.CreateMicroStickersRequests.Any(b => b.UserId == userId))
            {
                db.Entry(request).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                await db.SaveChangesAsync();
                return Ok();
            }
            db.CreateMicroStickersRequests.Add(request);
            await db.SaveChangesAsync();
            string uri = String.Format(AppSettings.Url, "api/micro-sticker-requests/") + request.User.Id.ToString();
            return Created(uri, request);
        }



        /// <summary>
        /// Delete Micro Sticker Request by UserId
        /// </summary>
        /// <param name="userId"></param>
        /// <response code="204">Deletes the micro sticker request</response>
        /// <response code="404">If the client puts non-existing userId into the request</response>
        [Route("{userId}")]
        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int userId)
        {
            CreateMicroStickersRequest request = db.CreateMicroStickersRequests.Where(i => i.UserId == userId).FirstOrDefault();
            if (request == null) return NotFound();
            db.CreateMicroStickersRequests.Remove(request);
            await db.SaveChangesAsync();
            return NoContent();
        }
    }
}
