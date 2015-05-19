using System;

namespace BlogEngine.Core.Data.Models
{
    public class CustomFilter
    {
        /// <summary>
        /// Short name
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Long (class) name
        /// </summary>
        public string FullName { get; set; }
        /// <summary>
        /// If filter enabled
        /// </summary>
        public bool Enabled { get; set; }
        /// <summary>
        /// Number of comments checked by filter
        /// </summary>
        public int Checked { get; set; }
        /// <summary>
        /// Spam comments identified
        /// </summary>
        public int Spam { get; set; }
        /// <summary>
        /// Number of mistakes made
        /// </summary>
        public int Mistakes { get; set; }
        /// <summary>
        /// Accuracy
        /// </summary>
        public string Accuracy
        {
            get
            {
                try
                {
                    if (this.Mistakes < 1 || this.Checked < 1)
                        return "100";

                    if (this.Mistakes >= this.Checked)
                        return "0";

                    var c = (double)this.Checked;
                    var m = (double)this.Mistakes;
                    double a = 100 - (100 / c * m);

                    return String.Format("{0:0.00}", a);
                }
                catch (Exception)
                {
                    return "";
                }
            }
        }
    }
}
