using System;
using System.Collections.Generic;
using System.Reflection;
using EnsureThat;
using Godot;
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

                    if (property.CanWrite)
                    {
                        TargetSetter = property.SetValue;
                    } else
                    {
                        var writeableProperty = property.DeclaringType?.GetProperty(property.Name);

                        if (writeableProperty == null)
                        {
                            throw new InvalidOperationException(
                                $"Property '{property.DeclaringType?.Name}.{property.Name}' is not writeable.");
                        }

                        TargetSetter = writeableProperty.SetValue;
                    }

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

        public override void Process(IAutowireContext context, Node node)
        {
            Ensure.Any.IsNotNull(context, nameof(context));
            Ensure.Any.IsNotNull(node, nameof(node));

            var dependency = GetDependency(context, node);

            if (Required && !HasValue(dependency, DependencyType))
            {
                var type = Member.DeclaringType;
                var prefix = type?.Namespace == null ? "" : type.Namespace + ".";

                var member = $"{prefix}{type?.Name}.{Member.Name}";

                throw new InvalidOperationException(
                    $"Cannot resolve required dependency for {member}.");
            }

            TargetSetter(node, dependency);
        }

        private bool HasValue(object dependency, Type type)
        {
            if (dependency == null)
            {
                return false;
            }

            return !Enumerable || EnumerableHelper.Any(dependency, type);
        }

        [CanBeNull]
        protected abstract object GetDependency([NotNull] IAutowireContext context, [NotNull] Node node);
    }
}
