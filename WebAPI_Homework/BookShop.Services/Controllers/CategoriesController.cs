namespace BookShop.Services.Controllers
{
    using System;
    using System.Linq;
    using System.Web.Http;
    using System.Web.OData;
    using BookShop.Models;
    using Data;
    using Models.BindingModels;
    using Models.ViewModels;

    [RoutePrefix("api/categories")]
    public class CategoriesController : ApiController
    {
        private readonly BookShopContext context = BookShopContext.Create();

        [Route("")]
        [EnableQuery]
        public IHttpActionResult GetCategories()
        {
            var categories = this.context.Categories.Select(c => new CategoryViewModel()
            {
                Id = c.Id,
                Name = c.Name
            });

            return this.Ok(categories);
        }

        [Route("{id}")]
        public IHttpActionResult GetCategory(int id)
        {
            var category = this.context.Categories.Where(c => c.Id == id).Select(c => new CategoryViewModel()
            {
                Id = c.Id,
                Name = c.Name
            }).FirstOrDefault();

            return this.Ok(category);
        }

        [Authorize(Roles = "Admin")]
        [Route("{id}")]
        [HttpPut]
        public IHttpActionResult EditCategory([FromUri]int id, CategoryBindingModel categoryBindingModel)
        {
            if (!this.ModelState.IsValid)
            {
                return this.BadRequest(this.ModelState);
            }

            var category = new Category()
            {
                Name = categoryBindingModel.Name
            };

            this.context.Categories.Add(category);

            try
            {
                this.context.SaveChanges();

                return this.Ok("Category added successfully");
            }
            catch (Exception)
            {
                return this.BadRequest("Category already exists");
            }
        }

        [Authorize(Roles = "Admin")]
        [Route("{id}")]
        [HttpDelete]
        public IHttpActionResult DeleteCategory(int id)
        {
            var category = this.context.Categories.FirstOrDefault(c => c.Id == id);

            this.context.Categories.Remove(category);

            try
            {
                this.context.SaveChanges();

                return this.Ok("Category deleted successfully");
            }
            catch (Exception)
            {
                return this.BadRequest();
            }
        }

        [Authorize(Roles = "Admin")]
        [Route("")]
        [HttpPost]
        public IHttpActionResult AddCategory(CategoryBindingModel categoryBindingModel)
        {
            var category = new Category() { Name = categoryBindingModel.Name };

            this.context.Categories.Add(category);

            try
            {
                this.context.SaveChanges();

                return this.Ok("Category added successfully");
            }
            catch (Exception)
            {
                return this.BadRequest();
            }
        }
    }
}