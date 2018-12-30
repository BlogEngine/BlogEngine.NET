using System;
using System.Data;
using Gwn.BlogEngine.Library.Events;

namespace Gwn.BlogEngine.Library.Interfaces
{
    /// <summary>
    /// BlogEngine Provider interface
    /// </summary>
    public interface IBeProvider
    {

        /// <summary>
        /// Inserts the specified sender.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="Gwn.BlogEngine.Library.Events.BeEventArgs"/> instance containing the event data.</param>
        void Insert(object sender, BeEventArgs e);

        /// <summary>
        /// Updates the specified sender.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="Gwn.BlogEngine.Library.Events.BeEventArgs"/> instance containing the event data.</param>
        void Update(object sender, BeEventArgs e);

        /// <summary>
        /// Deletes the specified sender.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="Gwn.BlogEngine.Library.Events.BeEventArgs"/> instance containing the event data.</param>
        void Delete(object sender, BeEventArgs e);

        /// <summary>
        /// Saveds the specified sender.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="Gwn.BlogEngine.Library.Events.BeEventArgs"/> instance containing the event data.</param>
        void Saved(object sender, BeEventArgs e);

    }
}