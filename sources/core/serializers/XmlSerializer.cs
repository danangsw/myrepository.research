using System;
using System.IO;
using System.Runtime.Serialization;
using System.Xml;
using System.Text;
using System.Text.RegularExpressions;

namespace MyRepository.Serializers
{
	[Serializable]
	public class XmlSerializer : ISerializeEntities
	{
        bool _ignoreNamespaces = false;

        public XmlSerializer(bool ignoreNamespaces)
        {
            _ignoreNamespaces = ignoreNamespaces;
        }

        public XmlSerializer()
        {
        }

		public void Serialize(Type entityType, object entity, Stream outputStream)
		{
			var xmlSerializer = new DataContractSerializer(entityType);

			xmlSerializer.WriteObject(outputStream, entity);
		}

		public object Deserialize(Type entityType, Stream inputStream)
		{
            if (_ignoreNamespaces)
            {
                return DeserializeIgnoringNamespace(entityType, inputStream);
            }

			var xmlSerializer = new DataContractSerializer(entityType);

			return xmlSerializer.ReadObject(inputStream);
		}

        public object DeserializeIgnoringNamespace(Type entityType, Stream inputStream)
        {
            string xml;

            using (var reader = new StreamReader(inputStream))
            {
                xml = reader.ReadToEnd();
            }

            var targetNamespace = GetDataContractSerializerNamespace(entityType);
            xml = Regex.Replace(xml, "xmlns=\"[^\"]+\"", "xmlns=\"" + targetNamespace + "\"");

            var ms = new MemoryStream(UTF8Encoding.UTF8.GetBytes(xml));
            var reader2 = XmlDictionaryReader.CreateTextReader(ms, new XmlDictionaryReaderQuotas());

            reader2.Read();

            if (reader2.NodeType != XmlNodeType.Element)
            {
                throw new SerializationException();
            }

            var rootNamespace = reader2.NamespaceURI;
            var rootName = reader2.Name;

            var ser = new DataContractSerializer(entityType, rootName, rootNamespace);

            return ser.ReadObject(reader2, false);
        }

        public string GetDataContractSerializerNamespace(Type subject)
        {
            var obj = subject.GetConstructor(Type.EmptyTypes).Invoke(null);
            
            using (var outputStream = new MemoryStream())
            {
                Serialize(subject, obj, outputStream);

                outputStream.Seek(0, SeekOrigin.Begin);

                using (var reader = XmlReader.Create(outputStream))
                {
                    reader.Read();
                    reader.MoveToAttribute("xmlns");

                    return reader.Value;
                }
            }
        }
	}
}
