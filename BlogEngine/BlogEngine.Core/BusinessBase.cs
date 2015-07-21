namespace BlogEngine.Core
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Globalization;
    using System.Security;
    using System.Text;
    using System.Threading;
    using System.Web.Security;

    /// <summary>
    /// This is the base class from which most business objects will be derived. 
    ///     To create a business object, inherit from this class.
    /// </summary>
    /// <typeparam name="T">
    /// The type of the derived class.
    /// </typeparam>
    /// <typeparam name="TKey">
    /// The type of the Id property.
    /// </typeparam>
    [Serializable]
    public abstract class BusinessBase<T, TKey> : IDataErrorInfo, INotifyPropertyChanged, INotifyPropertyChanging, IChangeTracking, IDisposable
        where T : BusinessBase<T, TKey>, new()
    {
        #region Constants and Fields

        /// <summary>
        /// The broken rules.
        /// </summary>
        /// <remarks>
        /// This has been updated from using the old StringDictionary class from .Net 1.1. The StringDictionary class
        /// used case-insensitive keys, so this one needs to have StringComparer.OrdinalIgnoreCase passed in the constructor for
        /// backwards compatibility.
        /// 
        /// INotifyPropertyChanging is implemented in case down the line someone wants to create a provider that uses BusinessBase
        /// objects as Linq-To-SQL entities(DataContext performance skyrockets when this is used). 
        /// 
        /// </remarks>
        private readonly Dictionary<String, String> brokenRules = new Dictionary<String, String>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// The changed properties.
        /// </summary>
        private readonly HashSet<string> changedProperties = new HashSet<string>();

        /// <summary>
        /// The date created.
        /// </summary>
        private DateTime dateCreated = DateTime.MinValue;

        /// <summary>
        /// The date modified.
        /// </summary>
        private DateTime dateModified = DateTime.MinValue;

        /// <summary>
        /// Initializes a new instance of the <see cref="BusinessBase{T,TKey}"/> class. 
        /// </summary>
        protected BusinessBase()
        {
            this.New = true;
            this.IsChanged = true;

            // for the case of Blog type, do not set BlogId because
            // that requires accessing Blog.CurrentInstance which can
            // lead to a stackoverflow when the app is first loading.
            // instead, allow the Blog constructor set the BlogId.
            if (this.GetType() != typeof(Blog))
                this.BlogId = Blog.CurrentInstance.Id;
        }

        #endregion

        #region Events

        /// <summary>
        ///     Occurs when the class is Saved
        /// </summary>
        public static event EventHandler<SavedEventArgs> Saved;

        /// <summary>
        ///     Occurs when the class is Saved
        /// </summary>
        public static event EventHandler<SavedEventArgs> Saving;

        /// <summary>
        ///     Occurs when this instance is marked dirty. 
        ///     It means the instance has been changed but not saved.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Event raised when an instance is about to change one of its property values.
        /// </summary>
        public event PropertyChangingEventHandler PropertyChanging;

        #endregion

        #region Properties

        /// <summary>
        ///     Gets or sets the date on which the instance was created.
        /// </summary>
        public DateTime DateCreated
        {
            get
            {
                return this.dateCreated;
            }

            set
            {
                this.SetValue("DateCreated", value, ref this.dateCreated);
            }
        }

        /// <summary>
        ///     Gets or sets the date on which the instance was modified.
        /// </summary>
        public DateTime DateModified
        {
            get
            {
                return (this.dateModified == DateTime.MinValue ? this.dateModified : this.dateModified.AddHours(BlogSettings.Instance.Timezone));
            }

            set
            {
                this.SetValue("DateModifier", value, ref this.dateModified);
                //  this.dateModified = value;
            }
        }

        /// <summary>
        ///     Gets an error message indicating what is wrong with this object.
        /// </summary>
        /// <returns>An error message indicating what is wrong with this object. The default is an empty string ("").</returns>
        public string Error
        {
            get
            {
                return this.ValidationMessage;
            }
        }

        /// <summary>
        ///     Gets or sets the unique Identification of the object.
        /// </summary>
        public virtual TKey Id { get; set; }

        /// <summary>
        ///     Gets or sets the Blog instance ID this object is under.
        /// </summary>
        public Guid BlogId { get; internal set; }

        /// <summary>
        ///     Gets the Blog instance this object is under.
        /// </summary>
        public Blog Blog
        {
            get { return Blog.GetBlog(BlogId); }
        }

        /// <summary>
        ///     Gets a value indicating whether if this object's data has been changed.
        /// </summary>
        public virtual bool IsChanged { get; private set; }

        /// <summary>
        ///     Gets a value indicating whether if this object is marked for deletion.
        /// </summary>
        public bool Deleted { get; private set; }

        /// <summary>
        ///     Gets a value indicating whether if this is a new object, False if it is a pre-existing object.
        /// </summary>
        public bool New { get; private set; }

        /// <summary>
        ///     Gets a value indicating whether the object is valid or not.
        /// </summary>
        public bool Valid
        {
            get
            {
                this.ValidationRules();
                return this.brokenRules.Count == 0;
            }
        }

        /// ///
        /// <summary>
        ///     Gets if the object has broken business rules, use this property to get access
        ///     to the different validation messages.
        /// </summary>
        public virtual string ValidationMessage
        {
            get
            {
                if (!this.Valid)
                {
                    var sb = new StringBuilder();
                    foreach (string messages in this.brokenRules.Values)
                    {
                        sb.AppendLine(messages);
                    }

                    return sb.ToString();
                }

                return string.Empty;
            }
        }

        /// <summary>
        ///     Gets a collection of the properties that have 
        ///     been marked as being dirty.
        /// </summary>
        protected virtual HashSet<string> ChangedProperties
        {
            get { return this.changedProperties; }
        }

        /// <summary>
        /// Gets whether or not the current user owns this object.
        /// </summary>
        /// <returns></returns>
        public virtual bool CurrentUserOwns
        {
            get { return false; }
        }

        /// <summary>
        /// Gets whether the current user can delete this object.
        /// </summary>
        /// <returns></returns>
        public virtual bool CanUserDelete
        {
            get { return Security.IsAdministrator; }
        }

        /// <summary>
        /// Gets whether the current user can edit this object.
        /// </summary>
        /// <returns></returns>
        public virtual bool CanUserEdit
        {
            get { return Security.IsAdministrator; }
        }

        /// <summary>
        /// Gets a value indicating whether the object has been disposed.
        /// <remarks>
        /// If the objects is disposed, it must not be disposed a second
        /// time. The Disposed property is set the first time the object
        /// is disposed. If the Disposed property is true, then the Dispose()
        /// method will not dispose again. This help not to prolong the object's
        /// life if the Garbage Collector.
        /// </remarks>
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance is disposed; otherwise, <c>false</c>.
        /// </value>
        protected bool Disposed { get; private set; }

        #endregion

        #region Indexers

        /// <summary>
        ///     Gets the <see cref = "System.String" /> with the specified column name.
        /// </summary>
        /// <param name="columnName">The column name.</param>
        public string this[string columnName]
        {
            get
            {
                return this.brokenRules.ContainsKey(columnName) ? this.brokenRules[columnName] : string.Empty;
            }
        }

        #endregion

        #region Operators

        /// <summary>
        /// Checks to see if two business objects are the same.
        /// </summary>
        /// <param name="first">The first.</param>
        /// <param name="second">The second.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator ==(BusinessBase<T, TKey> first, BusinessBase<T, TKey> second)
        {
            return ReferenceEquals(first, second) ||
                   ((object)first != null && (object)second != null && first.GetHashCode() == second.GetHashCode());
        }

        /// <summary>
        /// Checks to see if two business objects are different.
        /// </summary>
        /// <param name="first">The first.</param>
        /// <param name="second">The second.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator !=(BusinessBase<T, TKey> first, BusinessBase<T, TKey> second)
        {
            return !(first == second);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Loads an instance of the object based on the Id.
        /// </summary>
        /// <param name="id">The unique identifier of the object</param>
        /// <returns>The instance of the object based on the Id.</returns>
        public static T Load(TKey id)
        {
            var instance = new T();
            instance = instance.DataSelect(id);
            instance.Id = id;

            instance.MarkOld();
            return instance;
        }

        /// <summary>
        /// Marks the object for deletion. It will then be 
        ///     deleted when the object's Save() method is called.
        /// </summary>
        public virtual void Delete()
        {
            this.Deleted = true;
            this.IsChanged = true;
        }

        /// <summary>
        /// Comapares this object with another
        /// </summary>
        /// <param name="obj">
        /// The object to compare
        /// </param>
        /// <returns>
        /// True if the two objects as equal
        /// </returns>
        public override bool Equals(object obj)
        {
            return obj != null && (obj.GetType() == this.GetType() && obj.GetHashCode() == this.GetHashCode());
        }

        /// <summary>
        /// A uniquely key to identify this particullar instance of the class
        /// </summary>
        /// <returns>
        /// A unique integer value
        /// </returns>
        public override int GetHashCode()
        {
            return this.Id.GetHashCode();
        }

        /// <summary>
        /// Marks the object as being an clean, 
        ///     which means not dirty.
        /// </summary>
        public virtual void MarkOld()
        {
            this.IsChanged = false;
            this.New = false;
            this.changedProperties.Clear();
        }

        /// <summary>
        /// Saves the object to the data store (inserts, updates or deletes).
        /// </summary>
        /// <returns>The SaveAction.</returns>
        public virtual SaveAction Save()
        {
            if (this.Deleted && !this.CanUserDelete)
            {
                throw new SecurityException("You are not authorized to delete the object");
            }
            
            if (!this.Valid && !this.Deleted)
            {
                throw new InvalidOperationException(this.ValidationMessage);
            }

            if (this.Disposed && !this.Deleted)
            {
                throw new InvalidOperationException(
                    string.Format(CultureInfo.InvariantCulture, "You cannot save a disposed {0}", this.GetType().Name));
            }

            return this.IsChanged ? this.Update() : SaveAction.None;
        }

        #endregion

        #region Implemented Interfaces

        #region IChangeTracking

        /// <summary>
        /// Resets the object's state to unchanged by accepting the modifications.
        /// </summary>
        public void AcceptChanges()
        {
            this.Save();
        }

        #endregion

        #region IDisposable

        /// <summary>
        /// Disposes the object and frees ressources for the Garbage Collector.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion

        #region "INotifyPropertyChanged"

        /// <summary>
        /// Marks an object as being dirty, or changed.
        /// </summary>
        /// <param name="propertyName">
        /// The name of the property to mark dirty.
        /// </param>
        protected virtual void MarkChanged(string propertyName)
        {
            this.IsChanged = true;

            // No need to check for duplicates since changedProperties 
            // is just a HashSet.
            this.changedProperties.Add(propertyName);
            this.OnPropertyChanged(propertyName);
        }

        /// <summary>
        /// Raises the PropertyChanged event safely.
        /// </summary>
        /// <param name="propertyName">
        /// The property Name.
        /// </param>
        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion

        #region "INotifyPropertyChanging"

        /// <summary>
        /// Method called just before a property's value will change.
        /// </summary>
        /// <param name="propertyName">The name of the property whose value changing.</param>
        /// <remarks>
        /// 
        /// This method should only be called when a value is definitely going to be changed. This
        /// should occur after any value validation or other methods are called.
        /// 
        /// </remarks>
        protected virtual void OnPropertyChanging(string propertyName)
        {
            if (PropertyChanging != null)
            {
                PropertyChanging(this, new PropertyChangingEventArgs(propertyName));
            }
        }

        #endregion

        #endregion

        #region Methods

        /// <summary>
        /// Raises the Saved event.
        /// </summary>
        /// <param name="businessObject">
        /// The business Object.
        /// </param>
        /// <param name="action">
        /// The action.
        /// </param>
        protected static void OnSaved(BusinessBase<T, TKey> businessObject, SaveAction action)
        {
            if (Saved != null)
            {
                Saved(businessObject, new SavedEventArgs(action));
            }
        }

        /// <summary>
        /// Raises the Saving event
        /// </summary>
        /// <param name="businessObject">
        /// The business Object.
        /// </param>
        /// <param name="action">
        /// The action.
        /// </param>
        protected static void OnSaving(BusinessBase<T, TKey> businessObject, SaveAction action)
        {
            if (Saving != null)
            {
                Saving(businessObject, new SavedEventArgs(action));
            }
        }

        /// <summary>
        /// Add or remove a broken rule.
        /// </summary>
        /// <param name="propertyName">
        /// The name of the property.
        /// </param>
        /// <param name="errorMessage">
        /// The description of the error
        /// </param>
        /// <param name="isbroken">
        /// True if the validation rule is broken.
        /// </param>
        protected virtual void AddRule(string propertyName, string errorMessage, bool isbroken)
        {
            if (isbroken)
            {
                this.brokenRules[propertyName] = errorMessage;
            }
            else
            {
                this.brokenRules.Remove(propertyName);
            }
        }

        /// <summary>
        /// Deletes the object from the data store.
        /// </summary>
        protected abstract void DataDelete();

        /// <summary>
        /// Inserts a new object to the data store.
        /// </summary>
        protected abstract void DataInsert();

        /// <summary>
        /// Retrieves the object from the data store and populates it.
        /// </summary>
        /// <param name="id">
        /// The unique identifier of the object.
        /// </param>
        /// <returns>
        /// True if the object exists and is being populated successfully
        /// </returns>
        protected abstract T DataSelect(TKey id);

        /// <summary>
        /// Updates the object in its data store.
        /// </summary>
        protected abstract void DataUpdate();

        /// <summary>
        /// Disposes the object and frees ressources for the Garbage Collector.
        /// </summary>
        /// <param name="disposing">
        /// If true, the object gets disposed.
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
            try
            {
                if (this.Disposed)
                {
                    return;
                }

                if (!disposing)
                {
                    return;
                }

                this.changedProperties.Clear();
                this.brokenRules.Clear();
            }
            finally
            {
                this.Disposed = true;
            }
        }



        /// <summary>
        /// Use this method to change values of properties that participate in INotifyPropertyChanged event notification.
        /// </summary>
        /// <typeparam name="TValueType">The object type for both the new value and old value.</typeparam>
        /// <param name="propertyName">The name of the property that should be used when raising the PropertyChanged event.</param>
        /// <param name="newValue">The new value to be set on the property if it's different from oldValue</param>
        /// <param name="oldValue">The current value of the property.</param>
        /// <returns>True if the the property value has been changed, false otherwise.</returns>
        /// <remarks>
        /// 
        /// This is left as virtual so users can override this if they have their own validation needs.
        /// 
        /// </remarks>
        protected virtual bool SetValue<TValueType>(string propertyName, TValueType newValue, ref TValueType oldValue)
        {
            bool isChanged = (!Object.Equals(newValue, oldValue));

            if (isChanged)
            {
                OnPropertyChanging(propertyName);
                oldValue = newValue;
                MarkChanged(propertyName);
            }
            return isChanged;

        }

        /// <summary>
        /// Reinforces the business rules by adding additional rules to the 
        ///     broken rules collection.
        /// </summary>
        protected abstract void ValidationRules();

        /// <summary>
        /// Is called by the save method when the object is old and dirty.
        /// </summary>
        /// <returns>
        /// The update.
        /// </returns>
        private SaveAction Update()
        {
            var action = SaveAction.None;

            if (this.Deleted)
            {
                if (!this.New)
                {
                    action = SaveAction.Delete;
                    OnSaving(this, action);
                    this.DataDelete();
                }
            }
            else
            {
                if (this.New)
                {
                    if (this.dateCreated == DateTime.MinValue)
                    {
                        this.dateCreated = DateTime.Now;
                    }

                    this.dateModified = DateTime.Now;
                    action = SaveAction.Insert;
                    OnSaving(this, action);
                    this.DataInsert();
                }
                else
                {
                    this.dateModified = DateTime.Now;

                    action = SaveAction.Update;
                    OnSaving(this, action);
                    this.DataUpdate();
                }

                this.MarkOld();
            }

            OnSaved(this, action);
            return action;
        }

        #endregion
    }
}