using System;
using System.Reflection;
using EnsureThat;

namespace AlleyCat.Autowire
{
    public abstract class MemberAttributeProcessor<T> : AttributeProcessor<T> where T : Attribute
    {
        public MemberInfo Member { get; }

        protected MemberAttributeProcessor(MemberInfo member, T attribute) : base(attribute)
        {
            Ensure.That(member, nameof(member)).IsNotNull();

            Member = member;
        }
    }
}
