using System;
using JetBrains.Annotations;

namespace AlleyCat.Autowire
{
    [MeansImplicitUse(ImplicitUseKindFlags.Access)]
    [AttributeUsage(AttributeTargets.Method)]
    public class PostConstructAttribute : Attribute
    {
    }
}
