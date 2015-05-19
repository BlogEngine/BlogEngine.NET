// --------------------------------------------------------------------------------------------------------------------
// <summary>
//   Builds nested page list.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace App_Code.Controls
{
    using System;
    using System.Web;
    using System.Web.UI;
    using BlogEngine.Core;

    /// <summary>
    /// Summary description for PageMenu
    /// </summary>
    public class PageMenu : Control
    {
        public override void RenderControl(HtmlTextWriter writer)
        {
            var menu = new BlogEngine.Core.Web.Controls.PageMenu();

            menu.Home = Resources.labels.home;
            menu.Contact = Resources.labels.contact;
            menu.Archive = Resources.labels.archive;
            menu.Logon = Resources.labels.login;
            menu.Logoff = Resources.labels.logoff;

            writer.Write(menu.Html);
            writer.Write(Environment.NewLine);
        }
    }
}