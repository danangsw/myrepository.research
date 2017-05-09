using System;
namespace MyRepository
{
    public class RepositoryPersistConfigure : IRepositoryPersistSyntax, ISpecifyRepositorySyntax, IBuildRepositoryMetadata
    {
        RepositoryMetadata _metadata;

        public RepositoryPersistConfigure(RepositoryMetadata metadata)
        {
            _metadata = metadata;
        }

        public ISpecifyRepositorySyntax WithTypeAlias(string typeAlias)
        {
            _metadata.TypeAlias = typeAlias;

            RepositoryConfigure.ValidateTypeAliases();

            return this;
        }

        public IRepositoryPersistSyntax Immutable()
        {
            _metadata.Immutable = true;

            return this;
        }

        public IRepositoryPersistSyntax WithValidToStrategy()
        {
            _metadata.ValidToStrategy = true;

            return this;
        }

        public IRepositoryPersistSyntax WithValidToStrategy(Func<DateTime> dateProvider)
        {
            _metadata.ValidToStrategy = true;
            _metadata.ValidToDateProvider = dateProvider;

            return this;
        }

        public RepositoryMetadata GetMetadata()
        {
            return _metadata;
        }
    }
}
