namespace BookShop.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using Models;

    internal sealed class Configuration : DbMigrationsConfiguration<BookShopContext>
    {
        public Configuration()
        {
            this.AutomaticMigrationsEnabled = true;
            this.AutomaticMigrationDataLossAllowed = true;
            this.ContextKey = "BookShop.Data.BookShopContext";
        }

        protected override void Seed(BookShopContext context)
        {
            if (context.Books.Any())
            {
                return;
            }

            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;

            using (var reader = new StreamReader("SeedData/authors.txt"))
            {
                var line = reader.ReadLine();
                line = reader.ReadLine();
                var data = new string[2];
                Author author = new Author();

                while (line != null)
                {
                    data = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                    author = new Author()
                    {
                        FirstName = data[0],
                        LastName = data[1]
                    };

                    context.Authors.Add(author);

                    line = reader.ReadLine();
                }
            }

            context.SaveChanges();

            using (var reader = new StreamReader("SeedData/categories.txt"))
            {
                var line = reader.ReadLine();
                line = reader.ReadLine();
                Category category = new Category();

                while (line != null)
                {
                    category = new Category()
                    {
                        Name = line
                    };

                    context.Categories.Add(category);

                    line = reader.ReadLine();
                }
            }

            context.SaveChanges();

            using (var reader = new StreamReader("SeedData/books.txt"))
            {
                var random = new Random();
                var line = reader.ReadLine();
                line = reader.ReadLine();
                while (line != null)
                {
                    var authors = context.Authors.ToList();
                    var data = line.Split(new[] { ' ' }, 6);
                    var authorIndex = random.Next(0, authors.Count);
                    var author = authors[authorIndex];
                    var edition = (Edition)int.Parse(data[0]);
                    var releaseDate = DateTime.ParseExact(data[1], "d/M/yyyy", CultureInfo.InvariantCulture);
                    var copies = int.Parse(data[2]);
                    var price = decimal.Parse(data[3]);
                    var ageRestriction = (AgeRestriction)int.Parse(data[4]);
                    var title = data[5];

                    context.Books.Add(new Book()
                    {
                        Author = author,
                        Edition = edition,
                        ReleaseDate = releaseDate,
                        Copies = copies,
                        Price = price,
                        AgeRestriction = ageRestriction,
                        Title = title
                    });

                    line = reader.ReadLine();
                }
            }

            context.SaveChanges();

            foreach (var book in context.Books)
            {
                var rnd = new Random();
                var categoriesCount = rnd.Next(1, 10);

                for (int i = 0; i < categoriesCount; i++)
                {
                    book.Categories.Add(context.Categories.Find(rnd.Next(1, 7)));
                }
            }

            context.SaveChanges();
        }
    }
}
