using System;
using BlogEngine.Core;
using BlogEngine.Wiki.Interfaces;
using Gwn.BlogEngine.Library.Base;
using Gwn.BlogEngine.Library.Entities;
using Gwn.BlogEngine.Library.Events;
using Gwn.BlogEngine.Library.Extensions;

namespace BlogEngine.Wiki
{
    /// <summary>
    /// Gwn Wiki Engine
    /// </summary>
    public class GwnWikiEngine : BeExtensionEngineBase
    {
        /// <summary>
        /// Gets or sets the wiki provider.  Provided as a 
        /// convenience property (so we don't have to cast in code)
        /// </summary>
        /// <value>The wiki provider.</value>
        public IGwnWikiProvider WikiProvider
        {
            get { return (IGwnWikiProvider)Provider; }
            set { Provider = value; }
        }

        /// <summary>
        /// Initializes the <see cref="GwnWikiEngine"/> class.
        /// </summary>
        static GwnWikiEngine()
        {
            // Configure the provider that will handle the data processing
            Provider = new GwnWikiProvider();    
        }

        /// <summary>
        /// BlogEngine serving.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="Gwn.BlogEngine.Library.Events.BeEventArgs"/> instance containing the event data.</param>
        protected override void HandleBlogEngineServing(object sender, BeEventArgs e)
        {
            Logger.Log("{0}: Handling BlogEngineServing()", GetType().Name);

            var post = sender.Cast<Post>();

            // Uses an extension method to retrieve the Wiki Commands from 
            // the Content (current page)
            var wikiCommandsToProcess = post.Content.GetWikiCommands();

            // Process each Wiki command found in the e.Body
            foreach (var wikiCommand in wikiCommandsToProcess)
            {
                // Construct the original Wiki key (brackets were stripped off)
                var original = string.Format("[{0}]", wikiCommand.Original);

                // Get the display template for the wiki command - expects
                // a {0} value in the returned template
                var displayTemplateDataRow =
                    WikiProvider.GetDisplayTemplate(wikiCommand.Command, wikiCommand.CommandParameter);

                // A tuple will return template, selected DataRow and Table
                var displayTemplate = displayTemplateDataRow.Item1;
                var dataRow = displayTemplateDataRow.Item2;
                var dataTable = displayTemplateDataRow.Item3;

                // Not all square brackets will belong to a wiki command - if not
                // a valid command a null will be returned
                if (dataRow != null)
                {
                    string displayValue = "";
                    try
                    {
                        // Get the formatted text using the provided display template if
                        // this is a Page or Display - otherwise use provided command parameter
                        if ("Page.Display".Contains(wikiCommand.Command))
                            displayValue = string.Format(displayTemplate, wikiCommand.DisplayTemplate);
                        else
                            displayValue = string.Format(displayTemplate, wikiCommand.CommandParameter);

                        // Supported Template Tags follow:
                        displayValue = displayValue.Replace("[host]", post.PermaLink.Host);
                        displayValue = displayValue.Replace("[id]", post.Id.ToString());

                        // If PermaLink then use the perma link for the current CommandParameter
                        if (displayValue.Contains("PermaLink"))
                        {
                            var result = dataTable.FindRowByCommandParameter(wikiCommand.CommandParameter);
                            if (result != null)
                                displayValue = displayValue // Replace PermaLink with stored value
                                    .Replace("[PermaLink]", result[BeSettingRecord.PermaLinkIndex].ToString());
                        }
                    }
                    catch(Exception ex)
                    {
                        e.Body.Replace(original, ex.Message);
                    }

                    // Find the bracketed command and replace with generated display value
                    e.Body = e.Body.Replace(original, displayValue);
                }
            }
        }

        #region NOT USED - for reference only (they are overrides so they do not need to exist)

        /// <summary>
        /// Handles the blog engine saving (currently not used) 
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="Gwn.BlogEngine.Library.Events.BeEventArgs"/> instance containing the event data.</param>
        protected override void HandleBlogEngineSaving(object sender, BeEventArgs e)
        {
            Logger.Log("{0}: Handling HandleBlogEngineSaving()", GetType().Name);

            // Calls Provider.Saved(sender,e) from base - logs message
            base.HandleBlogEngineSaving(sender, e);
        }

        /// <summary>
        /// Handles the blog engine saved (currently not used)
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="Gwn.BlogEngine.Library.Events.BeEventArgs"/> instance containing the event data.</param>
        protected override void HandleBlogEngineSaved(object sender, BeEventArgs e)
        {
            Logger.Log("{0}: Handling HandleBlogEngineSaved()", GetType().Name);

            // Calls Provider.Insert, Update or Delete from base
            // as applicable (based on SaveAction value) - logs message
            base.HandleBlogEngineSaved(sender, e);
        }

        #endregion 
    }
}