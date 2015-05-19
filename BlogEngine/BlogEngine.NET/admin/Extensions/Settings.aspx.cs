namespace Admin.Extensions
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using BlogEngine.Core;
    using BlogEngine.Core.Web.Extensions;

    public partial class Settings : System.Web.UI.Page
    {
        protected List<ManagedExtension> ExtensionList()
        {
            var extensions = ExtensionManager.Extensions.Where(x => x.Key != "MetaExtension").ToList();

            if (!Blog.CurrentInstance.IsPrimary)
                extensions = extensions.Where(x => x.Value.SubBlogEnabled == true).ToList();

            extensions.Sort(
                (e1, e2) => e1.Value.Priority == e2.Value.Priority ? string.CompareOrdinal(e1.Key, e2.Key) : e1.Value.Priority.CompareTo(e2.Value.Priority));

            List<ManagedExtension> manExtensions = new List<ManagedExtension>();

            foreach (KeyValuePair<string, ManagedExtension> ext in extensions)
            {
                var oExt = ExtensionManager.GetExtension(@ext.Key);
                manExtensions.Add(oExt);
            }
            return manExtensions;
        }
        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Init"/> event to initialize the page.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> that contains the event data.</param>
        protected override void OnInit(EventArgs e)
        {
            Security.DemandUserHasRight(Rights.AccessAdminPages, true);

            var extname = Request.QueryString["ext"];
            var extension = ExtensionList().Where(x => x.Name == extname).FirstOrDefault();

            if (extension != null && extension.BlogSettings.Count > 0)
            {
                foreach (var s in extension.BlogSettings)
                {
                    if (!string.IsNullOrEmpty(s.Name) && !s.Hidden)
                    {
                        var uc = (UserControlSettings)Page.LoadControl("Settings.ascx");
                        uc.ID = s.Name;
                        ucPlaceHolder.Controls.Add(uc);
                    }
                }
            }
            base.OnInit(e);
        }
    }
}