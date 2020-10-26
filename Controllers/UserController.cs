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
    [Route("api/users")]
    [ApiController]
    public class UserController : ControllerBase
    {
        public UsersState db;
        private readonly ILogger _logger;
        public UserController(UsersState context, ILogger<UserController> logger)
        {
            db = context;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> Post(long chatId)
        { 
            BotUser user = new BotUser() { ChatId = chatId };
            if (db.BotUsers.Any(b => b.ChatId == user.ChatId)) return BadRequest("Cannot create an existing user.");
            db.BotUsers.Add(user);
            await db.SaveChangesAsync();
            string uri = String.Format(AppSettings.Url, "api/users/") + user.ChatId.ToString();
            return Created(uri, user);
        }

        [Route("/{chatId}")]
        [HttpGet]
        public IActionResult GetByChatId(long chatId)
        {
            BotUser user = db.BotUsers.Where(b => b.ChatId == chatId).FirstOrDefault();
            if(user == null)
            {
                return NotFound();
            }
            return new OkObjectResult(user);
        }

        [Route("/{chatId}")]
        [HttpPut]
        public async Task<IActionResult> Put(long chatId, [FromBody]BotUser user)
        {
            if(db.BotUsers.Any(b => b.ChatId == chatId))
            {
                db.Update(user);
                await db.SaveChangesAsync();
                return Ok();
            }
            db.BotUsers.Add(user);
            await db.SaveChangesAsync();
            string uri = String.Format(AppSettings.Url, "api/users/") + user.ChatId.ToString();
            return Created(uri, user);
        }

        [Route("/{chatId}/requests")]
        [HttpGet]
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
    }
}
