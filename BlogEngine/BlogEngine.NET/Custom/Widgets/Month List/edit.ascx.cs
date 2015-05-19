namespace Widgets.MonthList
{
	using System;
	using System.Collections.Specialized;

	using App_Code.Controls;

	public partial class Edit : WidgetEditBase
	{
		/// <summary>
		/// Handles the Load event of the Page control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		protected void Page_Load(object sender, EventArgs e)
		{
			if (!Page.IsPostBack && !Page.IsCallback)
			{
				StringDictionary settings = GetSettings();

				bool aGroupByYear = true;
				if (settings.ContainsKey("groupbyyear"))
					bool.TryParse(settings["groupbyyear"], out aGroupByYear);

				bool aRecentDatesAtTop = false;
				if (settings.ContainsKey("recentdatesattop"))
					bool.TryParse(settings["recentdatesattop"], out aRecentDatesAtTop);

				cbGroupByYear.Checked = aGroupByYear;
				cbRecentDatesAtTop.Checked = aRecentDatesAtTop;
			}
		}

		/// <summary>
		/// Saves this the Widget settings.
		/// </summary>
		public override void Save()
		{
			StringDictionary settings = GetSettings();

			settings["groupbyyear"] = cbGroupByYear.Checked.ToString();
			settings["recentdatesattop"] = cbRecentDatesAtTop.Checked.ToString();

			SaveSettings(settings);
		}
	}
}