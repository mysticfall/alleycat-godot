using System;
using EnsureThat;
using Godot;

namespace AlleyCat.Autowire
{
    public class AutowireContextProcessor : NodeProcessor
    {
        public override AutowirePhase ProcessPhase => AutowirePhase.Resolve;

        public override void Process(IAutowireContext context, Node node)
        {
            Ensure.Any.IsNotNull(context, nameof(context));

            if (node is IAutowireContext local)
            {
                if (node != context)
                {
                    local.Build();
                }
            }
            else
            {
                throw new ArgumentOutOfRangeException(
                    nameof(node),
                    "Node is not an instance of IAutowireContext.");
            }
        }
    }
}
