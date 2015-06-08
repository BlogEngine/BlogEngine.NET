namespace Widgets.Search
{
    using System;
    using System.Web;

    using App_Code.Controls;
    using BlogEngine.Core;

    public partial class Edit : WidgetEditBase
    {
        static string _buttonText = "search-button-text";
        static string _fieldText = "search-field-text";

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            if (this.Page.IsPostBack)
            {
                return;
            }

            var settings = this.GetSettings();

            this.txtButtonText.Text = settings.ContainsKey(_buttonText) ? settings[_buttonText] : Resources.labels.search;
            this.txtFieldText.Text = settings.ContainsKey(_fieldText) ? settings[_fieldText] : "Enter search term or APML url";

        }

        public override void Save()
        {
            var settings = this.GetSettings();
            
            settings[_buttonText] = this.txtButtonText.Text;
            settings[_fieldText] = this.txtFieldText.Text;

            this.SaveSettings(settings);
            Blog.CurrentInstance.Cache.Remove("widget_search");
        }
    }
}