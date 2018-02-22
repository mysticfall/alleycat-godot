using System;
using System.Collections.Generic;
using System.Linq;
using EnsureThat;
using Godot;
using JetBrains.Annotations;

namespace AlleyCat.Common
{
    public static class NodeExtensions
    {
        [NotNull]
        public static T GetNode<T>([NotNull] this Node node, [NotNull] NodePath path) where T : class
        {
            Ensure.Any.IsNotNull(node, nameof(node));
            Ensure.Any.IsNotNull(path, nameof(path));

            var result = node.GetNode(path) as T;

            if (result == null)
            {
                throw new InvalidOperationException(
                    $"Unable to find node '{path}' in '{node.Name}'.");
            }

            return result;
        }

        [CanBeNull]
        public static T GetNodeOrDefault<T>(
            [NotNull] this Node node,
            [NotNull] NodePath path,
            [CanBeNull] T defaultValue = default(T)) where T : class
        {
            Ensure.Any.IsNotNull(node, nameof(node));
            Ensure.Any.IsNotNull(path, nameof(path));

            return (node.HasNode(path) ? node.GetNode(path) as T : null) ?? defaultValue;
        }

        [NotNull]
        public static TChild GetOrCreateNode<TParent, TChild>(
            [NotNull] this TParent node,
            [NotNull] NodePath path,
            [NotNull] Func<Node, TChild> factory)
            where TParent : Node
            where TChild : Node
        {
            Ensure.Any.IsNotNull(node, nameof(node));
            Ensure.Any.IsNotNull(path, nameof(path));
            Ensure.Any.IsNotNull(factory, nameof(factory));

            var child = GetNodeOrDefault<TChild>(node, path);

            if (child == null)
            {
                var segments = path.GetConcatenatedSubnames().Split('/');

                Node parent;

                if (segments.Length <= 1)
                {
                    parent = node;
                }
                else
                {
                    var parentSegments = segments.Take(segments.Length - 1);
                    var parentPath = string.Join("/", parentSegments);

                    parent = node.GetNode(parentPath);
                }

                Ensure.Any.IsNotNull(parent, nameof(parent),
                    opt => opt.WithMessage($"Unable to determine parent node: '{path}'."));

                child = factory(parent);

                Ensure.Any.IsNotNull(child, nameof(child),
                    opt => opt.WithMessage("The specified node factory returned null."));

                node.AddChild(child);
            }

            return child;
        }

        [NotNull]
        public static T GetChild<T>([NotNull] this Node node) where T : class
        {
            Ensure.Any.IsNotNull(node, nameof(node));

            switch (node.GetChildren().FirstOrDefault(n => n is T))
            {
                case T result:
                    return result;
                default:
                    throw new InvalidOperationException(
                        $"Unable to find node of type '{typeof(T)}' in '{node.Name}'.");
            }
        }

        [CanBeNull]
        public static T GetChildOrDefault<T>(
            [NotNull] this Node node,
            [CanBeNull] T defaultValue = default(T)) where T : class
        {
            Ensure.Any.IsNotNull(node, nameof(node));

            switch (node.GetChildren().FirstOrDefault(n => n is T))
            {
                case T result:
                    return result;
                default:
                    return defaultValue;
            }
        }

        [NotNull]
        public static TChild GetOrCreateChild<TParent, TChild>(
            [NotNull] this TParent node,
            [NotNull] Func<TParent, TChild> factory)
            where TParent : Node
            where TChild : Node
        {
            Ensure.Any.IsNotNull(node, nameof(node));
            Ensure.Any.IsNotNull(factory, nameof(factory));

            switch (node.GetChildren().FirstOrDefault(n => n is TChild))
            {
                case TChild result:
                    return result;
                default:
                    var child = factory(node);

                    Ensure.Any.IsNotNull(child, nameof(child),
                        opt => opt.WithMessage("The specified node factory returned null."));

                    node.AddChild(child);

                    return child;
            }
        }

        [NotNull]
        public static IEnumerable<T> GetChildren<T>([NotNull] this Node node) 
        {
            Ensure.Any.IsNotNull(node, nameof(node));

            return node.GetChildren().OfType<T>();
        }
    }
}
