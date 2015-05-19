// --------------------------------------------------------------------------------------------------------------------
// <summary>
//   The widget.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Widgets.AuthorList
{
	using App_Code.Controls;
    using System.Collections.Specialized;

	/// <summary>
	/// The widget.
	/// </summary>
	public partial class Widget : WidgetBase
	{
		#region Properties

        string maxcount = "maxcount";
        string pattern = "pattern";
        string patternaggregated = "patternaggregated";

		/// <summary>
		/// Gets a value indicating whether the Widget is editable.
		/// </summary>
		public override bool IsEditable
		{
			get { return false; }
		}

		/// <summary>
		/// Gets the Widget name.
		/// </summary>
		public override string Name
		{
			get { return "AuthorList"; }
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

            int intCount = 10;
            if (settings.ContainsKey(maxcount))
                int.TryParse(settings[maxcount], out intCount);

            var strPatrn = "{0} ({1})";
            if (settings.ContainsKey(pattern))
                strPatrn = settings[pattern];

            var strAgrg = "{0}@{1} ({2})";
            if (settings.ContainsKey(patternaggregated))
                strAgrg = settings[patternaggregated];

            AuthorList1.MaxAuthors = intCount;
            AuthorList1.DisplayPattern = strPatrn;
            AuthorList1.PatternAggregated = strAgrg;
		}

		#endregion
	}
}