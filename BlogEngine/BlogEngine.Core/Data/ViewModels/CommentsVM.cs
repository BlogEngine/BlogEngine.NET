using BlogEngine.Core.Data.Models;
using System.Collections.Generic;

namespace BlogEngine.Core.Data.ViewModels
{
    /// <summary>
    /// Comments view model
    /// </summary>
    public class CommentsVM
    {
        public List<CommentItem> Items { get; set; }
        public CommentItem SelectedItem { get; set; }
        public CommentDetail Detail { get; set; }
    }
}
