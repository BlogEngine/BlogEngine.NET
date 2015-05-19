// --------------------------------------------------------------------------------------------------------------------
// <summary>
//   The tag cloud widget.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Widgets.TagCloud
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.UI.HtmlControls;

    using App_Code.Controls;

    using BlogEngine.Core;

    /// <summary>
    /// The tag cloud widget.
    /// </summary>
    public partial class WidgetsTagCloudWidget : WidgetBase
    {
        #region Constants and Fields

        /// <summary>
        ///     The link format.
        /// </summary>
        private const string LinkFormat = "<a href=\"{0}\" class=\"{1}\" title=\"{2}\">{3}</a> ";

        /// <summary>
        ///     The sync root.
        /// </summary>
        private static readonly object SyncRoot = new object();

        /// <summary>
        ///     The weighted list.
        /// </summary>
        private static Dictionary<Guid, Dictionary<string, string>> weightedList = new Dictionary<Guid, Dictionary<string, string>>();

        /// <summary>
        ///     The minimum posts.
        /// </summary>
        private int minimumPosts = 1;

        /// <summary>
        ///     The tag cloud size.
        /// </summary>
        private int tagCloudSize = -1;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes static members of the <see cref = "WidgetsTagCloudWidget" /> class.
        /// </summary>
        static WidgetsTagCloudWidget()
        {
            Post.Saved += (sender, args) => Reload();
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Gets wether or not the widget can be edited.
        ///     <remarks>
        ///         The only way a widget can be editable is by adding a edit.ascx file to the widget folder.
        ///     </remarks>
        /// </summary>
        /// <value></value>
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
        /// <value></value>
        public override string Name
        {
            get
            {
                return "Tag cloud";
            }
        }

        /// <summary>
        ///     Gets MinimumPosts.
        /// </summary>
        private int MinimumPosts
        {
            get
            {
                var settings = this.GetSettings();
                if (settings.ContainsKey("minimumposts"))
                {
                    int.TryParse(settings["minimumposts"], out this.minimumPosts);
                }

                return this.minimumPosts;
            }
        }

        /// <summary>
        ///     Gets TagCloudSize.
        /// </summary>
        private int TagCloudSize
        {
            get
            {
                var settings = this.GetSettings();
                if (settings.ContainsKey("tagcloudsize"))
                {
                    int.TryParse(settings["tagcloudsize"], out this.tagCloudSize);
                }

                return this.tagCloudSize;
            }
        }

        /// <summary>
        ///     Gets WeightedList.
        /// </summary>
        private Dictionary<string, string> WeightedList
        {
            get
            {
                Dictionary<string, string> list = null;
                Guid blogId = Blog.CurrentInstance.Id;

                if (!weightedList.TryGetValue(blogId, out list))
                {
                    lock (SyncRoot)
                    {
                        if (!weightedList.TryGetValue(blogId, out list))
                        {
                            list = new Dictionary<string, string>();
                            weightedList.Add(blogId, list);

                            this.SortList();
                        }
                    }
                }

                return list;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Reloads this instance.
        /// </summary>
        public static void Reload()
        {
            lock (SyncRoot)
            {
                if (weightedList.ContainsKey(Blog.CurrentInstance.Id))
                {
                    weightedList.Remove(Blog.CurrentInstance.Id);
                }

                Blog siteAggregationBlog = Blog.SiteAggregationBlog;
                if (siteAggregationBlog != null && weightedList.ContainsKey(siteAggregationBlog.Id))
                {
                    weightedList.Remove(siteAggregationBlog.Id);
                }
            }
        }

        /// <summary>
        /// This method works as a substitute for Page_Load. You should use this method for
        ///     data binding etc. instead of Page_Load.
        /// </summary>
        public override void LoadWidget()
        {
            foreach (var key in this.WeightedList.Keys)
            {
                using (var li = new HtmlGenericControl("li"))
                {
                    li.InnerHtml = string.Format(
                        LinkFormat,
                        string.Format("{0}/tag/{1}", Utils.AbsoluteWebRoot, Utils.RemoveIllegalCharacters(key)),
                        this.WeightedList[key],
                        Resources.labels.Tag + ": " + key,
                        key);
                    this.ulTags.Controls.Add(li);
                }
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Builds a raw list of all tags and the number of times
        ///     they have been added to a post.
        /// </summary>
        /// <returns>
        /// The create raw list.
        /// </returns>
        private static SortedDictionary<string, int> CreateRawList()
        {
            var dic = new SortedDictionary<string, int>(StringComparer.InvariantCultureIgnoreCase);
            foreach (var tag in Post.ApplicablePosts.Where(post => post.IsVisibleToPublic).SelectMany(post => post.Tags))
            {
                if (dic.ContainsKey(tag))
                {
                    dic[tag]++;
                }
                else
                {
                    dic[tag] = 1;
                }
            }

            return dic;
        }

        /// <summary>
        /// Sorts the list of tags based on how much they are used.
        /// </summary>
        private void SortList()
        {
            var dic = CreateRawList();
            var max = dic.Values.Max();

            var currentTagCount = 0;

            foreach (var key in
                dic.Keys.Where(key => dic[key] >= this.MinimumPosts).Where(
                    key => this.TagCloudSize <= 0 || currentTagCount < this.TagCloudSize))
            {
                currentTagCount++;

                var weight = ((double)dic[key] / max) * 100;
                if (weight >= 99)
                {
                    WeightedList.Add(key, "biggest");
                }
                else if (weight >= 70)
                {
                    WeightedList.Add(key, "big");
                }
                else if (weight >= 40)
                {
                    WeightedList.Add(key, "medium");
                }
                else if (weight >= 20)
                {
                    WeightedList.Add(key, "small");
                }
                else if (weight >= 3)
                {
                    WeightedList.Add(key, "smallest");
                }
            }
        }

        #endregion
    }
}