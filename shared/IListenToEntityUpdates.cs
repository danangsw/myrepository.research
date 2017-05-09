namespace MyRepository
{
    public interface IListenToEntityUpdates<TEntityType>
    {
        void Handle(string entityId, TEntityType entity);
    }
}
