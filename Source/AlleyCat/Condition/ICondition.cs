namespace AlleyCat.Condition
{
    public interface ICondition
    {
        bool Matches(object context);
    }

    namespace Generic
    {
        public interface ICondition<in T> : ICondition
        {
            bool Matches(T context);
        }
    }
}
