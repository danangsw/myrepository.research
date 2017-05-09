using System;
using System.Collections.Generic;
using System.Linq;

namespace MyRepository
{
    public static class RepositoryConfigure
    {
        internal static IBuildRepositoryObjects Builder { get; private set; }

        internal static IConfigureRepositoryObjects Configurator { get; private set; }

        static List<RepositoryMetadata> _repositories = new List<RepositoryMetadata>();

        internal static void ValidateTypeAliases()
        {
            var aliases = _repositories.ConvertAll(x => x.TypeAlias.ToLower());

            if (aliases.Count != aliases.Distinct().Count())
            {
                throw new ArgumentException("Duplicate type alias detected");
            }
        }

        public static void SetObjectBuilder(IBuildRepositoryObjects builder)
        {
            Builder = builder;
        }

        public static void SetObjectConfigurator(IConfigureRepositoryObjects configurator)
        {
            Configurator = configurator;            
        }
        
        public static IRepositoryPersistSyntax Persist<TEntityType>()
        {
            var metadata = new RepositoryMetadata { EntityType = typeof(TEntityType) };

            _repositories.Add(metadata);

            return new RepositoryPersistConfigure(metadata);
        }

        internal static IRepositoryScheme FindRepositoryByType(Type entityType)
        {
            var repositoryType = _repositories.Find(x => x.EntityType == entityType);

            if (repositoryType.RepositoryStrategy != null)
            {
                return repositoryType.RepositoryStrategy;
            }

            var strategy = (IRepositoryScheme)Builder.Build(repositoryType.RepositoryStrategyType);

            strategy.TypeStorageAlias = repositoryType.TypeAlias;
            strategy.Immutable = repositoryType.Immutable;

            return strategy;
        }

        public static void Reset()
        {
            _repositories.Clear();
        }
    }
}
