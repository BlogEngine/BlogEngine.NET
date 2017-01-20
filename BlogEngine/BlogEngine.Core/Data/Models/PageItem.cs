﻿using System;

namespace BlogEngine.Core.Data.Models
{
    /// <summary>
    /// Blog page
    /// </summary>
    public class PageItem
    {
        /// <summary>
        /// If checked in the UI
        /// </summary>
        public bool IsChecked { get; set; }
        /// <summary>
        /// Page ID
        /// </summary>
        public Guid Id { get; set; }
        /// <summary>
        /// Page slug
        /// </summary>
        public string Slug { get; set; }
        /// <summary>
        /// Page heading
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// Page title
        /// </summary>
        public string PageTitle { get; set; }
        /// <summary>
        /// Keywords
        /// </summary>
        public string Keywords { get; set; }
        /// <summary>
        /// Page author
        /// </summary>
        public bool ShowInList { get; set; }
        /// <summary>
        /// If page is published
        /// </summary>
        public bool IsPublished { get; set; }
        /// <summary>
        /// Front page
        /// </summary>
        public bool IsFrontPage { get; set; }
        /// <summary>
        ///     Gets or sets the date portion of published date
        /// </summary>
        public string DateCreated { get; set; }
        /// <summary>
        /// Parent page
        /// </summary>
        public SelectOption Parent{ get; set; }
        /// <summary>
        /// If has child pages
        /// </summary>
        public bool HasChildren { get; set; }
        /// <summary>
        /// Sort order of the page
        /// </summary>
        public int SortOrder { get; set; }
    }
}
