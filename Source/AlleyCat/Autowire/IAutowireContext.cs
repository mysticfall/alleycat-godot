using System;
using Godot;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;

namespace AlleyCat.Autowire
{
    public interface IAutowireContext : IServiceProvider
    {
        [NotNull]
        Node Node { get; }

        [CanBeNull]
        IAutowireContext Parent { get; }

        [NotNull]
        IServiceCollection ServiceCollection { get; }

        void Register([NotNull] object instance);
    }
}
