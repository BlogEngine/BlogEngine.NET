using BlogEngine.Core.Loggers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;

namespace BlogEngine.Core.Ioc
{
    public interface IIoc
    {
        string Name { get; set; }

        IUnityContainer Container { get; set; }

        IAeLogger Logger { get; set; }
    }
}
