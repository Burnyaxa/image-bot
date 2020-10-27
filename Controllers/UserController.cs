using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using image_bot.Models;
using Newtonsoft.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace image_bot.Controllers
{
    [Route("api/1.0.0/users")]
    [ApiController]
    [Produces("application/json")]
    public class UserController : ControllerBase
    {
        public UsersState db;
        private readonly ILogger _logger;
        public UserController(UsersState context, ILogger<UserController> logger)
        {
            db = context;
            _logger = logger;
        }

        /// <summary>
        /// Create a new User
        /// </summary>
        /// <param name="chatId"></param>
        /// <response code="201">Returns the newly created user</response>
        /// <response code="400">If the client puts invalid data into the request</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(BotUser))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Post(long chatId)
        { 
            BotUser user = new BotUser() { ChatId = chatId };
            if (db.BotUsers.Any(b => b.ChatId == user.ChatId)) return BadRequest();
            db.BotUsers.Add(user);
            await db.SaveChangesAsync();
            string uri = String.Format(AppSettings.Url, "api/1.0.0/users/") + user.ChatId.ToString();
            return Created(uri, user);
        }


        /// <summary>
        /// Get a User by ChatId
        /// </summary>
        /// <param name="chatId"></param>
        /// <response code="200">Returns the User</response>
        /// <response code="404">If the client puts non-existing userId into the request</response>
        [Route("{chatId}")]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BotUser))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetByChatId(long chatId)
        {
            BotUser user = db.BotUsers.Where(b => b.ChatId == chatId)
                .Include(b => b.ImageResizeRequests)
                .Include(b => b.ApplyFilterRequests)
                .Include(b => b.CreateMicroStickersRequests)
                .FirstOrDefault();
            if(user == null)
            {
                return NotFound();
            }
            return new OkObjectResult(user);
        }


        /// <summary>
        /// Create or Update a User by ChatId
        /// </summary>
        /// <param name="chatId"></param>
        /// <response code="200">Updates the User</response>
        /// <response code="201">Returns a new User</response>
        /// <response code="409">If update is impossible</response>"
        [Route("{chatId}")]
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(BotUser))]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Put(long chatId, [FromBody]BotUser user)
        {
            if(db.BotUsers.Any(b => b.ChatId == chatId))
            {
                db.Entry(user).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                db.BotUsers.Update(user);
                await db.SaveChangesAsync();
                return Ok();
            }
            db.BotUsers.Add(user);
            await db.SaveChangesAsync();
            string uri = String.Format(AppSettings.Url, "api/1.0.0/users/") + user.ChatId.ToString();
            return Created(uri, user);
        }

        /// <summary>
        /// Get a User's request by ChatId
        /// </summary>
        /// <param name="chatId"></param>
        /// <response code="200">Returns the User's request</response>
        /// <response code="404">If the client puts non-existing userId into the request</response>
        [Route("{chatId}/requests")]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BotUser))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetRequests(long chatId)
        {
            BotUser user = db.BotUsers.Where(b => b.ChatId == chatId).FirstOrDefault();
            if (user == null) return NotFound();
            switch (user.CurentCommand)
            {
                case BotCommand.Resize:
                    ImageResizeRequest imageResizeRequest = db.ImageResizeRequests.Include(b => b.User).Where(b => b.UserId == user.Id).FirstOrDefault();
                    if (imageResizeRequest == null) return NotFound();
                    return new OkObjectResult(imageResizeRequest);
                case BotCommand.CreateMicroStickers:
                    CreateMicroStickersRequest microStickersRequest = db.CreateMicroStickersRequests.Include(b => b.User).Where(b => b.UserId == user.Id).FirstOrDefault();
                    if (microStickersRequest == null) return NotFound();
                    return new OkObjectResult(microStickersRequest);
                case BotCommand.ApplyFilter:
                    ApplyFilterRequest applyFilterRequest = db.ApplyFilterRequests.Include(b => b.User).Where(b => b.UserId == user.Id).FirstOrDefault();
                    if (applyFilterRequest == null) return NotFound();
                    return new OkObjectResult(applyFilterRequest);
                default:
                    return NotFound();
            }
        }

        /// <summary>
        /// Delete a User by ChatId
        /// </summary>
        /// <param name="chatId"></param>
        /// <response code="204">Deletes the User</response>
        /// <response code="404">If the client puts non-existing chatId into the request</response>
        [Route("{chatId}")]
        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(long chatId)
        {
            BotUser user = db.BotUsers.Where(b => b.ChatId == chatId).FirstOrDefault();
            if (user == null) return NotFound();
            db.BotUsers.Remove(user);
            await db.SaveChangesAsync();
            return NoContent();
        }
    }
}
