// --------------------------------------------------------------------------------------------------------------------
// <summary>
//   The Post Calendar
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace App_Code.Controls
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Web;
    using System.Web.UI;
    using System.Web.UI.HtmlControls;
    using System.Web.UI.WebControls;
    using System.Collections.Generic;

    using BlogEngine.Core;

    /// <summary>
    /// The Post Calendar
    /// </summary>
    public class PostCalendar : Calendar, ICallbackEventHandler
    {
        #region Constants and Fields

        /// <summary>
        /// The callback.
        /// </summary>
        private string callback;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether ShowPostTitles.
        /// </summary>
        public bool ShowPostTitles
        {
            get
            {
                return (bool)(this.ViewState["ShowPostTitles"] ?? default(bool));
            }

            set
            {
                this.ViewState["ShowPostTitles"] = value;
            }
        }

        #endregion

        #region Implemented Interfaces

        #region ICallbackEventHandler

        /// <summary>
        /// Returns the results of a callback event that targets a control.
        /// </summary>
        /// <returns>The result of the callback.</returns>
        public string GetCallbackResult()
        {
            return this.callback;
        }

        /// <summary>
        /// Processes a callback event that targets a control.
        /// </summary>
        /// <param name="eventArgument">A string that represents an event argument to pass to the event handler.</param>
        public void RaiseCallbackEvent(string eventArgument)
        {
            DateTime date;
            if (!DateTime.TryParse(eventArgument, out date))
            {
                return;
            }

            this.VisibleDate = date;
            this.ShowTitle = false;
            using (var sw = new StringWriter())
            {
                this.RenderControl(new HtmlTextWriter(sw));
                this.callback = sw.ToString();
            }
        }

        #endregion

        #endregion

        #region Methods

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.WebControls.Calendar.DayRender"/> event of the <see cref="T:System.Web.UI.WebControls.Calendar"/> control and allows you to provide a custom handler for the <see cref="E:System.Web.UI.WebControls.Calendar.DayRender"/> event.
        /// </summary>
        /// <param name="cell">A <see cref="T:System.Web.UI.WebControls.TableCell"/> that contains information about the cell to render.</param>
        /// <param name="day">A <see cref="T:System.Web.UI.WebControls.CalendarDay"/> that contains information about the day to render.</param>
		protected override void OnDayRender(TableCell cell, CalendarDay day)
		{
			if (day.IsToday)
			{
				cell.Attributes["id"] += "today";
			}
			DateTime date = day.Date;
			var list = Post.GetPostsByDate(date, date);
			if (list.Count > 0)
			{
				cell.Controls.Clear();
				if (this.ShowPostTitles)
				{
					cell.Controls.Add(new LiteralControl(day.DayNumberText));
					foreach (var a in
						list.Where(post => post.IsVisible).Select(post => new HtmlAnchor { InnerHtml = string.Format("<br /><br />{0}", post.Title), HRef = post.RelativeOrAbsoluteLink }))
					{
						cell.Controls.Add(a);
					}
				}
				else
				{
					if (list[0].IsVisible)
					{
						var a = new HtmlAnchor
							{
								InnerHtml = day.DayNumberText,
								HRef =
									string.Format("{0}{1}/{2}/{3}/default{4}", Utils.RelativeOrAbsoluteWebRoot, date.Year, date.Month.ToString("00"), date.Day.ToString("00"), BlogConfig.FileExtension)
							};
						a.Attributes["class"] = "exist";
						cell.Controls.Add(a);
					}
					else
					{
						cell.Text = day.DayNumberText;
					}
				}
			}
			else
			{
				cell.Controls.Clear();
				cell.Text = day.DayNumberText;
			}
		}

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Load"/> event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.EventArgs"/> object that contains the event data.</param>
        protected override void OnLoad(EventArgs e)
        {
            this.Page.ClientScript.GetCallbackEventReference(this, "arg", null, "context");
            base.OnLoad(e);
        }

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.PreRender"/> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> that contains event data.</param>
        protected override void OnPreRender(EventArgs e)
        {
            if (!this.Page.IsCallback && !this.Page.IsPostBack)
            {
                this.VisibleDate = DateTime.Now;
            }

            if (!this.Page.IsPostBack && this.Context.Request.QueryString["date"] != null)
            {
                DateTime date;
                if (DateTime.TryParse(this.Context.Request.QueryString["date"], out date))
                {
                    this.VisibleDate = date;
                }
            }

            base.OnPreRender(e);

            if (!this.ShowPostTitles)
            {
                this.ShowTitle = false;
            }
        }

        /// <summary>
        /// Displays the <see cref="T:System.Web.UI.WebControls.Calendar"/> control on the client.
        /// </summary>
        /// <param name="writer">A <see cref="T:System.Web.UI.HtmlTextWriter"/> that contains the output stream for rendering on the client.</param>
        protected override void Render(HtmlTextWriter writer)
        {
            if (this.ShowPostTitles && !this.Page.IsCallback)
            {
                base.Render(writer);
            }
            else
            {
                if ((this.Page.IsPostBack && !this.Page.IsCallback) || this.VisibleDate == DateTime.MinValue)
                {
                    this.VisibleDate = DateTime.Now;
                }

                writer.Write("<div id=\"calendarContainer\">");
                writer.Write("<table class=\"calendar\" summary=\"\" style=\";border-collapse:collapse;\">");
                writer.Write("<tr><td>");

                var oldest = GetOldestPostDate();

                if (this.VisibleDate.Year != oldest.Year || this.VisibleDate.Month != oldest.Month)
                {
                    writer.Write("<a href=\"javascript:BlogEngine.Calendar.nav('{0}')\">{1}</a>&nbsp;&nbsp;", this.VisibleDate.AddMonths(-1).ToString("yyyy-MM-dd"), HttpUtility.HtmlEncode(this.PrevMonthText));
                }
                else
                {
                    writer.Write("{0}&nbsp;&nbsp;", HttpUtility.HtmlEncode(this.PrevMonthText));
                }

                writer.Write("</td><td style=\"text-align:center;width:100px\">{0}</td><td align=\"right\">", this.VisibleDate.ToString("MMMM yyyy"));

                if (this.VisibleDate.Year != DateTime.Now.Year || this.VisibleDate.Month != DateTime.Now.Month)
                {
                    writer.Write("&nbsp;&nbsp;<a href=\"javascript:BlogEngine.Calendar.nav('{0}')\">{1}</a>", this.VisibleDate.AddMonths(1).ToString("yyyy-MM-dd"), HttpUtility.HtmlEncode(this.NextMonthText));
                }
                else
                {
                    writer.Write("&nbsp;&nbsp;{0}", HttpUtility.HtmlEncode(this.NextMonthText));
                }

                writer.Write("</td></tr>");
                writer.Write("</table>");

                this.Attributes.Add("summary", "Post calendar");

                try
                {
                    base.Render(writer);
                }
                catch (Exception)
                {
                    writer.Write("<a href=\"javascript:void(location.reload(true))\">Reload page</a>");
                }

                writer.Write("</div>");

                if (!this.Page.IsCallback)
                {
                    writer.Write(this.Script());
                }
            }
        }

        /// <summary>
        /// Gets the oldest post date.
        /// </summary>
        /// <returns>The oldest post date.</returns>
        private static DateTime GetOldestPostDate()
        {
            List<Post> applicablePosts = Post.ApplicablePosts;
            return applicablePosts.Count > 0 ? applicablePosts[applicablePosts.Count - 1].DateCreated : DateTime.Now;
        }

        /// <summary>
        /// Scripts this instance.
        /// </summary>
        /// <returns>The script.</returns>
        private string Script()
        {
            var sb = new StringBuilder();
            sb.AppendFormat(
                @"<script type=""text/javascript"">
function setupBlogEngineCalendar() {{
  BlogEngine.Calendar = {{
  	  months: {{}},
	  nav: function(date) {{
		  var m = BlogEngine.Calendar.months;
		  if (m[date] == null || m[date] == 'undefined')  {{
		  	  {0}
		  }} else {{
			  BlogEngine.updateCalendar(m[date], date);
		  }}
	  }}
  }};
}}
</script>",
            this.Page.ClientScript.GetCallbackEventReference(this, "date", "BlogEngine.updateCalendar", "date"));
            this.Page.ClientScript.RegisterStartupScript(this.Page.GetType(), "calendarinit", BlogEngine.Core.Web.Scripting.Helpers.FormatInlineScript("BlogEngine.addLoadEvent(setupBlogEngineCalendar);"), false);

            /*
            ");
            sb.Append("var months = new Object();");
            sb.Append("function CalNav(date){");
            sb.Append("if (months[date] == null || months[date] == 'undefined')");
            sb.Append("{" + Page.ClientScript.GetCallbackEventReference(this, "date", "BlogEngine.updateCalendar", "date") + "}");
            sb.Append("else {BlogEngine.updateCalendar(months[date], date)}");
            sb.Append("}");
            sb.Append("</script>");
            */

            return sb.ToString();
        }

        #endregion
    }
}