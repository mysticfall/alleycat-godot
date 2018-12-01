using AlleyCat.Autowire;

namespace AlleyCat.Common
{
    [NonInjectable]
    public interface IHideable
    {
        bool Visible { get; set; }
    }
}
