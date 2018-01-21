using System;
using JetBrains.Annotations;

namespace AlleyCat.Autowire
{
    [AttributeUsage(AttributeTargets.Class)]
    [MeansImplicitUse(ImplicitUseKindFlags.Access)]
    public class AutowireContextAttribute : Attribute
    {
    }
}
