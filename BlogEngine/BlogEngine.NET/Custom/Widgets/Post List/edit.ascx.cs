namespace Widgets.PostList
{
    using System;
    using System.Web;
    using App_Code.Controls;
    using BlogEngine.Core;
    using System.Web.Security;

    public partial class Edit : WidgetEditBase
    {
        public override void Save()
        {
            var settings = this.GetSettings();

            settings["numberofposts"] = this.txtNumberOfPosts.Text;
            settings["cutegory"] = ddlCategories.SelectedValue;
            settings["author"] = ddlAuthors.SelectedValue;
            settings["sortorder"] = ddlSortBy.SelectedValue;
            settings["showimg"] = cbShowImg.Checked.ToString();
            settings["showdesc"] = cbShowDesc.Checked.ToString();
            
            this.SaveSettings(settings);

            foreach (var item in Blog.CurrentInstance.Cache)
            {
                var i = (System.Collections.DictionaryEntry)item;
                if (i.Key.ToString().Contains("widget_postlist_"))
                {
                    var theKey = i.Key.ToString().Replace(Blog.CurrentInstance.Id.ToString() + "_", "");
                    Blog.CurrentInstance.Cache.Remove(theKey);
                }
            }
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            if (this.Page.IsPostBack)
                return;
        
            var settings = this.GetSettings();

            // authors
            ddlAuthors.Items.Add("All");
            var authors = Membership.GetAllUsers();
            foreach (var author in authors)
            {
                ddlAuthors.Items.Add(author.ToString()); 
            }
            
            // categories
            ddlCategories.Items.Add("All");
            foreach (var cat in Category.Categories)
            {
                ddlCategories.Items.Add(cat.Title);
            }

            txtNumberOfPosts.Text = settings.ContainsKey("numberofposts") ? settings["numberofposts"] : "10";
            ddlCategories.SelectedValue = settings.ContainsKey("cutegory") ? settings["cutegory"] : "All";
            ddlAuthors.SelectedValue = settings.ContainsKey("author") ? settings["author"] : "All";
            ddlSortBy.SelectedValue = settings.ContainsKey("sortorder") ? settings["sortorder"] : "Published";

            cbShowDesc.Checked = settings.ContainsKey("showdesc") && 
                settings["showdesc"].Equals("true", StringComparison.OrdinalIgnoreCase) ? true : false;
            cbShowImg.Checked = settings.ContainsKey("showimg") &&
                settings["showimg"].Equals("true", StringComparison.OrdinalIgnoreCase) ? true : false;
        }
    }
}