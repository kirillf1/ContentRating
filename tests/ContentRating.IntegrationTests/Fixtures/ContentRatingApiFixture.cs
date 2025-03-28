// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using ContentRating.IntegrationTests.Auth;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ContentRating.IntegrationTests.Fixtures
{
    public class ContentRatingApiFixture
        : WebApplicationFactory<Program>,
            IAsyncLifetime
    {
        protected override IHost CreateHost(IHostBuilder builder)
        {
            builder.ConfigureHostConfiguration(config =>
            {
                config.AddInMemoryCollection(
                    new Dictionary<string, string> { }
                );
            });
            builder.ConfigureServices(services =>
            {
                services
                    .AddAuthentication(
                        defaultScheme: TestAuthHandler.SchemeName
                    )
                    .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(
                        TestAuthHandler.SchemeName,
                        options => { }
                    );
                services.AddSingleton<
                    IAuthorizationPolicyProvider,
                    TestPermissionAuthorizationPolicyProvider
                >();
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
        private IServiceScope? _servicesScope;

        public IServiceProvider GetServiceProvider()
        {
            _servicesScope ??= Services.CreateScope();
            return _servicesScope.ServiceProvider;
        }

        public Task InitializeAsync()
        {
            return Task.CompletedTask;
        }

        async Task IAsyncLifetime.DisposeAsync()
        {
            _servicesScope?.Dispose();
            await base.DisposeAsync();
        }
    }
}
