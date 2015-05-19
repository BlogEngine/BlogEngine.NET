namespace BlogEngine.Core.Providers
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml;

    /// <summary>
    /// The xml blog provider.
    /// </summary>
    public partial class XmlBlogProvider : BlogProvider
    {
        #region Public Methods

        /// <summary>
        /// Fills an unsorted list of Referrers.
        /// </summary>
        /// <returns>
        /// A List&lt;Referrer&gt; of all Referrers.
        /// </returns>
        public override List<Referrer> FillReferrers()
        {
            var folder = Path.Combine(this.Folder, "log");

            var referrers = new List<Referrer>();
            var oldFileDate = DateTime.Today.AddDays(-BlogSettings.Instance.NumberOfReferrerDays);

            var dirInfo = new DirectoryInfo(folder);
            if (dirInfo.Exists)
            {
                var logFiles = new List<FileInfo>(dirInfo.GetFiles());
                foreach (var file in logFiles)
                {
                    var fileName = file.Name.Replace(".xml", string.Empty);
                    var dateStrings = fileName.Split(new[] { '.' });
                    if (dateStrings.Length != 3)
                    {
                        file.Delete();
                        continue;
                    }

                    var day = new DateTime(
                        int.Parse(dateStrings[0]), int.Parse(dateStrings[1]), int.Parse(dateStrings[2]));
                    if (day < oldFileDate)
                    {
                        file.Delete();
                        continue;
                    }

                    referrers.AddRange(GetReferrersFromFile(file, day));
                }
            }

            return referrers;
        }

        /// <summary>
        /// Inserts a Referrer.
        /// </summary>
        /// <param name="referrer">
        /// Must be a valid Referrer object.
        /// </param>
        public override void InsertReferrer(Referrer referrer)
        {
            Referrer.Referrers.Add(referrer);

            referrer.MarkOld();
            var day = Referrer.Referrers.FindAll(r => r.Day.ToShortDateString() == referrer.Day.ToShortDateString());
            this.WriteReferrerFile(day, referrer.Day);
        }

        /// <summary>
        /// Gets a Referrer based on the Id.
        /// </summary>
        /// <param name="Id">
        /// The Referrer's Id.
        /// </param>
        /// <returns>
        /// A matching Referrer.
        /// </returns>
        public override Referrer SelectReferrer(Guid Id)
        {
            var refer = Referrer.Referrers.Find(r => r.Id.Equals(Id)) ?? new Referrer();

            refer.MarkOld();
            return refer;
        }

        /// <summary>
        /// Updates a Referrer.
        /// </summary>
        /// <param name="referrer">
        /// Must be a valid Referrer object.
        /// </param>
        public override void UpdateReferrer(Referrer referrer)
        {
            var day = Referrer.Referrers.FindAll(r => r.Day.ToShortDateString() == referrer.Day.ToShortDateString());
            this.WriteReferrerFile(day, referrer.Day);
        }

        #endregion

        #region Methods

        /// <summary>
        /// The get referrers from file.
        /// </summary>
        /// <param name="file">
        /// The file.
        /// </param>
        /// <param name="day">
        /// The day.
        /// </param>
        /// <returns>
        /// A list of Referrer.
        /// </returns>
        private static IEnumerable<Referrer> GetReferrersFromFile(FileInfo file, DateTime day)
        {
            var referrers = new List<Referrer>();

            var doc = new XmlDocument();
            doc.Load(file.FullName);

            var nodes = doc.SelectNodes("referrers/referrer");
            if (nodes != null)
            {
                foreach (var refer in
                    nodes.Cast<XmlNode>().Select(
                        node =>
                        new Referrer
                            {
                                Url = node.Attributes["url"] == null ? null : new Uri(node.Attributes["url"].InnerText),
                                Count =
                                    node.Attributes["count"] == null ? 0 : int.Parse(node.Attributes["count"].InnerText),
                                Day = day,
                                PossibleSpam =
                                    node.Attributes["isSpam"] == null
                                        ? false
                                        : bool.Parse(node.Attributes["isSpam"].InnerText),
                                ReferrerUrl = new Uri(node.InnerText),
                                Id = Guid.NewGuid()
                            }))
                {
                    refer.MarkOld();
                    referrers.Add(refer);
                }
            }

            return referrers;
        }

        /// <summary>
        /// The write referrer file.
        /// </summary>
        /// <param name="referrers">
        /// The referrers.
        /// </param>
        /// <param name="day">
        /// The day.
        /// </param>
        private void WriteReferrerFile(List<Referrer> referrers, DateTime day)
        {
            var folder = Path.Combine(this.Folder, "log");
            var fileName = Path.Combine(folder, string.Format("{0}.xml", day.ToString("yyyy.MM.dd")));
            var dirInfo = new DirectoryInfo(folder);
            if (!dirInfo.Exists)
            {
                dirInfo.Create();
            }

            using (var writer = new XmlTextWriter(fileName, Encoding.UTF8))
            {
                writer.Formatting = Formatting.Indented;
                writer.Indentation = 4;
                writer.WriteStartDocument(true);
                writer.WriteStartElement("referrers");

                foreach (var refer in referrers)
                {
                    writer.WriteStartElement("referrer");
                    writer.WriteAttributeString("url", refer.Url.ToString());
                    writer.WriteAttributeString("count", refer.Count.ToString());
                    writer.WriteAttributeString("isSpam", refer.PossibleSpam.ToString());
                    writer.WriteString(refer.ReferrerUrl.ToString());
                    writer.WriteEndElement();
                }

                writer.WriteEndElement();
            }
        }

        #endregion
    }
}