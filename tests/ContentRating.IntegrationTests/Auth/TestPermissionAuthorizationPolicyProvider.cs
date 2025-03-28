// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace ContentRating.IntegrationTests.Auth
{
    public class TestPermissionAuthorizationPolicyProvider(
        IOptions<AuthorizationOptions> options
    ) : DefaultAuthorizationPolicyProvider(options)
    {
        public override async Task<AuthorizationPolicy?> GetPolicyAsync(
            string policyName
        )
        {
            var policy = await base.GetPolicyAsync(policyName);
            if (policy is null)
            {
                return null;
            }

            return new AuthorizationPolicyBuilder(
                [.. policy.AuthenticationSchemes, TestAuthHandler.SchemeName]
            )
                .AddRequirements([.. policy.Requirements])
                .Build();
        }
    }
}
