using System;
using System.Collections.Generic;
using System.Transactions;

namespace MyRepository
{
    public class Repository : IRepository
    {
        public List<string> List(Type entityType)
        {
            var repository = RepositoryConfigure.FindRepositoryByType(entityType);

            return repository.List(entityType);
        }

        public object Read(Type entityType, string entityId)
        {
            var repository = RepositoryConfigure.FindRepositoryByType(entityType);

            return repository.Read(entityType, entityId);
        }

        public void Update(Type entityType, string entityId, Action<UpdateEntityArgs> update)
        {
            var repository = RepositoryConfigure.FindRepositoryByType(entityType);

            repository.Update(entityType, entityId, update);
        }

        public object ReadFromHistory(Type entityType, string entityId, DateTime timestamp)
        {
            var repository = RepositoryConfigure.FindRepositoryByType(entityType);

            return repository.ReadFromHistory(entityType, entityId, timestamp);
        }
        
        public object Subscribe(Type entityType, Action<string, object> handler)
        {
            var repository = RepositoryConfigure.FindRepositoryByType(entityType);

            return repository.Subscribe(entityType, handler);
        }

        public void Unsubscribe(Type entityType, object handle)
        {
            var repository = RepositoryConfigure.FindRepositoryByType(entityType);

            repository.Unsubscribe(entityType, handle);
        }

        public void Remove(Type entityType, string entityId)
        {
            var repository = RepositoryConfigure.FindRepositoryByType(entityType);

            repository.Remove(entityType, entityId);
        }
    }
}
