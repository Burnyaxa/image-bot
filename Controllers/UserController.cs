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
    [Route("api/user")]
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

        [Route("create")]
        [HttpPost]
        public async Task<IActionResult> Create(long chatId)
        {
            _logger.LogInformation(chatId.ToString());
            if(db.BotUsers.Any(c => c.ChatId == chatId))
            {
                return BadRequest();
            }
            db.BotUsers.Add(new BotUser() { ChatId = chatId});
            await db.SaveChangesAsync();
            return Ok();
        }

        [Route("update")]
        [HttpPost]
        public async Task<IActionResult> Update(long chatId, BotCommand command)
        {
            if (db.BotUsers.Any(c => c.ChatId == chatId))
            {
                BotUser user = db.BotUsers.Where(b => b.ChatId == chatId).First();
                user.CurentCommand = command;
                db.BotUsers.Update(user);
                await db.SaveChangesAsync();
                return Ok();
            }
            return BadRequest();
        }

        [Route("get-status")]
        [HttpGet]
        public IActionResult GetStatus(long chatId)
        {
            if (db.BotUsers.Any(c => c.ChatId == chatId))
            {
                BotUser user = db.BotUsers.Where(b => b.ChatId == chatId).First();
                switch (user.CurentCommand)
                {
                    case BotCommand.Resize:
                        var resizeImageStatus = db.ImageResizeRequests.Include(u => u.User).Where(u => u.UserId == user.Id);
                        return new OkObjectResult(resizeImageStatus.First().Status);
                    case BotCommand.ApplyFilter:
                        var applyFilterStatus = db.ApplyFilterRequests.Include(u => u.User).Where(u => u.UserId == user.Id);
                        return new OkObjectResult(applyFilterStatus.First().Status);
                        //TODO: Add micro-stickers case
                    default:
                        return BadRequest();
                }
            }
            return BadRequest();
        }

        [Route("get-status")]
        [HttpGet]
        public IActionResult GetStatus(BotUser user)
        {
            if (db.BotUsers.Any(c => c.ChatId == user.ChatId))
            {
                switch (user.CurentCommand)
                {
                    case BotCommand.Resize:
                        var resizeImageStatus = db.ImageResizeRequests.Include(u => u.User).Where(u => u.UserId == user.Id);
                        return new OkObjectResult(resizeImageStatus.First().Status);
                    case BotCommand.ApplyFilter:
                        var applyFilterStatus = db.ApplyFilterRequests.Include(u => u.User).Where(u => u.UserId == user.Id);
                        return new OkObjectResult(applyFilterStatus.First().Status);
                    //TODO: Add micro-stickers case
                    default:
                        return new BadRequestObjectResult(null);
                }
            }
            return new BadRequestObjectResult(null);
        }
    }
}
