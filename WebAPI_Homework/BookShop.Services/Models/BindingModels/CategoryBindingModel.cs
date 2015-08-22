namespace BookShop.Services.Models.BindingModels
{
    using System.ComponentModel.DataAnnotations;

    public class CategoryBindingModel
    {
        [Required]
        [MaxLength(50)]
        public string Name { get; set; }
    }
}