using System;
using System.Collections.Generic;
using BlogEngine.Core.Data.Models;

namespace BlogEngine.Core.Data.ViewModels
{
    /// <summary>
    /// Setting view model
    /// </summary>
    public class SettingsVM
    {
        /// <summary>
        /// Blog settings
        /// </summary>
        public Settings Settings { get; set; }

        /// <summary>
        /// Time zones
        /// </summary>
        public List<SelectOption> TimeZones
        {
            get
            {
                var zones = new List<SelectOption>();
                foreach (var zone in TimeZoneInfo.GetSystemTimeZones())
                {
                    zones.Add(new SelectOption { OptionName = zone.DisplayName, OptionValue = zone.Id });
                }
                return zones;
            }
        }
        /// <summary>
        /// Feed options
        /// </summary>
        public List<SelectOption> FeedOptions
        {
            get
            {
                var options = new List<SelectOption>();
                options.Add(new SelectOption { OptionName = "RSS 2.0", OptionValue = "Rss" });
                options.Add(new SelectOption { OptionName = "Atom 1.0", OptionValue = "Atom" });
                return options;
            }
        }
        /// <summary>
        /// Closed days options
        /// </summary>
        public List<SelectOption> CloseDaysOptions
        {
            get
            {
                var options = new List<SelectOption>();
                options.Add(new SelectOption { OptionName = "Never" , OptionValue = "0" });
                options.Add(new SelectOption { OptionName = "1", OptionValue = "1" });
                options.Add(new SelectOption { OptionName = "2", OptionValue = "2" });
                options.Add(new SelectOption { OptionName = "3", OptionValue = "3" });
                options.Add(new SelectOption { OptionName = "7", OptionValue = "7" });
                options.Add(new SelectOption { OptionName = "10", OptionValue = "10" });
                options.Add(new SelectOption { OptionName = "14", OptionValue = "14" });
                options.Add(new SelectOption { OptionName = "21", OptionValue = "21" });
                options.Add(new SelectOption { OptionName = "30", OptionValue = "30" });
                options.Add(new SelectOption { OptionName = "60", OptionValue = "60" });
                options.Add(new SelectOption { OptionName = "90", OptionValue = "90" });
                options.Add(new SelectOption { OptionName = "180", OptionValue = "180" });
                options.Add(new SelectOption { OptionName = "365", OptionValue = "365" });
                return options;
            }
        }

        //public List<SelectOption> FeedOptions
        //{
        //    get
        //    {
        //        var options = new List<SelectOption>();
        //        options.Add(new SelectOption { OptionName = "", OptionValue = "" });
        //        return options;
        //    }
        //}

    }
}
