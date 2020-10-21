using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using image_bot.Models;
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


    }
}
