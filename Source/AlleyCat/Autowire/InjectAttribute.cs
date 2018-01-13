using System;
using JetBrains.Annotations;

namespace AlleyCat.Autowire
{
    [MeansImplicitUse(ImplicitUseKindFlags.Assign)]
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public abstract class InjectAttribute : Attribute
    {
        public bool Required { get; }

        protected InjectAttribute(bool required = true)
        {
            Required = required;
        }
    }
}
