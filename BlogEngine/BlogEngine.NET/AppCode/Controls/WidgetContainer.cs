// --------------------------------------------------------------------------------------------------------------------
// <summary>
//   Themeable class for displaying WidgetBase derived controls.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace App_Code.Controls
{
    using System;
    using System.IO;
    using System.Text;
    using System.Threading;
    using System.Web.UI;
    using System.Web.Hosting;
    using BlogEngine.Core;

    using Resources;

    /// <summary>
    /// Themeable class for displaying WidgetBase derived controls.
    /// </summary>
    /// <remarks>
    /// WidgetContainer is meant to be the themeable parent class of any control that derives from WidgetBase. This way a theme can automatically
    /// apply some basic styling to the way widgets are displayed without having to edit each one or edit the WidgetBase class to change the
    /// rendered output.
    /// Inherited WidgetContainers must contain a control named phWidgetBody. This is the control that the WidgetContainer's child Widget is 
    /// injected inside of. phWidgetBody just needs to derive from Control to work, leaving flexibility for anyone creating a theme.
    /// If phWidgetBody isn't found, an exception isn't thrown, but a warning label is applied to the page. 
    /// </remarks>
    public abstract class WidgetContainer : UserControl
    {
        #region "Properties"

        /// <summary>
        /// Gets or sets the Widget this WidgetContainer holds.
        /// </summary>
        public WidgetBase Widget
        {
            get;
            set;
        }


        /// <summary>
        /// Gets a string representing the rendered html for administrative control of this WidgetContainer's child Widget.
        /// </summary>
        public string AdminLinks
        {
            get
            {
                if (Security.IsAuthorizedTo(Rights.ManageWidgets))
                {
                    if (this.Widget != null)
                    {
                        var sb = new StringBuilder();

                        var widgetId = this.Widget.WidgetId;

                        sb.AppendFormat("<a class=\"delete\" href=\"#\" onclick=\"BlogEngine.widgetAdmin.removeWidget('{0}');return false\" title=\"{1} widget\"><span class=\"widgetImg imgDelete\">&nbsp;</span></a>", widgetId, labels.delete);
                        sb.AppendFormat("<a class=\"edit\" href=\"#\" onclick=\"BlogEngine.widgetAdmin.editWidget('{0}', '{1}');return false\" title=\"{2} widget\"><span class=\"widgetImg imgEdit\">&nbsp;</span></a>", this.Widget.Name, widgetId, labels.edit);
                        sb.AppendFormat("<a class=\"move\" href=\"#\" onclick=\"BlogEngine.widgetAdmin.initiateMoveWidget('{0}');return false\" title=\"{1} widget\"><span class=\"widgetImg imgMove\">&nbsp;</span></a>", widgetId, labels.move);

                        return sb.ToString();
                    }
                }


                return String.Empty;
            }
        }

        #endregion
        
        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Load"/> event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.EventArgs"/> object that contains the event data.</param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            ProcessLoad();
        }

        private bool _processedLoad;
        /// <summary>
        /// Manually run the Initialization process.
        /// </summary>
        public void ProcessLoad()
        {
            if (_processedLoad) { return; }

            // phWidgetBody is the control that the Widget control
            // gets added to.
            var widgetBody = this.FindControl("phWidgetBody");

            if (widgetBody != null)
            {
                widgetBody.Controls.Add(this.Widget);
            }
            else
            {
                var warn = new LiteralControl
                {
                    Text = "Unable to find control with id \"phWidgetBody\" in theme's WidgetContainer."
                };
                this.Controls.Add(warn);
            }

            _processedLoad = true;
        }

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.PreRender"/> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> object that contains the event data.</param>
        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            // Hide the container if the Widget is null or also not visible.
            this.Visible = (this.Widget != null) && this.Widget.Visible;
        }

        /// <summary>
        /// The container will be processed when invoked, rather than waiting
        /// for the Load event to occur.
        /// </summary>
        public virtual void RenderContainer() { }

        /// <summary>
        /// Returns the virtual path of where a theme's widget container would expect to be located.
        /// </summary>
        /// <param name="existenceCheck">
        /// When true, the path to the theme folder to check for the WidgetContainer existence
        /// is returned.  When false, the path to the control that will be loaded is returned.
        /// If it's a Razor theme, the path will be RazorHost instead of the actual theme folder
        /// name.
        /// </param>
        /// <returns></returns>
        public static string GetThemeWidgetContainerVirtualPath(bool existenceCheck)
        {
            if (existenceCheck)
            {
                // if it's a Razor theme, check if WidgetContainer.cshtml exists.
                string filename = BlogSettings.Instance.IsRazorTheme ? "WidgetContainer.cshtml" : "WidgetContainer.ascx";
                return string.Format("~/Custom/Themes/{0}/{1}", BlogSettings.Instance.Theme, filename);
            }
            else
            {
                // when existenceCheck == false, the actual file that will be loaded needs to be
                // returned.  if it's a Razor theme, we will load WidgetContainer.ascx in the
                // RazorHost folder.  we assume that the RazorHost folder contains WidgetContainer.ascx.
                return string.Format("~/Custom/Themes/{0}/WidgetContainer.ascx", BlogSettings.Instance.GetThemeWithAdjustments(null));
            }
        }

        /// <summary>
        /// Returns the file path of where a theme's widget container would expect to be located.
        /// </summary>
        public static string GetThemeWidgetContainerFilePath(bool existenceCheck)
        {
            return HostingEnvironment.MapPath(GetThemeWidgetContainerVirtualPath(existenceCheck));
        }

        /// <summary>
        /// Returns whether the theme contains a widget container file.
        /// </summary>
        public static bool DoesThemeWidgetContainerExist(bool existenceCheck)
        {
            // This is for compatibility with older themes that do not have a WidgetContainer control.
            return File.Exists(GetThemeWidgetContainerFilePath(existenceCheck));
        }

        /// <summary>
        /// Loads the widget container (either the one located in the theme folder, or the default one if
        /// a theme widget container is missing), and adds the Widget to the widget container.
        /// </summary>
        public static WidgetContainer GetWidgetContainer(
            WidgetBase widgetControl, bool widgetContainerExists,
            string widgetContainerVirtualPath)
        {
            // If a custom WidgetContainer can't be found, create a new DefaultWidgetContainer instance as it
            // provides backwards compatibility with existing themes that may have depended on WidgetBase's
            // old rendering method.
            var widgetContainer = widgetContainerExists ? (WidgetContainer)widgetControl.Page.LoadControl(widgetContainerVirtualPath) : new DefaultWidgetContainer();

            widgetContainer.ID = "widgetContainer" + widgetControl.ID;
            widgetContainer.Widget = widgetControl;

            return widgetContainer;
        }

        /// <summary>
        /// Loads the widget container (either the one located in the theme folder, or the default one if
        /// a theme widget container is missing), and adds the Widget to the widget container.
        /// </summary>
        public static WidgetContainer GetWidgetContainer(
            WidgetBase widgetControl)
        {
            return GetWidgetContainer(widgetControl, DoesThemeWidgetContainerExist(true), GetThemeWidgetContainerVirtualPath(false));
        }
    }

    /// <summary>
    /// Default implementation of WidgetContainer that provides backwards compatibility with themes that do not have
    /// their own WidgetContainer user control.
    /// </summary>
    internal sealed class DefaultWidgetContainer : WidgetContainer
    {
        /// <summary>
        /// The widgetBody instance needed by all WidgetContainers.
        /// </summary>
        private readonly System.Web.UI.WebControls.PlaceHolder widgetBody = new System.Web.UI.WebControls.PlaceHolder
            {
                ID = "phWidgetBody" 
            };

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultWidgetContainer"/> class.
        /// </summary>
        internal DefaultWidgetContainer()
        {
            this.Controls.Add(this.widgetBody);
        }

        /// <summary>
        /// Sends server control content to a provided <see cref="T:System.Web.UI.HtmlTextWriter"/> object, which writes the content to be rendered on the client.
        /// </summary>
        /// <param name="writer">The <see cref="T:System.Web.UI.HtmlTextWriter"/> object that receives the server control content.</param>
        protected override void Render(HtmlTextWriter writer)
        {
            if (this.Widget == null)
            {
                throw new NullReferenceException("WidgetContainer requires its Widget property be set to a valid WidgetBase derived control");
            }

            var widgetName = this.Widget.Name;
            var widgetId = this.Widget.WidgetId;

            if (string.IsNullOrEmpty(this.Widget.Name))
            {
                throw new NullReferenceException("Name must be set on a widget");
            }

            var sb = new StringBuilder();

            sb.AppendFormat("<div class=\"widget {0}\" id=\"widget{1}\">", widgetName.Replace(" ", string.Empty).ToLowerInvariant(), widgetId);
            sb.Append(this.AdminLinks);
            if (this.Widget.ShowTitle)
            {
                sb.AppendFormat("<h4>{0}</h4>", this.Widget.Title);
            }

            sb.Append("<div class=\"content\">");

            writer.Write(sb.ToString());
            base.Render(writer);
            writer.Write("</div>");
            writer.Write("</div>");
        }
    }
}