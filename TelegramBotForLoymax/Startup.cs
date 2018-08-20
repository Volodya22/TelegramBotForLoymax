using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot;
using TelegramBotForLoymax.DataLayer;

namespace TelegramBotForLoymax
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();

            // создаем webhook
            var accessToken = Configuration["Settings:Token"];
            var telegramClient = new TelegramBotClient(accessToken);
            var webHookUrl = Configuration["Settings:WebHookUrl"];

            telegramClient.SetWebhookAsync(webHookUrl);
            
            // указываем расположение БД
            services.AddDbContext<AppDbContext>(options => options.UseSqlServer(Configuration.GetConnectionString("AppDbContext")));

            // DI
            services.AddScoped<ITelegramBotClient>(c => telegramClient);
            services.AddScoped<IDbContext, AppDbContext>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();
        }
    }
}
