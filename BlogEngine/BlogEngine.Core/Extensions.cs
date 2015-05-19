namespace BlogEngine.Core
{
    using System;

    /// <summary>
    /// Extension Methods.
    /// </summary>
    public static class Extensions
    {

        /// <summary>
        /// Checks whether a source string contains another string based on the supplied StringComparison.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="partial"></param>
        /// <param name="comparison"></param>
        /// <returns></returns>
        public static bool Contains(this string source, string partial, StringComparison comparison)
        {
            return (source.IndexOf(partial, comparison) >= 0);
        }

        /// <summary>
        /// Tries to parse the string into the specified Guid.
        /// </summary>
        /// <param name="guidString">The GUID string.</param>
        /// <param name="guid">The parsed GUID.</param>
        /// <returns>Whether it worked or not.</returns>
        public static bool TryParse(this string guidString, out Guid guid)
        {
            bool success;
            try
            {
                guid = new Guid(guidString);
                success = true;
            }
            catch
            {
                guid = Guid.Empty;
                success = false;
            }

            return success;
        }

        /// <summary>
        /// Tries to parse the string into the specified Guid.
        /// </summary>
        /// <param name="guidString">The GUID string.</param>
        /// <param name="guid">The parsed GUID.</param>
        /// <returns>Whether it worked or not.</returns>
        public static bool TryParse(this string guidString, out Guid? guid)
        {
            bool success;
            try
            {
                guid = new Guid(guidString);
                success = true;
            }
            catch
            {
                guid = null;
                success = false;
            }

            return success;
        }

        /// <summary>
        /// Tries to convert the string to the specified type.
        /// </summary>
        /// <typeparam name="T">The type to try to convert to.</typeparam>
        /// <param name="theString">The string to parse.</param>
        /// <param name="output">The output.</param>
        /// <returns>Whether it worked or not.</returns>
        /// <remarks>
        /// Abuse of a language feature? Probably.
        /// </remarks>
        public static bool TryParse<T>(this string theString, out T output)
        {
            bool success;
            try
            {
                output = (T)Convert.ChangeType(theString, typeof(T));
                success = true;
            }
            catch (Exception)
            {
                output = default(T);
                success = false;
            }

            return success;
        }
    }
}
