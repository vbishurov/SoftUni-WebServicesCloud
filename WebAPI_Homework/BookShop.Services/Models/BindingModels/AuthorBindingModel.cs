namespace BookShop.Services.Models.BindingModels
{
    using System.ComponentModel.DataAnnotations;

    public class AuthorBindingModel
    {
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }
    }
}