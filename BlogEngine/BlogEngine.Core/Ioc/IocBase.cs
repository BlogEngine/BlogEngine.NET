using BlogEngine.Core.Loggers;
using Unity;
using Unity.Attributes;

namespace BlogEngine.Core.Ioc
{
    public class IocBase : IIoc
    {
        private string _name;

        public string Name {
            get {
                if (_name == null) return GetType().Name;
                return _name;
            }
            set { _name = value; }
        }

        [Dependency] public IAeLogger Logger { get; set; }

        [Dependency] public IUnityContainer Container { get; set; }


    }

}
