using System;
using System.Reflection;
using EnsureThat;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;

namespace AlleyCat.Autowire
{
    public abstract class InjectAttributeProcessor<T> : MemberAttributeProcessor<T>
        where T : InjectAttribute
    {
        public bool Required => Attribute.Required;

        [NotNull]
        public Type TargetType { get; }

        public override AutowirePhase ProcessPhase => AutowirePhase.Resolve;

        [NotNull]
        protected Action<object, object> TargetSetter { get; }

        protected InjectAttributeProcessor([NotNull] MemberInfo member, [NotNull] T attribute)
            : base(member, attribute)
        {
            switch (member)
            {
                case FieldInfo field:
                    TargetType = field.FieldType;
                    TargetSetter = field.SetValue;

                    break;
                case PropertyInfo property:
                    TargetType = property.PropertyType;
                    TargetSetter = property.SetValue;

                    break;
                default:
                    throw new InvalidOperationException($"Unknown member type: {member}.");
            }
        }

        public override void Process(
            IServiceCollection collection, IServiceProvider provider, object service)
        {
            Ensure.Any.IsNotNull(collection, nameof(collection));
            Ensure.Any.IsNotNull(provider, nameof(provider));
            Ensure.Any.IsNotNull(service, nameof(service));

            var dependency = GetDependency(provider, service);

            if (Required && dependency == null)
            {
                var member = $"{Member.DeclaringType?.FullName}.{Member.Name}";

                throw new InvalidOperationException(
                    $"Cannot resolve required dependency for {member}.");
            }

            TargetSetter(service, dependency);
        }

        [CanBeNull]
        protected abstract object GetDependency(
            [NotNull] IServiceProvider provider, [NotNull] object service);
    }
}
