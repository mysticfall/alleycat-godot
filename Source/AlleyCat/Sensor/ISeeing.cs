namespace AlleyCat.Sensor
{
    public interface ISeeing
    {
        IVision Vision { get; }
    }

    namespace Generic
    {
        public interface ISeeing<out T> : ISeeing where T : IVision
        {
            new T Vision { get; }
        }
    }

    public static class SeeingExtensions
    {
        public static bool IsBlind(this ISeeing subject) => subject.Vision == null || !subject.Vision.Active;
    }
}
