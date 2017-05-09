using System;
using System.Collections.Generic;

namespace MyRepository
{
	public interface IRepository
	{
        List<string> List(Type entityType);
		object Read(Type entityType, string entityId);
        void Update(Type entityType, string entityId, Action<UpdateEntityArgs> update);
        object ReadFromHistory(Type entityType, string entityId, DateTime timestamp);
        object Subscribe(Type entityType, Action<string, object> handler);
        void Unsubscribe(Type entityType, object handler);
        void Remove(Type entityType, string entityId);
	}
}
