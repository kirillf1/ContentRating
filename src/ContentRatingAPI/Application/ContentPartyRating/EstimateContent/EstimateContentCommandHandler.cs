﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using ContentRating.Domain.AggregatesModel.ContentPartyRatingAggregate;
using ContentRating.Domain.AggregatesModel.ContentPartyRatingAggregate.Exceptions;
using ContentRatingAPI.Application.ContentPartyRating.ContentRaterService;

namespace ContentRatingAPI.Application.ContentPartyRating.EstimateContent
{
    public class EstimateContentCommandHandler : IRequestHandler<EstimateContentCommand, Result<bool>>
    {
        private readonly IContentPartyRatingRepository contentRatingRepository;
        private readonly IContentRaterService contentRaterService;

        public EstimateContentCommandHandler(IContentPartyRatingRepository contentRatingRepository, IContentRaterService contentRaterService)
        {
            this.contentRatingRepository = contentRatingRepository;
            this.contentRaterService = contentRaterService;
        }

        public async Task<Result<bool>> Handle(EstimateContentCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var contentRating = await contentRatingRepository.GetContentRating(request.ContentRatingId);
                if (contentRating is null)
                {
                    return Result.NotFound();
                }

                var raters = await contentRaterService.GetContentRatersForEstimation(
                    contentRating.RoomId,
                    request.EstimationInitiatorId,
                    request.RaterForChangeScoreId
                );

                var estimation = new Estimation(raters.Initiator, raters.TargetRater, new Score(request.NewScore));
                contentRating.EstimateContent(estimation);
                contentRatingRepository.Update(contentRating);
                await contentRatingRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
                return Result.Success(true);
            }
            catch (ForbiddenRatingOperationException ex)
            {
                return Result.Invalid(new ValidationError(ex.Message));
            }
        }
    }
}
