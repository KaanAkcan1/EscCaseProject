using EscCase.Business.Interfaces;
using EscCase.Business.Services;
using EscCase.Common.Entities.App;
using EscCase.Data.Contexts;
using EscCase.Data.Models;
using EscCase.Data.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace EscCase.Business
{
    public static class ServiceRegistration
    {
        public static IServiceCollection AddAppDbContext(this IServiceCollection services)
        {
            services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(Configuration.ConnectionString));

            return services;
        }

        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services
                .AddTransient<IAppUserService, AppUserService>()
                .AddScoped<JwtRefreshTokenService, JwtRefreshTokenService>()
                .AddScoped<IProductService, ProductService>()
                .AddScoped<IUnitOfWork, UnitOfWork>();

            return services;
        }

        public static IServiceCollection AddSecurity(this IServiceCollection services, JwtOptions jwtSettingsConfig)
        {
            jwtSettingsConfig = Configuration.JwtOption;

            services.AddIdentity<AppUser, IdentityRole<Guid>>(o =>
            {
                // Password settings
                o.Password.RequireDigit = false;
                o.Password.RequireLowercase = false;
                o.Password.RequireUppercase = false;
                o.Password.RequireNonAlphanumeric = false;
                o.Password.RequiredLength = 8;

                o.SignIn.RequireConfirmedEmail = false;
                o.SignIn.RequireConfirmedPhoneNumber = false;

                //User settings
                o.User.RequireUniqueEmail = true;
                o.User.AllowedUserNameCharacters =
                    "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";

                o.Lockout.AllowedForNewUsers = false;
                o.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(30);
                o.Lockout.MaxFailedAccessAttempts = 3;

            })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

            var key = System.Text.Encoding.UTF8.GetBytes(jwtSettingsConfig.AccessTokenSecret);

            services.AddAuthentication()
                .AddJwtBearer(x =>
                {
                    x.SaveToken = true;
                    x.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(key),
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ValidIssuers = new List<string>() { },
                        ValidAudiences = new List<string>() { },
                    };
                    x.Events = new JwtBearerEvents
                    {
                        //OnAuthenticationFailed = context =>
                        //{
                        //    context.Response.Redirect("/Account/Login");
                        //    return Task.CompletedTask;
                        //},
                        //OnChallenge = context =>
                        //{
                        //    context.HandleResponse();
                        //    context.Response.Redirect("/Account/Login");
                        //    return Task.CompletedTask;
                        //}
                    };
                });


            services.AddAuthorization(options =>
            {
                options.AddPolicy("EmployeeOnly", policy => policy.RequireClaim("EmployeeNumber"));
            });

            return services;
        }

    }

}
