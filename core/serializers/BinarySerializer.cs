using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace MyRepository.Serializers
{
	[Serializable]
	public class BinarySerializer : ISerializeEntities
	{
		public void Serialize(Type entityType, object entity, Stream outputStream)
		{
			var binaryFormatter = new BinaryFormatter();

			binaryFormatter.Serialize(outputStream, entity);
		}

		public object Deserialize(Type entityType, Stream inputStream)
		{
			var binaryFormatter = new BinaryFormatter();

			return binaryFormatter.Deserialize(inputStream);
		}
	}
}
