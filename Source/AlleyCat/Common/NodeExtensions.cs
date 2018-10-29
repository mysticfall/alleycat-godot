using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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
            Ensure.That(path, nameof(path)).IsNotNull();

            return node.HasNode(path) ? Optional(node.GetNode(path) as T) : None;
        }

        public static Option<T> FindComponent<T>(this Node node) where T : class
        {
            Ensure.That(node, nameof(node)).IsNotNull();

            return node.GetChildren().Find(n => n is T).OfType<T>().HeadOrNone();
        }

        public static T GetComponent<T>(this Node node, NodePath path) where T : class
        {
            Ensure.That(node, nameof(node)).IsNotNull();
            Ensure.That(path, nameof(path)).IsNotNull();

            if (!(node.GetNode(path) is T result))
            {
                throw new InvalidOperationException($"Unable to find node '{path}' in '{node.Name}'.");
            }

            return result;
        }

        public static TChild GetComponent<TParent, TChild>(
            this TParent node,
            NodePath path,
            Func<Node, TChild> factory)
            where TParent : Node
            where TChild : Node
        {
            Ensure.That(node, nameof(node)).IsNotNull();
            Ensure.That(path, nameof(path)).IsNotNull();
            Ensure.That(factory, nameof(factory)).IsNotNull();

            return node.FindComponent<TChild>(path).IfNone(() => CreateAndAdd(node, factory));
        }

        public static TChild GetComponent<TParent, TChild>(
            this TParent node,
            Func<TParent, TChild> factory)
            where TParent : Node
            where TChild : Node
        {
            Ensure.That(node, nameof(node)).IsNotNull();
            Ensure.That(factory, nameof(factory)).IsNotNull();

            return node.FindComponent<TChild>().IfNone(() => CreateAndAdd(node, factory));
        }

        private static TChild CreateAndAdd<TParent, TChild>(TParent node, Func<TParent, TChild> factory)
            where TParent : Node
            where TChild : Node
        {
            Debug.Assert(node != null, "node != null");
            Debug.Assert(factory != null, "factory != null");

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

        public static IEnumerable<T> GetChildComponents<T>(this Node node)
        {
            Ensure.That(node, nameof(node)).IsNotNull();

            return node.GetChildren().OfType<T>();
        }

        public static Option<T> FindParent<T>(this Node node) where T : class
        {
            Ensure.That(node, nameof(node)).IsNotNull();

            return Optional(node.GetParent()).OfType<T>().HeadOrNone();
        }

        public static IEnumerable<Node> GetAncestors(this Node node) => new NodeAncestors(node);

        public static Option<T> GetClosestAncestor<T>(this Node node) where T : class
        {
            Ensure.That(node, nameof(node)).IsNotNull();

            return GetAncestors(node).Find(a => a is T).OfType<T>().HeadOrNone();
        }

        public static Option<Node> GetClosestAncestor(this Node node, Func<Node, bool> predicate)
        {
            Ensure.That(node, nameof(node)).IsNotNull();
            Ensure.That(predicate, nameof(predicate)).IsNotNull();

            return GetAncestors(node).Find(predicate);
        }
    }
}

internal struct NodeAncestors : IEnumerable<Node>, IEnumerator<Node>
{
    public Node Current { get; private set; }

    object IEnumerator.Current => Current;

    public IEnumerator<Node> GetEnumerator() => this;

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    private readonly Node _node;

    private bool _initial;

    public NodeAncestors(Node node)
    {
        Ensure.That(node, nameof(node)).IsNotNull();

        _node = node;
        _initial = true;

        Current = null;
    }

    public bool MoveNext()
    {
        Current = _initial ? _node.GetParent() : Current?.GetParent();
        _initial = false;

        return Current != null;
    }

    public void Reset()
    {
        Current = null;
        _initial = true;
    }

    public void Dispose()
    {
    }
}
