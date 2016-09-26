namespace BlogEngine.Core
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Net.Mail;
    using System.Reflection;
    using System.Security;
    using System.Security.Cryptography;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Threading;
    using System.Web;
    using System.Web.Configuration;
    using System.Web.UI;
    using System.Web.UI.WebControls;
    using System.Web.UI.HtmlControls;
    using System.Xml;
    using System.DirectoryServices;
    using BlogEngine.Core.Web.Controls;
    using BlogEngine.Core.Web.Extensions;
    using System.Net.Sockets;

    /// <summary>
    /// Utilities for the entire solution to use.
    /// </summary>
    public static class Utils
    {
        #region Constants and Fields

        /// <summary>
        /// The cookie name for forcing the main theme on a mobile device.
        /// </summary>
        public const String ForceMainThemeCookieName = "forceMainTheme";

        /// <summary>
        /// The folder name where files are stored in the XmlFileSystemProvider.
        /// </summary>
        /// <value>"files"</value>
        public const string FilesFolder = "files";

        /// <summary>
        /// The pattern.
        /// </summary>
        private const string Pattern = "<head.*<link( [^>]*title=\"{0}\"[^>]*)>.*</head>";

        /// <summary>
        /// The application's relative web root.
        /// </summary>
        private static string applicationRelativeWebRoot;

        /// <summary>
        /// The href regex.
        /// </summary>
        private static readonly Regex HrefRegex = new Regex(
            "href=\"(.*)\"", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        /// <summary>
        /// The regex between tags.
        /// </summary>
        private static readonly Regex RegexBetweenTags = new Regex(@">\s+", RegexOptions.Compiled);

        /// <summary>
        /// The regex line breaks.
        /// </summary>
        private static readonly Regex RegexLineBreaks = new Regex(@"\n\s+", RegexOptions.Compiled);

        /// <summary>
        /// The regex mobile.
        /// </summary>
        private static readonly Regex RegexMobile =
            new Regex(BlogConfig.MobileServices, RegexOptions.IgnoreCase | RegexOptions.Compiled);

        /// <summary>
        /// The regex strip html.
        /// </summary>
        private static readonly Regex RegexStripHtml = new Regex("<[^>]*>", RegexOptions.Compiled);

        /// <summary>
        ///     Boolean for returning whether or not BlogEngine is currently running on Mono.
        /// </summary>
        private static readonly bool isMono = (Type.GetType("Mono.Runtime") != null);

        #endregion

        #region Events

        /// <summary>
        ///     Occurs after an e-mail has been sent. The sender is the MailMessage object.
        /// </summary>
        public static event EventHandler<EventArgs> EmailFailed;

        /// <summary>
        ///     Occurs after an e-mail has been sent. The sender is the MailMessage object.
        /// </summary>
        public static event EventHandler<EventArgs> EmailSent;

        /// <summary>
        ///     Occurs when a message will be logged. The sender is a string containing the log message.
        /// </summary>
        public static event EventHandler<EventArgs> OnLog;

        #endregion

        #region Properties

        /// <summary>
        ///     Gets the absolute root of the website.
        /// </summary>
        /// <value>A string that ends with a '/'.</value>
        public static Uri AbsoluteWebRoot
        {
            get { return Blog.CurrentInstance.AbsoluteWebRoot; }
        }

        /// <summary>
        ///     Gets the relative URL of the blog feed. If a Feedburner username
        ///     is entered in the admin settings page, it will return the 
        ///     absolute Feedburner URL to the feed.
        /// </summary>
        public static string FeedUrl
        {
            get
            {
                return !string.IsNullOrEmpty(BlogSettings.Instance.AlternateFeedUrl)
                           ? BlogSettings.Instance.AlternateFeedUrl
                           : string.Format("{0}syndication.axd", AbsoluteWebRoot);
            }
        }

        /// <summary>
        ///     Gets a value indicating whether we're running under Linux or a Unix variant.
        /// </summary>
        /// <value><c>true</c> if Linux/Unix; otherwise, <c>false</c>.</value>
        public static bool IsLinux
        {
            get
            {
                var p = (int)Environment.OSVersion.Platform;
                return (p == 4) || (p == 128);
            }
        }

        /// <summary>
        ///     Gets a value indicating whether the client is a mobile device.
        /// </summary>
        /// <value><c>true</c> if this instance is mobile; otherwise, <c>false</c>.</value>
        public static bool IsMobile
        {
            get
            {
                var context = HttpContext.Current;
                if (context != null)
                {
                    var request = context.Request;
                    if (request.Browser.IsMobileDevice)
                    {
                        return true;
                    }

                    if (!string.IsNullOrEmpty(request.UserAgent) && RegexMobile.IsMatch(request.UserAgent))
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        /// <summary>
        ///     Gets a value indicating whether we're running under Mono.
        /// </summary>
        /// <value><c>true</c> if Mono; otherwise, <c>false</c>.</value>
        public static bool IsMono
        {
            get
            {
                return isMono;
            }
        }

        /// <summary>
        ///     Gets the relative root of the current blog instance.
        /// </summary>
        /// <value>A string that ends with a '/'.</value>
        public static string RelativeWebRoot
        {
            get
            {
                return Blog.CurrentInstance.RelativeWebRoot;
            }
        }

        /// <summary>
        ///     Gets the application's relative root.
        /// </summary>
        /// <value>A string that ends with a '/'.</value>
        public static string ApplicationRelativeWebRoot
        {
            get
            {
                return applicationRelativeWebRoot ??
                       (applicationRelativeWebRoot =
                        VirtualPathUtility.ToAbsolute(BlogConfig.VirtualPath));
            }
        }

        /// <summary>
        ///     Returns the relative webroot if possible if the hostname of this blog instance matches the
        ///     hostname of the site aggregation blog.  If the hostname is different, then the
        ///     absolute webroot is returned.
        /// </summary>
        public static string RelativeOrAbsoluteWebRoot
        {
            get
            {
                if (Blog.CurrentInstance.DoesHostnameDifferFromSiteAggregationBlog)
                    return AbsoluteWebRoot.ToString();
                else
                    return RelativeWebRoot.ToString();
            }
        }

        /// <summary>
        ///     Gets if the current HTTP request is a homepage request, taking the blog
        ///     instance into consideration.
        /// </summary>
        public static bool IsCurrentRequestForHomepage
        {
            get
            {
                HttpContext context = HttpContext.Current;

                // Because the homepage uses querystring values to render non-homepage content
                // such as posts by tag, posts by date range, posts by author, etc, if there
                // are any of these known parameters, this will be considered not a homepage request.
                // Marketing tools such as Google AdWords and campaign tracking in Google Analytics
                // will add their own query string parameters, so it is important to still render
                // the home page even if these third-party tracking parameters are present.
                string[] knownQueryParameters =
                {
                    "id", "name", "tag", "year", "month", "date", "calendar", "apml", "page"
                };
                if (context.Request.QueryString.AllKeys
                    .Where(x => x != null)
                    .Any(x => knownQueryParameters.Any(y => x.Equals(y, StringComparison.OrdinalIgnoreCase))))
                    return false;

                string path = context.Request.Path;

                // If this is a virtual folder for a blog instance, unless "default.aspx" is
                // actually in the URL, default.aspx won't be reported in path, so we check
                // to see if path is the root of RelativeWebRoot (the blog instance's
                // relative web root).

                if (RelativeWebRoot.Equals(VirtualPathUtility.AppendTrailingSlash(path), StringComparison.OrdinalIgnoreCase))
                    return true;
                else if (path.Equals(string.Format("{0}default.aspx", RelativeWebRoot), StringComparison.OrdinalIgnoreCase))
                    return true;

                return false;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Returns whether the main theme should be forced for the current mobile device.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>
        /// A <see cref="Boolean"/> instance.
        /// </returns>
        public static Boolean ShouldForceMainTheme(HttpRequest request)
        {
            var forceMainThemeCookie = request.Cookies[ForceMainThemeCookieName];

            if (forceMainThemeCookie == null)
            {
                return false;
            }

            if (String.IsNullOrEmpty(forceMainThemeCookie.Value))
            {
                return false;
            }

            Boolean forceMainTheme;

            if (Boolean.TryParse(forceMainThemeCookie.Value, out forceMainTheme))
            {
                return forceMainTheme;
            }

            return false;
        }

        /// <summary>
        /// Parse the string representation of an enum field to a strongly typed enum value.
        /// </summary>
        public static T ParseEnum<T>(string value, T defaultValue) where T : struct, IConvertible
        {
            if (!typeof(T).IsEnum)
                throw new ArgumentException("T must be an enumerated type");

            if (string.IsNullOrEmpty(value))
                return defaultValue;

            foreach (T item in Enum.GetValues(typeof(T)))
            {
                if (item.ToString().Equals(value, StringComparison.OrdinalIgnoreCase))
                    return item;
            }
            return defaultValue;
        }

        private static Regex _identifierForDisplayRgx = new Regex(
            @"  (?<=[A-Z])(?=[A-Z][a-z])    # UC before me, UC lc after me
             |  (?<=[^A-Z])(?=[A-Z])        # Not UC before me, UC after me
             |  (?<=[A-Za-z])(?=[^A-Za-z])  # Letter before me, non letter after me
            ", RegexOptions.IgnorePatternWhitespace
        );
        /// <summary>
        /// Format's an identifier (e.g. variable, enum field value) for display purposes,
        /// by adding a space before each capital letter.  A value like EditOwnUser becomes
        /// "Edit Own User".  If multiple capital letters are in identifier, these letters
        /// will be treated as one word (e.g. XMLEditor becomes "XML Editor", not
        /// "X M L Editor").
        /// </summary>
        /// <param name="fieldName">An identifier ready to be formatted for display.</param>
        /// <returns>The identifier for display purposes.</returns>
        /// <remarks>Credit: http://stackoverflow.com/questions/3103730</remarks>
        public static string FormatIdentifierForDisplay(string fieldName)
        {
            StringBuilder sb = new StringBuilder();

            foreach (string part in _identifierForDisplayRgx.Split(fieldName))
            {
                if (sb.Length > 0) { sb.Append(" "); }
                sb.Append(part);
            }

            return sb.ToString();
        }

        /// <summary>
        /// Selects a listitem by value, case insensitively.
        /// </summary>
        /// <param name="control">The ListControl</param>
        /// <param name="value">The value to select</param>
        /// <returns>The ListItem found and selected</returns>
        public static ListItem SelectListItemByValue(ListControl control, string value)
        {
            control.ClearSelection();
            control.SelectedIndex = -1;

            foreach (ListItem li in control.Items)
            {
                if (string.Equals(value, li.Value, StringComparison.OrdinalIgnoreCase))
                {
                    li.Selected = true;
                    return li;
                }
            }

            return null;
        }

        /// <summary>
        /// Converts an object to its JSON representation.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string ConvertToJson(object obj)
        {
            return new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(obj);
        }

        /// <summary>
        /// Adds a JavaScript reference to the HTML head tag.
        /// </summary>
        /// <param name="page">
        /// The page to add the JavaScript include to.
        /// </param>
        /// <param name="url">
        /// The url string.
        /// </param>
        /// <param name="placeInBottom">
        /// The place In Bottom.
        /// </param>
        /// <param name="addDeferAttribute">
        /// The add Defer Attribute.
        /// </param>
        public static void AddJavaScriptInclude(System.Web.UI.Page page, string url, bool placeInBottom = false, bool addDeferAttribute = false)
        {
            if (placeInBottom)
            {
                var deferAttr = (addDeferAttribute ? " defer=\"defer\"" : string.Empty);
                var script = string.Format("<script type=\"text/javascript\"{0} src=\"{1}\"></script>", deferAttr, url);
                page.ClientScript.RegisterStartupScript(page.GetType(), url.GetHashCode().ToString(), script);
            }
            else
            {
                var script = new HtmlGenericControl("script");
                script.Attributes["type"] = "text/javascript";
                script.Attributes["src"] = url;
                if (addDeferAttribute)
                {
                    script.Attributes["defer"] = "defer";
                }

                string itemsKey = "next-script-insert-position";
                HttpContext context = HttpContext.Current;

                // Inserting scripts in the beginning of the HEAD tag so any user
                // scripts that may rely on our scripts (jQuery, etc) have these
                // scripts available to them.  Also maintaining order so subsequent
                // scripts are added after scripts we already added.

                int? nextInsertPosition = context == null ? null : context.Items[itemsKey] as int?;
                if (!nextInsertPosition.HasValue) { nextInsertPosition = 0; }

                page.Header.Controls.AddAt(nextInsertPosition.Value, script);
                if (context != null) { context.Items[itemsKey] = ++nextInsertPosition; }
            }
        }

        /// <summary>
        /// This method returns all code assemblies in app_code
        ///     If app_code has subdirectories for c#, vb.net etc
        ///     Each one will come back as a separate assembly
        ///     So we can support extensions in multiple languages
        /// </summary>
        /// <returns>
        /// List of code assemblies
        /// </returns>
        public static IEnumerable<Assembly> CodeAssemblies()
        {
            var codeAssemblies = new List<Assembly>();
            CompilationSection s = null;
            var assemblyName = "__code";
            try
            {
                try
                {
                    s = (CompilationSection)WebConfigurationManager.GetSection("system.web/compilation");
                }
                catch (SecurityException)
                {
                    // No read permissions on web.config due to the trust level (must be High or Full)
                }

                if (s != null && s.CodeSubDirectories != null && s.CodeSubDirectories.Count > 0)
                {
                    for (var i = 0; i < s.CodeSubDirectories.Count; i++)
                    {
                        assemblyName = string.Format("App_SubCode_{0}", s.CodeSubDirectories[i].DirectoryName);
                        codeAssemblies.Add(Assembly.Load(assemblyName));
                    }
                }
                else
                {
                    assemblyName = "App_Code";
                    codeAssemblies.Add(Assembly.Load(assemblyName));
                }
            }
            catch (FileNotFoundException)
            {
                /*ignore - code directory has no files*/
            }

            try
            {
                codeAssemblies.AddRange(GetCompiledExtensions());
            }
            catch (Exception ex)
            {
                Log("Error loading compiled assemblies: " + ex.Message);
            }

            return codeAssemblies;
        }

        /// <summary>
        /// Converts a relative URL to an absolute one.
        /// </summary>
        /// <param name="relativeUri">
        /// The relative Uri.
        /// </param>
        /// <returns>
        /// The absolute Uri.
        /// </returns>
        public static Uri ConvertToAbsolute(Uri relativeUri)
        {
            return ConvertToAbsolute(relativeUri.ToString());
        }

        /// <summary>
        /// Converts a relative URL to an absolute one.
        /// </summary>
        /// <param name="relativeUri">
        /// The relative Uri.
        /// </param>
        /// <returns>
        /// The absolute Uri.
        /// </returns>
        public static Uri ConvertToAbsolute(string relativeUri)
        {
            if (String.IsNullOrEmpty(relativeUri))
            {
                throw new ArgumentNullException("relativeUri");
            }

            var absolute = AbsoluteWebRoot.ToString();
            var index = absolute.LastIndexOf(RelativeWebRoot);

            return new Uri(absolute.Substring(0, index) + relativeUri);
        }

        /// <summary>
        /// Downloads a web page from the Internet and returns the HTML as a string. .
        /// </summary>
        /// <param name="url">
        /// The URL to download from.
        /// </param>
        /// <returns>
        /// The HTML or null if the URL isn't valid.
        /// </returns>
        [Obsolete("Use the RemoteFile class instead.")]
        public static string DownloadWebPage(Uri url)
        {
            try
            {
                var remoteFile = new RemoteFile(url, true);
                return remoteFile.GetFileAsString();
            }
            catch (Exception)
            {
                return null;
            }

        }

        /// <summary>
        /// Extract file name from given physical server path
        /// </summary>
        /// <param name="path">
        /// The Server path.
        /// </param>
        /// <returns>
        /// The File Name.
        /// </returns>
        public static string ExtractFileNameFromPath(string path)
        {
            return Path.GetFileName(path);
        }

        /// <summary>
        /// Finds semantic links in a given HTML document.
        /// </summary>
        /// <param name="type">
        /// The type of link. Could be foaf, apml or sioc.
        /// </param>
        /// <param name="html">
        /// The HTML to look through.
        /// </param>
        /// <returns>
        /// A list of Uri.
        /// </returns>
        public static List<Uri> FindLinks(string type, string html)
        {
            var matches = Regex.Matches(
                html, string.Format(Pattern, type), RegexOptions.IgnoreCase | RegexOptions.Singleline);
            var urls = new List<Uri>();

            foreach (var hrefMatch in
                matches.Cast<Match>().Where(match => match.Groups.Count == 2).Select(match => match.Groups[1].Value).
                    Select(link => HrefRegex.Match(link)).Where(hrefMatch => hrefMatch.Groups.Count == 2))
            {
                Uri url;
                var value = hrefMatch.Groups[1].Value;
                if (Uri.TryCreate(value, UriKind.Absolute, out url))
                {
                    urls.Add(url);
                }
            }

            return urls;
        }

        /// <summary>
        /// Converts all the relative paths in the specified content to absolute.
        /// </summary>
        /// <param name="content">
        /// The content.
        /// </param>
        /// <param name="publishable">
        /// The publishable.
        /// </param>
        public static string ConvertPublishablePathsToAbsolute(string content, IPublishable publishable)
        {
            content = content.Replace(
                string.Format("\"{0}image.axd", publishable.Blog.AbsoluteWebRoot.AbsolutePath), "\"" + publishable.Blog.AbsoluteWebRoot + "image.axd");
            content = content.Replace(
                string.Format("\"{0}file.axd", publishable.Blog.AbsoluteWebRoot.AbsolutePath), "\"" + publishable.Blog.AbsoluteWebRoot + "file.axd");
            content = content.Replace(string.Format("href=\"{0}{1}", publishable.Blog.RelativeWebRoot, string.Empty), string.Format("href=\"{0}", publishable.Blog.AbsoluteWebRoot));

            content = content.Replace("href=\"/", string.Format("href=\"{0}", publishable.Blog.AbsoluteWebRoot));

            return content;
        }

        /// <summary>
        /// Finds the semantic documents from a URL based on the type.
        /// </summary>
        /// <param name="url">
        /// The URL of the semantic document or a document containing semantic links.
        /// </param>
        /// <param name="type">
        /// The type. Could be "foaf", "apml" or "sioc".
        /// </param>
        /// <returns>
        /// A dictionary of the semantic documents. The dictionary is empty if no documents were found.
        /// </returns>
        public static Dictionary<Uri, XmlDocument> FindSemanticDocuments(Uri url, string type)
        {
            var list = new Dictionary<Uri, XmlDocument>();

            // ignoreRemoteDownloadSettings is set to true for the
            // RemoteFile instance for backwards compatibility.
            var remoteFile = new RemoteFile(url, true);
            var content = remoteFile.GetFileAsString();
            if (!string.IsNullOrEmpty(content))
            {
                var upper = content.ToUpperInvariant();

                if (upper.Contains("</HEAD") && upper.Contains("</HTML"))
                {
                    var urls = FindLinks(type, content);
                    foreach (var xmlUrl in urls)
                    {
                        var doc = LoadDocument(url, xmlUrl);
                        if (doc != null)
                        {
                            list.Add(xmlUrl, doc);
                        }
                    }
                }
                else
                {
                    var doc = LoadDocument(url, url);
                    if (doc != null)
                    {
                        list.Add(url, doc);
                    }
                }
            }

            return list;
        }

        /// <summary>
        /// Returns the default culture.  This is either the culture specified in the blog settings,
        ///     or the default culture installed with the operating system.
        /// </summary>
        /// <returns>
        /// The default culture.
        /// </returns>
        public static CultureInfo GetDefaultCulture()
        {
            var settingsCulture = BlogSettings.Instance.Culture;
            if (String.IsNullOrWhiteSpace(settingsCulture) ||
                settingsCulture.Equals("Auto", StringComparison.OrdinalIgnoreCase))
            {
                return CultureInfo.InstalledUICulture;
            }

            return CultureInfo.CreateSpecificCulture(settingsCulture);
        }

        /// <summary>
        /// Gets the sub domain.
        /// </summary>
        /// <param name="url">
        /// The URL to get the sub domain from.
        /// </param>
        /// <returns>
        /// The sub domain.
        /// </returns>
        public static string GetSubDomain(Uri url)
        {
           
            if (url.HostNameType == UriHostNameType.Dns)
            {
                var host = url.Host;
                if (host.Split('.').Length > 2)
                {
                    var lastIndex = host.LastIndexOf(".");
                    var index = host.LastIndexOf(".", lastIndex - 1);
                    return host.Substring(0, index);
                }
            }

            return null;
        }

        /// <summary>
        /// Encrypts a string using the SHA256 algorithm.
        /// </summary>
        /// <param name="plainMessage">
        /// The plain Message.
        /// </param>
        /// <returns>
        /// The hash password.
        /// </returns>
        public static string HashPassword(string plainMessage)
        {
            var data = Encoding.UTF8.GetBytes(plainMessage);
            using (HashAlgorithm sha = new SHA256Managed())
            {
                sha.TransformFinalBlock(data, 0, data.Length);
                return Convert.ToBase64String(sha.Hash);
            }
        }

        /// <summary>
        /// The body regex.
        /// </summary>
        private static readonly Regex BodyRegex = new Regex(@"\[UserControl:(.*?)\]", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        /// <summary>
        /// Injects any user controls if one is referenced in the text.
        /// </summary>
        /// <param name="containerControl">The control that the user controls will be injected in to.</param>
        /// <param name="content">The string to parse for user controls.</param>
        public static void InjectUserControls(System.Web.UI.Control containerControl, string content)
        {
            var currentPosition = 0;
            var matches = BodyRegex.Matches(content);
            var containerControls = containerControl.Controls;
            var page = containerControl.Page;

            foreach (Match match in matches)
            {
                // Add literal for content before custom tag should it exist.
                if (match.Index > currentPosition)
                {
                    containerControls.Add(new LiteralControl(content.Substring(currentPosition, match.Index - currentPosition)));
                }

                // Now lets add our user control.
                try
                {
                    var all = match.Groups[1].Value.Trim();
                    Control usercontrol;

                    if (!all.EndsWith(".ascx", StringComparison.OrdinalIgnoreCase))
                    {
                        var index = all.IndexOf(".ascx", StringComparison.OrdinalIgnoreCase) + 5;
                        usercontrol = page.LoadControl(all.Substring(0, index));

                        var parameters = page.Server.HtmlDecode(all.Substring(index));
                        var type = usercontrol.GetType();
                        var paramCollection = parameters.Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries);

                        foreach (var param in paramCollection)
                        {
                            var paramSplit = param.Split('=');
                            var name = paramSplit[0].Trim();
                            var value = paramSplit[1].Trim();
                            var property = type.GetProperty(name);
                            property.SetValue(
                                usercontrol,
                                Convert.ChangeType(value, property.PropertyType, CultureInfo.InvariantCulture),
                                null);
                        }
                    }
                    else
                    {
                        usercontrol = page.LoadControl(all);
                    }

                    containerControls.Add(usercontrol);

                    // Now we will update our position.
                    // currentPosition = myMatch.Index + myMatch.Groups[0].Length;
                }
                catch (Exception)
                {
                    // Whoopss, can't load that control so lets output something that tells the developer that theres a problem.
                    containerControls.Add(
                        new LiteralControl(string.Format("ERROR - UNABLE TO LOAD CONTROL : {0}", match.Groups[1].Value)));
                }

                currentPosition = match.Index + match.Groups[0].Length;
            }

            // Finally we add any trailing static text.
            containerControls.Add(
                new LiteralControl(content.Substring(currentPosition, content.Length - currentPosition)));
        }

        private static readonly Regex emailRegex = new Regex(
            @"^[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?$", RegexOptions.IgnoreCase);

        private static readonly Regex validIpV4AddressRegex = new Regex(@"^(([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])\.){3}([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])$", RegexOptions.IgnoreCase);
        private static readonly Regex validHostnameRegex = new Regex(@"^(([a-z]|[a-z][a-z0-9\-]*[a-z0-9])\.)*([a-z]|[a-z][a-z0-9\-]*[a-z0-9])$", RegexOptions.IgnoreCase);
        

        /// <summary>
        /// Email address by user name
        /// </summary>
        /// <param name="userName">User name</param>
        /// <returns>Email Address</returns>
        public static string GetUserEmail(string userName)
        {
            int count;
            var userCollection = System.Web.Security.Membership.Provider.GetAllUsers(0, 999, out count);
            var users = userCollection.Cast<System.Web.Security.MembershipUser>().ToList();
            var user = users.FirstOrDefault(u => u.UserName.Equals(userName, StringComparison.OrdinalIgnoreCase));

            if(user != null)
            {
                return user.Email;
            }
            return userName;
        }

        /// <summary>
        /// Validates an email address.
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public static bool IsEmailValid(string email)
        {
            if (!string.IsNullOrWhiteSpace(email))
            {
                return emailRegex.IsMatch(email.Trim());
            }

            return false;
        }

        /// <summary>
        /// Validates a host name.
        /// </summary>
        /// <param name="hostname"></param>
        /// <returns></returns>
        public static bool IsHostnameValid(string hostname)
        {
            if (!string.IsNullOrWhiteSpace(hostname))
            {
                return validHostnameRegex.IsMatch(hostname.Trim());
            }

            return false;
        }

        /// <summary>
        /// Validates an IPv4 address.
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        public static bool IsIpV4AddressValid(string address)
        {
            if (!string.IsNullOrWhiteSpace(address))
            {
                return validIpV4AddressRegex.IsMatch(address.Trim());
            }

            return false;
        }

        /// <summary>
        /// Validates an IPv6 address.
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        public static bool IsIpV6AddressValid(string address)
        {
            if (!string.IsNullOrWhiteSpace(address))
            {
                IPAddress ip;
                if (IPAddress.TryParse(address, out ip))
                {
                    return ip.AddressFamily == AddressFamily.InterNetworkV6;
                }
            }
            return false;
        }

        /// <summary>
        /// Run through all code assemblies and creates object
        ///     instance for types marked with extension attribute
        /// </summary>
        public static void LoadExtensions()
        {
            var codeAssemblies = CodeAssemblies();

            var sortedExtensions = new List<SortedExtension>();

            foreach (Assembly a in codeAssemblies)
            {
                var types = a.GetTypes();
                
                Type extensionsAttribute = typeof(ExtensionAttribute);

                sortedExtensions.AddRange(
                    from type in types
                    let attributes = type.GetCustomAttributes(extensionsAttribute, false)
                    from attribute in attributes
                    where (attribute.GetType() == extensionsAttribute)
                    let ext = (ExtensionAttribute)attribute
                    select new SortedExtension(ext.Priority, type.Name, type.FullName));

                sortedExtensions.Sort(
                    (e1, e2) =>
                    e1.Priority == e2.Priority
                        ? string.CompareOrdinal(e1.Name, e2.Name)
                        : e1.Priority.CompareTo(e2.Priority));

                foreach (var x in sortedExtensions.Where(x => ExtensionManager.ExtensionEnabled(x.Name)))
                {
                    a.CreateInstance(x.Type);
                }
            }

            // initialize comment rules and filters
            CommentHandlers.Listen();
        }

        /// <summary>
        /// Sends a message to any subscribed log listeners.
        /// </summary>
        /// <param name="message">
        /// The message to be logged.
        /// </param>
        public static void Log(object message)
        {
            if (OnLog != null)
            {
                OnLog(message, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Sends a message to any subscribed log listeners.
        /// </summary>
        /// <param name="format">A format string</param>
        /// <param name="args">Arguments to replace in the format string</param>
		public static void Log(string format, params object[] args)
		{
			if (OnLog != null)
			{
				OnLog(string.Format(format, args), EventArgs.Empty);
			}
		}

        /// <summary>
        /// Sends a message to any subscribed log listeners.
        /// </summary>
        /// <param name="methodName"></param>
        /// <param name="ex"></param>
        public static void Log(string methodName, Exception ex)
        {
            Log(String.Format("{0}: {1}", methodName, ex.Message));
        }

        /// <summary>
        /// Generates random password for password reset
        /// </summary>
        /// <returns>
        /// Random password
        /// </returns>
        public static string RandomPassword()
        {
            var chars = "abcdefghijklmnopqrstuvwxyz1234567890ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();
            var password = string.Empty;
            var random = new Random();

            for (var i = 0; i < 8; i++)
            {
                var x = random.Next(1, chars.Length);
                if (!password.Contains(chars.GetValue(x).ToString()))
                {
                    password += chars.GetValue(x);
                }
                else
                {
                    i--;
                }
            }

            return password;
        }

        /// <summary>
        /// Removes the HTML whitespace.
        /// </summary>
        /// <param name="html">
        /// The HTML to remove the whitespace from.
        /// </param>
        /// <returns>
        /// The html with the whitespace removed.
        /// </returns>
        public static string RemoveHtmlWhitespace(string html)
        {
            if (string.IsNullOrEmpty(html))
            {
                return string.Empty;
            }

            html = RegexBetweenTags.Replace(html, "> ");
            html = RegexLineBreaks.Replace(html, string.Empty);

            return html.Trim();
        }

        /// <summary>
        /// Renders a control to a string.
        /// </summary>
        /// <param name="control"></param>
        /// <returns></returns>
        public static string RenderControl(System.Web.UI.Control control)
        {
            if (control == null)
            {
                throw new ArgumentNullException("control");
            }

            using (var sWriter = new System.IO.StringWriter())
            {
                using (var hWriter = new System.Web.UI.HtmlTextWriter(sWriter))
                {
                    control.RenderControl(hWriter);
                }

                return sWriter.ToString();
            }

        }

        /// <summary>
        /// Strips all illegal characters from the specified title.
        /// </summary>
        /// <param name="text">
        /// The text to strip.
        /// </param>
        /// <returns>
        /// The remove illegal characters.
        /// </returns>
        public static string RemoveIllegalCharacters(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return text;
            }

            text = text.Replace(":", string.Empty);
            text = text.Replace("/", string.Empty);
            text = text.Replace("?", string.Empty);
            text = text.Replace("#", string.Empty);
            text = text.Replace("[", string.Empty);
            text = text.Replace("]", string.Empty);
            text = text.Replace("@", string.Empty);
            text = text.Replace("*", string.Empty);
            text = text.Replace(".", string.Empty);
            text = text.Replace(",", string.Empty);
            text = text.Replace("\"", string.Empty);
            text = text.Replace("&", string.Empty);
            text = text.Replace("'", string.Empty);

            text = text.Replace("–", "-"); // live writer passes char 8211  this inplace of a char 45 for hyphen
            text = RemoveUnicodePunctuation(text); // moves any unicode versions of punctuation

            text = text.Replace(" ", "-");
            text = RemoveDiacritics(text);
            text = RemoveExtraHyphen(text);

            return HttpUtility.HtmlEncode(text).Replace("%", string.Empty);
        }

        /// <summary>
        /// Sends a MailMessage object using the SMTP settings.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="smtpServer">SMTP server</param>
        /// <param name="smtpServerPort">SMTP server port</param>
        /// <param name="smtpUserName">User name</param>
        /// <param name="smtpPassword">Password</param>
        /// <param name="enableSsl">Enable SSL</param>
        /// <returns>
        /// Error message, if any.
        /// </returns>
        public static string SendMailMessage(MailMessage message, string smtpServer = "", string smtpServerPort = "",
            string smtpUserName = "", string smtpPassword = "", string enableSsl = "")
        {
            StringBuilder errorMsg = new StringBuilder();
            bool boolSssl = BlogSettings.Instance.EnableSsl;
            int intPort = BlogSettings.Instance.SmtpServerPort;

            if (message == null)
                throw new ArgumentNullException("message");

            try
            {
                if (string.IsNullOrEmpty(smtpServer))
                    smtpServer = BlogSettings.Instance.SmtpServer;

                if (string.IsNullOrEmpty(smtpUserName))
                    smtpUserName = BlogSettings.Instance.SmtpUserName;

                if (string.IsNullOrEmpty(smtpPassword))
                    smtpPassword = BlogSettings.Instance.SmtpPassword;

                if (!string.IsNullOrEmpty(smtpServerPort))
                    intPort = int.Parse(smtpServerPort);

                if (!string.IsNullOrEmpty(enableSsl))
                    bool.TryParse(enableSsl, out boolSssl);

                message.IsBodyHtml = true;
                message.BodyEncoding = Encoding.UTF8;
                var smtp = new SmtpClient(smtpServer);

                // don't send credentials if a server doesn't require it,
                // linux smtp servers don't like that 
                if (!string.IsNullOrEmpty(smtpUserName))
                {
                    smtp.Credentials = new NetworkCredential(smtpUserName, smtpPassword);
                }

                smtp.Port = intPort;
                smtp.EnableSsl = boolSssl;
                smtp.Send(message);
                OnEmailSent(message);
            }
            catch (Exception ex)
            {
                OnEmailFailed(message);

                errorMsg.Append("Error sending email in SendMailMessage: ");
                Exception current = ex;

                while (current != null)
                {
                    if (errorMsg.Length > 0) { errorMsg.Append(" "); }
                    errorMsg.Append(current.Message);
                    current = current.InnerException;
                }

                Utils.Log(errorMsg.ToString());
            }
            finally
            {
                // Remove the pointer to the message object so the GC can close the thread.
                message.Dispose();
            }

            return errorMsg.ToString();
        }

        /// <summary>
        /// Sends the mail message asynchronously in another thread.
        /// </summary>
        /// <param name="message">
        /// The message to send.
        /// </param>
        public static void SendMailMessageAsync(MailMessage message)
        {
            // Before entering a BG thread, retrieve the current instance blog settings.
            Guid blogId = Blog.CurrentInstance.Id;
            ThreadPool.QueueUserWorkItem(delegate
            {
                // because HttpContext is not available within this BG thread
                // needed to determine the current blog instance,
                // set override value here.
                Blog.InstanceIdOverride = blogId;

                SendMailMessage(message);
            });
        }

        /// <summary>
        /// Writes ETag and Last-Modified headers and sets the conditional get headers.
        /// </summary>
        /// <param name="date">
        /// The date for the headers.
        /// </param>
        /// <returns>
        /// The set conditional get headers.
        /// </returns>
        public static bool SetConditionalGetHeaders(DateTime date)
        {
            var now = date.Kind == DateTimeKind.Utc ? DateTime.UtcNow : DateTime.Now;

            // SetLastModified() below will throw an error if the 'date' is a future date.
            // If the date is 1/1/0001, Mono will throw a 404 error
            if (date > now || date.Year < 1900)
            {
                date = now;
            }

            var response = HttpContext.Current.Response;
            var request = HttpContext.Current.Request;

            var etag = string.Format("\"{0}\"", date.Ticks);
            var incomingEtag = request.Headers["If-None-Match"];

            DateTime incomingLastModifiedDate;
            DateTime.TryParse(request.Headers["If-Modified-Since"], out incomingLastModifiedDate);

            response.Cache.SetLastModified(date);
            response.Cache.SetCacheability(HttpCacheability.Public);
            response.Cache.SetETag(etag);

            if (String.Compare(incomingEtag, etag) == 0 || incomingLastModifiedDate == date)
            {
                response.Clear();
                response.StatusCode = (int)HttpStatusCode.NotModified;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Strips all HTML tags from the specified string.
        /// </summary>
        /// <param name="html">
        /// The string containing HTML
        /// </param>
        /// <returns>
        /// A string without HTML tags
        /// </returns>
        public static string StripHtml(string html)
        {
            return String.IsNullOrWhiteSpace(html) ? string.Empty : RegexStripHtml.Replace(html, string.Empty).Trim();
        }

        /// <summary>
        /// Translates the specified string using the resource files.
        /// </summary>
        /// <param name="text">
        /// The text to translate.
        /// </param>
        /// <returns>
        /// The translate.
        /// </returns>
        public static string Translate(string text)
        {
            return Translate(text, null, null);
        }

        /// <summary>
        /// Translates the specified string using the resource files.  If a translation
        ///     is not found, defaultValue will be returned.
        /// </summary>
        /// <param name="text">
        /// The text to translate.
        /// </param>
        /// <param name="defaultValue">
        /// The default Value.
        /// </param>
        /// <returns>
        /// The translate.
        /// </returns>
        public static string Translate(string text, string defaultValue)
        {
            return Translate(text, defaultValue, null);
        }

        /// <summary>
        /// Translates the specified string using the resource files and specified culture.
        ///     If a translation is not found, defaultValue will be returned.
        /// </summary>
        /// <param name="text">
        /// The text to translate.
        /// </param>
        /// <param name="defaultValue">
        /// The default Value.
        /// </param>
        /// <param name="culture">
        /// The culture.
        /// </param>
        /// <returns>
        /// The translate.
        /// </returns>
        public static string Translate(string text, string defaultValue, CultureInfo culture)
        {
            var resource = culture == null
                               ? HttpContext.GetGlobalResourceObject("labels", text)
                               : HttpContext.GetGlobalResourceObject("labels", text, culture);

            return resource != null
                       ? resource.ToString()
                       : (string.IsNullOrEmpty(defaultValue)
                              ? string.Format("Missing Resource [{0}]", text)
                              : defaultValue);
        }

        /// <summary>
        /// Helper method to quickly check if directory or file is writable
        /// </summary>
        /// <param name="url">Physical path</param>
        /// <param name="file">Optional File name</param>
        /// <returns>True if application can write file/directory</returns>
        public static bool CanWrite(string url, string file = "")
        {
            var dir = HttpContext.Current.Server.MapPath(url);

            if (dir != null && Directory.Exists(dir))
            {
                if (string.IsNullOrEmpty(file)) 
                    file = string.Format("test{0}.txt", DateTime.Now.ToString("ddmmhhssss"));

                try
                {
                    var p = Path.Combine(dir, file);

                    using (var fs = new FileStream(p, FileMode.Create))
                    using (var sw = new StreamWriter(fs))
                    {
                        sw.WriteLine(@"test");
                    }

                    File.Delete(p);
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }
            return false;
        }

        /// <summary>
        /// Recursively copies the contents of the source directory to the target directory, 
        /// without exclusions.
        /// </summary>
        /// <param name="source">The source directory.</param>
        /// <param name="target">The target directory.</param>
        public static void CopyDirectoryContents(DirectoryInfo source, DirectoryInfo target)
        {
            CopyDirectoryContents(source, target, null);
        }

        /// <summary>
        /// Recursively copies the contents of the source directory to the target directory, with 
        /// an optional list of directories to exclude.
        /// </summary>
        /// <param name="source">The source directory.</param>
        /// <param name="target">The target directory.</param>
        /// <param name="directoryNamesToExclude">An optional list of directory names to exclude from the copy.</param>
        public static void CopyDirectoryContents(DirectoryInfo source, DirectoryInfo target, List<string> directoryNamesToExclude)
        {
            foreach (DirectoryInfo dir in source.GetDirectories().Where(d => (directoryNamesToExclude ?? new List<string>()).FirstOrDefault(dn => dn.Equals(d.Name, StringComparison.OrdinalIgnoreCase)) == null))
                CopyDirectoryContents(dir, target.CreateSubdirectory(dir.Name), directoryNamesToExclude);

            foreach (FileInfo file in source.GetFiles())
                file.CopyTo(Path.Combine(target.FullName, file.Name));
        }

        /// <summary>
        /// If the directory does not exist, it is created.
        /// </summary>
        /// <param name="directoryName">The name of the directory to create.</param>
        /// <returns></returns>
        public static bool CreateDirectoryIfNotExists(string directoryName)
        {
            try
            {
                if (!Directory.Exists(directoryName))
                {
                    Directory.CreateDirectory(directoryName);
                }
            }
            catch (Exception ex)
            {
                Utils.Log("Utils.CreateDirectoryIfNotExists", ex);
                throw;  // re-throw error so error message bubbles up.
            }

            return true;
        }

        /// <summary>
        /// Recycle app pool
        /// </summary>
        public static bool RecycleIIS()
        {
            try
            {
                var poolId = HttpContext.Current.Request.ServerVariables["APP_POOL_ID"];
                var poolPath = @"IIS://" + Environment.MachineName + "/W3SVC/AppPools/" + poolId;

                using (var appPool = new DirectoryEntry(poolPath))
                {
                    appPool.Invoke("Recycle", null);
                }
                return true;
            }
            catch { }

            //try
            //{
            //    HttpRuntime.UnloadAppDomain();
            //    return true;
            //}
            //catch { }

            //try
            //{
            //    Process.GetCurrentProcess().Kill();
            //    return true;
            //}
            //catch { }

            string webConfigPath = HttpContext.Current.Request.PhysicalApplicationPath + "\\web.config";
            try
            {
                File.SetLastWriteTimeUtc(webConfigPath, DateTime.UtcNow);
                return true;
            }
            catch (Exception ex)
            {
                Log("Utils.Recycle() : " + ex.Message);
            }
            return false;
        }

        /// <summary>
        /// Gets the client's IP address.
        /// This method takes into account the X-Forwarded-For header,
        /// in case the blog is hosted behind a load balancer or proxy.
        /// </summary>
        /// <returns>The client's IP address.</returns>
        public static string GetClientIP()
        {
            var context = HttpContext.Current;
            if (context != null)
            {
                var request = context.Request;
                if (request != null)
                {
                    string xff = request.Headers["X-Forwarded-For"];
                    string clientIP = string.Empty;
                    if (!string.IsNullOrWhiteSpace(xff))
                    {
                        int idx = xff.IndexOf(',');
                        if (idx > 0)
                        {
                            // multiple IP addresses, pick the first one
                            clientIP = xff.Substring(0, idx);
                        }
                        else
                        {
                            clientIP = xff;
                        }
                    }

                    return string.IsNullOrWhiteSpace(clientIP) ? request.UserHostAddress : clientIP;
                }
            }

            return string.Empty;
        }

        #endregion

        #region Methods

        /// <summary>
        /// To support compiled extensions
        ///     This methed looks for DLLs in the "/bin" folder
        ///     and if assembly compiled with configuration
        ///     attributed set to "BlogEngineExtension" it will
        ///     be added to the list of code assemlies
        /// </summary>
        private static IEnumerable<Assembly> GetCompiledExtensions()
        {
            var assemblies = new List<Assembly>();
            var s = Path.Combine(HttpContext.Current.Server.MapPath("~/"), "bin");
            var fileEntries = Directory.GetFiles(s);
            foreach (var asm in from fileName in fileEntries
                                where fileName.EndsWith(".dll", StringComparison.OrdinalIgnoreCase)
                                select Assembly.LoadFrom(fileName)
                                into asm
                                let attr = asm.GetCustomAttributes(typeof(AssemblyConfigurationAttribute), false)
                                where attr.Length > 0
                                let aca = (AssemblyConfigurationAttribute)attr[0]
                                select asm)
            {
                try
                {
                    // check if assembly can be loaded
                    // before adding to collection
                    asm.GetTypes();
                    assemblies.Add(asm);
                }
                catch (Exception)
                {
                    // only use to debug - any third party DLL can through this, not a crytical error
                    // Log(string.Format("Error loading compiled extensions from assembly {0}: {1}", asm.FullName, ex.Message));
                }
            }
            return assemblies;
        }

        /// <summary>
        /// Loads the document.
        /// </summary>
        /// <param name="url">
        /// The URL of the document to load.
        /// </param>
        /// <param name="xmlUrl">
        /// The XML URL.
        /// </param>
        /// <returns>
        /// The XmlDocument.
        /// </returns>
        private static XmlDocument LoadDocument(Uri url, Uri xmlUrl)
        {
            string absoluteUrl;

            if (url.IsAbsoluteUri)
            {
                absoluteUrl = url.ToString();
            }
            else if (!url.ToString().StartsWith("/"))
            {
                absoluteUrl = url + xmlUrl.ToString();
            }
            else
            {
                absoluteUrl = string.Format("{0}://{1}{2}", url.Scheme, url.Authority, xmlUrl);
            }

            var readerSettings = new XmlReaderSettings
                {
                    MaxCharactersFromEntities = 1024, XmlResolver = new XmlSafeResolver() 
                };

            XmlDocument doc;
            try
            {
                using (var reader = XmlReader.Create(absoluteUrl, readerSettings))
                {
                    doc = new XmlDocument();
                    doc.Load(reader);
                }
            }
            catch (Exception)
            {
                return null;
            }
            
            return doc;
        }

        /// <summary>
        /// The on email failed.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        private static void OnEmailFailed(MailMessage message)
        {
            if (EmailFailed != null)
            {
                EmailFailed(message, EventArgs.Empty);
            }
        }

        /// <summary>
        /// The on email sent.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        private static void OnEmailSent(MailMessage message)
        {
            if (EmailSent != null)
            {
                EmailSent(message, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Removes the diacritics.
        /// </summary>
        /// <param name="text">
        /// The text to remove diacritics from.
        /// </param>
        /// <returns>
        /// The string with the diacritics removed.
        /// </returns>
        private static string RemoveDiacritics(string text)
        {
            var normalized = text.Normalize(NormalizationForm.FormD);
            var sb = new StringBuilder();

            foreach (var c in
                normalized.Where(c => CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark))
            {
                sb.Append(c);
            }

            return sb.ToString();
        }

        /// <summary>
        /// Removes any unicode punctuation
        /// </summary>
        /// <param name="text">
        /// The text to remove punctuation from.
        /// </param>
        /// <returns>
        /// The string with the punctuation removed.
        /// </returns>
        private static string RemoveUnicodePunctuation(string text)
        {
            var normalized = text.Normalize(NormalizationForm.FormD);
            var sb = new StringBuilder();

            foreach (var c in
                normalized.Where(c => CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.InitialQuotePunctuation &&
                                      CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.FinalQuotePunctuation))
            {
                sb.Append(c);
            }

            return sb.ToString();
        }

        /// <summary>
        /// Removes the extra hyphen.
        /// </summary>
        /// <param name="text">
        /// The text to remove the extra hyphen from.
        /// </param>
        /// <returns>
        /// The text with the extra hyphen removed.
        /// </returns>
        private static string RemoveExtraHyphen(string text)
        {
            if (text.Contains("--"))
            {
                text = text.Replace("--", "-");
                return RemoveExtraHyphen(text);
            }

            return text;
        }

        #endregion
    }
}
