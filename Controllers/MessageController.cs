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
            
            //return Ok();
            //_logger.LogInformation(db.Users.);
            if (update == null) return Ok();
            _logger.LogInformation(update.Message.Text);
            var commands = Bot.Commands;
            var message = update.Message;
            var botClient = await Bot.GetBotClientAsync();

            foreach (var command in commands)
            {
                if (command.Contains(message))
                {
                    await command.Execute(message, botClient);
                    return Ok();
                }
            }
            Models.BotCommand botCommand = db.BotUsers.Where(u => u.ChatId == update.Message.Chat.Id).First().CurentCommand;
            switch (botCommand)
            {
                case Models.BotCommand.Resize:
                    await commands.Where(c => c.Name == "/resize").First().Execute(message, botClient);
                    return Ok();
                case Models.BotCommand.ApplyFilter:
                    await commands.Where(c => c.Name == "/filter").First().Execute(message, botClient);
                    return Ok();
                case Models.BotCommand.CreateMicroStickers:
                    
                    await commands.Where(c => c.Name == "/micro-stickers").First().Execute(message, botClient);
                    
                    return Ok();
            }
            return Ok();
        }
    }
}
