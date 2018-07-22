using AlleyCat.Event;
using EnsureThat;
using Godot;
using JetBrains.Annotations;

namespace AlleyCat.UI
{
    public interface ITreeEvent : IEvent<Tree>
    {
    }

    public struct TreeItemSelectedEvent : ITreeEvent
    {
        public Tree Source { get; }

        public TreeItemSelectedEvent([NotNull] Tree source)
        {
            Ensure.Any.IsNotNull(source, nameof(source));

            Source = source;
        }
    }
}
