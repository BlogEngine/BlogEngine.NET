using BlogEngine.Core;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Compilation;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.WebPages.Html;

public static class RazorHelpers
{
    /// <summary>
    /// Returns the HTML markup for a widget zone.
    /// </summary>
    public static string RenderWidgetZone(this HtmlHelper helper, string zoneName)
    {
        return RenderControl(helper, "App_Code.Controls.WidgetZone", new List<KeyValuePair<string, object>>() { { new KeyValuePair<string, object>("ZoneName", zoneName) } });
    }

    /// <summary>
    /// Returns the HTML markup for a control which will be created by it's typename.
    /// </summary>
    public static string RenderControl(this HtmlHelper helper, string typeName, IEnumerable<KeyValuePair<string, object>> properties)
    {
        try
        {
            Type t = Type.GetType(typeName);

            if (t != null)
            {
                object obj = Activator.CreateInstance(t);
                if (obj != null)
                {
                    Control control = obj as Control;
                    if (control != null)
                    {
                        return RenderControl(helper, control, properties);
                    }
                    else
                    {
                        Utils.Log($"Unable to cast the instantiated object as a control: {typeName}");
                    }
                }
                else
                {
                    Utils.Log($"Unable to activate control: {typeName}");
                }
            }
            else
            {
                Utils.Log($"Unable to load control type: {typeName}");
            }
        }
        catch (Exception ex)
        {
            Utils.Log($"Unable to load control: {typeName}", ex);
        }

        return HttpUtility.HtmlEncode($"ERROR - UNABLE TO LOAD CONTROL : {typeName}");
    }

    /// <summary>
    /// Returns the HTML markup for a user control which will be created by it's virtual path and name.
    /// </summary>
    public static string RenderUserControl(this HtmlHelper helper, string controlVirtualPath, IEnumerable<KeyValuePair<string, object>> properties)
    {
        try
        {   
            System.Web.UI.Page page = HttpContext.Current.Handler as System.Web.UI.Page;
            if (page != null)
            {
                Control control = page.LoadControl(controlVirtualPath);
                return RenderControl(helper, control, properties);
            }
            else
            {
                Utils.Log($"Page is null when trying to render control: {controlVirtualPath}");
            }
        }
        catch (Exception ex)
        {
            Utils.Log($"Unable to load control: {controlVirtualPath}", ex);
        }

        return HttpUtility.HtmlEncode($"ERROR - UNABLE TO LOAD CONTROL : {controlVirtualPath}");
    }

    /// <summary>
    /// Returns the HTML markup for a control, invoking any properties on the control.
    /// </summary>
    public static string RenderControl(this HtmlHelper helper, Control control, IEnumerable<KeyValuePair<string, object>> properties)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(control.ID))
            {
                control.ID = $"control_{Guid.NewGuid()}";
            }
            
            if (properties != null && properties.Count() > 0)
            {
                var type = control.GetType();

                foreach (KeyValuePair<string, object> prop in properties)
                {
                    var property = type.GetProperty(prop.Key);
                    property.SetValue(
                        control,
                        Convert.ChangeType(prop.Value, property.PropertyType, CultureInfo.InvariantCulture),
                        null);
                }
            }

            // To ensure all the events in the lifecycle fire (Init, Load, etc), put
            // this control into a page and run that page to get the final control markup.
            System.Web.UI.Page page = new System.Web.UI.Page();
            page.EnableViewState = false;
            HtmlForm form = new HtmlForm();
            PlaceHolder ph = new PlaceHolder();
            const string delimiterStart = "-|-|-|-|-|-|-|-|- control start -|-|-|-|-|-|-|-|-";
            const string delimiterEnd = "-|-|-|-|-|-|-|-|- control start -|-|-|-|-|-|-|-|-";
            ph.Controls.Add(new LiteralControl(delimiterStart));
            ph.Controls.Add(control);
            ph.Controls.Add(new LiteralControl(delimiterEnd));
            form.Controls.Add(ph);
            page.Controls.Add(form);

            StringWriter output = new StringWriter();
            HttpContext.Current.Server.Execute(page, output, false);
            string markup = output.ToString();

            Match m = new Regex(string.Format("{0}(.*?){1}", Regex.Escape(delimiterStart), Regex.Escape(delimiterEnd)), RegexOptions.IgnoreCase | RegexOptions.Singleline).Match(markup);
            if (m.Success)
            {
                return m.Groups[1].Value;
            }

            return string.Empty;
        }
        catch (Exception ex)
        {
            Utils.Log($"Unable to load control: {control.GetType()}", ex);
        }

        return HttpUtility.HtmlEncode($"ERROR - UNABLE TO LOAD CONTROL : {control.GetType()}");
    }

    public const string PAGE_BODY_MARKER = "-|-|-|-|-|-|- PAGE-BODY -|-|-|-|-|-|-";
    public static string PageBody(this HtmlHelper helper)
    {
        return PAGE_BODY_MARKER;
    }

    public const string NESTED_COMMENTS_MARKER = "-|-|-|-|-|-|- NESTED-COMMENTS -|-|-|-|-|-|-";
    public static string NestedComments(this HtmlHelper helper)
    {
        return NESTED_COMMENTS_MARKER;
    }

    public const string WIDGET_CONTAINER_BODY_MARKER = "-|-|-|-|-|-|- WIDGET-CONTAINER-BODY -|-|-|-|-|-|-";
    public static string WidgetContainerBody(this HtmlHelper helper)
    {
        return WIDGET_CONTAINER_BODY_MARKER;
    }

    public const string RAZOR_HOST_PAGE_VPATH = "~/Custom/Themes/RazorHost/page.cshtml";

    public static string ParseRazor(string virtualPath, object model)
    {
        string pageVPath = virtualPath;

        try
        {
            Type t = BuildManager.GetCompiledType(pageVPath);
            if (t != null)
            {
                HttpContextWrapper wrapper = new HttpContextWrapper(HttpContext.Current);

                object inst = Activator.CreateInstance(t);

                System.Web.WebPages.WebPage webpage = inst as System.Web.WebPages.WebPage;
                webpage.VirtualPath = pageVPath;
                StringWriter writer = new StringWriter();
                webpage.ExecutePageHierarchy(new System.Web.WebPages.WebPageContext(wrapper, webpage, model), writer, webpage);
                string content = writer.ToString();

                return content;
            }
        }
        catch (Exception ex)
        {
            Utils.Log($"RazorHelper, ParseRazor, VirtualPath: {virtualPath}", ex);

            // return the error message since it will usually contain parsing
            // details when the Razor markup/syntax is invalid.  this will help
            // when debugging, so the error log does not need to be checked.
            return ex.Message;
        }

        return null;
    }
}
