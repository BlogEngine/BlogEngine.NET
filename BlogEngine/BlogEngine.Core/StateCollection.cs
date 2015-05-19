namespace BlogEngine.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// A generic collection with the ability to 
    ///     check if it has been changed.
    /// </summary>
    /// <typeparam name="T">
    /// The type of the list elements.
    /// </typeparam>
    [Serializable]
    public class StateList<T> : List<T>
    {
        ///// <summary>
        ///// Inserts an element into the collection at the specified index and marks it changed.
        ///// </summary>
        // protected override void InsertItem(int index, T item)
        // {
        // base.InsertItem(index, item);
        // _IsChanged = true;
        // }

        ///// <summary>
        ///// Removes all the items in the collection and marks it changed.
        ///// </summary>
        // protected override void ClearItems()
        // {
        // base.ClearItems();
        // _IsChanged = true;
        // }

        ///// <summary>
        ///// Removes the element at the specified index and marks the collection changed.
        ///// </summary>
        // protected override void RemoveItem(int index)
        // {
        // base.RemoveItem(index);
        // _IsChanged = true;
        // }

        ///// <summary>
        ///// Replaces the element at the specified index and marks the collection changed.
        ///// </summary>
        // protected override void SetItem(int index, T item)
        // {
        // base.SetItem(index, item);
        // _IsChanged = true;
        // }
        #region Constants and Fields

        /// <summary>
        /// Has code int.
        /// </summary>
        private int hasCode;

        #endregion

        #region Properties

        /// <summary>
        ///     Gets a value indicating whether this object's data has been changed.
        /// </summary>
        /// <returns>A value indicating if this object's data has been changed.</returns>
        public virtual bool IsChanged
        {
            get
            {
                return this.GetHashCode() != this.hasCode;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Determines whether the specified <see cref="T:System.Object"></see> is equal to the current <see cref="T:System.Object"></see>.
        /// </summary>
        /// <param name="obj">
        /// The <see cref="T:System.Object"></see> to compare with the current <see cref="T:System.Object"></see>.
        /// </param>
        /// <returns>
        /// true if the specified <see cref="T:System.Object"></see> is equal to the current <see cref="T:System.Object"></see>; otherwise, false.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            if (obj.GetType() == this.GetType())
            {
                return obj.GetHashCode() == this.GetHashCode();
            }

            return false;
        }

        /// <summary>
        /// Serves as a hash function for a particular type. <see cref="M:System.Object.GetHashCode"></see> is suitable for use in hashing algorithms and data structures like a hash table.
        /// </summary>
        /// <returns>
        /// A hash code for the current <see cref="T:System.Object"></see>.
        /// </returns>
        public override int GetHashCode()
        {
            var hash = this.Aggregate(string.Empty, (current, item) => current + item.GetHashCode().ToString());

            return hash.GetHashCode();
        }

        /// <summary>
        /// Marks the object as being clean, 
        ///     which means not changed.
        /// </summary>
        public virtual void MarkOld()
        {
            this.hasCode = this.GetHashCode();
            this.TrimExcess();
        }

        #endregion
    }
}