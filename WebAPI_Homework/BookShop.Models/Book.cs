namespace BookShop.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class Book
    {
        private ICollection<Category> categories;

        public Book()
        {
            this.categories = new HashSet<Category>();
        }

        public int Id { get; set; }

        [MinLength(1)]
        [MaxLength(50)]
        [Required]
        public string Title { get; set; }

        [MaxLength(1000)]
        public string Description { get; set; }

        [Required]
        public Edition Edition { get; set; }

        public decimal Price { get; set; }

        public int Copies { get; set; }

        public DateTime? ReleaseDate { get; set; }

        public int AuthorId { get; set; }

        public AgeRestriction AgeRestriction { get; set; }

        public virtual Author Author { get; set; }

        public virtual ICollection<Category> Categories
        {
            get
            {
                return this.categories;
            }

            set
            {
                this.categories = value;
            }
        }

        public virtual ICollection<Purchase> Purchases { get; set; }
    }
}
