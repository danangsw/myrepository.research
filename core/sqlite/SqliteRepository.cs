using log4net;
using System;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.IO;
using System.Reflection;
using System.Transactions;
using System.Text;

namespace PersistentObjects
{
	public class SqliteRepository : RepositoryBase
	{
		static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        protected override object BeginUpdateOperation()
        {
            return null;
        }

        protected override void EndUpdateOperation(object session)
        {
        }

        protected override object Get(Type entityType, string entityId)
        {
            throw new NotImplementedException();
        }

        protected override void Set(Type entityType, object entity, string entityId)
        {
            throw new NotImplementedException();
        }

        protected override void Delete(Type entityType, string entityId)
        {
            throw new NotImplementedException();
        }

		void Set(SqlHelper helper, Type entityType, object entity, string entityId)
		{
			Log.DebugFormat("Performing set of {0} with id {1}", entityType.FullName, entityId);

			string data;

			using (var memoryStream = new MemoryStream())
			{
                Serializer.Serialize(entityType, entity, memoryStream);

                data = Encoding.UTF8.GetString(memoryStream.ToArray());
			}

			if (Exists(helper, entityType, entityId))
			{
				Log.DebugFormat("Overwriting existing record of {0} with id {1}", entityType.FullName, entityId);
				Update(helper, entityType, entityId, data);
			}
			else
			{
				Log.DebugFormat("Inserting new record of {0} with id {1}", entityType.FullName, entityId);
				Insert(helper, entityType, entityId, data);
			}
		}

		void Insert(SqlHelper helper, Type entityType, string entityId, string data)
		{
			var commandParameters = new DbParameter[]
			{
				helper.CreateParameter("@timestamp", System.DateTime.UtcNow),
				helper.CreateParameter("@id", entityId),
				helper.CreateParameter("@view", data),
				helper.CreateParameter("@type", TypeStorageAlias)
			};

			helper.ExecuteNonQuery(CommandType.Text, SqlStatements.Insert, commandParameters);
		}

		void Update(SqlHelper helper, Type entityType, string entityId, string data)
		{
			var commandParameters = new DbParameter[]
			{
				helper.CreateParameter("@id", entityId),
				helper.CreateParameter("@view", data),
				helper.CreateParameter("@type", TypeStorageAlias),
				helper.CreateParameter("@timestamp", DateTime.UtcNow)
			};

			helper.ExecuteNonQuery(CommandType.Text, SqlStatements.Update, commandParameters);
		}

		void Delete(SqlHelper helper, Type entityType, string entityId)
		{
			var commandParameters = new DbParameter[]
			{
				helper.CreateParameter("@id", entityId),
				helper.CreateParameter("@type", TypeStorageAlias)
			};

			helper.ExecuteNonQuery(CommandType.Text, SqlStatements.Delete, commandParameters);
		}

		object Get(SqlHelper helper, Type entityType, string entityId)
		{
			string cmdText = SqlStatements.CreateSchema + SqlStatements.Get;

			var commandParameters = new DbParameter[]
			{
				helper.CreateParameter("@id", entityId),
				helper.CreateParameter("@type", TypeStorageAlias)
			};

			object result;

			using (var dbDataReader = helper.ExecuteReader(CommandType.Text, cmdText, commandParameters))
			{
				if (dbDataReader.Read())
				{
					Log.DebugFormat("Reading {0} with id {1}", entityType.FullName, entityId);
					
                    var field = helper.GetField<string>(dbDataReader, "view");
		
                    using (var inputStream = new MemoryStream(Convert.FromBase64String(field)))
                    {
					    result = Serializer.Deserialize(entityType, inputStream);
                    }
				}
				else
				{
					Log.DebugFormat("No record existed with id {0}", entityId);

					result = null;
				}
            }

			return result;
		}

		bool Exists(SqlHelper helper, Type entityType, string entityId)
		{
			string cmdText = SqlStatements.CreateSchema + SqlStatements.Exists;

			var commandParameters = new DbParameter[]
			{
				helper.CreateParameter("@id", entityId),
				helper.CreateParameter("@type", TypeStorageAlias)
			};

			bool result;

			using (DbDataReader dbDataReader = helper.ExecuteReader(CommandType.Text, cmdText, commandParameters))
			{
				if (dbDataReader.Read())
				{
					result = true;
					return result;
				}
			}

			result = false;

			return result;
		}

		void EnsureDatabaseForEntityType(Type entityType, string entityId)
		{
            var settings = GetDatabaseConnectionStringSettings(entityType, entityId);
			var sqlConnectionStringBuilder = new SqlConnectionStringBuilder(settings.ConnectionString);
			
            string text = Path.GetDirectoryName(sqlConnectionStringBuilder.DataSource);

			if (!string.IsNullOrEmpty(text))
			{
				if (!Path.IsPathRooted(sqlConnectionStringBuilder.DataSource))
				{
					text = Path.Combine(System.Environment.CurrentDirectory, text);
				}

				if (!Directory.Exists(text))
				{
					Directory.CreateDirectory(text);
				}
			}
		}

        ConnectionStringSettings GetDatabaseConnectionStringSettings(Type entityType, string entityId)
        {
            var strategyType = typeof(ISpecifySQLiteDatabaseForEntity<>).MakeGenericType(new Type[] { entityType });
            var strategy = (ISpecifySQLiteDatabaseForEntity)RepositoryObjectBuilder.Build(strategyType);

            ConnectionStringSettings settings;

            if (strategy == null)
            {
                settings = ConnectionString;
            }
            else
            {
                settings = strategy.GetAdoConnectionStringForEntity(entityId);
            }

            return settings;
        }

		SqlHelper GetHelper(Type entityType, string entityId)
		{
			var connectionString = GetDatabaseConnectionStringSettings(entityType, entityId);

			return new SqlHelper(connectionString);
		}
        		
        public ISerializeEntities Serializer
        {
            get;
            set;
        }

        public IBuildRepositoryObjects RepositoryObjectBuilder
        {
            get;
            set;
        }

        public ConnectionStringSettings ConnectionString
        {
            get;
            set;
        }
	}
}
