using System;
using Godot;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;

namespace AlleyCat.Autowire
{
    public interface IAutowireContext : IServiceProvider, IDependencyResolver
    {
        [NotNull]
        Node Node { get; }

        [CanBeNull]
        IAutowireContext Parent { get; }

        void AddService(Action<IServiceCollection> provider);

        void Register([NotNull] Node node);

        void Build();
    }
}
