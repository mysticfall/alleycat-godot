using System;
using EnsureThat;
using Godot;

namespace AlleyCat.Common
{
    public class NodeException : Exception
    {
        public Node Node { get; }

        protected NodeException(string message, Node node) : this(message, null, node)
        {
        }

        protected NodeException(string message, Exception innerException, Node node) : base(message, innerException)
        {
            Ensure.That(node, nameof(node)).IsNotNull();

            Node = node;
        }
    }
}
