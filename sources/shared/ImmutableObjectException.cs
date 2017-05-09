using System;

namespace MyRepository
{
    public class ImmutableObjectException : Exception
    {
        public ImmutableObjectException(string id, object entity) : 
            base(string.Format("Attempted to write to existing object with id {0}, ToString: {1}", id, entity.ToString()))
        {
        }
    }
}
