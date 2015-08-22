namespace BookShop.Services.Models.ViewModels
{
    using System.ComponentModel.DataAnnotations;

    public class CategoryViewModel
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }
    }
}