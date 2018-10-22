using System;
using System.Collections.Generic;
using System.Linq;
using EnsureThat;
using Godot;
using LanguageExt;
using static LanguageExt.Prelude;

namespace AlleyCat.Animation
{
    public interface IAnimationGraph : IDisposable
    {
        string Path { get; }

        AnimationRootNode Root { get; }

        Option<AnimationNode> FindAnimationNode(string name);

        Option<IAnimationGraph> FindGraph(string name);

        Option<IAnimationControl> FindControl(string name);
    }

    namespace Generic
    {
        public interface IAnimationGraph<out T> : IAnimationGraph where T : AnimationRootNode
        {
            new T Root { get; }
        }
    }

    public static class AnimationGraphExtensions
    {
        public static Option<T> FindAnimationNode<T>(this IAnimationGraph graph, string name)
            where T : AnimationNode
        {
            Ensure.That(graph, nameof(graph)).IsNotNull();
            Ensure.That(name, nameof(name)).IsNotNull();

            return graph.FindAnimationNode(name).OfType<T>().HeadOrNone();
        }

        public static Option<T> FindGraph<T>(this IAnimationGraph graph, string name)
            where T : IAnimationGraph
        {
            Ensure.That(graph, nameof(graph)).IsNotNull();
            Ensure.That(name, nameof(name)).IsNotNull();

            return graph.FindGraph(name).OfType<T>().HeadOrNone();
        }

        public static Option<T> FindControl<T>(this IAnimationGraph graph, string name)
            where T : IAnimationControl
        {
            Ensure.That(graph, nameof(graph)).IsNotNull();
            Ensure.That(name, nameof(name)).IsNotNull();

            return graph.FindControl(name).OfType<T>().HeadOrNone();
        }

        public static Option<IAnimationGraph> FindDescendantGraph(
            this IAnimationGraph graph, string path)
        {
            Option<IAnimationGraph> Find(IAnimationGraph parent, IEnumerable<string> segments) =>
                segments.Match(
                    () => None, 
                    parent.FindGraph,
                    (head, tail) => parent.FindGraph(head).Bind(p => Find(p, tail)));

            return Find(graph, path.Split("/"));
        }

        public static Option<T> FindDescendantGraph<T>(this IAnimationGraph graph, string path)
            where T : IAnimationGraph =>
            FindDescendantGraph(graph, path).OfType<T>().HeadOrNone();

        public static Option<IAnimationControl> FindDescendantControl(
            this IAnimationGraph graph, string path) =>
            path.Split("/").Rev().Match(
                () => None,
                graph.FindControl,
                (x, xs) => graph.FindDescendantGraph(string.Join("/", xs.Rev())).Bind(p => p.FindControl(x)));

        public static Option<T> FindDescendantControl<T>(
            this IAnimationGraph graph, string path) where T : IAnimationControl =>
            FindDescendantControl(graph, path).OfType<T>().HeadOrNone();
    }
}
