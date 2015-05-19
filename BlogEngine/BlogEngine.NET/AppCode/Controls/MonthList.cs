// --------------------------------------------------------------------------------------------------------------------
// <summary>
//   Builds a category list.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace App_Code.Controls
{
	using System;
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.Linq;
	using System.Web;
	using System.Web.Caching;
	using System.Web.UI;

	using Resources;

	using BlogEngine.Core;


	/// <summary>
	/// Displays a list of hyperlinks each of which points at a page
	/// containing blog posts written within a particular month.
	/// </summary>
	public class MonthList : Control
	{
		#region Nested Classes

		/// <summary>
		/// Specifies the way <see cref="App_Code.Controls.MonthList" /> control is displayed.
		/// </summary>
		[Flags]
		public enum MonthListDisplayOptions
		{
			/// <summary>
			/// A convenience constant that corresponds to default <see cref="App_Code.Controls.MonthList" />
			/// behavior where grouping by year is performed.
			/// </summary>
			Grouping_ByYear = 0,

			/// <summary>
			/// When set, the <see cref="App_Code.Controls.MonthList" /> control renders
			/// a flat month list, without grouping by year.
			/// </summary>
			Grouping_Flat = 1,

			/// <summary>
			/// A convenience constant that corresponds to default <see cref="App_Code.Controls.MonthList" />
			/// behavior where more recent dates appear at the bottom of the list.
			/// </summary>
			Sorting_RecentAtBottom = 0,

			/// <summary>
			/// When set, the the <see cref="App_Code.Controls.MonthList" /> control renders
			/// a month list where more recent dates appear at the top of the list.
			/// </summary>
			Sorting_RecentAtTop = 2
		}

		/// <summary>
		/// Represents a <see cref="System.DateTime" /> comparer that performs
		/// direct date comparison.
		/// </summary>
		class DateComparer_Asc : IComparer<DateTime>
		{
			int IComparer<DateTime>.Compare(DateTime x, DateTime y)
			{ return x.CompareTo(y); }
		}

		/// <summary>
		/// Represents a <see cref="System.DateTime" /> comparer that performs
		/// inverse date comparison.
		/// </summary>
		class DateComparer_Desc : IComparer<DateTime>
		{
			int IComparer<DateTime>.Compare(DateTime x, DateTime y)
			{ return -x.CompareTo(y); }
		}
		#endregion

		#region Constants and Fields

		/// <summary>
		/// The cache key.
		/// </summary>
		private const string CacheKey = "BE_MonthListCacheKey";

		/// <summary>
		/// The cache timeout in hours.
		/// </summary>
		private const double CacheTimeoutInHours = 1;

		/// <summary>
		/// The global instance of the direct date comparer.
		/// </summary>
		private static readonly IComparer<DateTime> ourComparer_Asc = new DateComparer_Asc();

		/// <summary>
		/// The global instance of the inverse date comparer.
		/// </summary>
		private static readonly IComparer<DateTime> ourComparer_Desc = new DateComparer_Desc();

		/// <summary>
		/// Stores the current year so that it remains constant while the control is being rendered.
		/// </summary>
		private int myCurrentYear;
		#endregion

		#region Constructors and Destructors

		/// <summary>
		/// Initializes static members of the <see cref="MonthList" /> class.
		/// </summary>
		static MonthList()
		{
			Post.Saved += Handler_Post_Saved;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="MonthList" /> class.
		/// </summary>
		public MonthList()
		{
			myCurrentYear = DateTime.Now.Year;
		}

		#endregion

		#region Properties

		/// <summary>
		/// Specifies whether the month lit should be render hierarchically, grouped by year.
		/// </summary>
		[DefaultValue(true)]
		public bool GroupByYear
		{
			get
			{
				return (DisplayFlags & (int)MonthListDisplayOptions.Grouping_Flat) == 0;
			}
			set
			{
				if (value) DisplayFlags &= ~(int)MonthListDisplayOptions.Grouping_Flat;
				else DisplayFlags |= (int)MonthListDisplayOptions.Grouping_Flat;
			}
		}

		/// <summary>
		/// Specifies the way months should be sorted: recent dates at top or at bottom.
		/// </summary>
		[DefaultValue(false)]
		public bool RecentDatesAtTop
		{
			get
			{
				return (DisplayFlags & (int)MonthListDisplayOptions.Sorting_RecentAtTop) != 0;
			}
			set
			{
				if (value) DisplayFlags |= (int)MonthListDisplayOptions.Sorting_RecentAtTop;
				else DisplayFlags &= ~(int)MonthListDisplayOptions.Sorting_RecentAtTop;
			}
		}

		/// <summary>
		/// Stores the controls display options in a compact manner.
		/// </summary>
		public virtual int DisplayFlags
		{
			get
			{
				object aVal = ViewState["DisplayFlags"];

				if (aVal == null)
					return 0;
				else
					return (int)aVal;
			}
			set { ViewState["DisplayFlags"] = value; }
		}
		#endregion

		#region Public Methods

		/// <summary>
		/// Outputs server control content to a provided <see cref="T:System.Web.UI.HtmlTextWriter"/>
		/// object and stores tracing information about the control if tracing is enabled.
		/// </summary>
		/// <param name="writer">
		/// The <see cref="T:System.Web.UI.HtmlTextWriter"/> object that receives the control content.
		/// </param>
		public override void RenderControl(HtmlTextWriter writer)
		{
			SortedDictionary<DateTime, int> months = GetPostsPerMonth(RecentDatesAtTop);

			if (months.Keys.Count == 0)
			{
				writer.RenderBeginTag(HtmlTextWriterTag.P);
				writer.Write(Resources.labels.none);
				writer.RenderEndTag();

				return;
			}

			writer.AddAttribute("id", "monthList");
			writer.RenderBeginTag(HtmlTextWriterTag.Ul);


			if (GroupByYear)
			{
				int current = 0;

				foreach (KeyValuePair<DateTime, int> aIt in months)
				{
					if (current != aIt.Key.Year)
					{
						if (current != 0)
							RenderYearGroupEnd(writer);

						RenderYearGroupBegin(writer, aIt.Key.Year);

						current = aIt.Key.Year;
					}

					RenderMonth(writer, aIt.Key, aIt.Value, false);
				}

				if (current != 0)
					RenderYearGroupEnd(writer);
			}
			else
			{
				foreach (KeyValuePair<DateTime, int> aIt in months)
					RenderMonth(writer, aIt.Key, aIt.Value, true);
			}

			writer.RenderEndTag();
		}

		#endregion

		#region Implementation-Specific Methods

		/// <summary>Obtains a collection of months with a corresponding month post count.</summary>
		/// <returns>
		/// A <see cref="T:System.Collections.Generic.SortedDictionary<DateTime, int>" />
		/// whose keys represent the first day of a month and values contain the month post count.
		/// </returns>
		private static SortedDictionary<DateTime, int> GetPostsPerMonth(bool theSortDesc)
		{
			var months = Blog.CurrentInstance.Cache[CacheKey] as SortedDictionary<DateTime, int>;
			IComparer<DateTime> aComparer = theSortDesc ? ourComparer_Desc : ourComparer_Asc;

			if (months != null)
			{
				if (months.Comparer != aComparer)
				{
                    Blog.CurrentInstance.Cache.Remove(CacheKey);
					months = null;
				}
			}

			if (months == null)
			{
				months = new SortedDictionary<DateTime, int>(aComparer);

				// let dictionary expire after 1 hour
				Blog.CurrentInstance.Cache.Insert(
						CacheKey, months, null, DateTime.Now.AddHours(CacheTimeoutInHours), Cache.NoSlidingExpiration);

				foreach (
					var month in Post.ApplicablePosts.Where(post => post.IsVisibleToPublic).Select(
						post => new DateTime(post.DateCreated.Year, post.DateCreated.Month, 1)))
				{
					int count;

					// if the date is not in the dictionary, count will be set to 0
					months.TryGetValue(month, out count);
					++count;
					months[month] = count;
				}
			}

			return months;
		}

		/// <summary>
		/// Handles the Saved event of the Post control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="BlogEngine.Core.SavedEventArgs"/> instance containing the event data.</param>
		private static void Handler_Post_Saved(object sender, SavedEventArgs e)
		{
			// invalidate cache whenever a post is modified
			Blog.CurrentInstance.Cache.Remove(CacheKey);

            Blog siteAggregationBlog = Blog.SiteAggregationBlog;
            if (siteAggregationBlog != null)
            {
                siteAggregationBlog.Cache.Remove(CacheKey);
            }
		}
		
		#region Rendering Methods
		/// <summary>
		/// Renders the beginning of HTML code corresponding to a specific year.
		/// </summary>
		/// <param name="theWriter">The text writer that receives rendered data.</param>
		/// <param name="theYear">The year being rendered</param>
		private void RenderYearGroupBegin(HtmlTextWriter theWriter, int theYear)
		{
			// 1. Year List Item
			string aYearStr = theYear.ToString();

			theWriter.AddAttribute("class", "year");
			theWriter.AddAttribute("onclick", "BlogEngine.toggleMonth('year" + aYearStr + "')");
			theWriter.RenderBeginTag(HtmlTextWriterTag.Li);
			theWriter.Write(aYearStr);

			// 2. Nested Month List
			theWriter.AddAttribute("id", "year" + aYearStr);

			if (theYear == myCurrentYear)
				theWriter.AddAttribute("class", "open");

			theWriter.RenderBeginTag(HtmlTextWriterTag.Ul);
		}

		/// <summary>
		/// Renders the end of HTML code corresponding to a year.
		/// </summary>
		/// <param name="theWriter">The text writer that receives rendered data.</param>
		private void RenderYearGroupEnd(HtmlTextWriter theWriter)
		{
			// 2. Nested Month List
			theWriter.RenderEndTag();


			// 1. Year List Item
			theWriter.RenderEndTag();
		}

		/// <summary>
		/// Renders HTML code corresponding to a specific month/year combination.
		/// </summary>
		/// <param name="theWriter">The text writer that receives rendered data.</param>
		/// <param name="theDate">The date (year and month) being rendered.</param>
		/// <param name="thePostCount">The number of posts published within the specified month.</param>
		/// <param name="theShowYear">True, if the year must be rendered; otherwise, false.</param>
		private void RenderMonth(HtmlTextWriter theWriter, DateTime theDate, int thePostCount, bool theShowYear)
		{
			theWriter.RenderBeginTag(HtmlTextWriterTag.Li);

			{
				string aHref = string.Format
				(
					"{0}{1}/{2}/default{3}",
					Utils.RelativeOrAbsoluteWebRoot,
					theDate.Year,
					theDate.ToString("MM"),
					BlogConfig.FileExtension
				);
				theWriter.AddAttribute("href", aHref);
				theWriter.RenderBeginTag(HtmlTextWriterTag.A);

				theWriter.Write(theShowYear ? theDate.ToString("MMMM yyyy") : theDate.ToString("MMMM"));

				theWriter.RenderEndTag();
			}

			theWriter.Write(" (");
			theWriter.Write(thePostCount.ToString());
			theWriter.Write(")");

			theWriter.RenderEndTag();
		}
		#endregion
		#endregion
	}
}