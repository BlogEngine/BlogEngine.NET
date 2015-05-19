namespace Widgets.AuthorList
{
    using System;
    using System.Collections.Specialized;
    using App_Code.Controls;

    public partial class Edit : WidgetEditBase
    {
        string maxcount = "maxcount";
        string pattern = "pattern";
        string patternaggregated = "patternaggregated";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack && !Page.IsCallback)
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

                txtMaxCount.Text = intCount.ToString();
                txtDisplayPattern.Text = strPatrn;
                txtDisplayPatternAggregated.Text = strAgrg;
            }
        }

        /// <summary>
        /// Saves this the Widget settings.
        /// </summary>
        public override void Save()
        {
            StringDictionary settings = GetSettings();

            settings["maxcount"] = txtMaxCount.Text;
            settings["pattern"] = txtDisplayPattern.Text;
            settings["patternaggregated"] = txtDisplayPatternAggregated.Text;

            SaveSettings(settings);
        }
    }
}