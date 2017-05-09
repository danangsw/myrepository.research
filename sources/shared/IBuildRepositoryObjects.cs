using System.Collections.Generic;
using System;

namespace MyRepository
{
    public interface IBuildRepositoryObjects
    {
        object[] BuildAll(Type objectType);
        object Build(Type objectType);
    }
}
