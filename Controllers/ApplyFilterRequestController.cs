using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using image_bot.Models;
using Microsoft.EntityFrameworkCore;
using CloudinaryDotNet;
using image_bot.Models;
using CloudinaryDotNet.Actions;

namespace image_bot.Controllers
{
    [Route("api/apply-filter-requests")]
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

        [HttpPost]
        public async Task<IActionResult> Post(int userId)
        {
            BotUser user = new BotUser() { Id = userId };
            if (db.ApplyFilterRequests.Any(b => b.UserId == user.Id)) return BadRequest("Cannot create an existing request.");
            ApplyFilterRequest applyFilterRequest = new ApplyFilterRequest() { UserId = user.Id };
            db.ApplyFilterRequests.Add(applyFilterRequest);
            await db.SaveChangesAsync();
            string uri = String.Format(AppSettings.Url, "api/apply-filter-requests/") + user.Id.ToString();
            return Created(uri, applyFilterRequest);
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
            ApplyFilterRequest applyFilterRequest = db.ApplyFilterRequests.Include(i => i.User).Where(i => i.UserId == user.Id).FirstOrDefault();
            if (applyFilterRequest == null)
            {
                return NotFound();
            }
            return new OkObjectResult(applyFilterRequest);
        }

        [Route("{userId}")]
        [HttpPut]
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
            string uri = String.Format(AppSettings.Url, "api/apply-filter-requests/") + request.User.Id.ToString();
            return Created(uri, request);
        }

        [Route("{userId}")]
        [HttpDelete]
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
