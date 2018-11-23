using EnsureThat;
using Godot;
using LanguageExt;
using static LanguageExt.Prelude;

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

        public override Option<AnimationNode> FindAnimationNode(string name)
        {
            Ensure.That(name, nameof(name)).IsNotNull();

            return Root.HasNode(name) ? Some(Root.GetNode(name)) : None;
        }
    }

    public static class BlendTreeExtensions
    {
        public static Option<BlendTree> FindBlendTree(this IAnimationGraph graph, string path)
        {
            Ensure.That(graph, nameof(graph)).IsNotNull();

            return graph.FindDescendantGraph<BlendTree>(path);
        }
    }
}
