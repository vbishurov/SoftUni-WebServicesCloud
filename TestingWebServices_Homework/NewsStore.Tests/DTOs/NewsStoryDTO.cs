namespace NewsStore.Tests.DTOs
{
    using System;

    public class NewsStoryDTO
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Content { get; set; }

        public DateTime PublishDate { get; set; }

        public override bool Equals(object obj)
        {
            if (obj.GetType() != this.GetType())
            {
                return false;
            }

            var otherNewsStory = obj as NewsStoryDTO;

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