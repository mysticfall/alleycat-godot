using System.Reflection;
using EnsureThat;
using Godot;
using JetBrains.Annotations;

namespace AlleyCat.Autowire
{
    public class AncestorAttributeProcessor : InjectAttributeProcessor<AncestorAttribute>
    {
        public AncestorAttributeProcessor([NotNull] MemberInfo member, [NotNull] AncestorAttribute attribute)
            : base(member, attribute)
        {
        }

        protected override object GetDependency(IAutowireContext context, Node node)
        {
            Ensure.Any.IsNotNull(context, nameof(context));

            Ensure.Any.IsNotNull(node, nameof(node),
                opts => opts.WithMessage(
                    "[Ancestor] attribute is only supported on members of a Node type class."));

            Ensure.Bool.IsFalse(Enumerable, nameof(node),
                opts => opts.WithMessage(
                    "[Ancestor] attribute can't be used on an IEnumerable<T> type field or property."));

            var ancestor = node;

            while ((ancestor = ancestor.GetParent()) != null)
            {
                if (!DependencyType.IsInstanceOfType(ancestor)) continue;

                return ancestor;
            }

            return null;
        }
    }
}
