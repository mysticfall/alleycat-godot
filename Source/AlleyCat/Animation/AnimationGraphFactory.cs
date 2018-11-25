using EnsureThat;
using Godot;
using LanguageExt;
using static LanguageExt.Prelude;

namespace AlleyCat.Animation
{
    public class AnimationGraphFactory : IAnimationGraphFactory
    {
        public Option<IAnimationGraph> TryCreate(
            string name, IAnimationGraph parent, AnimationGraphContext context)
        {
            Ensure.That(parent, nameof(parent)).IsNotNull();

            return parent.FindAnimationNode<AnimationRootNode>(name).Bind(node =>
            {
                var path = string.IsNullOrEmpty(parent.Key) ? name : string.Join("/", parent.Key, name);

                return TryCreate(path, node, context);
            });
        }

        public Option<IAnimationGraph> TryCreate(AnimationRootNode node, AnimationGraphContext context) =>
            TryCreate(string.Empty, node, context);

        protected virtual Option<IAnimationGraph> TryCreate(
            string path, AnimationRootNode node, AnimationGraphContext context)
        {
            Ensure.That(node, nameof(node)).IsNotNull();

            switch (node)
            {
                case AnimationNodeStateMachine states:
                    return Some<IAnimationGraph>(new AnimationStates(path, states, context));
                case AnimationNodeBlendTree blendTree:
                    return Some<IAnimationGraph>(new BlendTree(path, blendTree, context));
                default:
                    return None;
            }
        }
    }
}
