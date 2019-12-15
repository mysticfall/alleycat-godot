using AlleyCat.Autowire;
using EnsureThat;
using Godot;
using Microsoft.Extensions.Logging;

namespace AlleyCat.Game
{
    [NonInjectable]
    public abstract class DelegateNode<T> : GameNode, IDelegateNode<T> where T : Node
    {
        public T Node { get; }

        protected DelegateNode(T node, ILoggerFactory loggerFactory) : base(loggerFactory)
        {
            Ensure.That(node, nameof(node)).IsNotNull();

            Node = node;
        }
    }
}
