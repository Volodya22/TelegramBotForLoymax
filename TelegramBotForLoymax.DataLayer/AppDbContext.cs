using Microsoft.EntityFrameworkCore;
using TelegramBotForLoymax.Core.Models;

namespace TelegramBotForLoymax.DataLayer
{
    /// <summary>
    /// База даннных для пользователей
    /// </summary>
    public class AppDbContext : DbContext, IDbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
            
        }
        
        /// <summary>
        /// Таблица пользователей
        /// </summary>
        public DbSet<User> Users { get; set; }
    }
}
