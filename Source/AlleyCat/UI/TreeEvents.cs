using AlleyCat.Event;
using EnsureThat;
using Godot;

namespace AlleyCat.UI
{
    public interface ITreeEvent : IEvent<Tree>
    {
    }

    public struct TreeItemSelectedEvent : ITreeEvent
    {
        public Tree Source { get; }

        public TreeItemSelectedEvent(Tree source)
        {
            Ensure.That(source, nameof(source)).IsNotNull();

            Source = source;
        }
    }
}
