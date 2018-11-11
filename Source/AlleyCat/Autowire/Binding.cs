using System;
using System.Collections.Generic;
using EnsureThat;
using Godot;
using LanguageExt;
using Microsoft.Extensions.DependencyInjection;

namespace AlleyCat.Autowire
{
    public class Binding : Node, IServiceDefinitionProvider
    {
        public IEnumerable<Type> ProvidedTypes => _providedTypes;

        private IEnumerable<Type> _providedTypes = Seq<Type>.Empty;

        public override void _EnterTree()
        {
            base._EnterTree();

            var type = GetParent().GetType();

            _providedTypes = TypeUtils.FindInjectableTypes(type).Freeze();
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
