using EnsureThat;
using Godot;
using JetBrains.Annotations;

namespace AlleyCat.Animation
{
    public class BlendTree : AnimationGraph
    {
        public new AnimationNodeBlendTree Root { get; }

        public BlendTree(
            string path,
            AnimationNodeBlendTree root,
            AnimationGraphContext context) : base(path, root, context)
        {
            Root = root;
        }

        public override AnimationNode GetAnimationNode(string name)
        {
            Ensure.Any.IsNotNull(name, nameof(name));

            return Root.HasNode(name) ? Root.GetNode(name) : null;
        }
    }

    public static class BlendTreeExtensions
    {
        [CanBeNull]
        public static BlendTree GetBlendTree(
            [NotNull] this IAnimationGraph graph, [NotNull] string path)
        {
            Ensure.Any.IsNotNull(graph, nameof(graph));
            Ensure.Any.IsNotNull(path, nameof(path));

            return graph.GetDescendantGraph(path) as BlendTree;
        }
    }
}
