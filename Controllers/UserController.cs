using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using image_bot.Models;
using Newtonsoft.Json;
using Microsoft.EntityFrameworkCore;

namespace image_bot.Controllers
{
    [Route("api/user")]
    [ApiController]
    public class UserController : ControllerBase
    {
        public UsersState db;
        public UserController(UsersState context)
        {
            db = context;
        }

        [Route("create")]
        [HttpPost]
        public async Task<IActionResult> Create(BotUser user)
        {
            if(db.BotUsers.Any(c => c.ChatId == user.ChatId))
            {
                return BadRequest();
            }
            db.BotUsers.Add(user);
            await db.SaveChangesAsync();
            return Ok();
        }

        [Route("update")]
        [HttpPost]
        public async Task<IActionResult> Update(BotUser user)
        {
            if (db.BotUsers.Any(c => c.ChatId == user.ChatId))
            {
                db.BotUsers.Update(user);
                await db.SaveChangesAsync();
                return Ok();
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
