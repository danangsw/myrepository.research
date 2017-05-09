using System;
using System.Configuration;

namespace PersistentObjects
{
	public interface ISpecifySQLiteDatabaseForEntity<TEntityType> : ISpecifySQLiteDatabaseForEntity
	{		
	}

    public interface ISpecifySQLiteDatabaseForEntity 
    {
        ConnectionStringSettings GetAdoConnectionStringForEntity(string entityId);
    }
}
