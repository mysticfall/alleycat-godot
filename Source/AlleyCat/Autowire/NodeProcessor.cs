using Godot;

namespace AlleyCat.Autowire
{
    public abstract class NodeProcessor : INodeProcessor
    {
        public abstract AutowirePhase ProcessPhase { get; }

        public abstract void Process(IAutowireContext context, Node node);

        public int CompareTo(INodeProcessor other) => ProcessPhase.CompareTo(other.ProcessPhase);
    }
}
