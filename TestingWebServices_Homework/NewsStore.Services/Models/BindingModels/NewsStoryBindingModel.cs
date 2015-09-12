namespace NewsStore.Services.Models.BindingModels
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class NewsStoryBindingModel
    {
        [Required]
        [MinLength(4)]
        [MaxLength(50)]
        public string Title { get; set; }

        public string Content { get; set; }

        public DateTime PublishDate { get; set; }
    }
}