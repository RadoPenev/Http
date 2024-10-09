using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Http.Common
{
    public interface IServiceCollection
    {
        IServiceCollection Add<TService,TImplementation>() 
            where TService:class
            where TImplementation:TService;

        IServiceCollection Add<TService>()
            where TService : class;

        TService GetService<TService>() where TService : class;

        object CreateInstance(Type serviceType);
    }
}
