using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Telegram.Bot;
using Telegram.Bot.Types;
using TelegramBotForLoymax.DataLayer;
using TelegramBotForLoymax.Infrastructure;
using TelegramBotForLoymax.Services;

namespace TelegramBotForLoymax.Controllers
{
    /// <summary>
    /// Контроллер для обработки данных, полученных из webhook'ов
    /// </summary>
    [Produces("application/json")]
    [Route("api/telegram")]
    public class TelegramController : Controller
    {
        private readonly IDbContext _context;

        private readonly ITelegramBotClient _telegramBotClient;

        public TelegramController(IDbContext context, ITelegramBotClient telegramBotClient)
        {
            _context = context;
            _telegramBotClient = telegramBotClient;
        }

        /// <summary>
        /// Метод обработки полученных данных
        /// </summary>
        /// <param name="update">Полученные данные</param>
        [HttpPost]
        public void Post([FromBody] Update update)
        {
            switch (StateService.State)
            {
                case State.Waiting:
                    TelegramService.HandleWaiting(_telegramBotClient, update?.Message);
                    break;
                case State.SignUpLastName:
                    TelegramService.HandleSignUpLastName(_telegramBotClient, update?.Message);
                    break;
                case State.SignUpFirstName:
                    TelegramService.HandleSignUpFirstName(_telegramBotClient, update?.Message);
                    break;
                case State.SignUpPatronymic:
                    TelegramService.HandleSignUpPatronymic(_telegramBotClient, update?.Message);
                    break;
                case State.SignUpBirthDate:
                    TelegramService.HandleSignUpBirthDate(_context, _telegramBotClient, update?.Message);
                    break;
                case State.Show:
                    TelegramService.HandleShow(_context, _telegramBotClient, update?.Message);
                    break;
                case State.Remove:
                    TelegramService.HandleRemove(_context, _telegramBotClient, update?.Message);
                    break;
                default:
                    return;
            }
        }
    }
}