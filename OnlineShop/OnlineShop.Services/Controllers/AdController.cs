namespace OnlineShop.Services.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Http;
    using Microsoft.AspNet.Identity;
    using Models.BindingModels;
    using Models.ViewModels;
    using OnlineShop.Models;

    [RoutePrefix("api/ads")]
    public class AdController : BaseController
    {
        [HttpGet]
        [Route("")]
        public IHttpActionResult GetAds()
        {
            var ads = this.Context.Ads
                .Where(a => a.ClosedOn == null)
                .OrderByDescending(a => a.Type.Id)
                .ThenByDescending(a => a.PostedOn)
                .Select(AdViewModel.Project);

            if (!ads.Any())
            {
                return this.Ok("No ads found");
            }

            return this.Ok(ads);
        }

        [HttpPost]
        [Authorize]
        [Route("")]
        public IHttpActionResult AddAd([FromBody]CreateAdBingindModel model)
        {
            var userId = this.User.Identity.GetUserId();

            if (!this.ModelState.IsValid)
            {
                return this.BadRequest(this.ModelState);
            }

            if (!model.Categories.Any())
            {
                return this.BadRequest("Please supply categories");
            }

            if (model.Categories.Count() > 3)
            {
                return this.BadRequest("Please specify no more than 3 categories");
            }

            var type = this.Context.AdTypes.FirstOrDefault(at => at.Id == model.TypeId);

            if (type == null)
            {
                return this.BadRequest("Invalid type id");
            }

            Category category;
            List<Category> categories = new List<Category>();
            foreach (int categoryId in model.Categories)
            {
                category = this.Context.Categories.FirstOrDefault(c => c.Id == categoryId);

                if (category == null)
                {
                    return this.BadRequest(string.Format("Category id {0} is invalid.", categoryId));
                }

                categories.Add(category);
            }

            var ad = new Ad()
            {
                Categories = categories,
                ClosedOn = null,
                Description = model.Description,
                Name = model.Name,
                OwnerId = userId,
                Price = model.Price,
                PostedOn = DateTime.Now,
                Status = AdStatus.Open,
                TypeId = model.TypeId
            };

            this.Context.Ads.Add(ad);

            try
            {
                this.Context.SaveChanges();

                var newEntry = this.Context.Ads
                    .Where(a => a.Id == ad.Id)
                    .Select(AdViewModel.Project)
                    .FirstOrDefault();

                return this.Ok(newEntry);
            }
            catch (Exception e)
            {
                return this.InternalServerError(e);
            }
        }

        [HttpPut]
        [Authorize]
        [Route("{id}/close")]
        public IHttpActionResult CloseAd([FromUri]int id)
        {
            var ad = this.Context.Ads.FirstOrDefault(a => a.Id == id);

            if (ad == null)
            {
                return this.BadRequest(string.Format("Ad with id {0} does not exist", id));
            }

            var userId = this.User.Identity.GetUserId();

            if (ad.OwnerId != userId)
            {
                return this.BadRequest(string.Format("Ad with id {0} does not belong to user with id {1}".Insert(id, userId)));
            }

            ad.Status = AdStatus.Closed;
            ad.ClosedOn = DateTime.Now;

            try
            {
                this.Context.SaveChanges();

                return this.Ok(string.Format("Ad {0} closed successfully", id));
            }
            catch (Exception e)
            {
                return this.InternalServerError(e);
            }
        }
    }
}