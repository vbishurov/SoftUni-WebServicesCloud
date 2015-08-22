namespace OnlineShop.Services.Models.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using OnlineShop.Models;

    public class AdViewModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public decimal Price { get; set; }

        public OwnerViewModel Owner { get; set; }

        public string Type { get; set; }

        public DateTime PostedOn { get; set; }

        public ICollection<CategoryViewModel> Categories { get; set; }

        public static Expression<Func<Ad, AdViewModel>> Project
        {
            get
            {
                return ad => new AdViewModel()
                {
                    Description = ad.Description,
                    Categories = ad.Categories.Select(c => new CategoryViewModel() { Id = c.Id, Name = c.Name }).ToList(),
                    Id = ad.Id,
                    Name = ad.Name,
                    Owner = new OwnerViewModel() { Id = ad.Owner.Id, Username = ad.Owner.UserName },
                    PostedOn = ad.PostedOn,
                    Price = ad.Price,
                    Type = ad.Type.Name
                };
            }
        }
    }
}