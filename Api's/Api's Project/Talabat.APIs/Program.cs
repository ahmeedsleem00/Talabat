using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using StackExchange.Redis;
using System.Text;
using Talabat.APIs.Errors;
using Talabat.APIs.Extenstions;
using Talabat.APIs.Helpers;
//using Talabat.APIs.MiddleWares;
using Talabat.Core.Entities.Identity;
using Talabat.Core.Repositories.Interfaces;
using Talabat.Core.Services.Interfaces;
using Talabat.Repository.Data;
using Talabat.Repository.Identity;
using Talabat.Repository.Identity.DataSeed;
using Talabat.Repository.Repositories;
using Talabat.Service;

namespace Talabat.APIs
{
    public class Program
    {
        // Entry Point
        public static async Task Main(string[] args)
        {


            var webApplicationbuilder = WebApplication.CreateBuilder(args);

            // Add services to the container.


            #region Configure Services



            webApplicationbuilder.Services.AddControllers(); // Register Web Api Bult-in Services at the container

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            webApplicationbuilder.Services.AddEndpointsApiExplorer();


            webApplicationbuilder.Services.AddSwaggerGen();

            webApplicationbuilder.Services.AddDbContext<StoreDbContext>(options =>
            {

                options.UseSqlServer(webApplicationbuilder.Configuration.GetConnectionString("DefaultConnection"));

            });

            webApplicationbuilder.Services.AddDbContext<AppIdentityDbContext>(options =>
            {
                options.UseSqlServer(webApplicationbuilder.Configuration.GetConnectionString(name: "IdentityConnection"));
            });

            webApplicationbuilder.Services.AddSingleton<IConnectionMultiplexer>(  (serverProvider) =>
            {

                var connection = webApplicationbuilder.Configuration.GetConnectionString("Redis");

				return ConnectionMultiplexer.Connect(connection);
            });


            webApplicationbuilder.Services.AddScoped( typeof(IBasketRepository) , typeof(BasketRepository) );

            webApplicationbuilder.Services.AddAppLicationsServices();

            webApplicationbuilder.Services.AddIdentity<AppUser,IdentityRole>(options =>
            {
                //options.Password.RequireDigit = true;
                //options.Password.RequiredUniqueChars = 2;
                //options.Password.RequireNonAlphanumeric =true;

            }).AddEntityFrameworkStores<AppIdentityDbContext>();

            webApplicationbuilder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme    = JwtBearerDefaults.AuthenticationScheme;
            })
                                          .AddJwtBearer(options =>
                                          {
                                              options.TokenValidationParameters = new TokenValidationParameters()
                                              {
                                                  ValidateIssuer = true,
                                                  ValidIssuer = webApplicationbuilder.Configuration[key:"JWT:ValidIssure"],
                                                  ValidateAudience = true,
                                                  ValidAudience = webApplicationbuilder.Configuration[key: "JWT:ValidAudience"],
                                                  ValidateLifetime = true,
                                                  ValidateIssuerSigningKey=true,
                                                  IssuerSigningKey =  new SymmetricSecurityKey(Encoding.UTF8.GetBytes(webApplicationbuilder.Configuration["JWT:Key"]))

                                              };
                                          });

            webApplicationbuilder.Services.AddScoped<ITokenService, TokenService>();


            webApplicationbuilder.Services.AddCors(options =>
            {
                options.AddPolicy("MyPolicy", config =>
                {
                    config.AllowAnyHeader();
                    config.AllowAnyMethod();
                    config.WithOrigins(webApplicationbuilder.Configuration[key:"FrontEndBaseUrl"]);
                });
            });
			#endregion


		   var app = webApplicationbuilder.Build();

            var scope = app.Services.CreateScope();

            var services = scope.ServiceProvider;

            var _context = services.GetRequiredService<StoreDbContext>();
            //ASk CLR to Create object from StoreDbContext Explicitly

            var _IdentityDbContext = services.GetRequiredService<AppIdentityDbContext>();
            var loggerFactory = services.GetRequiredService<ILoggerFactory>();


            try
            {
               await _context.Database.MigrateAsync(); // Update Database (Business)
                // Data Seeding
              await StoreDbContextSeed.SeedAsync(_context);
              await  _IdentityDbContext.Database.MigrateAsync(); //Update Database (Identity)

                var _userManager = services.GetRequiredService<UserManager<AppUser>>();
                //ASk CLR to Create object from UserManager<AppUser> Explicitly

                await AppIdentityDbContextSeed.IdentitySeedAsync(_userManager);




            }
            catch (Exception ex) 
            {
               
                var logger = loggerFactory.CreateLogger<Program>();
                logger.LogError(ex, "An Error han been occured during appling the Migrations");

               //Console.WriteLine(ex.Message);
            }



            // Configure the HTTP request pipeline.

            #region Configure Kestrel Pipline

          //  app.UseMiddleware<ExceptionMiddleware>();


			if (app.Environment.IsDevelopment())
            { 
                app.UseSwagger();  // Middileware
                app.UseSwaggerUI();
            }

            app.UseStatusCodePagesWithReExecute("/errors/{0}");   // Redirect 

            app.UseHttpsRedirection();

            app.UseStaticFiles() ;

            app.UseCors(policyName:"MyPolicy");

            app.UseAuthentication();
            app.UseAuthorization();


            app.MapControllers();


            #endregion

            app.Run();


        }
    }
}
