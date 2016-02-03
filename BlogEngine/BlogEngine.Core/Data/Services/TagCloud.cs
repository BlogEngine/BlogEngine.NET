using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BlogEngine.Core.Data.Services
{
    /// <summary>
    /// Tag cloud
    /// </summary>
    public class TagCloud
    {
        private const string Link = "<a href=\"{0}\" class=\"{1}\" title=\"{2}\">{3}</a> ";
        private static readonly object SyncRoot = new object();
        private static Dictionary<Guid, Dictionary<string, string>> weightedList = new Dictionary<Guid, Dictionary<string, string>>();

        public int MinimumPosts { get; set; }
        public int TagCloudSize { get; set; }

        public TagCloud()
        {
            MinimumPosts = 1;
            TagCloudSize = -1;
        }

        /// <summary>
        /// List of tag links
        /// </summary>
        /// <returns></returns>
        public List<string> Links()
        {
            var links = new List<string>();
            foreach (var key in WeightedList.Keys)
            {
                var link = string.Format(
                    Link,
                    string.Format("{0}?tag={1}", Utils.AbsoluteWebRoot, HttpUtility.UrlEncode(key)),
                    WeightedList[key], string.Format("Tag: {0}", key), key);
                links.Add(link);  
            }
            return links;
        }

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

        private void SortList()
        {
            var dic = CreateRawList();
            var max = dic.Values.Max();

            var currentTagCount = 0;

            int count = currentTagCount;
            foreach (var key in dic.Keys.Where(key => dic[key] >= MinimumPosts).Where(key => TagCloudSize <= 0 || count <= TagCloudSize))
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

    }
}
