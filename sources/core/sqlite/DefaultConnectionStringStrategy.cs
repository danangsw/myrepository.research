using System.Configuration;

namespace PersistentObjects
{
    public class DefaultConnectionStringStrategy : ISpecifySQLiteDatabaseForEntity
    {
        ConnectionStringSettings _settings;

        public DefaultConnectionStringStrategy(string configName)
        {
            _settings = ConfigurationManager.ConnectionStrings[configName];
        }

        public ConnectionStringSettings GetAdoConnectionStringForEntity(string entityId)
        {
            return _settings;
        }
    }
}
