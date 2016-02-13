using BlogEngine.Core;
using System.Collections.Generic;
using System.Linq;

namespace BlogEngine.NET.Custom.Widgets
{
    public class CategoryList
    {
        private static bool HasPosts(Category cat)
        {
            return
                Post.ApplicablePosts.Where(post => post.IsVisible).SelectMany(post => post.Categories).Any(
                    category => category == cat);
        }

        /// <summary>
        /// Sorted list of categories
        /// </summary>
        /// <returns>List of categories</returns>
        public static SortedDictionary<string, Category> SortCategories()
        {
            var dic = new SortedDictionary<string, Category>();
            foreach (var cat in Category.ApplicableCategories.Where(HasPosts))
            {
                if (!dic.ContainsKey(cat.CompleteTitle()))
                {
                    dic.Add(cat.CompleteTitle(), cat);
                }
            }
            return dic;
        }
    }
}