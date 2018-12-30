using System;
using BlogEngine.Core;
using Gwn.BlogEngine.Library.Events;
using Gwn.BlogEngine.Library.Extensions;
using Gwn.BlogEngine.Library.Interfaces;
using Gwn.BlogEngine.Library.Logger;
using Gwn.BlogEngine.Library.Types;

namespace Gwn.BlogEngine.Library.Base
{
    public class BeExtensionBase
    {
        /// <summary>
        /// Occurs when [extension event].
        /// </summary>
        public event EventHandler<BeEventArgs> ExtensionEvent;

        #region Fields

        private static IBeEngine _beEngine;
        private static IBeLogger _logger;

        #endregion 

        #region Properties

        /// <summary>
        /// Gets or sets the logger.
        /// </summary>
        /// <value>The logger.</value>
        public static IBeLogger Logger
        {
            get
            {
                if (_logger == null)
                    _logger = new DebugLogger();
                return _logger;
            }
            set { _logger = value; }
        }

        /// <summary>
        /// Gets or sets the be engine.
        /// </summary>
        /// <value>The be engine.</value>
        public static IBeEngine BeEngine
        {
            get { return _beEngine; }
            set { _beEngine = value; }
        }

        #endregion 

        #region Constructor 
        /// <summary>
        /// Initializes a new instance of the <see cref="BeExtensionBase"/> class.
        /// </summary>
        public BeExtensionBase()
        {
            Post.Saving += Post_Saving;
            Post.Serving += Post_Serving;
            Post.Saved += Post_Saved;
        }
        #endregion 

        /// <summary>
        /// Handles the Serving event of the Post control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="BlogEngine.Core.ServingEventArgs"/> instance containing the event data.</param>
        public void Post_Serving(object sender, ServingEventArgs e)
        {
            Logger.Log("{0}: Handling Post_Serving()", GetType().Name);

            var idLink = sender.Cast<Post>().GetIdLink();
            var args = new BeEventArgs
                           {
                               Body = e.Body,
                               Content = e.Body,
                               Id = idLink.Item1,
                               PermaLink = idLink.Item2,
                               Cancel = e.Cancel,
                               ContentBy = e.ContentBy,
                               Location = e.Location,
                               ProcessType = BeProcessType.Serving
                           };

            _beEngine.ProcessContent(sender, args);
            
            // Update e.Body so any updates will be transferred
            e.Body = args.Body;

            // Raise event (for unit test)
            OnExtensionEvent(args);
        }

        /// <summary>
        /// Handles the Saving event of the Post control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="BlogEngine.Core.SavedEventArgs"/> instance containing the event data.</param>
        public void Post_Saving(object sender, SavedEventArgs e)
        {
            Logger.Log("{0}: Handling Post_Saving()", GetType().Name);

            var idLink = sender.Cast<Post>().GetIdLink();
            var args = new BeEventArgs {
                        Content = sender.Cast<Post>().Content,
                        Id = idLink.Item1,
                        PermaLink = idLink.Item2,
                        SaveAction = e.Action,
                        ProcessType = BeProcessType.Saving
                    };
            _beEngine.ProcessContent(sender, args);

            OnExtensionEvent(args);
        }

        /// <summary>
        /// Handles the Saved event of the Post control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="BlogEngine.Core.SavedEventArgs"/> instance containing the event data.</param>
        void Post_Saved(object sender, SavedEventArgs e)
        {
            Logger.Log("{0}: Handling Post_Saved()", GetType().Name);

            var idLink = sender.Cast<Post>().GetIdLink();
            var args = new BeEventArgs {
                    Content = sender.Cast<Post>().Content,
                    Id = idLink.Item1,
                    PermaLink = idLink.Item2,
                    SaveAction = e.Action,
                    ProcessType = BeProcessType.Saved
                };

            _beEngine.ProcessContent(sender, args);

            OnExtensionEvent(args);
        }



        private void OnExtensionEvent(BeEventArgs e)
        {
            Logger.Log("{0}: Handling OnExtensionEvent()", GetType().Name);

            var handler = ExtensionEvent;
            if (handler != null) handler(this, e);
        }


    }
}
