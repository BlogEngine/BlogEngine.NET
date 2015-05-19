namespace BlogEngine.Core.Data.Models
{
    public class Stats
    {
        public int PublishedPostsCount { get; set; }

        public int DraftPostsCount { get; set; }

        public int PublishedPagesCount { get; set; }

        public int DraftPagesCount { get; set; }

        public int PublishedCommentsCount { get; set; }

        public int UnapprovedCommentsCount { get; set; }

        public int SpamCommentsCount { get; set; }

        public int CategoriesCount { get; set; }

        public int TagsCount { get; set; }

        public int UsersCount { get; set; }

        public string SubscribersCount { get; set; }
    }
}
