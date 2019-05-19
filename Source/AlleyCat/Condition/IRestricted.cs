using AlleyCat.Autowire;

namespace AlleyCat.Condition
{
    [NonInjectable]
    public interface IRestricted
    {
        bool AllowedFor(object context);
    }

    namespace Generic
    {
        [NonInjectable]
        public interface IRestricted<in T> : IRestricted
        {
            bool AllowedFor(T context);
        }
    }
}
