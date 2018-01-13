using System;
using System.Reflection;
using EnsureThat;
using JetBrains.Annotations;

namespace AlleyCat.Autowire
{
    public abstract class MemberAttributeProcessor<T> : AttributeProcessor<T> where T : Attribute
    {
        [NotNull]
        public MemberInfo Member { get; }

        protected MemberAttributeProcessor([NotNull] MemberInfo member, [NotNull] T attribute)
            : base(attribute)
        {
            Ensure.Any.IsNotNull(member, nameof(member));

            Member = member;
        }
    }
}
