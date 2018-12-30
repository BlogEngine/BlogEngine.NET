using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace Gwn.BlogEngine.Library.Extensions
{
    /// <summary>
    /// Object extension 
    /// </summary>
    public static class ObjectExtension
    {
        /// <summary>
        /// Casts the specified sender.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sender">The sender.</param>
        /// <returns></returns>
        public static T Cast<T>(this object sender)
        {
            if (sender is T) return (T)sender;
            return default(T);
        }

        /// <summary>
        /// Converts object to lower string.
        /// </summary>
        /// <param name="objectToConvert">The object to convert.</param>
        /// <returns></returns>
        public static string ToLowerString(this object objectToConvert)
        {
            return objectToConvert.ToString().ToLower();
        }

        /// <summary>
        /// Converts object to lower string.
        /// </summary>
        /// <param name="objectToConvert">The object to convert.</param>
        /// <param name="compareString">The compare string.</param>
        /// <returns></returns>
        public static bool SameValueAs(this object objectToConvert, string compareString)
        {
            return objectToConvert.ToString().ToLower()==compareString.ToLower();
        }

        /// <summary>
        /// Serializes the object.
        /// </summary>
        /// <param name="objToSerialize">The obj to serialize.</param>
        /// <returns></returns>
        public static string SerializeObject(this object objToSerialize)
        {
            if (objToSerialize == null)
                return null;
            string xml = null;
            var memoryStream = new MemoryStream();
            var serializer = new XmlSerializer(objToSerialize.GetType());
            var streamWriter = new StreamWriter(memoryStream, Encoding.UTF8);
            serializer.Serialize(streamWriter, objToSerialize);
            memoryStream = (MemoryStream)streamWriter.BaseStream;
            xml = memoryStream.ToArray().Utf8ByteArrayToString();
            return xml;
        }

        /// <summary>
        /// Deserializes the object.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="xml">The XML.</param>
        /// <returns></returns>
        public static T DeserializeObject<T>(this string xml)
        {
            var serializer = new XmlSerializer(typeof(T));
            var memoryStream = new MemoryStream(xml.StringToUtf8ByteArray());
            return (T)serializer.Deserialize(memoryStream);
        }

    }
}
