namespace NewsStore.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class NewsStory
    {
        public int Id { get; set; }

        [Required]
        [MinLength(4)]
        [MaxLength(50)]
        [Index("IX_Title", IsUnique = true)]
        public string Title { get; set; }

        public string Content { get; set; }

        public DateTime PublishDate { get; set; }

        public override bool Equals(object obj)
        {
            if (obj.GetType() != this.GetType())
            {
                return false;
            }

            var otherNewsStory = obj as NewsStory;

            if (this.Title != otherNewsStory.Title)
            {
                return false;
            }

            if (this.Content != otherNewsStory.Content)
            {
                return false;
            }

            if (this.PublishDate != otherNewsStory.PublishDate)
            {
                return false;
            }

            return true;
        }
    }
}
