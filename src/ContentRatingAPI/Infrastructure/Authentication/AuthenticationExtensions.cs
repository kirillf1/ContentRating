// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Security.Cryptography;
using AspNetCore.Identity.Mongo;
using AspNetCore.Identity.Mongo.Model;
using ContentRatingAPI.Application.Identity;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Models;

namespace ContentRatingAPI.Infrastructure.Authentication
{
    public static class AuthenticationExtensions
    {
        public static IServiceCollection AddApplicationAuthentication(this IHostApplicationBuilder builder)
        {
            var services = builder.Services;
            var configuration = builder.Configuration;
            services.Configure<JwtOptions>(configuration.GetSection("Authentication:JWT"));
            services.AddSwaggerGen(c =>
            {
                c.AddSecurityDefinition(
                    "Bearer",
                    securityScheme: new OpenApiSecurityScheme
                    {
                        Name = "Authorization",
                        Description = "Enter the Bearer Authorization string as following: `Bearer Generated-JWT-Token`",
                        In = ParameterLocation.Header,
                        Type = SecuritySchemeType.ApiKey,
                        BearerFormat = "JWT",
                        Scheme = "Bearer",
                    }
                );

                c.AddSecurityRequirement(
                    new OpenApiSecurityRequirement
                    {
                        {
                            new OpenApiSecurityScheme
                            {
                                Name = "Bearer",
                                In = ParameterLocation.Header,
                                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" },
                            },
                            new string[] { }
                        },
                    }
                );
            });
            builder.Services.AddTransient<IJwtProvider, JwtProvider>();
            builder.Services.ConfigureOptions<ConfigureJwtBearerOptions>();

            builder.Services.AddIdentityMongoDbProvider<ApplicationUser, MongoRole<Guid>, Guid>(
                identity =>
                {
                    identity.User.AllowedUserNameCharacters += " ";
                },
                mongo =>
                {
                    mongo.ConnectionString = configuration["Authentication:IdentityDbConnectionString"];
                }
            );

            builder
                .Services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
                    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                })
                .AddCookie(options => { })
                .AddJwtBearer()
                .AddGoogle(options =>
                {
                    var googleAuthNSection = builder.Configuration.GetSection("Authentication:Google");
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
