using System;
using System.Reflection;
using EnsureThat;
using Godot;
using JetBrains.Annotations;

namespace AlleyCat.Autowire
{
    public class NodeAttributeProcessor : InjectAttributeProcessor<NodeAttribute>
    {
        [CanBeNull]
        public string NodePath => Attribute.Path;

        public NodeAttributeProcessor([NotNull] MemberInfo member, [NotNull] NodeAttribute attribute)
            : base(member, attribute)
        {
        }

        protected override object GetDependency(IServiceProvider provider, object service)
        {
            Ensure.Any.IsNotNull(provider, nameof(provider));

            var node = service as Node;

            Ensure.Any.IsNotNull(node, nameof(service), 
                opts => opts.WithMessage(
                    "[Node] attribute is only supported on members of a Node type class."));

            var path = string.IsNullOrWhiteSpace(NodePath) ? Member.Name : NodePath;

            return ((Node) service).GetNode(path);
        }
    }
}
