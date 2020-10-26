using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using image_bot.Models;
using Microsoft.AspNetCore.Mvc;
//using Telegram.Bot.Types;
using CloudinaryDotNet.Actions;
using CloudinaryDotNet;
using Microsoft.EntityFrameworkCore;

namespace image_bot.Controllers
{
    [Route("api/filter")]
    [ApiController]
    [Produces("application/json")]
    public class FilterController : Controller
    {
        public Cloudinary cloudinary;
        public Account account;
        public FilterController()
        {
            account = new Account(AppSettings.CloudName, AppSettings.CloudKey, AppSettings.CloudSecret);
            cloudinary = new Cloudinary(account);
        }

        /// <summary>
        /// Get a user of a bot by ID
        /// </summary>
        /// <param name="chatId">The identifier of a chat with requested user</param>
        /// <returns>BotUser with ApplyFilterRequests included</returns>
        private BotUser GetBotUser(long chatId)
        {
            return db.BotUsers
                .Where(b => b.ChatId == chatId)
                .Include(u => u.ApplyFilterRequests)
                .First();
        }

        [Route("create-request")]
        [HttpPost]
        public async Task<ActionResult> CreateRequest(long chatId)
        {
            BotUser botUser = GetBotUser(chatId);
            botUser.CurentCommand = BotCommand.ApplyFilter;
            db.BotUsers.Update(botUser);
            ApplyFilterRequest request = new ApplyFilterRequest() { UserId = botUser.Id };
            if (db.ApplyFilterRequests.Any(i => i.UserId == botUser.Id))
            {
                return BadRequest();
            }
            db.ApplyFilterRequests.Add(request);
            await db.SaveChangesAsync();
            return Ok();
        }

        [Route("choose")]
        [HttpPost]
        public async Task<IActionResult> ChooseFilter(long chatId, string requestedFilter)
        {
            BotUser user = GetBotUser(chatId);
            AvailableFilters chosenFilter;

            // if the passed value is an available filter name
            if (Enum.TryParse(requestedFilter, out chosenFilter))
            {
                ApplyFilterRequest filterRequest = user.ApplyFilterRequests
                    .FirstOrDefault();
                    
                filterRequest.ChosenFilter = chosenFilter;
                filterRequest.Status = ApplyFilterStus.AwaitingImage;
                db.Update(filterRequest);
                await db.SaveChangesAsync();
                return Ok();
            }
            return BadRequest();
        }

        [Route("apply")]
        [HttpPost]
        public async Task<IActionResult> ApplyFilter([FromBody] ImageToFilter image)
        {
            // upload image to cloud
            ImageUploadParams uploadParams = new ImageUploadParams()
            {
                File = new FileDescription(image.Url),
                Transformation = new Transformation()
                    .Effect(image.Filter.ToString())
            };
            // image uploaded
            // uploadResult has url
            var uploadResult = await cloudinary.UploadAsync(uploadParams);
            return Created(uploadResult.Url, uploadResult);
        }

        [Route("delete-request")]
        [HttpDelete]
        public async Task<IActionResult> DeleteRequest(long chatId)
        {
            BotUser user = GetBotUser(chatId);
            ApplyFilterRequest request = user.ApplyFilterRequests.FirstOrDefault();
            if (!db.ApplyFilterRequests.Any(r => r.Id == request.Id))
            {
                return BadRequest();
            }
            db.ApplyFilterRequests.Remove(request);
            user.CurentCommand = BotCommand.Start;
            db.BotUsers.Update(user);
            await db.SaveChangesAsync();
            return Ok();
        }
    }
}
