namespace AlleyCat.Condition
{
    public interface IRestricted
    {
        bool AllowedFor(object context);
    }

    namespace Generic
    {
        public interface IRestricted<in T> : IRestricted
        {
            bool AllowedFor(T context);
        }
    }
}
