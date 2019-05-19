using AlleyCat.Autowire;
using EnsureThat;
using Godot;
using Microsoft.Extensions.Logging;

namespace AlleyCat.Game
{
    [NonInjectable]
    public abstract class DelegateObject<T> : GameObject, IDelegateObject<T> where T : Node
    {
        public T Node { get; }

        protected DelegateObject(T node, ILoggerFactory loggerFactory) : base(loggerFactory)
        {
            Ensure.That(node, nameof(node)).IsNotNull();

            Node = node;
        }
    }
}
