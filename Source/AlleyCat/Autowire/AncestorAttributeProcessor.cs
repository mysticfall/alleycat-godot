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
                DependencyType.IsInstanceOfType(c)
                    ? Some(c)
                    : Some(c).OfType<IGameObjectFactory>().Bind(f => f.Service.ToOption())).Freeze();

            return EnumerableHelper.OfType(ancestors, DependencyType);
        }
    }
}
