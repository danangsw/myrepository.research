using System;

namespace MyRepository
{
    public class RepositoryMetadata
    {
        public Type EntityType { get; set; }
        public string TypeAlias { get; set; }
        public Type RepositoryStrategyType { get; set; }
        public bool Immutable { get; set; }
        public IRepositoryScheme RepositoryStrategy { get; set; }
        public bool ValidToStrategy { get; set; }
        public Func<DateTime> ValidToDateProvider { get; set; }
    }
}
