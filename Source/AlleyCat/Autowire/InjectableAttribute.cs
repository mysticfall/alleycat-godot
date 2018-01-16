using System;
using EnsureThat;
using JetBrains.Annotations;

namespace AlleyCat.Autowire
{
    [AttributeUsage(AttributeTargets.Class)]
    public abstract class InjectableAttribute : Attribute
    {
        [NotNull]
        public Type[] Types { get; }

        protected InjectableAttribute([NotNull] Type[] types)
        {
            Ensure.Collection.HasItems(types, nameof(types));

            Types = types;
        }
    }
}
