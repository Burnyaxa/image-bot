using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using image_bot.Models;
using Microsoft.AspNetCore.Mvc;
using Telegram.Bot.Types;
using CloudinaryDotNet.Actions;
using CloudinaryDotNet;

namespace image_bot.Controllers
{
    [Route("api/filter")]
    public class FilterController : Controller
    {
        public Cloudinary cloudinary;

        public UsersState db;
        public FilterController(UsersState context)
        {
            db = context;

            Account account = new Account(
                AppSettings.CloudName,
                AppSettings.CloudKey,
                AppSettings.CloudKey);
            cloudinary = new Cloudinary(account);
        }

        [Route("choose")]
        [HttpPost]
        public async Task<IActionResult> ChooseFilter(BotUser user, string requestedFilter)
        {
            AvailableFilters chosenFilter;
            if (Enum.TryParse(requestedFilter, out chosenFilter))
            {
                ApplyFilterRequest filterRequest = user.ApplyFilterRequests.FirstOrDefault();
                filterRequest.ChosenFilter = chosenFilter;
                filterRequest.Status = ApplyFilterStus.AwaitingImage;
                db.Update(filterRequest);
                await db.SaveChangesAsync();
                return Ok();
            }
            return BadRequest();
        }

        [Route("filter")]
        [HttpPost]
        public async Task<IActionResult> ApplyFilter(BotUser user, string imageLink)
        {
            // upload image to cloud
            ImageUploadParams uploadParams = new ImageUploadParams()
            {
                File = new FileDescription(imageLink),
                EagerTransforms = new List<Transformation>()
                {
                    new Transformation().Effect(user.ApplyFilterRequests.FirstOrDefault().ChosenFilter.ToString())
                }
            };
            // image uploaded
            // uploadResult has url
            var uploadResult = cloudinary.Upload(uploadParams);
            ApplyFilterRequest filterRequest = user.ApplyFilterRequests.FirstOrDefault();
            filterRequest.FilteredImageURL = uploadResult.Url.ToString();
            
            db.Update(filterRequest);
            await db.SaveChangesAsync();
            return new OkObjectResult(filterRequest.FilteredImageURL);
        }
    }
}
