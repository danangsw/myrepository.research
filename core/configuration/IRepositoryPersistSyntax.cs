using System;
namespace MyRepository
{
    public interface IRepositoryPersistSyntax
    {
        ISpecifyRepositorySyntax WithTypeAlias(string typeAlias);
        IRepositoryPersistSyntax WithValidToStrategy();
        IRepositoryPersistSyntax WithValidToStrategy(Func<DateTime> dateProvider);
        IRepositoryPersistSyntax Immutable();
    }
}
