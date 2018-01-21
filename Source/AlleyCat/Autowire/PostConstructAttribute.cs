using System;
using JetBrains.Annotations;

namespace AlleyCat.Autowire
{
    [AttributeUsage(AttributeTargets.Method)]
    [MeansImplicitUse(ImplicitUseKindFlags.Access)]
    public class PostConstructAttribute : Attribute
    {
    }
}
