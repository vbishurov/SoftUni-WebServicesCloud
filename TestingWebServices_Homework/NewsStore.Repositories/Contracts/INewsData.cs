namespace NewsStore.Repositories.Contracts
{
    using Models;

    public interface INewsData
    {
        IRepository<NewsStory> News { get; }

        int SaveChanges();

    }
}
