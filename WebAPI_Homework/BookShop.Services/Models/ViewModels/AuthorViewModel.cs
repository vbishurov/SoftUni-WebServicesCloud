namespace BookShop.Services.Models.ViewModels
{
    using System.Collections.Generic;

    public class AuthorViewModel
    {
        public int Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public IList<string> BookTitles { get; set; }
    }
}