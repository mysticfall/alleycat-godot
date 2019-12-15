using AlleyCat.Common;
using Godot;
using LanguageExt;
using static LanguageExt.Prelude;

namespace AlleyCat.Game
{
    public static class ResourceExtensions
    {
        public static Validation<string, T> Validate<T>(this Resource resource)
        {
            if (resource is IServiceFactory factory)
            {
                return factory.Service.Bind(s => s is T u
                    ? Success<string, T>(u)
                    : Fail<string, T>($"Factory returned an unknown resource type: '{s?.GetType()}'."));
            }

            return Fail<string, T>($"Unknown resource type: '{resource?.GetType()}'.");
        }
    }
}
