using System;
using System.Collections;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using AlleyCat.Autowire;
using EnsureThat;
using Godot;
using LanguageExt;
using LanguageExt.UnsafeValueAccess;
using static LanguageExt.Prelude;

namespace AlleyCat.Autowire
{
    public class ServiceAttributeProcessor : InjectAttributeProcessor<ServiceAttribute>, IDependencyConsumer
    {
        public HashSet<Type> Requires => HashSet(DependencyType);

        public ServiceAttributeProcessor(
            MemberInfo member, ServiceAttribute attribute) : base(member, attribute)
        {
        }

        protected override IEnumerable GetDependencies(IAutowireContext context, Node node)
        {
            IEnumerable enumerable = new ServiceDependencyCollector(
                context, node, Enumerable ? TargetType : DependencyType);

            if (Attribute.Local)
            {
                enumerable = enumerable.Cast<object>().Take(1);
            }

            if (Enumerable)
            {
                enumerable = enumerable
                    .OfType<IEnumerable>()
                    .Fold(EnumerableHelper.Empty(DependencyType),
                        (e1, e2) => EnumerableHelper.Concat(e1, e2, DependencyType));

                enumerable = EnumerableHelper.Cast(enumerable, DependencyType);
            }
            else
            {
                enumerable = EnumerableHelper.OfType(enumerable, DependencyType);
            }

            return enumerable;
        }
    }
}

internal struct ServiceDependencyCollector : IEnumerable, IEnumerator
{
    public IEnumerator GetEnumerator() => this;

    object IEnumerator.Current => _value.ValueUnsafe();

    private readonly IAutowireContext _context;

    private readonly Node _targetNode;

    private readonly Type _targetType;

    private Option<IAutowireContext> _current;

    private Option<object> _value;

    public ServiceDependencyCollector(
        IAutowireContext context, Node targetNode, Type targetType)
    {
        Ensure.That(context, nameof(context)).IsNotNull();
        Ensure.That(targetNode, nameof(targetNode)).IsNotNull();
        Ensure.That(targetType, nameof(targetType)).IsNotNull();

        _context = context;
        _targetNode = targetNode;
        _targetType = targetType;

        _current = Some(context);
        _value = None;
    }

    public bool MoveNext()
    {
        if (_current.IsNone) return false;

        _value = _current.Bind(FindService);
        _current = _current.Bind(c => c.Parent);

        return _value.IsSome || _current.IsSome;
    }

    private Option<object> FindService(IAutowireContext context)
    {
        Debug.Assert(context != null, "context != null");

        var targetType = _targetType;
        var targetNode = _targetNode;

        return context.FindService(targetType).BiBind(
            Some,
            () =>
            {
                var factoryType = typeof(IServiceFactory<>).MakeGenericType(targetType);
                var factory = context.FindService(factoryType);

                return factory.Bind(f =>
                {
                    var method = typeof(IServiceFactory<>)
                        .MakeGenericType(targetType)
                        .GetMethod("Create", new[] {typeof(IAutowireContext), typeof(object)});

                    Debug.Assert(method != null, "method != null");

                    var service = method.Invoke(f, new object[] {context, targetNode});

                    return Optional(service);
                });
            }
        );
    }

    public void Reset()
    {
        _current = Some(_context);
        _value = None;
    }
}
