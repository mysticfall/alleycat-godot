using System.Linq;
using System.Reflection;
using EnsureThat;
using Godot;
using JetBrains.Annotations;

namespace AlleyCat.Autowire
{
    public class NodeAttributeProcessor : InjectAttributeProcessor<NodeAttribute>
    {
        [CanBeNull]
        public FieldInfo NodePathField { get; }

        public NodeAttributeProcessor([NotNull] MemberInfo member, [NotNull] NodeAttribute attribute)
            : base(member, attribute)
        {
        }

        public NodeAttributeProcessor(
            [NotNull] MemberInfo member,
            [CanBeNull] FieldInfo pathField,
            [NotNull] NodeAttribute attribute) : base(member, attribute)
        {
            NodePathField = pathField;
        }

        protected override object GetDependency(IAutowireContext context, Node node)
        {
            Ensure.Any.IsNotNull(context, nameof(context));

            Ensure.Any.IsNotNull(node, nameof(node),
                opts => opts.WithMessage(
                    "[Node] attribute is only supported on members of a Node type class."));

            var path = GetNodePath(node);

            object dependency;

            if (Enumerable)
            {
                var parent = path == null ? node : node.GetNode(path);
                var list = parent?.GetChildren().Where(DependencyType.IsInstanceOfType).ToList();

                dependency = EnumerableHelper.Cast(list, DependencyType);
            }
            else
            {
                dependency = node.GetNode(path ?? NormalizeMemberName(Member.Name));
            }

            return dependency;
        }

        [CanBeNull]
        protected NodePath GetNodePath([NotNull] Node node)
        {
            Ensure.Any.IsNotNull(node, nameof(node));

            var path = NodePathField?.GetValue(node) as NodePath;

            if (path == null && !string.IsNullOrEmpty(Attribute.Path))
            {
                path = Attribute.Path;
            }

            return path;
        }

        [NotNull]
        protected static string NormalizeMemberName([NotNull] string name)
        {
            Ensure.String.IsNotNullOrWhiteSpace(name, nameof(name));

            var normalized = name.StartsWith("_") ? name.Substring(1) : name;

            return normalized.Length < 2 ? normalized : normalized.Substring(0, 1).ToUpper() + normalized.Substring(1);
        }
    }
}
