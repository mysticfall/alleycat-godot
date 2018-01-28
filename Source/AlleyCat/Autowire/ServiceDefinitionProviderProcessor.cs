using System;
using System.Diagnostics;
using EnsureThat;
using Godot;

namespace AlleyCat.Autowire
{
    public class ServiceDefinitionProviderProcessor : NodeProcessor
    {
        public override AutowirePhase ProcessPhase => AutowirePhase.Register;

        public override void Process(IAutowireContext context, Node node)
        {
            Ensure.Any.IsNotNull(context, nameof(context));

            if (node is IServiceDefinitionProvider provider)
            {
                var target = context.Node == node ? context.Parent : context;

                Debug.Assert(target != null, "context.Parent != null");

                target.AddService(provider.AddServices);
            }
            else
            {
                throw new ArgumentOutOfRangeException(
                    nameof(node),
                    "Node is not an instance of IServiceDefinitionProvider.");
            }
        }
    }
}
