using System;
using System.Linq;
using EnsureThat;
using Godot;
using JetBrains.Annotations;

namespace AlleyCat.Animation
{
    public interface IAnimationGraph : IDisposable
    {
        string Path { get; }

        AnimationRootNode Root { get; }

        [CanBeNull]
        AnimationNode GetAnimationNode([NotNull] string name);

        [CanBeNull]
        IAnimationGraph GetGraph([NotNull] string name);

        [CanBeNull]
        IAnimationControl GetControl([NotNull] string name);
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
        [CanBeNull]
        public static IAnimationGraph GetDescendantGraph(
            [NotNull] this IAnimationGraph graph, [NotNull] string path)
        {
            Ensure.Any.IsNotNull(graph, nameof(graph));
            Ensure.Any.IsNotNull(path, nameof(path));

            var result = graph;

            foreach (var segment in path.Split("/"))
            {
                result = result.GetGraph(segment);

                if (result == null) break;
            }

            return result;
        }

        [CanBeNull]
        public static IAnimationControl GetDescendantControl(
            [NotNull] this IAnimationGraph graph, [NotNull] string path)
        {
            Ensure.Any.IsNotNull(graph, nameof(graph));
            Ensure.Any.IsNotNull(path, nameof(path));

            var segments = path.Split("/");

            if (segments.Length == 1)
            {
                return graph.GetControl(path);
            }

            var length = segments.Length - 1;

            var parentPath = string.Join("/", segments.Take(length));
            var parent = graph.GetDescendantGraph(parentPath);

            return parent?.GetControl(segments.Skip(length).First());
        }
    }
}
