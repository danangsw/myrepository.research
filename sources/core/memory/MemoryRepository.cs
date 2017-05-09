using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.IO;

namespace MyRepository
{
	public class MemoryRepository : RepositoryScheme
	{
        class TimeStampedEntity
        {
            public TimeStampedEntity(byte[] entity, DateTime validTo)
            {
                Entity = entity;
                ValidTo = validTo;
            }

            public byte[] Entity { get; set; }
            public DateTime ValidTo { get; set; }
        }

        class StoredEntity
        {
            List<TimeStampedEntity> _entities = new List<TimeStampedEntity>();

            internal StoredEntity(byte[] entity, DateTime validFrom)
            {
                _entities.Add(new TimeStampedEntity(entity,validFrom));
            }
            
            internal void AddEntity(byte[] entity, DateTime validFrom)
            {  
                var existing = _entities.Find(x => x.ValidTo == validFrom);

                if (existing == null)
                {
                    _entities.Add(new TimeStampedEntity(entity, validFrom));
                }
                else
                {
                    existing.Entity = entity;
                }
            }

            internal byte[] GetCurrent()
            {
                if (_entities.Count == 0)
                {
                    return null;
                }

                return _entities[_entities.Count - 1].Entity;
            }

            internal byte[] GetFor(string entityId, DateTime? timestamp)
            {
                if (!timestamp.HasValue)
                {
                    return GetCurrent();
                }

                var newerVersions = _entities.FindAll(x => x.ValidTo > timestamp.Value);

                if (newerVersions.Count == _entities.Count)
                {
                    return null;
                }

                if(newerVersions.Count == 0)
                {
                    return GetCurrent();
                }

                return _entities[_entities.Count - newerVersions.Count - 1].Entity;
            }
        }

		class EntityCollection : Dictionary<string, StoredEntity> { }
        
		readonly Dictionary<string, EntityCollection> _entityStores = new Dictionary<string, EntityCollection>();

        static readonly ReaderWriterLockSlim AccessLock = new ReaderWriterLockSlim();
        
		public override object Get(Type entityType, string entityId)
		{
            AccessLock.EnterReadLock();

            object result;

			try
			{				
                EnsureEntityStore(entityType);

				var entityCollection = _entityStores[TypeStorageAlias];

				result = entityCollection.ContainsKey(entityId) ?
                    DeserializeEntityFromArray(entityCollection[entityId].GetCurrent(), entityType) 
                    : null;
			}
			finally
			{
				AccessLock.ExitReadLock();
			}

			return result;
		}

		public override void Set(Type entityType, object entity, string entityId)
		{
			AccessLock.EnterWriteLock();

			try
			{
			    EnsureEntityStore(entityType);

				var entityCollection = _entityStores[TypeStorageAlias];

				if (entityCollection.ContainsKey(entityId))
				{
                    if (entity == null)
                    {
                        entityCollection.Remove(entityId);
                    }
                    else
                    {
                        entityCollection[entityId].AddEntity(SerializeEntityToArray(entity), new DateTime());
                    }
				}
				else if(entity != null)
				{
                    entityCollection.Add(entityId, new StoredEntity(SerializeEntityToArray(entity), new DateTime()));
				}
			}
			finally
			{
				AccessLock.ExitWriteLock();
			}
		}

        public override void Delete(Type entityType, string entityId)
        {
            AccessLock.EnterReadLock();

            try
            {
                _entityStores[TypeStorageAlias].Remove(entityId);
            }
            finally
            {
                AccessLock.ExitReadLock();
            }
        }

        public override void Update(Type entityType, string entityId, Action<UpdateEntityArgs> updateAction)
        {
            UpdateEntity(entityType, entityId, updateAction);
        }

		bool Exists(Type entityType, string entityId)
		{
			AccessLock.EnterReadLock();

			bool result;

			try
			{
				EnsureEntityStore(entityType);

				var entityCollection = _entityStores[TypeStorageAlias];

				result = entityCollection.ContainsKey(entityId);
			}
			finally
			{
				AccessLock.ExitReadLock();
			}

			return result;
		}

		void EnsureEntityStore(Type storageType)
		{
			if (!_entityStores.ContainsKey(TypeStorageAlias))
			{
				_entityStores.Add(TypeStorageAlias, new EntityCollection());
			}
		}

        public override object Get(Type entityType, string entityId, DateTime? timeStamp)
        {
            AccessLock.EnterReadLock();

            object result = null;

            try
            {
                EnsureEntityStore(entityType);

                var entityCollection = _entityStores[TypeStorageAlias];

                if(entityCollection.ContainsKey(entityId))
                {
                    var entity = entityCollection[entityId].GetFor(entityId, timeStamp);

                    if (entity != null)
                    {
                        result = DeserializeEntityFromArray(entity, entityType);
                    }
                }
            }
            finally
            {
                AccessLock.ExitReadLock();
            }

            return result;
        }

        public override void Set(Type entityType, object entity, string entityId, DateTime validFrom)
        {
            AccessLock.EnterWriteLock();

            try
            {
                EnsureEntityStore(entityType);

                var entityCollection = _entityStores[TypeStorageAlias];

                if (ValidToStrategy)
                {
                    validFrom = ValidToDateProvider();
                }

                if (entityCollection.ContainsKey(entityId))
                {
                    if (entity == null)
                    {
                        entityCollection.Remove(entityId);
                    }
                    else
                    {
                        entityCollection[entityId].AddEntity(SerializeEntityToArray(entity), validFrom);
                    }
                }
                else if (entity!= null)
                {
                    entityCollection.Add(entityId, new StoredEntity(SerializeEntityToArray(entity), validFrom));
                }
            }
            finally
            {
                AccessLock.ExitWriteLock();
            }
        }

        byte[] SerializeEntityToArray(object entity)
        {
            using (var output = new MemoryStream())
            {
                EntitySerializer.Serialize(entity.GetType(), entity, output);

                return output.ToArray();
            }
        }

        object DeserializeEntityFromArray(byte[] entity, Type entityType)
        {
            using (var input = new MemoryStream(entity))
            {
                return EntitySerializer.Deserialize(entityType, input);
            }
        }

        public ISerializeEntities EntitySerializer
        {
            get;
            set;
        }

        public override List<string> List(Type entityType)
        {
            throw new NotImplementedException();
        }
    }
}
