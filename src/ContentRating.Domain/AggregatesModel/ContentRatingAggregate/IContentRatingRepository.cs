using ContentRating.Domain.Shared;


namespace ContentRating.Domain.AggregatesModel.ContentRatingAggregate
{
    public interface IContentRatingRepository: IRepository<ContentRating>
    {
        ContentRating Add(ContentRating contentRating);
        ContentRating Update(ContentRating contentRating);
        void Delete(ContentRating contentRating);
        Task<ContentRating> GetContentRating(Guid id);
        Task<IEnumerable<ContentRating>> GetContentRatingsByRoom(Guid roomId);

    }
}
