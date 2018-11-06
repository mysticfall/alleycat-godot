using AlleyCat.Common;
using EnsureThat;
using Godot;
using LanguageExt;
using static LanguageExt.Prelude;

namespace AlleyCat.Game
{
    public interface IEntity : ILabelled, IValidatable
    {
    }

    public static class EntityExtensions
    {
        public static Option<IEntity> FindEntity(this Node node)
        {
            Ensure.That(node, nameof(node)).IsNotNull();

            return node.OfType<IEntity>() | Optional(node.GetParent()).Bind(FindEntity);
        }
    }
}
