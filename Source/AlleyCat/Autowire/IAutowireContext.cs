﻿using System;
using System.Collections.Generic;
using EnsureThat;
using Godot;
using LanguageExt;
using Microsoft.Extensions.DependencyInjection;
using static LanguageExt.Prelude;

namespace AlleyCat.Autowire
{
    public interface IAutowireContext : IDependencyResolver, IDisposable
    {
        Node Node { get; }

        Option<IAutowireContext> Parent { get; }

        void AddService(Action<IServiceCollection> provider);

        Option<T> FindService<T>();

        Option<object> FindService(Type type);
    }

    public static class AutowireContextExtensions
    {
        public static IEnumerable<T> FindServices<T>(this AutowireContext context)
        {
            Ensure.That(context, nameof(context)).IsNotNull();

            return context.FindService<IEnumerable<T>>().AsEnumerable().Bind(identity);
        }
    }
}
