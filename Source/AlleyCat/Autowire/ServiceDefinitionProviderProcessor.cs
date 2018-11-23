using System;
using System.Diagnostics;
using EnsureThat;
using Godot;
using static LanguageExt.Prelude;

namespace AlleyCat.Autowire
{
    public class ServiceDefinitionProviderProcessor : NodeProcessor
    {
        public override AutowirePhase ProcessPhase => AutowirePhase.Register;

        public override void Process(IAutowireContext context, Node node)
        {
            Ensure.That(context, nameof(context)).IsNotNull();

            if (node is IServiceDefinitionProvider provider)
            {
                var target = context.Node == node ? context.Parent : Some(context);

                Debug.Assert(target.IsSome, "target.IsSome");

                target.Iter(t => t.AddService(provider.AddServices));
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
