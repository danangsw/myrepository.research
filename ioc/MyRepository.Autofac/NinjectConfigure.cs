using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ninject;

namespace MyRepository.Ninject
{
    public static class NinjectConfigure
    {
        public static void Configure(IKernel kernel)
        {
            var builder = new NinjectObjectBuilder(kernel);
            RepositoryConfigure.SetObjectBuilder(builder);
            RepositoryConfigure.SetObjectConfigurator(builder);
        }
    }
}
