using System;
using System.Collections.Generic;
using System.Data;
using BlogEngine.Wiki.Interfaces;
using Gwn.BlogEngine.Library.Base;
using Gwn.BlogEngine.Library.Entities;
using Gwn.BlogEngine.Library.Events;
using Gwn.BlogEngine.Library.Extensions;

namespace BlogEngine.Wiki
{
    /// <summary>
    /// Gwn Wiki Provider
    /// </summary>
    public class GwnWikiProvider : BeExtensionProviderBase, IGwnWikiProvider
    {
        /// <summary>
        /// Initializes the <see cref="GwnWikiProvider"/> class.
        /// </summary>
        static GwnWikiProvider()
        {
            Logger.Log("GwnWikiProvider: Constructor()");
            
            // Needed by extension settings - must match extension class name
            ProviderName = "GwnWikiExtension";

            // Default settings to use for extension.  BeSettingRecord provides
            // a means to strongly type the Settings.

            DefaultSettings = new List<BeSettingRecord> {
                new BeSettingRecord {
                        Command = "Page",
                        CommandParameter = "PageDefault",
                        DisplayTemplate = "<b><i>{0}</i></b>"
                    },

                new BeSettingRecord {
                        Command = "Display",
                        CommandParameter = "DisplayDefault",
                        DisplayTemplate = "<a target='_blank' href='[PermaLink]'>{0}</a>"
                    }
            };
        }

        /// <summary>
        /// Handles the insert - if base.HandleInsert(sender,e) is called then
        /// the data will be saved
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="Gwn.BlogEngine.Library.Events.BeEventArgs"/> instance containing the event data.</param>
        protected override void HandleInsert(object sender, BeEventArgs e)
        {
            Logger.Log("{0}: Handling HandleInsert()", GetType().Name);

            var templateRecord = GetTemplatePageCommand(sender, e);
            var displayTemplate = templateRecord.Item1;
            var pageCommand = templateRecord.Item2;
            var commandParameterSrch = templateRecord.Item3;

            // Nothing to do if page command was not found
            if (pageCommand == null) return; 

            // If command was not found in persisted storage then we
            // need to add it so other pages can reference it
            if (commandParameterSrch == null)
            {
                // Create the record that will be added
                var data = new BeSettingRecord
                {
                    Command = pageCommand.Command,
                    CommandParameter = pageCommand.CommandParameter,
                    DisplayTemplate = displayTemplate,
                    Id = Guid.Parse(e.Id),
                    PermaLink = e.PermaLink
                };

                // Add the newly created record
                Settings.AddValues(data.GetExtensionData());

                // Saves record
                base.HandleInsert(sender, e);
            }
            
        }

        /// <summary>
        /// Handles the update.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="Gwn.BlogEngine.Library.Events.BeEventArgs"/> instance containing the event data.</param>
        protected override void HandleUpdate(object sender, BeEventArgs e)
        {
            Logger.Log("{0}: Handling HandleUpdate()", GetType().Name);

            var templateRecord = GetTemplatePageCommand(sender, e);
            var displayTemplate = templateRecord.Item1;
            var commandParameterSrch = templateRecord.Item3;

            if (commandParameterSrch == null)
            {
                Insert(sender, e);
                return; // We're done
            }

            // If the current display template matches the persisted template
            // then there is nothing to update.  If different then update
            if (!commandParameterSrch[BeSettingRecord.DisplayTemplateIndex].SameValueAs(displayTemplate))
                base.HandleUpdate(sender, e);    // Saves record

        }

        /// <summary>
        /// Gets the display template.
        /// </summary>
        /// <param name="wikiCommand">The wiki command.</param>
        /// <param name="wikiCommandParameter">The wiki command parameter.</param>
        /// <returns></returns>
        public Tuple<string, DataRow, DataTable> GetDisplayTemplate(string wikiCommand, string wikiCommandParameter)
        {
            Logger.Log("{0}: Handling GetDisplayTemplate()", GetType().Name);

            var template = LeaveAloneTemplate;      // Default to leaveAloneTemplate
            var table = Settings.GetDataTable();    // Get Settings as data table

            // Search for the parameter, i.e. MultiTargeting
            var result = table.FindRowByCommandParameter(wikiCommandParameter);
            if (result == null && "Page.Display".Contains(wikiCommand))
                return new Tuple<string, DataRow, DataTable>(template, result, table);

            // If this is a custom command we'll search for it
            if (result == null)
            {
                result = table.FindRowByCommand(wikiCommand);
                if (result != null)
                {
                    template = result[BeSettingRecord.DisplayTemplateIndex].ToString();
                    if(result!=null)
                    {
                        template = template.Replace("{{", "<").Replace("}}}", "}>").Replace("}}",">");
                    }
                }
            }
            else
            {
                // If this template is using the xxxxxDefault then we'll need to
                // retrieve the template based on the wikiCommand (Page or Display)
                if (result[BeSettingRecord.DisplayTemplateIndex].SameValueAs("DisplayDefault")
                    || result[BeSettingRecord.DisplayTemplateIndex].SameValueAs("PageDefault"))
                    result = table.FindRowByCommandParameter(wikiCommand + "Default");

                if (result != null) // If the template was found then set it
                    template = result[BeSettingRecord.DisplayTemplateIndex].ToString();
            }
            return new Tuple<string, DataRow, DataTable>(template, result, table);
        }

        /// <summary>
        /// Gets the template page command.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="Gwn.BlogEngine.Library.Events.BeEventArgs"/> instance containing the event data.</param>
        /// <returns></returns>
        private static Tuple<string, BeSettingRecord, DataRow> GetTemplatePageCommand(object sender, BeEventArgs e)
        {
            // Get Settings as data table
            var table = Settings.GetDataTable();

            // We are only concerned with updating Page commands and
            // each post will only have one Page configured
            var pageCommand = e.CommandList.FindCommand("Page");
            if (pageCommand == null)
                return new Tuple<string, BeSettingRecord, DataRow>(null, null, null);

            // Retrieve the configured template
            var displayTemplate = pageCommand.DisplayTemplate;

            // If the template is the split camel case of the commandParameter
            // then convert it to PageDefault
            if (displayTemplate == pageCommand.CommandParameter.SplitCamelCase())
                displayTemplate = "PageDefault";

            // Search for the original commandParameter, i.e., PortableClassLibrary
            var commandParameterSrch =
                table.FindRowByCommandParameter(pageCommand.CommandParameter);

            return new Tuple<string, BeSettingRecord, DataRow>(
                displayTemplate, pageCommand, commandParameterSrch);
        }


    }
}