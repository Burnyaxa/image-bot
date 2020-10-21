using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using image_bot.Models;
namespace image_bot.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        public UsersState db;
        public UserController(UsersState context)
        {
            db = context;
        }

        [HttpPost]
        public async Task<IActionResult> Create(User user)
        {
            if(db.Users.Any(c => c.ChatId == user.ChatId))
            {
                return BadRequest();
            }
            db.Users.Add(user);
            await db.SaveChangesAsync();
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> Update(User user)
        {
            if (db.Users.Any(c => c.ChatId == user.ChatId))
            {
                db.Users.Update(user);
                await db.SaveChangesAsync();
                return Ok();
            }
            return BadRequest();
        }


    }
}
