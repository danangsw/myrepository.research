using System;
using System.Collections.Generic;

namespace MyRepository
{
    public interface IRepositoryScheme
    {
        bool ValidToStrategy { get; set; }
        bool Immutable { get; set; }
        string TypeStorageAlias { get; set; }
        object Read(Type entityType, string entityId);
        object ReadFromHistory(Type entityType, string entityId, DateTime timestamp);
        void Update(Type entityType, string entityId, Action<UpdateEntityArgs> update);
        object Subscribe(Type entityType, Action<string, object> handler);
        void Unsubscribe(Type entityType, object handler);
        List<string> List(Type entityType);
        void Remove(Type entityType, string entityId);
    }
}
