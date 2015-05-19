namespace Recaptcha
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;

    using BlogEngine.Core.Web.Extensions;

    /// <summary>
    /// Methods to save and retrieve reCaptcha logs
    /// </summary>
    public static class RecaptchaLogger
    {
        #region Public Methods

        /// <summary>
        /// Read log data from data store
        /// </summary>
        /// <returns>
        /// List of log items
        /// </returns>
        public static List<RecaptchaLogItem> ReadLogItems()
        {
            var settings = ExtensionManager.GetSettings("Recaptcha", "RecaptchaLog");
            var table = settings.GetDataTable();
            var log = new List<RecaptchaLogItem>();

            if (table.Rows.Count > 0)
            {
                log.AddRange(
                    table.Rows.Cast<DataRow>().Select(
                        row =>
                        new RecaptchaLogItem
                            {
                                Response = (string)row["Response"],
                                Challenge = (string)row["Challenge"],
                                CommentId = new Guid((string)row["CommentID"]),
                                Enabled = bool.Parse(row["Enabled"].ToString()),
                                Necessary = bool.Parse(row["Necessary"].ToString()),
                                NumberOfAttempts = ushort.Parse(row["NumberOfAttempts"].ToString()),
                                TimeToComment = double.Parse(row["TimeToComment"].ToString()),
                                TimeToSolveCapcha = double.Parse(row["TimeToSolveCapcha"].ToString())
                            }));
            }

            return log;
        }

        /// <summary>
        /// Saves log data to datastore as extension settings
        /// </summary>
        /// <param name="items">
        /// List of log items
        /// </param>
        public static void SaveLogItems(List<RecaptchaLogItem> items)
        {
            if (items.Count <= 0)
            {
                return;
            }

            var settings = ExtensionManager.GetSettings("Recaptcha", "RecaptchaLog");
            var table = settings.GetDataTable();

            if (table.Rows.Count > 0)
            {
                for (var i = table.Rows.Count - 1; i > -1; i--)
                {
                    foreach (var par in settings.Parameters)
                    {
                        par.DeleteValue(i);
                    }
                }
            }

            foreach (var item in items)
            {
                settings.AddValues(
                    new[]
                        {
                            item.Response, item.Challenge, item.CommentId.ToString(), item.TimeToComment.ToString(), 
                            item.TimeToSolveCapcha.ToString(), item.NumberOfAttempts.ToString(), 
                            item.Enabled.ToString(), item.Necessary.ToString()
                        });
            }

            ExtensionManager.SaveSettings("Recaptcha", settings);
        }

        #endregion
    }
}