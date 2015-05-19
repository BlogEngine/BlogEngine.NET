// --------------------------------------------------------------------------------------------------------------------
// <summary>
//   The widget.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Widgets.MonthList
{
	using System.Collections.Specialized;
	using App_Code.Controls;

	/// <summary>
	/// The widget.
	/// </summary>
	public partial class Widget : WidgetBase
	{
		#region Properties

		/// <summary>
		/// Gets a value indicating whether the Widget is editable.
		/// </summary>
		public override bool IsEditable
		{
			get { return true; }
		}

		/// <summary>
		/// Gets the Widget name.
		/// </summary>
		public override string Name
		{
			get { return "Month List"; }
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// This method works as a substitute for Page_Load. You should use this method for
		/// data binding etc. instead of Page_Load.
		/// </summary>
		public override void LoadWidget()
		{
			StringDictionary settings = GetSettings();

			if (settings.ContainsKey("groupbyyear"))
			{
				bool aGroupByYear = true;

				if (bool.TryParse(settings["groupbyyear"], out aGroupByYear))
					ctlMonthList.GroupByYear = aGroupByYear;
			}

			if (settings.ContainsKey("recentdatesattop"))
			{
				bool aRecentDatesAtTop = false;

				if (bool.TryParse(settings["recentdatesattop"], out aRecentDatesAtTop))
					ctlMonthList.RecentDatesAtTop = aRecentDatesAtTop;
			}
		}

		#endregion
	}
}