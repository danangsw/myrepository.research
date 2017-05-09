using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Configuration;

namespace PersistentObjects
{
	public class SqlHelper : IDisposable
	{
		readonly DbConnection _connection;
		readonly DbProviderFactory _factory;
		DbTransaction _transaction;

		public SqlHelper(ConnectionStringSettings connectionString)
		{
			_factory = DbProviderFactories.GetFactory(connectionString.ProviderName);
			_connection = _factory.CreateConnection();
			_connection.ConnectionString = connectionString.ConnectionString;
		}
		
        public void Dispose()
		{
			_connection.Dispose();
		}

		public DbDataReader ExecuteReader(CommandType cmdType, string cmdText, params DbParameter[] commandParameters)
		{
			var dbCommand = _factory.CreateCommand();
			PrepareCommand(dbCommand, _connection, cmdType, cmdText, commandParameters);
			var result = dbCommand.ExecuteReader();
			CommitTransaction();
			return result;
		}

		public void ExecuteNonQuery(CommandType cmdType, string cmdText, params DbParameter[] commandParameters)
		{
			var dbCommand = _factory.CreateCommand();
			PrepareCommand(dbCommand, _connection, cmdType, cmdText, commandParameters);
			dbCommand.ExecuteNonQuery();
			CommitTransaction();
		}

		public object ExecuteScalar(CommandType cmdType, string cmdText, params DbParameter[] commandParameters)
		{
			var dbCommand = _factory.CreateCommand();
			PrepareCommand(dbCommand, _connection, cmdType, cmdText, commandParameters);
			object result = dbCommand.ExecuteScalar();
			CommitTransaction();
			return result;
		}

		void CommitTransaction()
		{
			if (_transaction != null)
			{
				_transaction.Commit();
				_transaction = null;
			}
		}

		void PrepareCommand(DbCommand cmd, DbConnection conn, CommandType cmdType, string cmdText, IEnumerable<DbParameter> cmdParms)
		{
			if (conn.State != ConnectionState.Open)
			{
				conn.Open();
			}

			cmd.Connection = conn;
			cmd.CommandText = cmdText;

			if (_transaction != null)
			{
				cmd.Transaction = _transaction;
			}

			cmd.CommandType = cmdType;
			cmd.Parameters.Clear();

			if (cmdParms != null)
			{
				foreach (var current in cmdParms)
				{
					cmd.Parameters.Add(current);
				}
			}
		}

		public DbParameter CreateParameter(string name, string value)
		{
			var dbParameter = _factory.CreateParameter();

			dbParameter.ParameterName = name;
			dbParameter.DbType = DbType.String;

			if (value == null)
			{
				dbParameter.Value = DBNull.Value;
			}
			else
			{
				dbParameter.Value = value;
			}

			return dbParameter;
		}

		public DbParameter CreateParameter(string name, System.DateTime? value)
		{
			var dbParameter = _factory.CreateParameter();

			dbParameter.ParameterName = name;
			dbParameter.DbType = DbType.DateTime;
			
            if (!value.HasValue)
			{
				dbParameter.Value = DBNull.Value;
			}
			else
			{
				dbParameter.Value = value;
			}

			return dbParameter;
		}

		public DbParameter CreateParameter(string name, byte[] value)
		{
			var dbParameter = _factory.CreateParameter();

			dbParameter.ParameterName = name;
			dbParameter.DbType = DbType.Binary;
			dbParameter.Value = value;

			return dbParameter;
		}

		public DbParameter CreateParameter(string name, System.DateTime value)
		{
			var dbParameter = _factory.CreateParameter();

			dbParameter.ParameterName = name;
			dbParameter.DbType = DbType.DateTime;
			dbParameter.Value = value;

			return dbParameter;
		}

		public DbParameter CreateParameter(string name, decimal value)
		{
			var dbParameter = _factory.CreateParameter();

			dbParameter.ParameterName = name;
			dbParameter.DbType = DbType.Decimal;
			dbParameter.Value = value;

			return dbParameter;
		}

		public DbParameter CreateParameter(string name, bool value)
		{
			var dbParameter = _factory.CreateParameter();

			dbParameter.ParameterName = name;
			dbParameter.DbType = DbType.Boolean;
			dbParameter.Value = value;

			return dbParameter;
		}

		public DbParameter CreateParameter(string name, bool? value)
		{
			var dbParameter = _factory.CreateParameter();

			dbParameter.ParameterName = name;
			dbParameter.DbType = DbType.Boolean;

			if (value.HasValue)
			{
				dbParameter.Value = value.Value;
			}
			else
			{
				dbParameter.Value = DBNull.Value;
			}
			
            return dbParameter;
		}

		public DbParameter CreateParameter(string name, int value)
		{
			var dbParameter = _factory.CreateParameter();

			dbParameter.ParameterName = name;
			dbParameter.DbType = DbType.Int32;
			dbParameter.Value = value;
			
            return dbParameter;
		}

		public DbParameter CreateParameter(string name, int? value)
		{
			var dbParameter = _factory.CreateParameter();

			dbParameter.ParameterName = name;
			dbParameter.DbType = DbType.Int32;
			
            if (value.HasValue)
			{
				dbParameter.Value = value.Value;
			}
			else
			{
				dbParameter.Value = DBNull.Value;
			}
			
            return dbParameter;
		}

		public DbParameter CreateParameter(string name, System.Guid value)
		{
			var dbParameter = _factory.CreateParameter();

			dbParameter.ParameterName = name;
			dbParameter.DbType = DbType.String;
			dbParameter.Value = value;
			
            return dbParameter;
		}

		public FieldType GetField<FieldType>(DbDataReader reader, string name)
		{
			var ordinal = reader.GetOrdinal(name);

			FieldType result;

			if (reader.IsDBNull(ordinal))
			{
				result = default(FieldType);
			}
			else
			{
				result = (FieldType)((object)reader.GetValue(ordinal));
			}

			return result;
		}
	}
}
