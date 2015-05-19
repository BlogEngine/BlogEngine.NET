using System;
using System.Collections.Generic;
using System.Web.UI.HtmlControls;
using System.Linq;
using System.Web;
using System.Web.UI;
using BlogEngine.Core;

namespace BlogEngine.Core.Web.Controls
{
    /// <summary>
    /// The PageMenu class.
    /// </summary>
    public class PageMenu
    {
        #region Properties

        private bool _ulIdSet = false;
        string _curPage = HttpUtility.UrlEncode(GetPageName(HttpContext.Current.Request.RawUrl.ToLower()));

        /// <summary>
        /// Home label.
        /// </summary>
        public string Home { get; set; }

        /// <summary>
        /// Contact label.
        /// </summary>
        public string Contact { get; set; }

        /// <summary>
        /// Archive label.
        /// </summary>
        public string Archive { get; set; }

        /// <summary>
        /// Logon label.
        /// </summary>
        public string Logon { get; set; }

        /// <summary>
        /// Logoff label.
        /// </summary>
        public string Logoff { get; set; }

        /// <summary>
        /// The HTML for the control to display on the page.
        /// </summary>
        public string Html
        {
            get
            {
                HtmlGenericControl ul = BindPages();
                System.IO.StringWriter sw = new System.IO.StringWriter();
                ul.RenderControl(new HtmlTextWriter(sw));
                return sw.ToString();
            }
        }

        #endregion

        private HtmlGenericControl BindPages()
        {
            // recursivly get all children of the root page
            HtmlGenericControl ul = GetChildren(Guid.Empty);

            // items that will be appended to the end of menu list
            AddMenuItem(ul, Contact, Utils.RelativeWebRoot + "contact.aspx");

            if (Security.IsAuthenticated)
            {
                AddMenuItem(ul, Logoff, Utils.RelativeWebRoot + "Account/login.aspx?logoff");
            }
            else
            {
                AddMenuItem(ul, Logon, Utils.RelativeWebRoot + "Account/login.aspx");
            }

            return ul;
        }

        bool HasChildren(Guid pageId)
        {
            bool returnValue = false;

            foreach (BlogEngine.Core.Page page in BlogEngine.Core.Page.Pages)
            {
                if (page.ShowInList && page.IsPublished)
                {
                    if (page.Parent == pageId)
                    {
                        returnValue = true;
                        break;
                    }
                }
            }

            return returnValue;
        }

        HtmlGenericControl GetChildren(Guid parentId)
        {
            HtmlGenericControl ul = new HtmlGenericControl("ul");

            if (!_ulIdSet)
            {
                ul.Attributes.Add("id", "menu-topmenu");
                ul.Attributes.Add("class", "menu");
                _ulIdSet = true;

                AddMenuItem(ul, Home, Utils.RelativeWebRoot + "default.aspx");
                AddMenuItem(ul, Archive, Utils.RelativeWebRoot + "archive.aspx");
            }

            foreach (BlogEngine.Core.Page page in BlogEngine.Core.Page.Pages)
            {
                if (page.ShowInList && page.IsPublished)
                {
                    if (page.Parent == parentId)
                    {
                        HtmlGenericControl li = new HtmlGenericControl("li");
                        string pageName = HttpUtility.UrlEncode(GetPageName(page.RelativeLink.ToString().ToLower()));

                        HtmlAnchor anc = new HtmlAnchor();
                        anc.HRef = page.RelativeLink.ToString();
                        anc.InnerHtml = page.Title;
                        anc.Title = page.Description;

                        if (pageName == _curPage)
                        {
                            anc.Attributes.Add("class", "current");
                        }

                        li.Controls.Add(anc);

                        if (HasChildren(page.Id))
                        {
                            HtmlGenericControl subUl = GetChildren(page.Id);
                            li.Controls.Add(subUl);
                        }
                        ul.Controls.Add(li);
                    }
                }
            }

            return ul;
        }

        private void AddMenuItem(HtmlGenericControl ul, string pageName, string pageUrl)
        {
            HtmlGenericControl li = new HtmlGenericControl("li");
            HtmlAnchor anc = new HtmlAnchor();

            anc.HRef = pageUrl;
            anc.InnerHtml = pageName;
            anc.Title = pageName;

            if (GetPageName(pageUrl).ToLower() == _curPage.ToLower())
            {
                anc.Attributes.Add("class", "current");
            }

            li.Controls.Add(anc);
            ul.Controls.Add(li);
        }

        static string GetPageName(string requestPath)
        {
            if (requestPath.IndexOf('?') != -1)
                requestPath = requestPath.Substring(0, requestPath.IndexOf('?'));
            return requestPath.Remove(0, requestPath.LastIndexOf("/") + 1).ToLower();
        }
    }
}
