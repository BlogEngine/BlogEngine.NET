using BlogEngine.Core;
using Gwn.BlogEngine.Library.Events;
using Gwn.BlogEngine.Library.Extensions;
using Gwn.BlogEngine.Library.Interfaces;
using Gwn.BlogEngine.Library.Logger;
using Gwn.BlogEngine.Library.Types;

namespace Gwn.BlogEngine.Library.Base
{
    public class BeExtensionEngineBase : IBeEngine
    {
        private static IBeProvider _provider;
        private static IBeLogger _logger;

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
        /// Gets or sets the provider.
        /// </summary>
        /// <value>The provider.</value>
        public static IBeProvider Provider
        {
            get { return _provider; }
            set { _provider = value; }
        }


        /// <summary>
        /// Processes the content.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="Gwn.BlogEngine.Library.Events.BeEventArgs"/> instance containing the event data.</param>
        public void ProcessContent(object sender, BeEventArgs e)
        {
            Logger.Log("{0}: BeExtensionEngineBase.ProcessContent()", GetType().Name);

            // Create a commands collection from the content for use 
            // by the applicable process
            e.CommandList = e.Content.GetWikiCommands();

            // Use the ProcessType to determine which process to execute
            switch (e.ProcessType)
            {
                case BeProcessType.Saving:
                    HandleBlogEngineSaving(sender, e);
                    break;

                case BeProcessType.Serving:
                    HandleBlogEngineServing(sender, e);
                    break;

                case BeProcessType.Saved:
                    HandleBlogEngineSaved(sender, e);
                    break;
            }
        }


        /// <summary>
        /// Handles the blog engine saving.
        /// Calls Provider.Insert, Update or Delete from base
        /// as applicable (based on SaveAction value)
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="Gwn.BlogEngine.Library.Events.BeEventArgs"/> instance containing the event data.</param>
        protected virtual void HandleBlogEngineSaving(object sender, BeEventArgs e)
        {
            Logger.Log("{0}: BeExtensionEngineBase.HandleBlogEngineSaving()", GetType().Name);
            switch (e.SaveAction)
            {
                case SaveAction.Insert: _provider.Insert(sender, e); break;
                case SaveAction.Update: _provider.Update(sender, e); break;
                case SaveAction.Delete: _provider.Delete(sender, e); break;
                case SaveAction.None:
                    break;
            }
        }

        /// <summary>
        /// Handles the blog engine serving.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="Gwn.BlogEngine.Library.Events.BeEventArgs"/> instance containing the event data.</param>
        protected virtual void HandleBlogEngineServing(object sender, BeEventArgs e)
        {
            Logger.Log("{0}: BeExtensionEngineBase.HandleBlogEngineServing()", GetType().Name);
        }

        /// <summary>
        /// Handles the blog engine saved.  Calls Provider.Saved(sender,e) from base
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="Gwn.BlogEngine.Library.Events.BeEventArgs"/> instance containing the event data.</param>
        protected virtual void HandleBlogEngineSaved(object sender, BeEventArgs e)
        {
            Logger.Log("{0}: BeExtensionEngineBase.HandleBlogEngineSaved()", GetType().Name);
            _provider.Saved(sender, e);
        }


    }
}
