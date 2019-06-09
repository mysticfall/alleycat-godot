using System;
using EnsureThat;
using JetBrains.Annotations;

namespace AlleyCat.Autowire
{
    [AttributeUsage(AttributeTargets.Class)]
    [MeansImplicitUse(ImplicitUseKindFlags.Access)]
    public abstract class InjectableAttribute : System.Attribute
    {
        public Type[] Types { get; }

        protected InjectableAttribute(Type[] types)
        {
            Ensure.That(types, nameof(types)).HasItems();

            Types = types;
        }
    }
}
