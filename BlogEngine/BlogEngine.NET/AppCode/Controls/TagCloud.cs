// --------------------------------------------------------------------------------------------------------------------
// <summary>
//   The tag cloud.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace App_Code.Controls
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using System.Web.UI;

    using BlogEngine.Core;

    using Resources;

    /// <summary>
    /// The tag cloud.
    /// </summary>
    public class TagCloud : Control
    {
        #region Constants and Fields

        /// <summary>
        /// The link format.
        /// </summary>
        private const string Link = "<a href=\"{0}\" class=\"{1}\" title=\"{2}\">{3}</a> ";

        /// <summary>
        ///     The sync root.
        /// </summary>
        private static readonly object SyncRoot = new object();

        /// <summary>
        /// The weighted list.
        /// </summary>
        private static Dictionary<Guid, Dictionary<string, string>> weightedList = new Dictionary<Guid, Dictionary<string, string>>();

        /// <summary>
        /// The minimum posts.
        /// </summary>
        private int minimumPosts = 1;

        /// <summary>
        /// The tag cloud size.
        /// </summary>
        private int tagCloudSize = -1;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets MinimumPosts.
        /// </summary>
        public int MinimumPosts
        {
            get
            {
                return this.minimumPosts;
            }

            set
            {
                this.minimumPosts = value;
            }
        }

        /// <summary>
        /// Gets or sets TagCloudSize.
        /// </summary>
        public int TagCloudSize
        {
            get
            {
                return this.tagCloudSize;
            }

            set
            {
                this.tagCloudSize = value;
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
        /// Outputs server control content to a provided <see cref="T:System.Web.UI.HtmlTextWriter"/> object and stores tracing information about the control if tracing is enabled.
        /// </summary>
        /// <param name="writer">The <see cref="T:System.Web.UI.HtmlTextWriter"/> object that receives the control content.</param>
        public override void RenderControl(HtmlTextWriter writer)
        {
            if (this.WeightedList.Keys.Count == 0)
            {
                writer.Write("<p>{0}</p>", labels.none);
            }

            writer.Write("<ul id=\"tagcloud\" class=\"tagcloud\">");

            foreach (var key in this.WeightedList.Keys)
            {
                writer.Write("<li>");
                writer.Write(
                    string.Format(
                        Link, 
                        string.Format("{0}/tag/{1}", Utils.AbsoluteWebRoot, HttpUtility.UrlEncode(key)), 
                        this.WeightedList[key], 
                        string.Format("Tag: {0}", key), 
                        key));
                writer.Write("</li>");
            }

            writer.Write("</ul>");
            writer.Write(Environment.NewLine);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Builds a raw list of all tags and the number of times
        /// they have been added to a post.
        /// </summary>
        /// <returns>A sorted dictionary of int with string keys.</returns>
        private static SortedDictionary<string, int> CreateRawList()
        {
            var dic = new SortedDictionary<string, int>();
            foreach (var tag in Post.Posts.Where(post => post.IsVisibleToPublic).SelectMany(post => post.Tags))
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

            int count = currentTagCount;
            foreach (var key in
                dic.Keys.Where(key => dic[key] >= this.MinimumPosts).Where(key => this.TagCloudSize <= 0 || count <= this.TagCloudSize))
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