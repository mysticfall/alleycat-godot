using AlleyCat.Autowire;
using EnsureThat;
using Godot;
using LanguageExt;
using static LanguageExt.Prelude;

namespace AlleyCat.Animation
{
    [Singleton(typeof(IAnimationManager), typeof(IAnimationStateManager))]
    public class AnimationStateManager : AnimationManager, IAnimationStateManager
    {
        public AnimationTree AnimationTree => (AnimationTree) _animationTree;

        public string Path => string.Empty;

        public AnimationRootNode Root => (AnimationRootNode) _graph.Map(g => g.Root);

        protected AnimationGraphContext Context => (AnimationGraphContext) _context;

        [Service] private Option<AnimationTree> _animationTree = None;

        [Service(false)] private Option<IAnimationGraphFactory> _graphFactory = None;

        [Service(false)] private Option<IAnimationControlFactory> _controlFactory = None;

        private Option<AnimationGraphContext> _context = None;

        private Option<IAnimationGraph> _graph = None;

        protected override void OnInitialize()
        {
            base.OnInitialize();

            AnimationTree.ProcessMode = AnimationTree.AnimationProcessMode.Manual;

            _graphFactory |= new AnimationGraphFactory();
            _controlFactory |= new AnimationControlFactory();

            _context = from graphFactory in _graphFactory
                from controlFactory in _controlFactory
                select new AnimationGraphContext(
                    Player, AnimationTree, OnAdvance, graphFactory, controlFactory);

            _graph = _graphFactory.Bind(
                f => f.TryCreate((AnimationRootNode) AnimationTree.TreeRoot, Context));
        }

        public Option<AnimationNode> FindAnimationNode(string name)
        {
            Ensure.That(name, nameof(name)).IsNotNull();

            return _graph.Bind(g => g.FindAnimationNode(name));
        }

        public Option<IAnimationGraph> FindGraph(string name)
        {
            Ensure.That(name, nameof(name)).IsNotNull();

            return _graph.Bind(g => g.FindGraph(name));
        }

        public Option<IAnimationControl> FindControl(string name)
        {
            Ensure.That(name, nameof(name)).IsNotNull();

            return _graph.Bind(g => g.FindControl(name));
        }

        protected override void ProcessFrames(float delta) => AnimationTree.Advance(delta);
    }
}
