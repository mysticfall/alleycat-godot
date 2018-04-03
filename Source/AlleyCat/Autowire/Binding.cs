using System;
using System.Collections.Generic;
using System.Linq;
using EnsureThat;
using Godot;
using Microsoft.Extensions.DependencyInjection;

namespace AlleyCat.Autowire
{
    public class Binding : Node, IServiceDefinitionProvider
    {
        public IEnumerable<Type> ProvidedTypes { get; private set; }

        public override void _EnterTree()
        {
            base._EnterTree();

            var types = new List<Type>();

            var type = GetParent().GetType();
            var nodeType = typeof(Node);

            var ignoredTypes = new HashSet<Type>
            {
                typeof(IDisposable)
            };

            types.AddRange(type.GetInterfaces().Where(t => !ignoredTypes.Contains(t)));

            while (type != null && type != nodeType)
            {
                types.Add(type);

                type = type.BaseType;
            }

            ProvidedTypes = types;
        }

        public override void _Ready()
        {
            base._Ready();

            this.Autowire(GetParent()?.GetParent()?.GetAutowireContext());
        }

        public void AddServices(IServiceCollection collection)
        {
            Ensure.Any.IsNotNull(collection, nameof(collection));

            var parent = GetParent();

            if (parent == null) return;

            foreach (var type in ProvidedTypes)
            {
                collection.AddSingleton(type, parent);
            }
        }
    }
}
