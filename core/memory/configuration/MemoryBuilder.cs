namespace MyRepository
{
    public class MemoryBuilder : IMemoryBuilder
    {
        MemoryRepository _repository;

        internal MemoryBuilder(MemoryRepository repository)
        {
            _repository = repository;
        }
    }
}
