using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using image_bot.Models;
using Microsoft.EntityFrameworkCore;

namespace image_bot.Controllers
{
    [Route("api/image")]
    [ApiController]
    public class ImageController : ControllerBase
    {
        public UsersState db;

        public ImageController(UsersState context)
        {
            db = context;
        }

        [Route("set-parameters")]
        [HttpPost]
        public async Task<IActionResult> SetParameters(BotUser user, int height, int width)
        {
            ImageResizeRequest request = db.ImageResizeRequests.Include(u => u.User).Where(u => u.UserId == user.Id).First();
            request.Height = height;
            request.Width = width;
            db.ImageResizeRequests.Update(request);
            await db.SaveChangesAsync();
            return Ok();
        }


    }
}
