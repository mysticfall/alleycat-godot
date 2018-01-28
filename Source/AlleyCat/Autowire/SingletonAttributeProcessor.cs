using System;
using System.Collections.Generic;
using System.Diagnostics;
using EnsureThat;
using Godot;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;

namespace AlleyCat.Autowire
{
    public class SingletonAttributeProcessor : InjectableAttributeProcessor<SingletonAttribute>, IDependencyProvider
    {
        public ISet<Type> Provides => new HashSet<Type>(Attribute.Types);

        public SingletonAttributeProcessor([NotNull] SingletonAttribute attribute) : base(attribute)
        {
        }

        public override void Process(IAutowireContext context, Node node)
        {
            Ensure.Any.IsNotNull(context, nameof(context));
            Ensure.Any.IsNotNull(node, nameof(node));

            var target = context.Node == node ? context.Parent : context;

            Debug.Assert(target != null, "context.Parent != null");

            foreach (var type in Attribute.Types)
            {
                target.AddService(c => c.AddSingleton(type, node));
            }
        }
    }
}
