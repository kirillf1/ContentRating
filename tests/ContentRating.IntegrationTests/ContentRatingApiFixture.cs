using ContentRating.IntegrationTests.Auth;
using ContentRatingAPI.Infrastructure.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContentRating.IntegrationTests
{
    public class ContentRatingApiFixture : WebApplicationFactory<Program>, IAsyncLifetime
    {
        protected override IHost CreateHost(IHostBuilder builder)
        {
            //builder.ConfigureHostConfiguration(config =>
            //{
            //    config.AddInMemoryCollection(new Dictionary<string, string>
            //{
            //    { $"ConnectionStrings:{Postgres.Resource.Name}", _postgresConnectionString },
            //    { "Identity:Url", IdentityApi.GetEndpoint("http").Url }
            //});
            //});
            builder.ConfigureServices(services =>
            {
                services.AddAuthentication(defaultScheme: TestAuthHandler.SchemeName)
                    .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(TestAuthHandler.SchemeName, options => { });
                services.AddSingleton<IAuthorizationPolicyProvider, TestPermissionAuthorizationPolicyProvider>();
                services.AddAuthorization(opt =>
                {                   
                    opt.DefaultPolicy = new AuthorizationPolicyBuilder()
                        .AddAuthenticationSchemes(TestAuthHandler.SchemeName)
                    .Combine(opt.DefaultPolicy)
                    .Build();         
                });
            });
            builder.UseEnvironment("Development");
            return base.CreateHost(builder);
        }
        public static Guid UserId => Guid.Parse(TestAuthHandler.IDENTITY_ID);
        public Task InitializeAsync()
        {
            return Task.CompletedTask;
        }

        async Task IAsyncLifetime.DisposeAsync()
        {
            await base.DisposeAsync();
        }
       
    }
}
