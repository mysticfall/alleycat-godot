using System.Collections;
using System.Linq;
using System.Reflection;
using AlleyCat.Common;
using AlleyCat.Game;
using EnsureThat;
using Godot;
using static LanguageExt.Prelude;

namespace AlleyCat.Autowire
{
    public class AncestorAttributeProcessor : InjectAttributeProcessor<AncestorAttribute>
    {
        public AncestorAttributeProcessor(MemberInfo member, AncestorAttribute attribute)
            : base(member, attribute)
        {
        }

        protected override IEnumerable GetDependencies(IAutowireContext context, Node node)
        {
            Ensure.That(node, nameof(node)).IsNotNull();

            var ancestors = node.GetAncestors().Bind(c =>
            {
                var a = c.FindDelegate().IfNone(c);

                return DependencyType.IsInstanceOfType(a)
                    ? Some(a)
                    : Some(a).OfType<IGameNodeFactory>().Bind(f => f.Service.ToOption());
            });

            return EnumerableHelper.OfType(ancestors.Freeze(), DependencyType);
        }
    }
}
