using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyRepository
{
    public static class RepositoryExtensions
    {
        public static List<string> List<TEntity>(this IRepository repository)
        {
            return repository.List(typeof(TEntity));
        }

        public static TEntity Read<TEntity>(this IRepository repository, string entityId)
        {
            return (TEntity)((object)repository.Read(typeof(TEntity), entityId));
        }

        public static TEntity ReadFromHistory<TEntity>(this IRepository repository, string entityId, DateTime timestamp)
        {
            return (TEntity)((object)repository.ReadFromHistory(typeof(TEntity), entityId, timestamp));
        }

        public static void Remove<TEntity>(this IRepository repository, string entityId)
        {
            repository.Remove(typeof(TEntity), entityId);
        }

        public static void Update<TEntity>(this IRepository repository, string entityId, Action<UpdateEntityArgs<TEntity>> updateAction)
        {
            Action<UpdateEntityArgs> update = delegate(UpdateEntityArgs args)
            {
                var updateEntityArgs = args.ToGeneric<TEntity>();

                updateAction(updateEntityArgs);

                args.UpdateFromGeneric(updateEntityArgs);
            };

            repository.Update(typeof(TEntity), entityId, new GenericUpdateWrapper<TEntity>(updateAction).Execute);
        }

        public static object Subscribe<TEntity>(this IRepository repository, Action<string, TEntity> handler)
        {
            return repository.Subscribe(typeof(TEntity), (entityId, entity) => handler(entityId, (TEntity)entity));
        }

        public static void Unsubscribe<TEntity>(this IRepository repository, object handler)
        {
            repository.Unsubscribe(typeof(TEntity), handler);
        }

        public class GenericUpdateWrapper<TEntity> : MarshalByRefObject
        {
            Action<UpdateEntityArgs<TEntity>> _callback;

            public GenericUpdateWrapper(Action<UpdateEntityArgs<TEntity>> callback)
            {
                _callback = callback;
            }

            public void Execute(UpdateEntityArgs args)
            {
                var updateEntityArgs = args.ToGeneric<TEntity>();

                _callback(updateEntityArgs);

                args.UpdateFromGeneric(updateEntityArgs);
            }
        }

        public static UpdateEntityArgs<T> ToGeneric<T>(this UpdateEntityArgs args)
        {
            return new UpdateEntityArgs<T>
            {
                EntityCreated = args.EntityCreated,
                Entity = (T)((object)args.Entity),
                TimeStamp = args.TimeStamp
            };
        }

        public static void UpdateFromGeneric<T>(this UpdateEntityArgs args, UpdateEntityArgs<T> genericArgs)
        {
            args.Entity = genericArgs.Entity;
            args.TimeStamp = genericArgs.TimeStamp;
            args.CancelUpdate = genericArgs.CancelUpdate;
        }
    }
}
