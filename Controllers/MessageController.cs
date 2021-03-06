﻿using System;
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
    [Route("api/1.0.0/message")]
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

        /// <summary>
        /// Receive an Update from Telegram Client
        /// </summary>
        /// <param name="update"></param>
        /// <response code="200">Update received</response>
        [Route("update")]
        [IgnoreAntiforgeryToken]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
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
                    command.Execute(message, botClient);
                    return Ok();
                }
            }
            Models.BotCommand botCommand = db.BotUsers.Where(u => u.ChatId == update.Message.Chat.Id).First().CurentCommand;
            switch (botCommand)
            {
                case Models.BotCommand.Resize:
                    commands.Where(c => c.Name == "/resize").First().Execute(message, botClient);
                    return Ok();
                case Models.BotCommand.ApplyFilter:
                    commands.Where(c => c.Name == "/filter").First().Execute(message, botClient);
                    return Ok();
                case Models.BotCommand.CreateMicroStickers:
                    commands.Where(c => c.Name == "/micro-stickers").First().Execute(message, botClient);
                    return Ok();
            }
            return Ok();
        }
    }
}
