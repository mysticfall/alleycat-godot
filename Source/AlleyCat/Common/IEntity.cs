using AlleyCat.Game;
using EnsureThat;
using Godot;
using LanguageExt;
using static LanguageExt.Prelude;

namespace AlleyCat.Common
{
    public interface IEntity : ILabelled, IValidatable
    {
    }

    public static class EntityExtensions
    {
        public static Option<IEntity> FindEntity(this Node node)
        {
            Ensure.That(node, nameof(node)).IsNotNull();

            return Optional(node as IEntity) |
                   Optional(node.GetParent())
                       .Filter(p => !(p is IScene))
                       .Bind(FindEntity);
        }
    }
}
