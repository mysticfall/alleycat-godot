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
        public string NodePath => Attribute.Path;

        public NodeAttributeProcessor([NotNull] MemberInfo member, [NotNull] NodeAttribute attribute)
            : base(member, attribute)
        {
        }

        protected override object GetDependency(IAutowireContext context, Node node)
        {
            Ensure.Any.IsNotNull(context, nameof(context));

            Ensure.Any.IsNotNull(node, nameof(node),
                opts => opts.WithMessage(
                    "[Node] attribute is only supported on members of a Node type class."));

            var hasPath = !string.IsNullOrWhiteSpace(NodePath);

            object dependency;

            if (Enumerable)
            {
                var parent = hasPath ? node.GetNode(NodePath) : node;
                var list = parent?.GetChildren().Where(DependencyType.IsInstanceOfType).ToList();

                dependency = EnumerableHelper.Cast(list, DependencyType);
            }
            else
            {
                var path = hasPath ? NodePath : NormalizeMemberName(Member.Name);

                dependency = node.GetNode(path);
            }

            return dependency;
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
