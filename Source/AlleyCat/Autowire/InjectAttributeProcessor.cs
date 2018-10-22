using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using EnsureThat;
using Godot;
using LanguageExt;

namespace AlleyCat.Autowire
{
    public abstract class InjectAttributeProcessor<T> : MemberAttributeProcessor<T>
        where T : InjectAttribute
    {
        public Type TargetType { get; }

        public Type DependencyType { get; }

        public bool Optional { get; }

        public bool Enumerable { get; }

        public override AutowirePhase ProcessPhase => AutowirePhase.Resolve;

        protected Action<object, object> TargetSetter { get; }

        protected InjectAttributeProcessor(MemberInfo member, T attribute) : base(member, attribute)
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
                    }
                    else
                    {
                        var writableProperty = property.DeclaringType?.GetProperty(
                            property.Name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

                        if (writableProperty == null)
                        {
                            throw new InvalidOperationException(
                                $"Property '{property.DeclaringType?.Name}.{property.Name}' is not writable.");
                        }

                        TargetSetter = writableProperty.SetValue;
                    }

                    break;
                default:
                    throw new InvalidOperationException($"Unknown member type: {member}.");
            }

            DependencyType = TargetType;

            if (!TargetType.IsGenericType) return;

            var genType = TargetType.GetGenericTypeDefinition();

            if (genType == typeof(Option<>))
            {
                DependencyType = TargetType.GetGenericArguments()[0];

                Debug.Assert(DependencyType != null, "DependencyType != null");

                Optional = true;
            }
            else if (genType == typeof(IEnumerable<>))
            {
                DependencyType = TargetType.GetGenericArguments()[0];

                Debug.Assert(DependencyType != null, "DependencyType != null");

                Enumerable = true;
            }
        }

        public override void Process(IAutowireContext context, Node node)
        {
            Ensure.That(context, nameof(context)).IsNotNull();
            Ensure.That(node, nameof(node)).IsNotNull();

            object dependency;

            var enumerable = GetDependencies(context, node);

            bool hasValue;

            if (Enumerable)
            {
                dependency = enumerable;
                hasValue = enumerable.OfType<object>().Any();
            } else if (Optional)
            {
                var option = OptionalHelper.HeadOrNone(enumerable, DependencyType);

                dependency = option;
                hasValue = option.IsSome;
            }
            else
            {
                dependency = enumerable.OfType<object>().FirstOrDefault();
                hasValue = dependency != null;
            }

            if (Attribute.Required && !hasValue)
            {
                throw new InvalidOperationException(
                    $"Cannot resolve required dependency for {node.GetPath()}.{Member.Name}.");
            }

            TargetSetter(node, dependency);
        }

        protected abstract IEnumerable GetDependencies(IAutowireContext context, Node node);
    }
}
