using Microsoft.EntityFrameworkCore;
using TelegramBotForLoymax.Core.Models;

namespace TelegramBotForLoymax.DataLayer
{
    /// <summary>
    /// Интерфейс для работы с БД
    /// </summary>
    public interface IDbContext
    {
        /// <summary>
        /// Таблица пользователей
        /// </summary>
        DbSet<User> Users { get; set; }

        /// <summary>
        /// Метод сохранения данных
        /// </summary>
        int SaveChanges();
    }
}
