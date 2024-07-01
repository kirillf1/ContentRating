using AspNetCore.Identity.Mongo.Model;
using AspNetCore.Identity.Mongo;
using ContentRatingAPI.Application.Identity;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Security.Cryptography;

namespace ContentRatingAPI.Infrastructure.Authentication
{
    public static class AuthenticationExtensions
    {
        public static IServiceCollection AddApplicationAuthentication(this IHostApplicationBuilder builder)
        {
            var services = builder.Services;
            var configuration = builder.Configuration;
            services.Configure<JwtOptions>(configuration.GetSection("Authentication:JWT"));
            builder.Services.AddTransient<IJwtProvider, JwtProvider>();
            builder.Services.ConfigureOptions<ConfigureJwtBearerOptions>();
            
            builder.Services.AddIdentityMongoDbProvider<ApplicationUser, MongoRole<Guid>, Guid>(identity =>
                {
                    identity.User.AllowedUserNameCharacters += " ";
                    
                },
                mongo =>
                {
                    mongo.ConnectionString = configuration["Authentication:IdentityDbConnectionString"];
                }
            );

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;

            })
                .AddCookie(options=>
                {
                })
                .AddJwtBearer()
                .AddGoogle(options =>
                {
                    IConfigurationSection googleAuthNSection =
                                builder.Configuration.GetSection("Authentication:Google");
                    options.ClientId = googleAuthNSection["ClientId"];
                    options.ClientSecret = googleAuthNSection["ClientSecret"];
                    options.Scope.Add("https://www.googleapis.com/auth/youtube.readonly");
                    options.Scope.Add("profile");
                    options.SaveTokens = true;
                });

            builder.Services.AddSingleton(c =>
            {
                var keyPath = builder.Configuration["Authentication:JWT:AsymmetricKeyPath"];
                var rsa = RSA.Create();
                rsa.ImportRSAPrivateKey(File.ReadAllBytes(keyPath), out var _);
                return rsa;
            });

            return services;
        }
    }
}
