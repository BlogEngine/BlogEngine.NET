namespace BlogEngine.Core.Providers
{
    using System;
    using System.Collections.Specialized;
    using System.IO;

    /// <summary>
    /// A storage provider for BlogEngine that uses XML files.
    ///     <remarks>
    /// To build another provider, you can just copy and modify
    ///         this one. Then add it to the web.config's BlogEngine section.
    ///     </remarks>
    /// </summary>
    public partial class XmlBlogProvider : BlogProvider
    {
        #region Public Methods

        /// <summary>
        /// Loads the stop words used in the search feature.
        /// </summary>
        /// <returns>
        /// A StringCollection.
        /// </returns>
        public override StringCollection LoadStopWords()
        {
            var fileName = string.Format("{0}stopwords.txt", this.Folder);
            if (!File.Exists(fileName))
            {
                return new StringCollection();
            }

            using (var reader = new StreamReader(fileName))
            {
                var file = reader.ReadToEnd();
                var words = file.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

                var col = new StringCollection();
                col.AddRange(words);

                return col;
            }
        }

        #endregion
    }
}