using System;
using System.Collections.Generic;
using System.Linq;
using BlogEngine.Core;

namespace BlogEngine.NET.Custom.Widgets
{
    public class MonthList
    {
        class DateComparer_Desc : IComparer<DateTime>
        {
            int IComparer<DateTime>.Compare(DateTime x, DateTime y) { return -x.CompareTo(y); }
        }
        class IntComparer_Desc : IComparer<int>
        {
            int IComparer<int>.Compare(int x, int y) { return -x.CompareTo(y); }
        }

        private static readonly IComparer<DateTime> date_Desc = new DateComparer_Desc();
        private static readonly IComparer<int> int_Desc = new IntComparer_Desc();

        public MonthList() { }

        public SortedDictionary<int, List<MonthItem>> GetList()
        {
            var months = new SortedDictionary<DateTime, int>(date_Desc);
            var years = new SortedDictionary<int, List<MonthItem>>(int_Desc);

            foreach (var month in Post.ApplicablePosts.Where(post => post.IsVisibleToPublic).
                Select(post => new DateTime(post.DateCreated.Year, post.DateCreated.Month, 1)))
            {
                int count;
                months.TryGetValue(month, out count);
                ++count;
                months[month] = count;
            }
            foreach (KeyValuePair<DateTime, int> aIt in months)
            {
                if (!years.Keys.Contains(aIt.Key.Year))
                {
                    years.Add(aIt.Key.Year, new List<MonthItem>());
                }
                var item = new MonthItem();
                item.Title = aIt.Key.ToString("MMMM");
                item.Url = string.Format("{0}{1}/{2}", Utils.RelativeOrAbsoluteWebRoot, aIt.Key.Year, aIt.Key.Month.ToString("00"));
                item.Count = aIt.Value;
                years[aIt.Key.Year].Add(item);
            }
            return years;
        }
    }

    public class MonthItem
    {
        public string Title { get; set; }
        public string Url { get; set; }
        public int Count { get; set; }
    }
}