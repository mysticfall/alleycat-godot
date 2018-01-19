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

            var path = string.IsNullOrWhiteSpace(NodePath) ? NormalizeMemberName(Member.Name) : NodePath;

            return ((Node) service).GetNode(path);
        }

        [NotNull]
        protected static string NormalizeMemberName([NotNull] string name)
        {
            Ensure.String.IsNotNullOrWhiteSpace(name, nameof(name));

            var normalized = name.StartsWith("_") ? name.Substring(1) : name;

            return normalized.Length < 2 ? normalized :
                normalized.Substring(0, 1).ToUpper() + normalized.Substring(1);
        }
    }
}
