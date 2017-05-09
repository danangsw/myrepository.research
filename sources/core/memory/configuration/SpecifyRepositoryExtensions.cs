namespace MyRepository
{
    public static class SpecifyRepositoryExtensions
    {
        public static IMemoryBuilder MemoryRepository(this ISpecifyRepositorySyntax config, ISerializeEntities entitySerializer)
        {
            var metadata = ((IBuildRepositoryMetadata)config).GetMetadata();

            var repository = new MemoryRepository
            {
                TypeStorageAlias = metadata.TypeAlias,
                ValidToStrategy = metadata.ValidToStrategy,
                ValidToDateProvider = metadata.ValidToDateProvider,
                EntitySerializer = entitySerializer,
                Immutable = metadata.Immutable
            };

            metadata.RepositoryStrategy = repository;

            return new MemoryBuilder(repository);
        }
    }
}
