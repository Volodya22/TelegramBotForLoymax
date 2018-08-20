using System;
using System.ComponentModel.DataAnnotations;

namespace TelegramBotForLoymax.Core.Models
{
    /// <summary>
    /// Класс пользователя
    /// </summary>
    public class User
    {
        /// <summary>
        /// Идентификатор
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Фамилия
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// Имя
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// Отчество
        /// </summary>
        public string Patronymic { get; set; }

        /// <summary>
        /// Дата рождения
        /// </summary>
        public DateTime BirthDate { get; set; }
    }
}
