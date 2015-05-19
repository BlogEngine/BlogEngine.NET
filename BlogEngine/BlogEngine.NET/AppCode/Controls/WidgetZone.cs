// --------------------------------------------------------------------------------------------------------------------
// <summary>
//   The widget zone.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace App_Code.Controls
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using System.Web.UI;
    using System.Web.UI.WebControls;
    using System.Xml;

    using BlogEngine.Core;
    using BlogEngine.Core.DataStore;
	using BlogEngine.Core.Providers.CacheProvider;

    using Resources;
    using System.Web.Hosting;

    /// <summary>
    /// The widget zone.
    /// </summary>
    public class WidgetZone : PlaceHolder
    {
        #region Constants and Fields

        /// <summary>
        ///     The zone name.
        /// </summary>
        /// <remarks>
        ///     For backwards compatibility or if a ZoneName is omitted, provide a default ZoneName.
        /// </remarks>
        private string zoneName = "be_WIDGET_ZONE";
        private const string staticZone = "be_WIDGET_ZONE";

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes static members of the <see cref = "WidgetZone" /> class.
        /// </summary>
        static WidgetZone()
        {
            WidgetEditBase.Saved += (sender, args) => OnZonesUpdated();
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Gets or sets the name of the data-container used by this instance
        /// </summary>
        public string ZoneName
        {
            get
            {
                return zoneName;
            }

            set
            {
                zoneName = Utils.RemoveIllegalCharacters(value);
            }
        }

        /// <summary>
        /// Gets the XML document.
        /// </summary>
        /// <value>The XML document.</value>
        private XmlDocument XmlDocument
        {
            get
            {
                // look up the document by zone name
                return Blog.CurrentInstance.Cache[ZoneName] == null ? null : (XmlDocument)Blog.CurrentInstance.Cache[ZoneName];
            }
        }

        #endregion

        #region Methods


		/// <summary>
		/// This method is intended to be called once during FirstRequestInitialization
		/// right before LoadExtension() which is another time consuming routine.
		/// By preloading the widgets asyncronously in this method, this time consuming
		/// work is done in a manner that reduces the time to load the first page.  
		/// </summary>
		/// <param name="zoneName">Typically "be_WIDGET_ZONE"</param>
		public static void PreloadWidgetsAsync(string zoneName) {
			//8/16/11 RonC Method Added 

			//Need to access the Cache to force it to be constructed
			//while we still have HttpContext.
			CacheProvider cache = Blog.CurrentInstance.Cache;

			// Need blogSettings to pass to Ping since the current blog instance won't
			// be detectable once in a BG thread.
			Guid blogId = Blog.CurrentInstance.Id;
			ThreadPool.QueueUserWorkItem(delegate {
				// because HttpContext is not available within this BG thread
				// needed to determine the current blog instance,
				// set override value here.
				Blog.InstanceIdOverride = blogId;

				XmlDocument doc;

				// check the cache for the document. if not loaded yet, load it & put in cache
				if (Blog.CurrentInstance.Cache[zoneName] == null) {
					doc = RetrieveXml(zoneName);
					if (doc != null) {
						Blog.CurrentInstance.Cache[zoneName] = doc;
					}
				} else {
					doc = (XmlDocument)Blog.CurrentInstance.Cache[zoneName];
				}

				var zone = doc.SelectNodes("//widget");
				if (zone == null) {
					return;
				}

				System.Web.UI.Page page = new System.Web.UI.Page();

				foreach (XmlNode widget in zone) {
					var fileName = string.Format("{0}Custom/Widgets/{1}/widget.ascx", Utils.ApplicationRelativeWebRoot, widget.InnerText);
					try {
						bool isAdminWidget = (fileName.ToLower().IndexOf("/administration/widget.ascx") >= 0);
						if (!isAdminWidget || (isAdminWidget && Security.IsAuthenticated)) {

							//Loading the widget control now, will cause it to be in
							//memory when a real page request comes in later because
							//the .Net framework will cached the control.
							page.LoadControl(fileName);
						}
					} catch {
						//mask the exceptions since we are just preloading controls
						//Later when the control is loaded for a real page request
						//The system will show an appropriate error.
					}
				}

			});//end delegate

		}//PreloadWidgets


        
        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Init"/> event.
        /// </summary>
        /// <param name="e">
        /// An <see cref="T:System.EventArgs"/> object that contains the event data.
        /// </param>
        protected override void OnInit(EventArgs e)
        {
            if (this.XmlDocument == null)
            {
                // if there's no document for this zone name yet, load it
                var doc = RetrieveXml(this.ZoneName);
                if (doc != null)
                {
                    Blog.CurrentInstance.Cache[ZoneName] = doc;
                }
            }

            base.OnInit(e);
        }

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Load"></see> event.
        /// </summary>
        /// <param name="e">
        /// The <see cref="T:System.EventArgs"></see> object that contains the event data.
        /// </param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

			XmlNodeList zone = null;
			if (this.XmlDocument != null)
			{
				zone = this.XmlDocument.SelectNodes("//widget");
			}

            if (zone == null)
            {
                return;
            }

            // This is for compatibility with older themes that do not have a WidgetContainer control.
            var widgetContainerExists = WidgetContainer.DoesThemeWidgetContainerExist(true);
            var widgetContainerVirtualPath = WidgetContainer.GetThemeWidgetContainerVirtualPath(false);

            foreach (XmlNode widget in zone)
            {
                var fileName = string.Format("{0}Custom/Widgets/{1}/widget.ascx", Utils.ApplicationRelativeWebRoot, widget.InnerText);
                try
                {
					//Mod RonC Added this conditional so that the admin widget isn't loaded for non authenticated
					//         visitors.  This shaves 1 sec off of a cold server first page fetch 
					//         for a non admin visitor.  
					bool isAdminWidget = (fileName.ToLower().IndexOf("/administration/widget.ascx") >= 0);	
					if (!isAdminWidget || (isAdminWidget && Security.IsAuthenticated)) {

						var control = (WidgetBase)this.Page.LoadControl(fileName);
						if (widget.Attributes != null) {
							control.WidgetId = new Guid(widget.Attributes["id"].InnerText);
							control.Title = widget.Attributes["title"].InnerText;
							control.ShowTitle = control.IsEditable
													? bool.Parse(widget.Attributes["showTitle"].InnerText)
													: control.DisplayHeader;
						}

						control.ID = control.WidgetId.ToString().Replace("-", string.Empty);
						control.Zone = this.zoneName;

						control.LoadWidget();

						// This will return the WidgetContainer with the control in it.
						var widgetContainer = WidgetContainer.GetWidgetContainer(control, widgetContainerExists, widgetContainerVirtualPath);
						this.Controls.Add(widgetContainer);
					}
                }
                catch (Exception ex)
                {
                    var lit = new Literal
                        {
                           Text = string.Format("<p style=\"color:red\">Widget {0} not found.<p>", widget.InnerText) 
                        };
                    lit.Text += ex.Message;
                    if (widget.Attributes != null)
                    {
                        lit.Text +=
                            string.Format(
                                "<a class=\"delete\" href=\"#\" onclick=\"BlogEngine.widgetAdmin.removeWidget('{0}');return false\" title=\"{1} widget\">X</a>", 
                                widget.Attributes["id"].InnerText, 
                                labels.delete);
                    }

                    this.Controls.Add(lit);
                }
            }
        }

        /// <summary>
        /// Sends server control content to a provided <see cref="T:System.Web.UI.HtmlTextWriter"></see> 
        ///     object, which writes the content to be rendered on the client.
        /// </summary>
        /// <param name="writer">
        /// The <see cref="T:System.Web.UI.HtmlTextWriter"></see> object 
        ///     that receives the server control content.
        /// </param>
        protected override void Render(HtmlTextWriter writer)
        {
            writer.Write("<div id=\"widgetzone_{0}\" class=\"widgetzone\">", this.zoneName);

            base.Render(writer);

            writer.Write("</div>");

            if (!Security.IsAuthorizedTo(Rights.ManageWidgets))
            {
                return;
            }

            var selectorId = string.Format("widgetselector_{0}", this.zoneName);
            writer.Write("<select id=\"{0}\" class=\"widgetselector\">", selectorId);
            var di = new DirectoryInfo(HostingEnvironment.MapPath(string.Format("{0}Custom/Widgets", Utils.ApplicationRelativeWebRoot)));
            foreach (var dir in di.GetDirectories().Where(dir => File.Exists(Path.Combine(dir.FullName, "widget.ascx"))))
            {
                writer.Write("<option value=\"{0}\">{1}</option>", dir.Name, dir.Name);
            }

            writer.Write("</select>&nbsp;&nbsp;");
            writer.Write(
                "<input type=\"button\" value=\"Add\" onclick=\"BlogEngine.widgetAdmin.addWidget(BlogEngine.$('{0}').value, '{1}')\" />", 
                selectorId, 
                this.zoneName);
            writer.Write("<div class=\"clear\" id=\"clear\">&nbsp;</div>");
        }

        /// <summary>
        /// Called when [zones updated].
        /// </summary>
        private static void OnZonesUpdated()
        {
            Blog.CurrentInstance.Cache.Remove(staticZone);
        }

        /// <summary>
        /// Retrieves the XML.
        /// </summary>
        /// <param name="zoneName">
        /// The zone Name.
        /// </param>
        /// <returns>
        /// An Xml Document.
        /// </returns>
        private static XmlDocument RetrieveXml(string zoneName)
        {
            var ws = new WidgetSettings(zoneName) { SettingsBehavior = new XmlDocumentBehavior() };
            var doc = (XmlDocument)ws.GetSettings();
            return doc;
        }

        #endregion
    }
}