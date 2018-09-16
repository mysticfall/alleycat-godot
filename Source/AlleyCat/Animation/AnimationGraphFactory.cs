using System;
using EnsureThat;
using Godot;

namespace AlleyCat.Animation
{
    public class AnimationGraphFactory : IAnimationGraphFactory
    {
        public AnimationGraph Create(
            string name, IAnimationGraph parent, AnimationGraphContext context)
        {
            Ensure.Any.IsNotNull(name, nameof(name));
            Ensure.Any.IsNotNull(parent, nameof(parent));
            Ensure.Any.IsNotNull(context, nameof(context));

            if (!(parent?.GetAnimationNode(name) is AnimationRootNode node)) return null;

            var path = string.IsNullOrEmpty(parent.Path) ? name : string.Join("/", parent.Path, name);

            return Create(path, node, context);
        }

        public AnimationGraph Create(AnimationRootNode node, AnimationGraphContext context)
        {
            Ensure.Any.IsNotNull(node, nameof(node));
            Ensure.Any.IsNotNull(context, nameof(context));

            return Create(string.Empty, node, context);
        }

        protected virtual AnimationGraph Create(
            string path, AnimationRootNode node, AnimationGraphContext context)
        {
            switch (node)
            {
                case AnimationNodeStateMachine states:
                    return new AnimationStates(path, states, context);
                case AnimationNodeBlendTree blendTree:
                    return new BlendTree(path, blendTree, context);
                default:
                    throw new ArgumentOutOfRangeException($"Unknown root node type: '{node.GetType()}'.");
            }
        }
    }
}
