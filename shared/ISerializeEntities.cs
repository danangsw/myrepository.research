using System;
using System.IO;
namespace MyRepository
{
	public interface ISerializeEntities
	{
		void Serialize(Type entityType, object entity, Stream outputStream);
		object Deserialize(Type entityType, Stream inputStream);
	}
}
