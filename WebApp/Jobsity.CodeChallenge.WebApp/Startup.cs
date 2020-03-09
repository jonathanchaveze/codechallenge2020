using Jobsity.CodeChallenge.Bot.Models;
using Jobsity.CodeChallenge.WebApp.Models;
using Jobsity.CodeChallenge.WebApp.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Npgsql;
using System;

namespace Jobsity.CodeChallenge.WebApp
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
            #region MVC Configuration

            services.AddRouting(options => options.LowercaseUrls = true);
            services.AddControllersWithViews();

            #endregion

            #region Persistance Service Configuration

            var databaseUrl = Configuration["DATABASE_URL"];

            if (string.IsNullOrEmpty(databaseUrl))
            {
                Console.WriteLine("DATABASE_URL is not specified in SecretStore.");
                System.Environment.Exit(-1);
            }

            var databaseUri = new Uri(databaseUrl);
            var userInfo = databaseUri.UserInfo.Split(':');

            var builder = new NpgsqlConnectionStringBuilder
            {
                Host = databaseUri.Host,
                Port = Convert.ToInt32(databaseUri.Port),
                Database = databaseUri.LocalPath.TrimStart('/'),
                Username = userInfo[0],
                Password = userInfo[1],
                SslMode = SslMode.Require,
                TrustServerCertificate = true
            };

            services.AddDbContext<ChatPersistance>(o => o.UseNpgsql(builder.ConnectionString));

            #endregion

            #region Identity Configuration

            services.AddIdentity<ChatUser, IdentityRole>()
                    .AddEntityFrameworkStores<ChatPersistance>()
                    .AddDefaultTokenProviders();

            services.Configure<IdentityOptions>(options =>
            {
                // Password settings.
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequiredLength = 6;
                options.Password.RequiredUniqueChars = 0;

                // Lockout settings.
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.AllowedForNewUsers = true;

                // User settings.
                options.User.AllowedUserNameCharacters =
                "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
                options.User.RequireUniqueEmail = true;

                // SignIn settings.
                options.SignIn.RequireConfirmedEmail = false;
                options.SignIn.RequireConfirmedPhoneNumber = false;
            });

            #endregion

            #region Profile Picture Service Configuration

            services.AddSingleton<ProfilePictureService, ProfilePictureService>(o =>
            {
                return new ProfilePictureService();
            });

            #endregion

            #region SignalR Configuration

            services.AddSignalR();

            #endregion

            #region Bot Configuration

            var botUserId = Configuration["ChatBot:UserId"];
            var stockServiceEndpoint = Configuration["ChatBot:StockServiceEndpoint"];

            services.AddSingleton<ChatBot, ChatBot>(o =>
            {
                return new ChatBot(botUserId, stockServiceEndpoint);
            });

            #endregion
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
                endpoints.MapHub<ChatHub>("/chatHub");
            });
        }
    }
}