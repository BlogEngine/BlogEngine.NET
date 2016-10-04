namespace BlogEngine.Core
{
    using System;
    using System.Collections.Generic;

    using BlogEngine.Core.Providers;

    /// <summary>
    /// Referrers are web sites that users followed to get to your blog.
    /// </summary>
    [Serializable]
    public class Referrer : BusinessBase<Referrer, Guid>, IComparable<Referrer>
    {
        #region Constants and Fields

        /// <summary>
        /// The sync root.
        /// </summary>
        private static readonly object SyncRoot = new object();

        /// <summary>
        /// The referrers.
        /// </summary>
        private static Dictionary<Guid, List<Referrer>> referrers;

        /// <summary>
        /// The referrers by day.
        /// </summary>
        private static Dictionary<DateTime, List<Referrer>> referrersByDay;

        /// <summary>
        /// The count.
        /// </summary>
        private int count;

        /// <summary>
        /// The day of the DateTime.
        /// </summary>
        private DateTime day;

        /// <summary>
        /// The possible spam.
        /// </summary>
        private bool possibleSpam;

        /// <summary>
        /// The referrer.
        /// </summary>
        private Uri referrer;

        /// <summary>
        /// The url Uri.
        /// </summary>
        private Uri url;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref = "Referrer" /> class.
        /// </summary>
        public Referrer()
        {
            this.Id = Guid.NewGuid();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Referrer"/> class.
        /// </summary>
        /// <param name="referrer">
        /// The ReferrerUrl for the Referrer.
        /// </param>
        public Referrer(Uri referrer)
            : this()
        {
            this.ReferrerUrl = referrer;
        }

        static Referrer()
        {
            Blog.Saved += (s, e) =>
            {
                if (e.Action == SaveAction.Delete)
                {
                    Blog blog = s as Blog;
                    if (blog != null)
                    {
                        // remove deleted blog from static 'referrers'

                        if (referrers != null && referrers.ContainsKey(blog.Id))
                            referrers.Remove(blog.Id);
                    }
                }
            };
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Gets all of the Referrers from the data store.
        /// </summary>
        public static List<Referrer> Referrers
        {
            get
            {
                Blog blog = Blog.CurrentInstance;

                if (referrers == null || !referrers.ContainsKey(blog.Id))
                {
                    lock (SyncRoot)
                    {
                        if (referrers == null || !referrers.ContainsKey(blog.Id))
                        {
                            if (referrers == null)
                                referrers = new Dictionary<Guid, List<Referrer>>();

                            referrers[blog.Id] = BlogService.FillReferrers();
                            ParseReferrers();
                        }
                    }
                }

                return referrers[blog.Id];
            }
        }

        /// <summary>
        ///     Gets an automatically maintained Dictionary of Referrers separated by Day.
        /// </summary>
        public static Dictionary<DateTime, List<Referrer>> ReferrersByDay
        {
            get
            {
                if (Referrers == null)
                {
                    ParseReferrers();
                }

                return referrersByDay;
            }
        }

        /// <summary>
        ///     Gets or sets the Count of the object.
        /// </summary>
        public int Count
        {
            get
            {
                return this.count;
            }

            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException("Count must be greater than or equal to 0.");
                }
                else
                {
                    base.SetValue("Count", value, ref this.count);
                }
            }
        }

        /// <summary>
        ///     Gets or sets the Day of the object.
        /// </summary>
        public DateTime Day
        {
            get
            {
                return this.day;
            }

            set
            {
                base.SetValue("Day", value, ref this.day);
            }
        }

        /// <summary>
        ///     Gets or sets a value indicating whether the referrer is possibly spam.
        /// </summary>
        public bool PossibleSpam
        {
            get
            {
                return this.possibleSpam;
            }

            set
            {
                base.SetValue("PossibleSpam", value, ref this.possibleSpam);
            }
        }

        /// <summary>
        ///     Gets or sets the referrer address of the object.
        /// </summary>
        public Uri ReferrerUrl
        {
            get
            {
                return this.referrer;
            }

            set
            {
                base.SetValue("ReferrerUrl", value, ref this.referrer);
            }
        }

        /// <summary>
        ///     Gets or sets the referrer Url of the object.
        /// </summary>
        public Uri Url
        {
            get
            {
                return this.url;
            }

            set
            {
                base.SetValue("Url", value, ref this.url);
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Returns a <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
        /// </returns>
        public override string ToString()
        {
            return this.ReferrerUrl.ToString();
        }

        #endregion

        #region Implemented Interfaces

        #region IComparable<Referrer>

        /// <summary>
        /// Compares the current object with another object of the same type.
        /// </summary>
        /// <param name="other">
        /// An object to compare with this object.
        /// </param>
        /// <returns>
        /// A 32-bit signed integer that indicates the relative order of the objects being compared. 
        ///     The return value has the following meanings: Value Meaning Less than zero This object is 
        ///     less than the other parameter.Zero This object is equal to other. Greater than zero This object is greater than other.
        /// </returns>
        public int CompareTo(Referrer other)
        {
            var compareThis = $"{ReferrerUrl} {Url}";
            var compareOther = $"{other.ReferrerUrl} {other.Url}";
            return compareThis.CompareTo(compareOther);
        }

        #endregion

        #endregion

        #region Methods

        /// <summary>
        /// Deletes the object from the data store.
        /// </summary>
        protected override void DataDelete()
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Inserts a new object to the data store.
        /// </summary>
        protected override void DataInsert()
        {
            OnSaving(this, SaveAction.Insert);
            if (this.New)
            {
                BlogService.InsertReferrer(this);
                AddReferrer(this);
            }

            OnSaved(this, SaveAction.Insert);
        }

        /// <summary>
        /// Retrieves the object from the data store and populates it.
        /// </summary>
        /// <param name="id">
        /// The unique identifier of the object.
        /// </param>
        /// <returns>
        /// The object that was selected from the data store.
        /// </returns>
        protected override Referrer DataSelect(Guid id)
        {
            return BlogService.SelectReferrer(id);
        }

        /// <summary>
        /// Updates the object in its data store.
        /// </summary>
        protected override void DataUpdate()
        {
            OnSaving(this, SaveAction.Update);
            if (this.IsChanged)
            {
                BlogService.UpdateReferrer(this);
            }

            OnSaved(this, SaveAction.Update);
        }

        /// <summary>
        /// Reinforces the business rules by adding additional rules to the
        ///     broken rules collection.
        /// </summary>
        protected override void ValidationRules()
        {
            this.AddRule("Referrer", "Referrer must be set", this.ReferrerUrl == null);
            this.AddRule("Day", "Day must be set", this.Day == DateTime.MinValue);
        }

        /// <summary>
        /// The add referrer.
        /// </summary>
        /// <param name="referrer">
        /// The referrer.
        /// </param>
        private static void AddReferrer(Referrer referrer)
        {
            List<Referrer> day;
            if (ReferrersByDay.ContainsKey(referrer.Day))
            {
                day = ReferrersByDay[referrer.Day];
            }
            else
            {
                day = new List<Referrer>();
                ReferrersByDay.Add(referrer.Day, day);
            }

            if (!day.Contains(referrer))
            {
                day.Add(referrer);
            }
        }

        /// <summary>
        /// The parse referrers.
        /// </summary>
        private static void ParseReferrers()
        {
            referrersByDay = new Dictionary<DateTime, List<Referrer>>();
            foreach (var refer in Referrers)
            {
                if (referrersByDay.ContainsKey(refer.Day))
                {
                    referrersByDay[refer.Day].Add(refer);
                }
                else
                {
                    referrersByDay.Add(refer.Day, new List<Referrer> { refer });
                }
            }
        }

        #endregion
    }
}