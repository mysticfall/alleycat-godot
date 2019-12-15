using AlleyCat.Autowire;
using LanguageExt;

namespace AlleyCat.Common
{
    [NonInjectable]
    public interface IServiceFactory
    {
        Validation<string, object> Service { get; }
    }

    namespace Generic
    {
        public interface IServiceFactory<T> : IServiceFactory
        {
            new Validation<string, T> Service { get; }
        }
    }
}
