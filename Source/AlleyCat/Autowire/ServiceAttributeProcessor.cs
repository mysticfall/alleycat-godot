using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using EnsureThat;
using Godot;
using JetBrains.Annotations;

namespace AlleyCat.Autowire
{
    public class ServiceAttributeProcessor : InjectAttributeProcessor<ServiceAttribute>, IDependencyConsumer
    {
        public ISet<Type> Requires => new HashSet<Type> {DependencyType};

        public ServiceAttributeProcessor(
            [NotNull] MemberInfo member, [NotNull] ServiceAttribute attribute) : base(member, attribute)
        {
        }

        protected override object GetDependency(IAutowireContext context, Node node)
        {
            Ensure.Any.IsNotNull(context, nameof(context));
            Ensure.Any.IsNotNull(node, nameof(node));

            var factoryType = typeof(IServiceFactory<>).MakeGenericType(TargetType);

            IEnumerable enumerable = null;

            var current = context;

            while (current != null)
            {
                var dependency = current.GetService(TargetType);

                if (dependency == null)
                {
                    var factory = current.GetService(factoryType);

                    if (factory != null)
                    {
                        var method = typeof(IServiceFactory<>)
                            .MakeGenericType(TargetType)
                            .GetMethod("Create", new[] {typeof(IAutowireContext), typeof(object)});

                        Debug.Assert(method != null, "method != null");

                        dependency = method.Invoke(factory, new object[] {context, node});
                    }
                }

                if (dependency != null)
                {
                    if (Enumerable)
                    {
                        if (enumerable == null)
                        {
                            enumerable = (IEnumerable) dependency;
                        }
                        else
                        {
                            enumerable = EnumerableHelper.Concat(enumerable, (IEnumerable) dependency, DependencyType);
                        }
                    }
                    else
                    {
                        return dependency;
                    }
                }

                current = Attribute.IncludeInherited ? current.Parent : null;
            }

            return enumerable;
        }
    }
}
