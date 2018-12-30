using System;
using System.Text;

namespace Gwn.BlogEngine.Library.Extensions
{
    public static class ByteExtension
    {
        /// <summary>
        /// UTs the f8 byte array to string.
        /// </summary>
        /// <param name="characters">The characters.</param>
        /// <returns></returns>
        public static String Utf8ByteArrayToString(this Byte[] characters)
        {
            var encoding = new UTF8Encoding();
            return encoding.GetString(characters, 0, characters.Length);
        }
        /// <summary>
        /// Strings to UT f8 byte array.
        /// </summary>
        /// <param name="byteString">The p XML string.</param>
        /// <returns></returns>
        public static Byte[] StringToUtf8ByteArray(this String byteString)
        {
            var encoding = new UTF8Encoding();
            var byteArray = encoding.GetBytes(byteString);
            return byteArray;
        }
    }

}
