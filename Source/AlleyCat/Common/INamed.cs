namespace AlleyCat.Common
{
    public interface INamed : IIdentifiable
    {
        string DisplayName { get; }
    }
}
