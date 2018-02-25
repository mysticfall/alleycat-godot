namespace AlleyCat.Locomotion
{
    public interface ILocomotive
    {
        ILocomotion Locomotion { get; }
    }

    namespace Generic
    {
        public interface ILocomotive<out T> : ILocomotive where T : ILocomotion
        {
            new T Locomotion { get; }
        }
    }
}
