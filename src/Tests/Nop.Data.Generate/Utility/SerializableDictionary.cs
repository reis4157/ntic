using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Releasor
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;
    using System.Xml;
    using System.Xml.Schema;
    using System.Xml.Serialization;

    /// <summary>
    /// Represents an XML serializable collection of keys and values.
    /// </summary>
    /// <typeparam name="TKey">The type of the keys in the dictionary.</typeparam>
    /// <typeparam name="TValue">The type of the values in the dictionary.</typeparam>
    [Serializable]
    [XmlRoot("dictionary")]
    public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, IXmlSerializable
    {
        /// <summary>
        /// The default XML tag name for an item.
        /// </summary>
        private const string DefaultItemTag = "item";

        /// <summary>
        /// The default XML tag name for a key.
        /// </summary>
        private const string DefaultKeyTag = "key";

        /// <summary>
        /// The default XML tag name for a value.
        /// </summary>
        private const string DefaultValueTag = "value";

        /// <summary>
        /// The XML serializer for the key type.
        /// </summary>
        private static readonly XmlSerializer keySerializer = new XmlSerializer(typeof(TKey));

        /// <summary>
        /// The XML serializer for the value type.
        /// </summary>
        private static readonly XmlSerializer valueSerializer = new XmlSerializer(typeof(TValue));

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="SerializableDictionary&lt;TKey, TValue&gt;"/> class.
        /// </summary>
        public SerializableDictionary()
        {
        }

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="SerializableDictionary&lt;TKey, TValue&gt;"/> class.
        /// </summary>
        /// <param name="info">A
        /// <see cref="T:System.Runtime.Serialization.SerializationInfo"/> object
        /// containing the information required to serialize the
        /// <see cref="T:System.Collections.Generic.Dictionary`2"/>.
        /// </param>
        /// <param name="context">A
        /// <see cref="T:System.Runtime.Serialization.StreamingContext"/> structure
        /// containing the source and destination of the serialized stream
        /// associated with the
        /// <see cref="T:System.Collections.Generic.Dictionary`2"/>.
        /// </param>
        protected SerializableDictionary(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        /// <summary>
        /// Gets the XML tag name for an item.
        /// </summary>
        protected virtual string ItemTagName
        {
            get
            {
                return DefaultItemTag;
            }
        }

        /// <summary>
        /// Gets the XML tag name for a key.
        /// </summary>
        protected virtual string KeyTagName
        {
            get
            {
                return DefaultKeyTag;
            }
        }

        /// <summary>
        /// Gets the XML tag name for a value.
        /// </summary>
        protected virtual string ValueTagName
        {
            get
            {
                return DefaultValueTag;
            }
        }

        /// <summary>
        /// Gets the XML schema for the XML serialization.
        /// </summary>
        /// <returns>An XML schema for the serialized object.</returns>
        public XmlSchema GetSchema()
        {
            return null;
        }

        /// <summary>
        /// Deserializes the object from XML.
        /// </summary>
        /// <param name="reader">The XML representation of the object.</param>
        public void ReadXml(XmlReader reader)
        {
            var wasEmpty = reader.IsEmptyElement;

            reader.Read();
            if (wasEmpty)
            {
                return;
            }

            try
            {
                while (reader.NodeType != XmlNodeType.EndElement &&
                       reader.NodeType != XmlNodeType.None)
                {
                    this.ReadItem(reader);
                    reader.MoveToContent();
                }
            }
            finally
            {
                if (reader.NodeType != XmlNodeType.None)
                    reader.ReadEndElement();
            }
        }

        /// <summary>
        /// Serializes this instance to XML.
        /// </summary>
        /// <param name="writer">The XML writer to serialize to.</param>
        public void WriteXml(XmlWriter writer)
        {
            foreach (var keyValuePair in this)
            {
                this.WriteItem(writer, keyValuePair);
            }
        }

        public void WriteXml(String fileName)
        {
            XmlWriter writer = null;
            try
            {
                XmlWriterSettings setting = new XmlWriterSettings();
                setting.Encoding = Encoding.UTF8;
                setting.Indent = true;
                //setting.NewLineChars = "\n";
                setting.ConformanceLevel = ConformanceLevel.Fragment;
                setting.OmitXmlDeclaration = true;
                writer = XmlWriter.Create(fileName, setting);
                WriteXml(writer);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (writer != null)
                    writer.Close();
            }
        }

        public void ReadXml(String fileName)
        {
            XmlReader reader = null;
            try
            {
                XmlReaderSettings setting = new XmlReaderSettings();
                setting.IgnoreWhitespace = true;
                setting.IgnoreComments = true;
                setting.ConformanceLevel = ConformanceLevel.Fragment;
                reader = XmlReader.Create(fileName, setting);
                ReadXml(reader); 
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (reader != null)
                    reader.Close();
            }
        }

        /// <summary>
        /// Deserializes the dictionary item.
        /// </summary>
        /// <param name="reader">The XML representation of the object.</param>
        private void ReadItem(XmlReader reader)
        {
            reader.ReadStartElement(this.ItemTagName);
            try
            {
                this.Add(this.ReadKey(reader), this.ReadValue(reader));
            }
            finally
            {
                reader.ReadEndElement();
            }
        }

        /// <summary>
        /// Deserializes the dictionary item's key.
        /// </summary>
        /// <param name="reader">The XML representation of the object.</param>
        /// <returns>The dictionary item's key.</returns>
        private TKey ReadKey(XmlReader reader)
        {
            reader.ReadStartElement(this.KeyTagName);
            try
            {
                return (TKey)keySerializer.Deserialize(reader);
            }
            finally
            {
                reader.ReadEndElement();
            }
        }

        /// <summary>
        /// Deserializes the dictionary item's value.
        /// </summary>
        /// <param name="reader">The XML representation of the object.</param>
        /// <returns>The dictionary item's value.</returns>
        private TValue ReadValue(XmlReader reader)
        {
            reader.ReadStartElement(this.ValueTagName);
            try
            {
                return (TValue)valueSerializer.Deserialize(reader);
            }
            finally
            {
                reader.ReadEndElement();
            }
        }

        /// <summary>
        /// Serializes the dictionary item.
        /// </summary>
        /// <param name="writer">The XML writer to serialize to.</param>
        /// <param name="keyValuePair">The key/value pair.</param>
        private void WriteItem(XmlWriter writer, KeyValuePair<TKey, TValue> keyValuePair)
        {
            writer.WriteStartElement(this.ItemTagName);
            try
            {
                this.WriteKey(writer, keyValuePair.Key);
                this.WriteValue(writer, keyValuePair.Value);
            }
            finally
            {
                writer.WriteEndElement();
            }
        }

        /// <summary>
        /// Serializes the dictionary item's key.
        /// </summary>
        /// <param name="writer">The XML writer to serialize to.</param>
        /// <param name="key">The dictionary item's key.</param>
        private void WriteKey(XmlWriter writer, TKey key)
        {
            writer.WriteStartElement(this.KeyTagName);
            try
            {
                keySerializer.Serialize(writer, key);
            }
            finally
            {
                writer.WriteEndElement();
            }
        }

        /// <summary>
        /// Serializes the dictionary item's value.
        /// </summary>
        /// <param name="writer">The XML writer to serialize to.</param>
        /// <param name="value">The dictionary item's value.</param>
        private void WriteValue(XmlWriter writer, TValue value)
        {
            writer.WriteStartElement(this.ValueTagName);
            try
            {
                valueSerializer.Serialize(writer, value);
            }
            finally
            {
                writer.WriteEndElement();
            }
        }
    }
}
