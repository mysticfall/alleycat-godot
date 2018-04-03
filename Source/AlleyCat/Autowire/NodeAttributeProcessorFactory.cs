using System.Reflection;
using System.Text;
using EnsureThat;
using Godot;
using JetBrains.Annotations;

namespace AlleyCat.Autowire
{
    public class NodeAttributeProcessorFactory : MemberAttributeProcessorFactory<NodeAttribute>
    {
        protected override INodeProcessor CreateProcessor(MemberInfo member, NodeAttribute attribute)
        {
            Ensure.Any.IsNotNull(member, nameof(member));
            Ensure.Any.IsNotNull(attribute, nameof(attribute));

            var fieldName = ToPrivateFieldName(member.Name);
            var field = member.DeclaringType?.GetField(fieldName,
                BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.DeclaredOnly);

            return new NodeAttributeProcessor(member, field, attribute);
        }

        [NotNull]
        protected static string ToPrivateFieldName([NotNull] string name)
        {
            Ensure.String.IsNotNullOrWhiteSpace(name, nameof(name));

            if (name.StartsWith("_"))
            {
                return name + "Path";
            }

            return new StringBuilder()
                .Append("_")
                .Append(name.Left(1).ToLower())
                .Append(name.Substring(1))
                .ToString();
        }
    }
}
