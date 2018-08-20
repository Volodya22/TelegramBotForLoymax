using System;
using System.Globalization;
using System.Linq;
using Telegram.Bot;
using Telegram.Bot.Types;
using TelegramBotForLoymax.DataLayer;
using TelegramBotForLoymax.Infrastructure;

namespace TelegramBotForLoymax.Services
{
    /// <summary>
    /// Сервис для обработки полученных данных и ответа на них
    /// </summary>
    public static class TelegramService
    {
        /// <summary>
        /// Обработка введеных пользователем данных
        /// </summary>
        /// <param name="telegramBotClient">Telegram-клиент</param>
        /// <param name="message">Сообщение</param>
        public static void HandleWaiting(ITelegramBotClient telegramBotClient, Message message)
        {
            // если пользователь вызвал команду
            if (message.Entities != null && message.Entities.Any())
            {
                switch (message.Text)
                {
                    // команда регистрации пользователя
                    case "/signup":
                        StateService.State = State.SignUpLastName;

                        telegramBotClient.SendTextMessageAsync(message.Chat?.Id, "Введите фамилию пользователя");
                        break;
                    // команда отображения данных о пользователе
                    case "/show":
                        StateService.State = State.Show;

                        telegramBotClient.SendTextMessageAsync(message.Chat?.Id, "Введите идентификатор пользователя");
                        break;
                    // команда удаления данных о пользователе
                    case "/remove":
                        StateService.State = State.Remove;

                        telegramBotClient.SendTextMessageAsync(message.Chat?.Id, "Введите идентификатор пользователя");
                        break;
                    // отмена текущей команды
                    case "/cancel":
                        StateService.RefreshData();

                        telegramBotClient.SendTextMessageAsync(message.Chat?.Id, "Команда отменена");
                        break;
                }
            }
            else
            {
                // отображаем сообщение о некорректно введеных данных
                telegramBotClient.SendTextMessageAsync(message?.Chat?.Id, "Введите команду из списка имеющихся!");
            }
        }

        /// <summary>
        /// Обработка ввода фамилии пользователя
        /// </summary>
        /// <param name="telegramBotClient">Telegram-клиент</param>
        /// <param name="message">Сообщение</param>
        public static void HandleSignUpLastName(ITelegramBotClient telegramBotClient, Message message)
        {
            // проверяем, что не вызвана отмена
            if (CheckForCancel(telegramBotClient, message))
            {
                return;
            }

            // проверяем, что сообщение не пустое
            if (!string.IsNullOrEmpty(message?.Text))
            {
                // сохраняем данные и запрашиваем следующее значение
                StateService.User.LastName = message.Text;
                StateService.State = State.SignUpFirstName;

                telegramBotClient.SendTextMessageAsync(message.Chat?.Id, "Введите имя пользователя");
            }
            else
            {
                // отображаем сообщение о некорректно введеных данных
                telegramBotClient.SendTextMessageAsync(message?.Chat?.Id, "Значение не может быть пустым!");
            }
        }

        /// <summary>
        /// Обработка ввода имени пользователя
        /// </summary>
        /// <param name="telegramBotClient">Telegram-клиент</param>
        /// <param name="message">Сообщение</param>
        public static void HandleSignUpFirstName(ITelegramBotClient telegramBotClient, Message message)
        {
            // проверяем, что не вызвана отмена
            if (CheckForCancel(telegramBotClient, message))
            {
                return;
            }

            // проверяем, что сообщение не пустое
            if (!string.IsNullOrEmpty(message?.Text))
            {
                // сохраняем данные и запрашиваем следующее значение
                StateService.User.FirstName = message.Text;
                StateService.State = State.SignUpPatronymic;

                telegramBotClient.SendTextMessageAsync(message.Chat?.Id, "Введите отчество пользователя");
            }
            else
            {
                // отображаем сообщение о некорректно введеных данных
                telegramBotClient.SendTextMessageAsync(message?.Chat?.Id, "Значение не может быть пустым!");
            }
        }

        /// <summary>
        /// Обработка ввода отчества пользователя
        /// </summary>
        /// <param name="telegramBotClient">Telegram-клиент</param>
        /// <param name="message">Сообщение</param>
        public static void HandleSignUpPatronymic(ITelegramBotClient telegramBotClient, Message message)
        {
            // проверяем, что не вызвана отмена
            if (CheckForCancel(telegramBotClient, message))
            {
                return;
            }

            // проверяем, что сообщение не пустое
            if (!string.IsNullOrEmpty(message?.Text))
            {
                // сохраняем данные и запрашиваем следующее значение
                StateService.User.Patronymic = message.Text;
                StateService.State = State.SignUpBirthDate;

                telegramBotClient.SendTextMessageAsync(message.Chat?.Id, "Введите дату рождения пользователя в формате ДД.ММ.ГГГГ");
            }
            else
            {
                // отображаем сообщение о некорректно введеных данных
                telegramBotClient.SendTextMessageAsync(message?.Chat?.Id, "Значение не может быть пустым!");
            }
        }

