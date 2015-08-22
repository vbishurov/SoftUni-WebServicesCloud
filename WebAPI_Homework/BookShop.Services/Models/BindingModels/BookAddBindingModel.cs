namespace BookShop.Services.Models.BindingModels
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using BookShop.Models;

    public class BookAddBindingModel
    {
        [Required]
        public string Title { get; set; }

        public string Description { get; set; }

        public decimal Price { get; set; }

        public int Copies { get; set; }

        public Edition Edition { get; set; }

        public AgeRestriction AgeRestriction { get; set; }

        public DateTime? ReleaseDate { get; set; }

        public int AuthorId { get; set; }

        [Required]
        public string Categories { get; set; }
    }
}