using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using image_bot.Models;
using Microsoft.AspNetCore.Mvc;
using Telegram.Bot.Types;

namespace image_bot.Controllers
{
    [Route("api/filter")]
    public class FilterController : Controller
    {
        public UsersState db;
        public FilterController(UsersState context)
        {
            db = context;
        }

        [Route("choose")]
        [HttpPost]
        public async Task<IActionResult> ChooseFilter(BotUser user, string requestedFilter)
        {
            AvailableFilters chosenFilter;
            if (Enum.TryParse(requestedFilter, out chosenFilter))
            {
                user.ApplyFilterRequests[0].ChosenFilter = chosenFilter;
                db.Update(user);
                await db.SaveChangesAsync();
                return Ok();
            }
            return BadRequest();
        }

        [Route("filter")]
        [HttpPost]
        public async Task<IActionResult> ChooseFilter(BotUser user, string messageLink)
        {
            
        }
    }
}
