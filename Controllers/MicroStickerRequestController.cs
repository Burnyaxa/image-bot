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


        [HttpPost]
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


        [Route("{userId}")]
        [HttpGet]
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

        [Route("{userId}")]
        [HttpPut]
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

        [Route("{userId}")]
        [HttpDelete]
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
