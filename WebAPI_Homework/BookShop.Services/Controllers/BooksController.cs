namespace BookShop.Services.Controllers
{
    using System;
    using System.Linq;
    using System.Security.Claims;
    using System.Security.Principal;
    using System.Web;
    using System.Web.Http;
    using System.Web.OData;
    using BookShop.Models;
    using Data;
    using Microsoft.AspNet.Identity;
    using Models.BindingModels;
    using Models.ViewModels;

    [RoutePrefix("api/books")]
    public class BooksController : ApiController
    {
        private readonly BookShopContext context = BookShopContext.Create();

        [Route("{id}")]
        public IHttpActionResult GetBook(int id)
        {
            var book = this.context.Books.Where(b => b.Id == id).Select(b => new BookViewModel()
            {
                AgeRestriction = b.AgeRestriction,
                Author = b.Author.FirstName + " " + b.Author.LastName,
                AuthorId = b.AuthorId,
                Categories = b.Categories.Select(c => c.Name).ToList(),
                Copies = b.Copies,
                Description = string.IsNullOrEmpty(b.Description) ? "No Description" : b.Description,
                Edition = b.Edition,
                Id = b.Id,
                Price = b.Price,
                ReleaseDate = b.ReleaseDate,
                Title = b.Title

            }).FirstOrDefault();

            if (book == null)
            {
                return this.NotFound();
            }

            return this.Ok(book);
        }

        [HttpGet]
        [EnableQuery]
        [Route("")]
        public IHttpActionResult SearchBooks(string search)
        {
            if (string.IsNullOrEmpty(search))
            {
                return this.BadRequest("Search keyword is mandatory");
            }

            var books = this.context.Books.Where(b => b.Title.Contains(search)).OrderBy(b => b.Title).Select(b => new SearchBookViewModel()
            {
                Title = b.Title,
                Id = b.Id
            }).Take(10);

            if (!books.Any())
            {
                return this.NotFound();
            }

            return this.Ok(books);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut]
        [Route("{id}")]
        public IHttpActionResult EditBook([FromUri]int id, [FromBody]BookEditBindingModel bookBindingModel)
        {
            if (!this.ModelState.IsValid)
            {
                return this.BadRequest(this.ModelState);
            }

            var book = this.context.Books.FirstOrDefault(b => b.Id == id);

            if (book == null)
            {
                return this.NotFound();
            }

            book.Title = bookBindingModel.Title;
            book.Description = bookBindingModel.Description;
            book.Price = bookBindingModel.Price;
            book.Edition = bookBindingModel.Edition;
            book.AgeRestriction = bookBindingModel.AgeRestriction;
            book.Copies = bookBindingModel.Copies;
            book.AuthorId = bookBindingModel.AuthorId;
            book.ReleaseDate = bookBindingModel.ReleaseDate;

            try
            {
                this.context.SaveChanges();

                return this.Ok("Book edited successfully");
            }
            catch (Exception)
            {
                return this.BadRequest();
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete]
        [Route("{id}")]
        public IHttpActionResult DeleteBook([FromUri]int id)
        {
            if (!this.ModelState.IsValid)
            {
                return this.BadRequest(this.ModelState);
            }

            var book = this.context.Books.FirstOrDefault(b => b.Id == id);

            if (book == null)
            {
                return this.NotFound();
            }

            this.context.Books.Remove(book);

            try
            {
                this.context.SaveChanges();

                return this.Ok("Book deleted successfully");
            }
            catch (Exception)
            {
                return this.BadRequest();
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [Route("")]
        public IHttpActionResult AddBook(BookAddBindingModel bookAddBindingModel)
        {
            if (!this.ModelState.IsValid)
            {
                return this.BadRequest(this.ModelState);
            }

            var categoryNames = bookAddBindingModel.Categories.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries);

            var categories = this.context.Categories.Where(c => categoryNames.Contains(c.Name)).ToList();

            this.context.Books.Add(new Book()
            {
                Title = bookAddBindingModel.Title,
                AuthorId = bookAddBindingModel.AuthorId,
                Categories = categories,
                Copies = bookAddBindingModel.Copies,
                Description = bookAddBindingModel.Description,
                AgeRestriction = bookAddBindingModel.AgeRestriction,
                Edition = bookAddBindingModel.Edition,
                Price = bookAddBindingModel.Price,
                ReleaseDate = bookAddBindingModel.ReleaseDate
            });

            try
            {
                this.context.SaveChanges();

                return this.Ok("Book added successfully");
            }
            catch (Exception)
            {
                return this.BadRequest();
            }
        }

        [Authorize(Roles = "Admin")]
        [Route("buy/{id}")]
        [HttpPut]
        [Authorize]
        public IHttpActionResult BuyBook([FromUri]int id)
        {
            var book = this.context.Books.FirstOrDefault(b => b.Id == id);

            if (book == null)
            {
                return this.NotFound();
            }

            var rnd = new Random();

            var purchase = new Purchase()
            {
                Book = book,
                BookId = book.Id,
                Date = DateTime.Now.AddDays(rnd.Next(-100, 0)),
                IsRecalled = false,
                Price = book.Price,
                User = this.context.Users.FirstOrDefault(u => u.UserName == this.User.Identity.Name)
            };

            book.Copies--;

            this.context.Purchases.Add(purchase);

            try
            {
                this.context.SaveChanges();

                return this.Ok("Book purchased successfully");
            }
            catch (Exception)
            {
                return this.BadRequest();
            }
        }

        [Authorize(Roles = "Admin")]
        [Route("recall/{id}")]
        [HttpPut]
        [Authorize]
        public IHttpActionResult RecallBook([FromUri]int id)
        {
            var book = this.context.Books.FirstOrDefault(b => b.Id == id);

            if (book == null)
            {
                return this.NotFound();
            }

            var purchase = book.Purchases.Where(p => p.User.Id == this.User.Identity.GetUserId() && !p.IsRecalled).OrderByDescending(b => b.Date).FirstOrDefault();

            if (purchase == null)
            {
                return this.BadRequest();
            }

            if (purchase.Date < DateTime.Now.AddDays(-30))
            {
                return this.BadRequest("You can not recall the purchase. 30 days have passed");
            }

            purchase.IsRecalled = true;
            book.Copies++;

            try
            {
                this.context.SaveChanges();

                return this.Ok("Purchase recalled successfully");
            }
            catch (Exception)
            {
                return this.BadRequest();
            }
        }
    }
}