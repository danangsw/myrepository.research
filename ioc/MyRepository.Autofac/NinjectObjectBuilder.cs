using Ninject;
using MyRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyRepository.Ninject
{
    public class NinjectObjectBuilder : IBuildRepositoryObjects, IConfigureRepositoryObjects
    {
        readonly IKernel _kernel;

        public NinjectObjectBuilder(IKernel kernel)
        {
            _kernel = kernel;
        }

        public object[] BuildAll(Type objectType)
        {
            return _kernel.GetAll(objectType).ToArray();
        }
        
        public object Build(Type objectType)
        {
            return _kernel.Get(objectType);
        }

        public void ConfigureRepositoryScheme<TRepositoryScheme>() where TRepositoryScheme : IRepositoryScheme
        {
            _kernel
                .Bind<TRepositoryScheme>()
                .ToSelf()
                .InSingletonScope();
        }
        
    }
}
