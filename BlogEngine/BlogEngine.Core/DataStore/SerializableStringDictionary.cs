namespace BlogEngine.Core.DataStore
{
    using System;
    using System.Collections;
    using System.Collections.Specialized;
    using System.Xml;
    using System.Xml.Schema;
    using System.Xml.Serialization;

    /// <summary>
    /// Serializable String Dictionary
    /// </summary>
    [CLSCompliant(true)]
    [Serializable]
    public class SerializableStringDictionary : StringDictionary, IXmlSerializable
    {
        #region Implemented Interfaces

        #region IXmlSerializable

        /// <summary>
        /// This method is reserved and should not be used. When implementing the IXmlSerializable interface, you should return null (Nothing in Visual Basic) from this method, and instead, if specifying a custom schema is required, apply the <see cref="T:System.Xml.Serialization.XmlSchemaProviderAttribute"/> to the class.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Xml.Schema.XmlSchema"/> that describes the XML representation of the object that is produced by the <see cref="M:System.Xml.Serialization.IXmlSerializable.WriteXml(System.Xml.XmlWriter)"/> method and consumed by the <see cref="M:System.Xml.Serialization.IXmlSerializable.ReadXml(System.Xml.XmlReader)"/> method.
        /// </returns>
        public XmlSchema GetSchema()
        {
            return null;
        }

        /// <summary>
        /// Generates an object from its XML representation.
        /// </summary>
        /// <param name="reader">The <see cref="T:System.Xml.XmlReader"/> stream from which the object is deserialized.</param>
        public void ReadXml(XmlReader reader)
        {
            this.Clear();
            if (!reader.ReadToDescendant("SerializableStringDictionary"))
            {
                return;
            }

            if (!reader.ReadToDescendant("DictionaryEntry"))
            {
                return;
            }
            
            do
            {
                reader.MoveToAttribute("Key");
                var key = reader.ReadContentAsString();
                reader.MoveToAttribute("Value");
                var value = reader.ReadContentAsString();

                this.Add(key, value);
            }
            while (reader.ReadToNextSibling("DictionaryEntry"));
        }

        /// <summary>
        /// Converts an object into its XML representation.
        /// </summary>
        /// <param name="writer">The <see cref="T:System.Xml.XmlWriter"/> stream to which the object is serialized.</param>
        public void WriteXml(XmlWriter writer)
        {
            writer.WriteStartElement("SerializableStringDictionary");
            foreach (DictionaryEntry entry in this)
            {
                writer.WriteStartElement("DictionaryEntry");
                writer.WriteAttributeString("Key", (string)entry.Key);
                writer.WriteAttributeString("Value", (string)entry.Value);
                writer.WriteEndElement();
            }

            writer.WriteEndElement();
        }

        #endregion

        #endregion
    }
}