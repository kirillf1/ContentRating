// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FluentValidation;

namespace ContentRatingAPI.Application.ContentPartyEstimationRoom.StartContentPartyEstimation
{
    public class StartContentPartyEstimationCommandValidator : AbstractValidator<StartContentPartyEstimationCommand>
    {
        public StartContentPartyEstimationCommandValidator()
        {
            RuleFor(c => c.Name).NotEmpty().Length(3, 50);
            RuleFor(c => c.ContentListId).NotEqual(Guid.Empty);
            RuleFor(c => c.CreatorName).NotEmpty();
            RuleFor(c => c.RoomId).NotEqual(Guid.Empty);
        }
    }
}
