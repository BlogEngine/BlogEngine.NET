using System;

namespace BlogEngine.Core.Data.Models
{
    /// <summary>
    /// The blog
    /// </summary>
    public class Blog
    {
        /// <summary>
        /// If checked in the UI
        /// </summary>
        public bool IsChecked { get; set; }

        /// <summary>
        /// Blog ID
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Blog name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Virtual Path.
        /// </summary>
        public string VirtualPath { get; set; }

        /// <summary>
        /// Storage Container Name.
        /// </summary>
        public string StorageContainerName { get; set; }

        /// <summary>
        /// Hostname.
        /// </summary>
        public string Hostname { get; set; }

        /// <summary>
        /// IsAnyTextBeforeHostnameAccepted.
        /// </summary>
        public bool IsAnyTextBeforeHostnameAccepted { get; set; }

        /// <summary>
        /// Physical path to the Storage Location (READ ONLY).
        /// This is "StorageLocation" after running it thru MapPath.
        /// </summary>
        public string PhysicalStorageLocation { get; set; }

        /// <summary>
        /// Relative Web Root (READ ONLY).
        /// </summary>
        public string RelativeWebRoot { get; set; }

        /// <summary>
        /// Absolute Web Root (READ ONLY).
        /// </summary>
        public Uri AbsoluteWebRoot { get; set; }

        /// <summary>
        /// Whether the blog instance is the Primary instance.
        /// </summary>
        public bool IsPrimary { get; set; }

        /// <summary>
        /// Active status
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// Whether the blog instance is a Site Aggregation instance.
        /// </summary>
        public bool IsSiteAggregation { get; set; }

        /// <summary>
        /// If the current user can delete this page.
        /// </summary>
        public bool CanUserDelete { get; set; }

        /// <summary>
        /// If the current user can edit this page.
        /// </summary>
        public bool CanUserEdit { get; set; }
    }
}
