using System;
using Godot;

namespace AlleyCat.Autowire
{
    public interface INodeProcessor : IComparable<INodeProcessor>
    {
        AutowirePhase ProcessPhase { get; }

        void Process(IAutowireContext context, Node node);
    }
}
