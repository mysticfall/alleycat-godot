using System;
using JetBrains.Annotations;

namespace AlleyCat.Autowire
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    [MeansImplicitUse(ImplicitUseKindFlags.Assign)]
    public abstract class InjectAttribute : Attribute
    {
        public bool Required { get; }

        protected InjectAttribute(bool required = false)
        {
            Required = required;
        }
    }
}
