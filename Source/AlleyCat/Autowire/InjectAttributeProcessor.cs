using System;
using System.Collections.Generic;
using System.Reflection;
using EnsureThat;
using JetBrains.Annotations;

namespace AlleyCat.Autowire
{
    public abstract class InjectAttributeProcessor<T> : MemberAttributeProcessor<T>
        where T : InjectAttribute
    {
        public bool Required => Attribute.Required;

        [NotNull]
        public Type TargetType { get; }

        [NotNull]
        public Type DependencyType { get; }

        public bool Enumerable { get; }

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

            if (TargetType.IsGenericType && TargetType.GetGenericTypeDefinition() == typeof(IEnumerable<>))
            {
                DependencyType = TargetType.GetGenericArguments()[0] ?? TargetType;
                Enumerable = DependencyType != TargetType;
            }
            else
            {
                DependencyType = TargetType;
            }
        }

        public override void Process(IAutowireContext context, object service)
        {
            Ensure.Any.IsNotNull(context, nameof(context));
            Ensure.Any.IsNotNull(service, nameof(service));

            var dependency = GetDependency(context, service);

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
            [NotNull] IAutowireContext context, [NotNull] object service);
    }
}
