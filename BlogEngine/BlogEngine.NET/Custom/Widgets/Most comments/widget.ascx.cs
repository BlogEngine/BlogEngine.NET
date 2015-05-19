// --------------------------------------------------------------------------------------------------------------------
// <summary>
//   The most comments widget.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Widgets.MostComments
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Web;
    using System.Web.UI.WebControls;

    using App_Code.Controls;

    using BlogEngine.Core;

    using Resources;

    /// <summary>
    /// The visitor.
    /// </summary>
    public struct Visitor
    {
        #region Constants and Fields

        /// <summary>
        ///     The comments.
        /// </summary>
        public int Comments;

        /// <summary>
        ///     The country.
        /// </summary>
        public string Country;

        /// <summary>
        ///     The email.
        /// </summary>
        public string Email;

        /// <summary>
        ///     The visitor name.
        /// </summary>
        public string Name;

        /// <summary>
        ///     The website.
        /// </summary>
        public Uri Website;

        #endregion
    }

    /// <summary>
    /// The most comments widget.
    /// </summary>
    public partial class Widget : WidgetBase
    {
        #region Constants and Fields

        /// <summary>
        ///     The avatar size.
        /// </summary>
        private int avatarSize = 50;

        /// <summary>
        ///     The 60 days.
        /// </summary>
        private int days = 60;

        /// <summary>
        ///     The number of visitors.
        /// </summary>
        private int numberOfVisitors = 3;

        /// <summary>
        ///     The show comments.
        /// </summary>
        private bool showComments = true;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes static members of the <see cref = "Widget" /> class.
        /// </summary>
        static Widget()
        {
            Post.CommentAdded += ClearCache;
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Gets whether the widget can be edited.
        ///     <remarks>
        ///         The only way a widget can be editable is by adding a edit.ascx file to the widget folder.
        ///     </remarks>
        /// </summary>
        public override bool IsEditable
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        ///     Gets the name. It must be exactly the same as the folder that contains the widget.
        /// </summary>
        public override string Name
        {
            get
            {
                return "Most comments";
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// This method works as a substitute for Page_Load. You should use this method for
        ///     data binding etc. instead of Page_Load.
        /// </summary>
        public override void LoadWidget()
        {
            this.LoadSettings();

            var visitors = Blog.CurrentInstance.Cache["most_comments"] as List<Visitor>;

            if (visitors == null)
            {
                visitors = BuildList();
                Blog.CurrentInstance.Cache.Insert("most_comments", visitors);
            }

            this.rep.ItemDataBound += this.RepItemDataBound;
            this.rep.DataSource = visitors;
            this.rep.DataBind();
        }

        #endregion

        #region Methods

        private static void ClearCache(object sender, EventArgs e)
        {
            Blog.CurrentInstance.Cache.Remove("most_comments");

            Blog siteAggregationBlog = Blog.SiteAggregationBlog;
            if (siteAggregationBlog != null)
            {
                siteAggregationBlog.Cache.Remove("most_comments");
            }
        }

        /// <summary>
        /// Finds the visitor.
        /// </summary>
        /// <param name="email">
        /// The email.
        /// </param>
        /// <param name="comments">
        /// The comments.
        /// </param>
        /// <returns>
        /// The Visitor.
        /// </returns>
        private static Visitor FindVisitor(string email, int comments)
        {
            foreach (var visitor in from post in Post.ApplicablePosts
                                    from comment in post.ApprovedComments
                                    where comment.Email == email
                                    select
                                        new Visitor
                                            {
                                                Name = comment.Author, 
                                                Country = comment.Country, 
                                                Website = comment.Website, 
                                                Comments = comments, 
                                                Email = email
                                            })
            {
                return visitor;
            }

            return new Visitor();
        }

        /// <summary>
        /// Sorts the dictionary.
        /// </summary>
        /// <param name="dic">
        /// The dictionary.
        /// </param>
        /// <returns>
        /// The sorted dictionary.
        /// </returns>
        private static Dictionary<string, int> SortDictionary(Dictionary<string, int> dic)
        {
            var list = dic.Keys.Select(key => new KeyValuePair<string, int>(key, dic[key])).ToList();

            list.Sort((obj1, obj2) => obj2.Value.CompareTo(obj1.Value));

            return list.ToDictionary(pair => pair.Key, pair => pair.Value);
        }

        /// <summary>
        /// Builds the list.
        /// </summary>
        private List<Visitor> BuildList()
        {
            var visitors = new Dictionary<string, int>();
            foreach (var comment in from post in Post.ApplicablePosts
                                    from comment in post.ApprovedComments
                                    where
                                        (comment.DateCreated.AddDays(this.days) >= DateTime.Now &&
                                         !string.IsNullOrEmpty(comment.Email)) && comment.Email.Contains("@")
                                    where !post.Author.Equals(comment.Author, StringComparison.OrdinalIgnoreCase)
                                    select comment)
            {
                if (visitors.ContainsKey(comment.Email))
                {
                    visitors[comment.Email] += 1;
                }
                else
                {
                    visitors.Add(comment.Email, 1);
                }
            }

            visitors = SortDictionary(visitors);
            
            // var max = Math.Min(visitors.Count, this.numberOfVisitors);
            var count = 0;
            var list = new List<Visitor>();

            foreach (var v in visitors.Keys.Select(key => FindVisitor(key, visitors[key])))
            {
                list.Add(v);

                count++;

                if (count == this.numberOfVisitors)
                {
                    break;
                }
            }

            return list;
        }

        /// <summary>
        /// The load settings.
        /// </summary>
        private void LoadSettings()
        {
            var settings = this.GetSettings();
            try
            {
                if (settings.ContainsKey("avatarsize"))
                {
                    this.avatarSize = int.Parse(settings["avatarsize"]);
                }

                if (settings.ContainsKey("numberofvisitors"))
                {
                    this.numberOfVisitors = int.Parse(settings["numberofvisitors"]);
                }

                if (settings.ContainsKey("days"))
                {
                    this.days = int.Parse(settings["days"]);
                }

                if (settings.ContainsKey("showcomments"))
                {
                    this.showComments = settings["showcomments"].Equals("true", StringComparison.OrdinalIgnoreCase);
                }
            }
            catch
            {
            }
        }

        /// <summary>
        /// Handles the ItemDataBound event of the rep control.
        /// </summary>
        /// <param name="sender">
        /// The source of the event.
        /// </param>
        /// <param name="e">
        /// The <see cref="System.Web.UI.WebControls.RepeaterItemEventArgs"/> instance containing the event data.
        /// </param>
        private void RepItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem)
            {
                return;
            }

            var visitor = (Visitor)e.Item.DataItem;
            var imgAvatar = (Image)e.Item.FindControl("imgAvatar");
            var imgCountry = (Image)e.Item.FindControl("imgCountry");
            var name = (Literal)e.Item.FindControl("litName");
            var number = (Literal)e.Item.FindControl("litNumber");
            var litCountry = (Literal)e.Item.FindControl("litCountry");

            imgAvatar.ImageUrl = BlogEngine.Core.Data.Services.Avatar.GetSrc(visitor.Email);
            imgAvatar.AlternateText = visitor.Name;
            imgAvatar.Width = Unit.Pixel(this.avatarSize);

            if (this.showComments)
            {
                number.Text = string.Format("{0} {1}<br />", visitor.Comments, labels.comments.ToLowerInvariant());
            }

            if (visitor.Website != null)
            {
                const string LinkFormat = "<a rel=\"nofollow contact\" class=\"url fn\" href=\"{0}\">{1}</a>";
                name.Text = string.Format(LinkFormat, visitor.Website, visitor.Name);
            }
            else
            {
                name.Text = string.Format("<span class=\"fn\">{0}</span>", visitor.Name);
            }

            if (!string.IsNullOrEmpty(visitor.Country))
            {
                imgCountry.ImageUrl = string.Format("{0}Content/images/blog/flags/{1}.png", Utils.RelativeOrAbsoluteWebRoot, visitor.Country);
                imgCountry.AlternateText = visitor.Country;

                foreach (var ri in
                    CultureInfo.GetCultures(CultureTypes.SpecificCultures).Select(ci => new RegionInfo(ci.Name)).Where(
                        ri => ri.TwoLetterISORegionName.Equals(visitor.Country, StringComparison.OrdinalIgnoreCase)))
                {
                    litCountry.Text = ri.DisplayName;
                    break;
                }
            }
            else
            {
                imgCountry.Visible = false;
            }
        }

        #endregion
    }
}