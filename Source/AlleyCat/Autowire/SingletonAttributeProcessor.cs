using System;
using System.Diagnostics;
using System.Linq;
using EnsureThat;
using Godot;
using LanguageExt;
using Microsoft.Extensions.DependencyInjection;
using static LanguageExt.Prelude;

namespace AlleyCat.Autowire
{
    public class SingletonAttributeProcessor : InjectableAttributeProcessor<SingletonAttribute>, IDependencyProvider
    {
        public HashSet<Type> Provides { get; }

        public SingletonAttributeProcessor(SingletonAttribute attribute) : base(attribute)
        {
            Provides = toHashSet(Attribute.Types);
        }

        public override void Process(IAutowireContext context, Node node)
        {
            Ensure.That(context, nameof(context)).IsNotNull();
            Ensure.That(node, nameof(node)).IsNotNull();

            var target = context.Node == node ? context.Parent : Some(context);

            Debug.Assert(target.IsSome, "target.IsSome");

            target
                .SelectMany(t => Provides, (t, tpe) => (t, tpe))
                .Iter(v => v.t.AddService(c => c.AddSingleton(v.tpe, node)));
        }
    }
}
