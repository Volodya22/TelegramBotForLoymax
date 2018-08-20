using System;
using TelegramBotForLoymax.Core.Models;
using TelegramBotForLoymax.Infrastructure;

namespace TelegramBotForLoymax.Services
{
    /// <summary>
    /// Класс для хранения текущего состояния сервиса
    /// </summary>
    public static class StateService
    {
        /// <summary>
        /// Текущее состояние сервиса
        /// </summary>
        public static State State { get; set; } = State.Waiting;

        /// <summary>
        /// Пользователь для хренения введеных пользователем данных
        /// </summary>
        public static User User { get; set; } = new User();

        /// <summary>
        /// Сброс хранимых данных о пользователе и состоянии
        /// </summary>
        public static void RefreshData()
        {
            State = State.Waiting;

            User.Id = 0;
            User.LastName = "";
            User.FirstName = "";
            User.Patronymic = "";
            User.BirthDate = DateTime.Now;
        }
    }
}
