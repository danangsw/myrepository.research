using System;
using System.IO;
using System.Text;

namespace MyRepository
{
    using Serializers;
    using System.Transactions;
    using System.Collections.Generic;

	public abstract class RepositoryScheme : IRepositoryScheme
	{
        List<UpdateSubscription> _subscriptions = new List<UpdateSubscription>();

		protected RepositoryScheme()
		{
		}
        
        public object Read(Type entityType, string entityId)
        {
            if (ValidToStrategy)
            {
                return Get(entityType, entityId, null);
            }

            return Get(entityType, entityId);
        }

        protected void UpdateEntity(Type entityType, string entityId, Action<UpdateEntityArgs> updateAction)
        {
            UpdateEntity(entityType, entityId, updateAction,
                () => Get(entityType, entityId),
                args => Set(entityType, args.Entity, entityId),
                args => Set(entityType, args.Entity, entityId, args.TimeStamp.Value),
                args => Delete(entityType, entityId));
        }

        protected void UpdateEntity(Type entityType, string entityId, Action<UpdateEntityArgs> updateAction, Func<object> get, Action<UpdateEntityArgs> set, Action<UpdateEntityArgs> setTimestamped, Action<UpdateEntityArgs> delete)
        {
            var updateEntityArgs = new UpdateEntityArgs();

            var obj = Get(entityType, entityId);

            if (obj == null)
            {
                var constructor = entityType.GetConstructor(new Type[0]);
                obj = constructor.Invoke(new object[0]);
                updateEntityArgs.EntityCreated = true;
            }
            else if (Immutable)
            {
                throw new ImmutableObjectException(entityId, obj);
            }

            updateEntityArgs.Entity = obj;

            updateAction(updateEntityArgs);

            if (ValidToStrategy)
            {
                if (!updateEntityArgs.TimeStamp.HasValue)
                {
                    updateEntityArgs.TimeStamp = ValidToDateProvider();
                }

                setTimestamped(updateEntityArgs);

                ExecuteActionHandlers(entityId, entityType, updateEntityArgs.Entity);
            }
            else
            {
                if (updateEntityArgs.Entity == null)
                {
                    if (!updateEntityArgs.EntityCreated)
                    {
                        delete(updateEntityArgs);

                        ExecuteActionHandlers(entityId, entityType, updateEntityArgs.Entity);
                    }
                }
                else if (!updateEntityArgs.CancelUpdate)
                {
                    set(updateEntityArgs);

                    ExecuteActionHandlers(entityId, entityType, updateEntityArgs.Entity);
                }
            }
        }

        public object ReadFromHistory(Type entityType, string entityId, DateTime timestamp)
        {
            return Get(entityType, entityId, timestamp);
        }

        public void Remove(Type entityType, string entityId)
        {
            Delete(entityType, entityId);
        }

        public abstract void Update(Type entityType, string entityId, Action<UpdateEntityArgs> updateAction);
        public abstract object Get(Type entityType, string entityId);
        public abstract void Set(Type entityType, object entity, string entityId);
        public abstract object Get(Type entityType, string entityId, DateTime? timestamp);
        public abstract void Set(Type entityType, object entity, string entityId, DateTime timestamp);
        public abstract void Delete(Type entityType, string entityId);
        public abstract List<string> List(Type entityType);

        public object Subscribe(Type entityType, Action<string, object> handler)
        {
            var subscription = new UpdateSubscription(handler);

            _subscriptions.Add(subscription);

            return subscription;
        }

        public void Unsubscribe(Type entityType, object handler)
        {
            _subscriptions.Remove((UpdateSubscription)handler);
        }

        void ExecuteActionHandlers(string entityId, Type entityType, object entity)
        {
            foreach (var subscription in _subscriptions)
            {
                subscription.Handler(entityId, entity);
            }

            // action handlers require configuration of a builder
            if (RepositoryConfigure.Builder != null)
            {
                var iType = typeof(IListenToEntityUpdates<>);
                var handlerType = iType.MakeGenericType(entityType);
                var method = handlerType.GetMethod("Handle");

                var actionHandlers = RepositoryConfigure.Builder.BuildAll(handlerType);

                foreach (var handler in actionHandlers)
                {
                    method.Invoke(handler, new object[] { entityId, entity });
                }
            }
        }

        public string TypeStorageAlias { get; set; }
        public bool Immutable { get; set; }
        public bool ValidToStrategy { get; set; }
        public Func<DateTime> ValidToDateProvider { get; set; }
    }
}
