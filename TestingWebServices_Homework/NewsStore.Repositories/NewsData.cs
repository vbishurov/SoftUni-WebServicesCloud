namespace NewsStore.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using Contracts;
    using Models;

    public class NewsData : INewsData
    {
        private DbContext context;
        private IDictionary<Type, object> repositories;

        public NewsData(DbContext context)
        {
            this.context = context;
            this.repositories = new Dictionary<Type, object>();
        }

        public IRepository<NewsStory> News
        {
            get
            {
                return this.GetRepository<NewsStory>();
            }
        }

        public int SaveChanges()
        {
            return this.context.SaveChanges();
        }

        public int SaveChangesAsync()
        {
            return this.context.SaveChanges();
        }

        private IRepository<T> GetRepository<T>() where T : class
        {
            var type = typeof(T);
            if (this.repositories.ContainsKey(type))
            {
                return (IRepository<T>)this.repositories[type];
            }

            var typeOfRepository = typeof(GenericRepository<T>);
            var repository = Activator.CreateInstance(
                typeOfRepository, this.context);

            this.repositories.Add(type, repository);

            return (IRepository<T>)this.repositories[type];
        }
    }
}
