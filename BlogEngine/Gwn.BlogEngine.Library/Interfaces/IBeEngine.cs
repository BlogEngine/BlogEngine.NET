using Gwn.BlogEngine.Library.Events;

namespace Gwn.BlogEngine.Library.Interfaces
{
    /// <summary>
    /// BlogEngine engine interface
    /// </summary>
    public interface IBeEngine
    {
        /// <summary>
        /// Processes the content.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="Gwn.BlogEngine.Library.Events.BeEventArgs"/> instance containing the event data.</param>
        void ProcessContent(object sender, BeEventArgs e);
    }
}
