namespace MyRepository
{
    public class RepositoryBuilder : IRepositoryBuilder
    {
        readonly RepositoryMetadata _metadata;

        public RepositoryBuilder(RepositoryMetadata metadata)
        {
            _metadata = metadata;
        }

        public IRepositoryBuilder Immutable()
        {
            _metadata.Immutable = true;

            return this;
        }
    }
}
