using System;
using Godot;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;

namespace AlleyCat.Autowire
{
    public interface IAutowireContext : IServiceProvider, IDependencyResolver, IDisposable
    {
        [NotNull]
        Node Node { get; }

        [CanBeNull]
        IAutowireContext Parent { get; }

        void AddService(Action<IServiceCollection> provider);
    }
}
