using System;
using System.Collections.Generic;
using System.Web.Caching;
using System.Web.UI;

namespace BlogEngine.Core.Web.Controls
{
    /// <summary>
    /// Base class for comment form control
    /// </summary>
    public class CommentFormBase : UserControl
    {
        /// <summary>
        /// Current post
        /// </summary>
        public IPublishable PostItem { get; set; }


    }
}
