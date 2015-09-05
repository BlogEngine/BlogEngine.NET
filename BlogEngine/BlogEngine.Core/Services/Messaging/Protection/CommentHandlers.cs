namespace BlogEngine.Core
{
    using System;
    using System.ComponentModel;
    using System.Data;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;
    using System.Threading;

    using BlogEngine.Core.Web.Extensions;

    /// <summary>
    /// Rules, Filters and anti-spam services use this class
    ///     to handle adding new comment to the blog
    /// </summary>
    public static class CommentHandlers
    {
        #region Constants and Fields

        /// <summary>
        ///     The custom filters.
        /// </summary>
        private static ExtensionSettings customFilters;

        /// <summary>
        ///     The filters.
        /// </summary>
        private static ExtensionSettings filters;

        #endregion

        #region Public Methods

        /// <summary>
        /// Add item to white or black list
        /// </summary>
        /// <param name="subject">
        /// Item subject.
        /// </param>
        /// <param name="value">
        /// Item value.
        /// </param>
        /// <param name="isspam">
        /// True if comment is spam
        /// </param>
        public static void AddItemToFilter(string subject, string value, bool isspam)
        {
            var indx = 0;
            var match = false;

            var dt = filters.GetDataTable();
            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    var rowSubject = row["Subject"].ToString();
                    var filter = row["Filter"].ToString().Trim();

                    if (rowSubject.Equals(subject, StringComparison.OrdinalIgnoreCase) &&
                        filter.Equals(value, StringComparison.OrdinalIgnoreCase))
                    {
                        match = true;
                        break;
                    }

                    indx++;
                }
            }

            if (match)
            {
                var log = "Removed old filter ";

                // remove old filter
                foreach (var par in filters.Parameters)
                {
                    log += string.Format(":{0}", par.Values[indx]);
                    par.DeleteValue(indx);
                }

                ExtensionManager.SaveSettings("MetaExtension", filters);
                Utils.Log(log);
            }

            // add value to filters
            var id = Guid.NewGuid().ToString();
            var action = isspam ? "Block" : "Allow";
            var blackWhiteList = isspam ? "Black" : "White";
            var f = new[] { id, action, subject, "Equals", value };

            filters.AddValues(f);
            ExtensionManager.SaveSettings("MetaExtension", filters);

            Utils.Log(string.Format("{0} added to {1} list: {2}", subject, blackWhiteList, value));
        }

        /// <summary>
        /// Add comment IP to white or black list
        /// </summary>
        /// <param name="ip">
        /// Comment IP
        /// </param>
        /// <param name="isspam">
        /// True if comment is spam
        /// </param>
        public static void AddIpToFilter(string ip, bool isspam)
        {
            AddItemToFilter("IP", ip, isspam);
        }

        /// <summary>
        /// Add comment email address to white or black list
        /// </summary>
        /// <param name="email">
        /// Email address.
        /// </param>
        /// <param name="isspam">
        /// True if comment is spam
        /// </param>
        public static void AddEmailToFilter(string email, bool isspam)
        {
            if (string.IsNullOrEmpty(email) || !email.Contains("@")) { return; }
            AddItemToFilter("Email", email, isspam);
        }

        /// <summary>
        /// Instantiates custom filter object
        /// </summary>
        /// <param name="className">
        /// Name of the class to instantiate
        /// </param>
        /// <returns>
        /// Object as ICustomFilter
        /// </returns>
        public static ICustomFilter GetCustomFilter(string className)
        {
            try
            {
                var codeAssemblies = Utils.CodeAssemblies();
                return
                    codeAssemblies.Select(a => a.GetType(className)).Where(t => t != null).Select(
                        t => (ICustomFilter)Activator.CreateInstance(t)).FirstOrDefault();
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// Initiate adding comment event listener
        /// </summary>
        public static void Listen()
        {
            Post.AddingComment += PostAddingComment;

            InitFilters();
            InitCustomFilters();
        }

        /// <summary>
        /// Report to service if comment is really spam
        /// </summary>
        /// <param name="comment">
        /// Comment object
        /// </param>
        public static void ReportMistake(Comment comment)
        {
            var m = comment.ModeratedBy;
            var dt = customFilters.GetDataTable();
            var i = 0;

            // rejecting auto-approved comment; all made mistake letting it through
            bool spamMissedByAll = comment.IsApproved && comment.ModeratedBy == "Auto";

            foreach (var filterName in from DataRow row in dt.Rows select row[0].ToString())
            {
                if (filterName == m || spamMissedByAll)
                {
                    if (!ExtensionManager.ExtensionEnabled(ExtensionNameFromClassName(filterName)))
                        continue;

                    var customFilter = GetCustomFilter(filterName);

                    if (customFilter != null)
                    {
                        var mistakes = int.Parse(customFilters.Parameters[4].Values[i]);
                        customFilters.Parameters[4].Values[i] = (mistakes + 1).ToString();

                        ExtensionManager.SaveSettings("MetaExtension", customFilters);

                        if (BlogSettings.Instance.CommentReportMistakes)
                            customFilter.Report(comment);
                    }
                    if (!spamMissedByAll) break;
                }
                i++;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// The init custom filters.
        /// </summary>
        private static void InitCustomFilters()
        {
            var settings = new ExtensionSettings("BeCustomFilters");

            settings.AddParameter("FullName", "Name", 100, true, true);
            settings.AddParameter("Name");
            settings.AddParameter("Checked");
            settings.AddParameter("Cought");
            settings.AddParameter("Reported");
            settings.AddParameter("Priority");

            customFilters = ExtensionManager.InitSettings("MetaExtension", settings);
            if (customFilters == null)
            {
                return;
            }

            var dt = customFilters.GetDataTable();
            var codeAssemblies = Utils.CodeAssemblies();

            foreach (var type in
                codeAssemblies.Cast<Assembly>().Select(a => a.GetTypes()).SelectMany(
                    types => (from type in types
                              where type.GetInterface("BlogEngine.Core.ICustomFilter") != null
                              let found = dt.Rows.Cast<DataRow>().Any(row => row[0].ToString() == type.Name)
                              where !found
                              select type)))
            {
                // if new filter found in the assembly, add it to settings
                if (!customFilters.IsKeyValueExists(type.FullName))
                {
                    customFilters.AddValues(new[] { type.FullName, type.Name, "0", "0", "0", "0" });
                    ExtensionManager.SaveSettings("MetaExtension", customFilters);
                }
            }
        }

        /// <summary>
        /// Inits the filters.
        /// </summary>
        private static void InitFilters()
        {
            var settings = new ExtensionSettings("BeCommentFilters");

            settings.AddParameter("ID", "ID", 20, true, true, ParameterType.Integer);
            settings.AddParameter("Action");
            settings.AddParameter("Subject");
            settings.AddParameter("Operator");
            settings.AddParameter("Filter");

            filters = ExtensionManager.InitSettings("MetaExtension", settings);
        }

        /// <summary>
        /// Moderates the comment by the filter.
        /// </summary>
        /// <param name="comment">
        /// The comment.
        /// </param>
        /// <returns>
        /// Whether the comment is moderated by a filter.
        /// </returns>
        private static bool ModeratedByFilter(Comment comment)
        {
            var dt = filters.GetDataTable();

            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    var action = row["Action"].ToString();
                    var subject = row["Subject"].ToString();
                    var oper = row["Operator"].ToString();
                    var filter = row["Filter"].ToString().Trim().ToLower(CultureInfo.InvariantCulture);

                    var comm = comment.Content.ToLower(CultureInfo.InvariantCulture);
                    var auth = comment.Author.ToLower(CultureInfo.InvariantCulture);

                    var wsite = string.Empty;
                    if (comment.Website != null)
                    {
                        wsite = comment.Website.ToString().ToLower(CultureInfo.InvariantCulture);
                    }

                    var email = comment.Email.ToLower(CultureInfo.InvariantCulture);

                    var match = false;

                    if (oper == "Equals")
                    {
                        switch (subject)
                        {
                            case "IP":
                                if (comment.IP == filter)
                                {
                                    match = true;
                                }

                                break;
                            case "Author":
                                if (auth == filter)
                                {
                                    match = true;
                                }

                                break;
                            case "Website":
                                if (wsite == filter)
                                {
                                    match = true;
                                }

                                break;
                            case "Email":
                                if (email == filter)
                                {
                                    match = true;
                                }

                                break;
                            case "Comment":
                                if (comm == filter)
                                {
                                    match = true;
                                }

                                break;
                        }
                    }
                    else
                    {
                        switch (subject)
                        {
                            case "IP":
                                if (comment.IP.Contains(filter))
                                {
                                    match = true;
                                }

                                break;
                            case "Author":
                                if (auth.Contains(filter))
                                {
                                    match = true;
                                }

                                break;
                            case "Website":
                                if (wsite.Contains(filter))
                                {
                                    match = true;
                                }

                                break;
                            case "Email":
                                if (email.Contains(filter))
                                {
                                    match = true;
                                }

                                break;
                            case "Comment":
                                if (comm.Contains(filter))
                                {
                                    match = true;
                                }

                                break;
                        }
                    }

                    if (!match)
                    {
                        continue;
                    }

                    if (action == "Delete")
                    {
                        comment.IsApproved = false;
                        comment.ModeratedBy = "Delete";
                        return true;
                    }

                    comment.IsApproved = action != "Block";

                    // if filter out, set as spam
                    // so that no need to moderate
                    if (!comment.IsApproved)
                        comment.IsSpam = true;

                    comment.ModeratedBy = "Filter";
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Returns whether the comment is moderated by a rule.
        /// </summary>
        /// <param name="comment">
        /// The comment.
        /// </param>
        /// <returns>
        /// Whether comment is moderated by a rule.
        /// </returns>
        private static bool ModeratedByRule(Comment comment)
        {
            // trust authenticated users
            if (Security.IsAuthenticated && BlogSettings.Instance.TrustAuthenticatedUsers)
            {
                comment.IsApproved = true;
                comment.ModeratedBy = "Rule:authenticated";
                return true;
            }

            var blackCnt = 0;
            var whiteCnt = 0;

            // check if this user already has approved or
            // rejected comments and belongs to white/black list
            foreach (var c in from p in Post.Posts
                              from c in p.Comments
                              where c.Email.ToLowerInvariant() == comment.Email.ToLowerInvariant() || c.IP == comment.IP
                              select c)
            {
                
#if DEBUG
                // disable for local testing
                if (c.IP == "127.0.0.1") continue;
#endif
                

                if (c.IsApproved)
                {
                    whiteCnt++;
                }
                else
                {
                    blackCnt++;
                }
            }

            // user posted 10 or more approved comments - trust
            if (whiteCnt >= 10)
            {
                comment.IsApproved = true;
                comment.ModeratedBy = "Rule:white list";
                return true;
            }

            // user posted 10 or more spam comments - reject
            if (blackCnt >= 10)
            {
                comment.IsSpam = true;
                comment.IsApproved = false;
                comment.ModeratedBy = "Rule:black list";
                return true;
            }

            return false;
        }

        /// <summary>
        /// Handles the AddingComment event of the Post control.
        /// </summary>
        /// <param name="sender">
        /// The source of the event.
        /// </param>
        /// <param name="e">
        /// The <see cref="System.ComponentModel.CancelEventArgs"/> instance containing the event data.
        /// </param>
        private static void PostAddingComment(object sender, CancelEventArgs e)
        {
            var comment = (Comment)sender;

            // if not moderated, comments approved by default
            // and then go through filters that can reject spam
            if (!BlogSettings.Instance.EnableCommentsModeration)
            {
                comment.IsApproved = true;
                comment.ModeratedBy = "Auto";
            }

            if (!ModeratedByRule(comment))
            {
                if (ModeratedByFilter(comment))
                {
                    if (!comment.IsApproved && comment.ModeratedBy == "Delete")
                    {
                        e.Cancel = true;
                    }
                }
                else
                {
                    RunCustomModerators(comment);
                }
            }
        }

        /// <summary>
        /// The run custom moderators.
        /// </summary>
        /// <param name="comment">
        /// The comment.
        /// </param>
        private static void RunCustomModerators(Comment comment)
        {
            var dt = customFilters.GetDataTable();
            dt.DefaultView.Sort = "Name";

            foreach (DataRowView row in dt.DefaultView)
            {
                var filterName = row[0].ToString();

                if (!ExtensionManager.ExtensionEnabled(ExtensionNameFromClassName(filterName)))
                    continue;

                var customFilter = GetCustomFilter(filterName);

                if (customFilter == null || !customFilter.Initialize())
                {
                    continue;
                }

                if (customFilter.Check(comment))
                {
                    // caught spam!
                    comment.IsSpam = true;
                    comment.IsApproved = false;
                    comment.ModeratedBy = filterName;

                    Utils.Log(
                    string.Format("Custom filter [{0}] - found spam; comment id: {1}", filterName, comment.Id));

                    UpdateCustomFilter(filterName, false);
                    break;
                }
                else
                {
                    UpdateCustomFilter(filterName, true);
                }
            }
        }

        /// <summary>
        /// The update custom filter.
        /// </summary>
        /// <param name="filter">
        /// The filter.
        /// </param>
        /// <param name="approved">
        /// The approved.
        /// </param>
        private static void UpdateCustomFilter(string filter, bool approved)
        {
            var dt = customFilters.GetDataTable();
            var i = 0;

            foreach (var filterName in dt.Rows.Cast<DataRow>().Select(row => row[0].ToString()))
            {
                if (filterName == filter)
                {
                    var total = int.Parse(customFilters.Parameters[2].Values[i]);
                    var spam = int.Parse(customFilters.Parameters[3].Values[i]);

                    customFilters.Parameters[2].Values[i] = (total + 1).ToString();
                    if (!approved)
                    {
                        customFilters.Parameters[3].Values[i] = (spam + 1).ToString();
                    }

                    break;
                }

                i++;
            }

            ExtensionManager.SaveSettings("MetaExtension", customFilters);
        }

        private static string ExtensionNameFromClassName(string className)
        {
            return className.Contains(".") ? className.Substring(className.LastIndexOf(".") + 1) : className;
        }

        #endregion
    }
}