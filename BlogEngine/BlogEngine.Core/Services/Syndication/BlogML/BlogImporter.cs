namespace BlogEngine.Core.API.BlogML
{
    using System;
    using System.Globalization;
    using System.Text.RegularExpressions;

    /// <summary>
    /// Blog Importer
    /// </summary>
    public class BlogImporter
    {
        #region Public Methods

        /// <summary>
        /// Add new blog post to system
        /// </summary>
        /// <returns>
        /// string containing unique post identifier
        /// </returns>
        public string AddPost(BlogMlExtendedPost extPost)
        {
            if (!Security.IsAdministrator)
            {
                throw new InvalidOperationException("BlogImporter.AddPost: Wrong credentials");
            }

            using (var p = new Post())
            {
                p.Title = extPost.BlogPost.Title;
                p.DateCreated = extPost.BlogPost.DateCreated;
                p.DateModified = extPost.BlogPost.DateModified;
                p.Content = extPost.BlogPost.Content.UncodedText;
                p.Description = extPost.BlogPost.Excerpt.UncodedText;
                p.IsPublished = extPost.BlogPost.Approved;

                if (!string.IsNullOrEmpty(extPost.PostUrl))
                {
                    // looking for a Slug with patterns such as:
                    //    /some-slug.aspx
                    //    /some-slug.html
                    //    /some-slug
                    //
                    Match slugMatch = Regex.Match(extPost.PostUrl, @"/([^/\.]+)(?:$|\.[\w]{1,10}$)", RegexOptions.IgnoreCase);
                    if (slugMatch.Success)
                        p.Slug = slugMatch.Groups[1].Value.Trim();
                }

                if(extPost.BlogPost.Authors != null && extPost.BlogPost.Authors.Count > 0)
                    p.Author = extPost.BlogPost.Authors[0].Ref;

                if (extPost.Categories != null && extPost.Categories.Count > 0)
                    p.Categories.AddRange(extPost.Categories);

                if(extPost.Tags != null && extPost.Tags.Count > 0)
                    p.Tags.AddRange(extPost.Tags);

                // skip if post with this url already exists
                var s = PostUrl(p.Slug, p.DateCreated);
                var list = Post.Posts.FindAll(ps => ps.RelativeLink == s);
                if (list.Count > 0)
                {
                    return string.Empty;
                }

                if(extPost.Comments != null && extPost.Comments.Count > 0)
                {
                    foreach (var comment in extPost.Comments)
                    {
                        p.ImportComment(comment);
                    }
                }

                p.Import();
                return p.Id.ToString();
            }
        }

        /// <summary>
        /// Force Reload of all posts
        /// </summary>
        public void ForceReload()
        {
            if (!Security.IsAdministrator)
            {
                throw new InvalidOperationException("BlogImporter.ForeceReload: Wrong credentials");
            }

            Post.Reload();
        }

        /// <summary>
        /// post url
        /// </summary>
        /// <param name="slug">post slug</param>
        /// <param name="dateCreated">date created</param>
        /// <returns></returns>
        static string PostUrl(string slug, DateTime dateCreated)
        {
            var theslug = Utils.RemoveIllegalCharacters(slug) + BlogConfig.FileExtension;

            return BlogSettings.Instance.TimeStampPostLinks
                       ? string.Format(
                           "{0}post/{1}{2}",
                           Utils.RelativeWebRoot,
                           dateCreated.ToString("yyyy/MM/dd/", CultureInfo.InvariantCulture),
                           theslug)
                       : $"{Utils.RelativeWebRoot}post/{theslug}";
        }

        #endregion
    }
}