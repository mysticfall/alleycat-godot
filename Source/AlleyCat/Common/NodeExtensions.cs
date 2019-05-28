using System;
using System.Collections.Generic;
using System.Linq;
using AlleyCat.Event;
using AlleyCat.Game;
using EnsureThat;
using Godot;
using LanguageExt;
using static LanguageExt.Prelude;

namespace AlleyCat.Common
{
    public static class NodeExtensions
    {
        public static Option<T> FindComponent<T>(this Node node, NodePath path) where T : class
        {
            Ensure.That(node, nameof(node)).IsNotNull();

            return node.HasNode(path) ? Optional(node.GetNode(path)).Bind(OfType<T>) : None;
        }

        public static Option<object> FindComponent(this Node node, NodePath path, Type type)
        {
            Ensure.That(node, nameof(node)).IsNotNull();
            Ensure.That(type, nameof(type)).IsNotNull();

            return node.HasNode(path) ? Optional(node.GetNode(path)).Bind(n => OfType(n, type)) : None;
        }

        public static Option<T> FindComponent<T>(this Node node) where T : class
        {
            Ensure.That(node, nameof(node)).IsNotNull();

            // ReSharper disable once ConvertClosureToMethodGroup
            return node.GetChildren().Cast<Node>().Bind(n => OfType<T>(n)).HeadOrNone();
        }

        public static Option<object> FindComponent(this Node node, Type type)
        {
            Ensure.That(node, nameof(node)).IsNotNull();
            Ensure.That(type, nameof(type)).IsNotNull();

            return node.GetChildren().Cast<Node>().Bind(n => OfType(n, type)).HeadOrNone();
        }

        public static T GetComponent<T>(this Node node, NodePath path) where T : class
        {
            Ensure.That(node, nameof(node)).IsNotNull();

            return Optional(node.GetNode(path)).Bind(OfType<T>).IfNone(() =>
                throw new InvalidOperationException($"Unable to find node '{path}' in '{node.Name}'."));
        }

        public static TChild GetComponent<TParent, TChild>(
            this TParent node,
            NodePath path,
            Func<Node, TChild> factory)
            where TParent : Node
            where TChild : Node
        {
            Ensure.That(node, nameof(node)).IsNotNull();

            return node.FindComponent<TChild>(path).IfNone(() => CreateAndAdd(node, factory));
        }

        public static TChild GetComponent<TParent, TChild>(
            this TParent node,
            Func<TParent, TChild> factory)
            where TParent : Node
            where TChild : Node
        {
            Ensure.That(node, nameof(node)).IsNotNull();

            return node.FindComponent<TChild>().IfNone(() => CreateAndAdd(node, factory));
        }

        private static TChild CreateAndAdd<TParent, TChild>(TParent node, Func<TParent, TChild> factory)
            where TParent : Node
            where TChild : Node
        {
            Ensure.That(node, nameof(node)).IsNotNull();
            Ensure.That(factory, nameof(factory)).IsNotNull();

            var child = factory(node);

            if (child == null)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(factory),
                    "The factory returned a null instance.");
            }

            node.AddChild(child);

            return child;
        }

        public static IEnumerable<T> GetChildComponents<T>(this Node node) where T : class
        {
            Ensure.That(node, nameof(node)).IsNotNull();

            // ReSharper disable once ConvertClosureToMethodGroup
            return node.GetChildren().Cast<Node>().Bind(n => OfType<T>(n));
        }

        public static Option<T> FindParent<T>(this Node node) where T : class
        {
            Ensure.That(node, nameof(node)).IsNotNull();

            return Optional(node.GetParent()).Bind(OfType<T>);
        }

        public static IEnumerable<Node> GetAncestors(this Node node)
        {
            Ensure.That(node, nameof(node)).IsNotNull();

            var parent = node;

            while ((parent = parent.GetParent()) != null)
            {
                yield return parent;
            }
        }

        public static Option<T> FindClosestAncestor<T>(this Node node) where T : class =>
            GetAncestors(node).Bind(n => n.OfType<T>()).HeadOrNone();

        public static Option<Node> FindClosestAncestor(this Node node, Func<Node, bool> predicate) =>
            GetAncestors(node).Find(predicate);

        public static Option<T> OfType<T>(this Node node) where T : class
        {
            Ensure.That(node, nameof(node)).IsNotNull();

            switch (node)
            {
                case T result:
                    return result;
                case IGameObjectFactory factory:
                    return factory.Service.ToOption().OfType<T>().HeadOrNone();
                default:
                    return node.FindDelegate().Bind(OfType<T>).HeadOrNone();
            }
        }

        public static Option<object> OfType(this Node node, Type type)
        {
            Ensure.That(node, nameof(node)).IsNotNull();
            Ensure.That(type, nameof(type)).IsNotNull();

            switch (node)
            {
                case Node result when type.IsInstanceOfType(result):
                    return result;
                case IGameObjectFactory factory:
                    return factory.Service.ToOption().Filter(type.IsInstanceOfType).HeadOrNone();
                default:
                    return node.FindDelegate().Bind(n => OfType(n, type)).HeadOrNone();
            }
        }

        public static void RemoveAndFree(this Node node)
        {
            Ensure.That(node, nameof(node)).IsNotNull();

            Optional(node.GetParent()).Iter(p => p.RemoveChild(node));

            node.QueueFree();
        }

        public static void FreeChild(this Node parent, Node child)
        {
            Ensure.That(parent, nameof(parent)).IsNotNull();
            Ensure.That(child, nameof(child)).IsNotNull();

            parent.RemoveChild(child);
            child.QueueFree();
        }

        public static IObservable<Unit> OnTreeExited(this Node node)
        {
            return node.FromSignal("tree_exited").AsUnitObservable();
        }

        public static IObservable<Unit> OnTreeExiting(this Node node)
        {
            return node.FromSignal("tree_exiting").AsUnitObservable();
        }
    }
}
