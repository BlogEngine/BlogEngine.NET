using BlogEngine.Core.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