        /// <summary>
        /// Обработка ввода даты рождения пользователя
        /// </summary>
        /// <param name="context">БД</param>
        /// <param name="telegramBotClient">Telegram-клиент</param>
        /// <param name="message">Сообщение</param>
        public static void HandleSignUpBirthDate(IDbContext context, ITelegramBotClient telegramBotClient, Message message)
        {
            // проверяем, что не вызвана отмена
            if (CheckForCancel(telegramBotClient, message))
            {
                return;
            }

            try
            {
                // проверяем введеные данные
                StateService.User.BirthDate = DateTime.ParseExact(message?.Text, "dd.MM.yyyy", CultureInfo.InvariantCulture);

                // создаем пользователя
                context.Users.Add(StateService.User);
                context.SaveChanges();

                telegramBotClient.SendTextMessageAsync(message?.Chat?.Id, $"Пользователь успешно создан с идентификатором {StateService.User.Id}!");

                // очищаем данные
                StateService.RefreshData();
            }
            catch (Exception e)
            {
                // отобрадаем ошибку в случае неверного формата даты
                telegramBotClient.SendTextMessageAsync(message?.Chat?.Id, $"Введены данные в некорректном формате ({message?.Text}) ({StateService.User.Id})! {e.InnerException.Message}");
            }
        }

        /// <summary>
        /// Обработка вызова метода отображения данных о пользователе
        /// </summary>
        /// <param name="context">БД</param>
        /// <param name="telegramBotClient">Telegram-клиент</param>
        /// <param name="message">Сообщение</param>
        public static void HandleShow(IDbContext context, ITelegramBotClient telegramBotClient, Message message)
        {
            // проверяем, что не вызвана отмена
            if (CheckForCancel(telegramBotClient, message))
            {
                return;
            }

            // проверяем введеный идентификатор на корректность
            if (int.TryParse(message?.Text, out var id))
            {
                var user = context.Users.FirstOrDefault(u => u.Id == id);

                // если пользователь с указанным идентификатором отсутствует, то отображаем соответствующее сообщение
                if (user == null)
                {
                    telegramBotClient.SendTextMessageAsync(message?.Chat?.Id, "Пользователь с данным идентификатором отсутствует!");

                    StateService.State = State.Waiting;

                    return;
                }

                StateService.State = State.Waiting;

                //если пользователь существует, то показываем данные о нем
                telegramBotClient.SendTextMessageAsync(message?.Chat?.Id, $"ФИО: {user.LastName} {user.FirstName} {user.Patronymic}\nДата рождения: {user.BirthDate:dd.MM.yyyy}");
            }
            else
            {
                // отображаем сообщение о некорректно введеных данных
                telegramBotClient.SendTextMessageAsync(message?.Chat?.Id, "Введите корректное значение идентификатора!");
            }
        }

        /// <summary>
        /// Обработка вызова метода удаления данных о пользователе
        /// </summary>
        /// <param name="context">БД</param>
        /// <param name="telegramBotClient">Telegram-клиент</param>
        /// <param name="message">Сообщение</param>
        public static void HandleRemove(IDbContext context, ITelegramBotClient telegramBotClient, Message message)
        {
            // проверяем, что не вызвана отмена
            if (CheckForCancel(telegramBotClient, message))
            {
                return;
            }

            // проверяем введеный идентификатор на корректность
            if (int.TryParse(message?.Text, out var id))
            {
                var user = context.Users.FirstOrDefault(u => u.Id == id);

                // если пользователь с указанным идентификатором отсутствует, то отображаем соответствующее сообщение
                if (user == null)
                {
                    telegramBotClient.SendTextMessageAsync(message?.Chat?.Id, "Пользователь с данным идентификатором отсутствует!");

                    StateService.State = State.Waiting;

                    return;
                }

                //если пользователь существует, то удаляем данные о нем
                try
                {
                    context.Users.Remove(user);
                    context.SaveChanges();

                    StateService.State = State.Waiting;

                    telegramBotClient.SendTextMessageAsync(message?.Chat?.Id, "Данные о пользователе успешно удалены!");
                }
                catch (Exception)
                {
                    // отображаем ошибку в случае неудачного удаления
                    telegramBotClient.SendTextMessageAsync(message?.Chat?.Id, "При удалении данных о пользователе произошла ошибка!");
                }
            }
            else
            {
                // отображаем сообщение о некорректно введеных данных
                telegramBotClient.SendTextMessageAsync(message?.Chat?.Id, "Введите корректное значение идентификатора!");
            }
        }

        /// <summary>
        /// Проверка на отмену выполнения текущей команды
        /// </summary>
        /// <param name="telegramBotClient">Telegram-клиент</param>
        /// <param name="message">Сообщение</param>
        /// <returns>true - если отмена</returns>
        private static bool CheckForCancel(ITelegramBotClient telegramBotClient, Message message)
        {
            // в случае вызова команды отмены обнуляем все данные
            if (message?.Text != "/cancel")
            {
                return false;
            }

            StateService.RefreshData();

            telegramBotClient.SendTextMessageAsync(message.Chat?.Id, "Команда отменена");

            return true;
        }
    }
}
