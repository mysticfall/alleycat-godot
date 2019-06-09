using System;
using JetBrains.Annotations;

namespace AlleyCat.Autowire
{
    [AttributeUsage(AttributeTargets.Method)]
    [MeansImplicitUse(ImplicitUseKindFlags.Access)]
    public class PostConstructAttribute : System.Attribute
    {
        public bool Deferred { get; }

        public PostConstructAttribute(bool deferred = false)
        {
            Deferred = deferred;
        }

        public override string ToString() => "[PostConstruct]";
    }
}
