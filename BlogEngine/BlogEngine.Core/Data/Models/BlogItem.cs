namespace BlogEngine.Core.Data.Models
{
    /// <summary>
    /// Blog item
    /// </summary>
    public class BlogItem
    {
        /// <summary>
        /// Blog name
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// User name
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// Email
        /// </summary>
        public string Email { get; set; }
        /// <summary>
        /// Password
        /// </summary>
        public string Password { get; set; }

        public string ConfirmPassword { get; set; }
    }
}
