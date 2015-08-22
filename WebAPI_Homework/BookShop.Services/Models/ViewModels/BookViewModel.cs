namespace BookShop.Services.Models.ViewModels
{
    using System;
    using System.Collections.Generic;
    using BookShop.Models;

    public class BookViewModel
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string Author { get; set; }

        public int AuthorId { get; set; }

        public IList<string> Categories { get; set; }

        public decimal Price { get; set; }

        public Edition Edition { get; set; }

        public AgeRestriction AgeRestriction { get; set; }

        public int Copies { get; set; }

        public DateTime? ReleaseDate { get; set; }
    }
}