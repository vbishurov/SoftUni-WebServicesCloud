namespace BookShop.Models
{
    using System;

    public class Purchase
    {
        public int Id { get; set; }

        public virtual ApplicationUser User { get; set; }

        public int BookId { get; set; }

        public virtual Book Book { get; set; }

        public decimal Price { get; set; }

        public DateTime Date { get; set; }

        public bool IsRecalled { get; set; }
    }
}
