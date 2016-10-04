using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace BlogEngine.Core.Web.Scripting
{
    /// <summary>
    /// Class to process links dynamically injected into page header
    /// </summary>
    public class PageHeader
    {
        List<LiteralControl> HeaderLinks = new List<LiteralControl>();

        /// <summary>
        /// Add link to collection
        /// </summary>
        /// <param name="type">Type</param>
        /// <param name="rel">Relation</param>
        /// <param name="title">Title</param>
        /// <param name="href">Href</param>
        public void AddLink(string type, string rel, string title, string href)
        {
            var link = new LiteralControl();

            var ptrn = "\n\t<link rel=\"{0}\" title=\"{1}\" href=\"{2}\" />";
            if (!string.IsNullOrEmpty(type))
            {
                ptrn = "\n\t<link type=\"{0}\" rel=\"{1}\" title=\"{2}\" href=\"{3}\" />";
                link.Text = string.Format(ptrn, type, rel, title, href);
            }
            else
            {
                link.Text = string.Format(ptrn, rel, title, href);
            }
            if (!HeaderLinks.Contains(link))
            {
                HeaderLinks.Add(link);
            }
        }

        /// <summary>
        /// Render links by adding them to the page header
        /// </summary>
        /// <param name="page">Base page</param>
        public void Render(System.Web.UI.Page page)
        {
            int idx = 0;
            foreach (var link in HeaderLinks)
            {
                page.Header.Controls.AddAt(idx, link);
                idx++;
            }
            foreach (var style in GetStyles())
            {
                page.Header.Controls.AddAt(idx, style);
                idx++;
            }

            idx = GetIndex(page);
            foreach (var script in GetScripts())
            {
                page.Header.Controls.AddAt(idx, script);
                idx++;
            }
        }

        #region Private Methods

        List<LiteralControl> GetStyles()
        {
            var headerStyles = new List<LiteralControl>();
            var tmpl = "\n\t<link href=\"{0}\" rel=\"stylesheet\" type=\"text/css\" />";

            foreach (var f in GetFiles($"{Utils.ApplicationRelativeWebRoot}Content/Auto"))
            {
                var href = $"{Utils.ApplicationRelativeWebRoot}Content/Auto/{f}";
                headerStyles.Add(new LiteralControl(string.Format(tmpl, href)));
            }
            return headerStyles;
        }

        List<LiteralControl> GetScripts()
        {
            var headerScripts = new List<LiteralControl>();
            var tmpl = "\n\t<script type=\"text/javascript\" src=\"{0}\"></script>";
            var lang = BlogSettings.Instance.Language;

            // if specific culture set in blog settings, use it instead
            if (BlogSettings.Instance.Culture.ToLower() != "auto")
                lang = BlogSettings.Instance.Culture;

            var rsrc = HttpHandlers.ResourceHandler.GetScriptPath(new CultureInfo(lang));
            headerScripts.Add(new LiteralControl(string.Format(tmpl, rsrc)));

            foreach (var f in GetFiles($"{Utils.ApplicationRelativeWebRoot}Scripts/Auto"))
            {
                var href = $"{Utils.ApplicationRelativeWebRoot}Scripts/Auto/{f}";
                headerScripts.Add(new LiteralControl(string.Format(tmpl, href)));
            }
            return headerScripts;
        }

        static List<string> GetFiles(string url)
        {
            List<string> files = new List<string>();
            string path = System.Web.HttpContext.Current.Server.MapPath(url);

            var folder = new DirectoryInfo(path);
            if (folder.Exists)
            {
                foreach (var file in folder.GetFiles())
                {
                    files.Add(file.Name);
                }
            }
            return files;
        }

        static int GetIndex(System.Web.UI.Page page)
        {
            // insert global scripts just before first script tag in the header
            // or after last css style tag if no script tags in the header found
            int cnt = 0;
            int idx = 0;
            string ctrlText = "";

            foreach (Control ctrl in page.Header.Controls)
            {
                cnt++;
                try
                {
                    if (ctrl.GetType() == typeof(LiteralControl))
                    {
                        LiteralControl lc = (LiteralControl)ctrl;
                        ctrlText = lc.Text.ToLower();
                    }
                    if (ctrl.GetType() == typeof(Literal))
                    {
                        Literal lc = (Literal)ctrl;
                        ctrlText = lc.Text.ToLower();
                    }
                    if (ctrl.GetType() == typeof(DataBoundLiteralControl))
                    {
                        DataBoundLiteralControl lc = (DataBoundLiteralControl)ctrl;
                        ctrlText = lc.Text.ToLower();
                    }
                    if (ctrl.GetType() == typeof(HtmlLink))
                    {
                        HtmlLink hl = (HtmlLink)ctrl;
                        if (!string.IsNullOrEmpty(hl.Attributes["type"]))
                        {
                            ctrlText = hl.Attributes["type"].ToLower();
                        }
                    }
                    if (ctrlText.Contains("text/css"))
                        idx = cnt;

                    if (ctrlText.Contains("text/javascript"))
                    {
                        idx = cnt;
                        // offset by 1 as we need inject before
                        if (idx > 1) idx = idx - 1;
                        break;
                    }
                }
                catch
                {
                    break;
                }
            }
            return idx;
        }

        #endregion
    }
}