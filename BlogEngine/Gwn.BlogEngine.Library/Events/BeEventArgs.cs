using System;
using System.Collections.Generic;
using BlogEngine.Core;
using Gwn.BlogEngine.Library.Entities;
using Gwn.BlogEngine.Library.Types;

namespace Gwn.BlogEngine.Library.Events
{
    /// <summary>
    /// 
    /// </summary>
    public class BeEventArgs : EventArgs
    {
        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        /// <value>The id.</value>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the perma link.
        /// </summary>
        /// <value>The perma link.</value>
        public string PermaLink { get; set; }

        /// <summary>
        /// Gets or sets the content.
        /// </summary>
        /// <value>The content.</value>
        public string Content { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="BeEventArgs"/> is cancel.
        /// </summary>
        /// <value><c>true</c> if cancel; otherwise, <c>false</c>.</value>
        public bool Cancel { get; set; }

        /// <summary>
        /// Gets or sets the location.
        /// </summary>
        /// <value>The location.</value>
        public ServingLocation Location { get; set; }

        /// <summary>
        /// Gets or sets the content by.
        /// </summary>
        /// <value>The content by.</value>
        public ServingContentBy ContentBy { get; set; }

        /// <summary>
        /// Gets or sets the type of the process.
        /// </summary>
        /// <value>The type of the process.</value>
        public BeProcessType ProcessType { get; set; }

        /// <summary>
        /// Gets or sets the action.
        /// </summary>
        /// <value>The action.</value>
        public SaveAction SaveAction { get; set; }

        /// <summary>
        /// Gets or sets the command list.
        /// </summary>
        /// <value>The command list.</value>
        public List<BeSettingRecord> CommandList { get; set; }

        public string Body { get; set; }
    }
}
