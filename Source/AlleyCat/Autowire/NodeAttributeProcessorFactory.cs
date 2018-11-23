using System.Reflection;
using System.Text;
using EnsureThat;
using Godot;

namespace AlleyCat.Autowire
{
    public class NodeAttributeProcessorFactory : MemberAttributeProcessorFactory<NodeAttribute>
    {
        protected override INodeProcessor CreateProcessor(MemberInfo member, NodeAttribute attribute)
        {
            Ensure.That(member, nameof(member)).IsNotNull();

            var fieldName = ToPrivateFieldName(member.Name);

            FieldInfo FindField(string name) => member.DeclaringType?.GetField(name,
                BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.DeclaredOnly);

            var field = FindField(fieldName + "Path") ?? FindField(fieldName);

            return new NodeAttributeProcessor(field, member, attribute);
        }

        protected static string ToPrivateFieldName(string name)
        {
            Ensure.That(name, nameof(name)).IsNotNullOrWhiteSpace();

            if (name.StartsWith("_")) return name;

            return new StringBuilder()
                .Append("_")
                .Append(name.Left(1).ToLower())
                .Append(name.Substring(1))
                .ToString();
        }
    }
}
