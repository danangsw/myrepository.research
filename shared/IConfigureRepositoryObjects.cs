using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyRepository
{
    public interface IConfigureRepositoryObjects
    {
        void ConfigureRepositoryScheme<TRepositoryScheme>() where TRepositoryScheme : IRepositoryScheme;
    }
}
