// --------------------------------------------------------------------------------------------------------------------
// <summary>
//   The categories widget.
// </summary>
// --------------------------------------------------------------------------------------------------------------------


namespace Widgets.CategoryList
{
    using App_Code.Controls;

    /// <summary>
    /// The categories widget.
    /// </summary>
    public partial class Widget : WidgetBase
    {
        #region Properties

        /// <summary>
        /// Gets a value indicating whether or not the widget can be edited.
        /// <remarks>
        /// The only way a widget can be editable is by adding a edit.ascx file to the widget folder.
        /// </remarks>
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
        /// Gets the name. It must be exactly the same as the folder that contains the widget.
        /// </summary>
        /// <value></value>
        public override string Name
        {
            get
            {
                return "Category list";
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// This method works as a substitute for Page_Load. You should use this method for
        /// data binding etc. instead of Page_Load.
        /// </summary>
        public override void LoadWidget()
        {
            var settings = this.GetSettings();
            var showRssIcon = true;
            var showPostCount = true;
            if (settings.ContainsKey("showrssicon"))
            {
                bool.TryParse(settings["showrssicon"], out showRssIcon);
                bool.TryParse(settings["showpostcount"], out showPostCount);
            }

            this.uxCategoryList.ShowRssIcon = showRssIcon;
            this.uxCategoryList.ShowPostCount = showPostCount;
        }

        #endregion
    }
}