using System;
using System.Collections.Generic;
using System.Linq;
using EnsureThat;
using Godot;
using LanguageExt;
using Microsoft.Extensions.DependencyInjection;

namespace AlleyCat.Autowire
{
    public class Binding : Node, IServiceDefinitionProvider
    {
        public IEnumerable<Type> ProvidedTypes => _providedTypes;

        private Seq<Type> _providedTypes = Seq<Type>.Empty;

        public override void _EnterTree()
        {
            base._EnterTree();

            var types = new Lst<Type>();

            var type = GetParent().GetType();
            var nodeType = typeof(Node);

            var ignoredTypes = new System.Collections.Generic.HashSet<Type>
            {
                typeof(IDisposable)
            };

            types = types.AddRange(type.GetInterfaces().Where(t => !ignoredTypes.Contains(t)));

            while (type != null && type != nodeType)
            {
                types += type;

                type = type.BaseType;
            }

            _providedTypes = types.ToSeq();
        }

        public override void _Ready()
        {
            base._Ready();

            var context = GetParent()?.GetParent()?.GetAutowireContext() as AutowireContext;

            this.Autowire(context);
        }

        public void AddServices(IServiceCollection collection)
        {
            Ensure.That(collection, nameof(collection)).IsNotNull();

            var parent = GetParent();

            if (parent == null)
            {
                throw new InvalidOperationException(
                    "Can't add service when a binding is not attached to a parent.");
            }

            ProvidedTypes.Iter(t => collection.AddSingleton(t, parent));
        }
    }
}
