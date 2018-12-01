using AlleyCat.Autowire;

namespace AlleyCat.Common
{
    [NonInjectable]
    public interface IValidatable
    {
        bool Valid { get; }
    }
}
