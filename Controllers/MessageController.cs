using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Telegram.Bot.Types;
using image_bot.Models;
using System.Diagnostics;
using Microsoft.Extensions.Logging;

namespace image_bot.Controllers
{
    [Route("api/message")]
    [ApiController]
    public class MessageController : ControllerBase
    {
        private readonly ILogger _logger;
        public UsersState db;
        public MessageController(ILogger<MessageController> logger, UsersState context)
        {
            _logger = logger;
            db = context;
        }
        // GET api/values
        [HttpGet]
        public string Get()
        {
            _logger.LogInformation("ok.");
            return "Method GET unuvalable";
        }
        // POST api/values
        [Route("update")]
        [IgnoreAntiforgeryToken]
        [HttpPost]
        public async Task<OkResult> Update([FromBody]Update update)
        {

            //_logger.LogInformation(db.Users.);
            if (update == null) return Ok();

            var commands = Bot.Commands;
            var message = update.Message;
            var botClient = await Bot.GetBotClientAsync();

            foreach (var command in commands)
            {
                if (command.Contains(message))
                {
                    await command.Execute(message, botClient);
                    break;
                }
            }
            return Ok();
        }
    }
}
