namespace BookShop.Services.Controllers
{
    using System;
    using System.Linq;
    using System.Web.Http;
    using BookShop.Models;
    using Data;
    using Models.BindingModels;
    using Models.ViewModels;

    [RoutePrefix("api/authors")]
    public class AuthorsController : ApiController
    {
        private readonly BookShopContext context = BookShopContext.Create();

        [Route("{id}")]
        public IHttpActionResult GetAuthor(int id)
        {
            var author = this.context.Authors.Where(a => a.Id == id).Select(a => new AuthorViewModel()
            {
                Id = a.Id,
                FirstName = a.FirstName,
                LastName = a.LastName,
                BookTitles = a.Books.Select(b => b.Title).ToList()
            }).FirstOrDefault();

            if (author == null)
            {
                return this.NotFound();
            }

            return this.Ok(author);
        }

        [Authorize(Roles = "Admin")]
        [Route("")]
        public IHttpActionResult CreateUser(AuthorBindingModel author)
        {
            if (!this.ModelState.IsValid)
            {
                return this.BadRequest(this.ModelState);
            }

            try
            {
                this.context.Authors.Add(new Author()
                {
                    FirstName = author.FirstName,
                    LastName = author.LastName
                });

                this.context.SaveChanges();

                return this.Ok("Author created successfully");
            }
            catch (Exception)
            {
                return this.BadRequest("There was a problem creating author. Please try again!");
            }
        }

        [Route("{id}/books")]
        public IHttpActionResult GetBooksByAuthor(int id)
        {
            var books = this.context.Authors.Where(a => a.Id == id)
                .Select(a => a.Books.Select(b => new BookViewModel()
                {
                    AgeRestriction = b.AgeRestriction,
                    AuthorId = b.AuthorId,
                    Categories = b.Categories.Select(c => c.Name).ToList(),
                    Copies = b.Copies,
                    Description = string.IsNullOrEmpty(b.Description) ? "No Description" : b.Description,
                    Edition = b.Edition,
                    Id = b.Id,
                    Price = b.Price,
                    ReleaseDate = b.ReleaseDate,
                    Title = b.Title,
                    Author = a.FirstName + " " + a.LastName
                }));

            return this.Ok(books);
        }
    }
}