using AlleyCat.Autowire;
using Godot;

namespace AlleyCat.Animation
{
    [Singleton(typeof(IAnimationManager), typeof(IAnimationStateManager))]
    public class AnimationStateManager : AnimationManager, IAnimationStateManager
    {
        [Service]
        public AnimationTree AnimationTree { get; private set; }

        public string Path => string.Empty;

        public AnimationRootNode Root => _graph?.Root;

        protected AnimationGraphContext Context { get; private set; }

        [Service(false)] private IAnimationGraphFactory _graphFactory;

        [Service(false)] private IAnimationControlFactory _controlFactory;

        private AnimationGraph _graph;

        protected override void OnInitialize()
        {
            base.OnInitialize();

            AnimationTree.ProcessMode = AnimationTree.AnimationProcessMode.Manual;

            if (_graphFactory == null)
            {
                _graphFactory = new AnimationGraphFactory();
            }

            if (_controlFactory == null)
            {
                _controlFactory = new AnimationControlFactory();
            }

            Context = new AnimationGraphContext(
                Player, AnimationTree, OnAdvance, _graphFactory, _controlFactory);

            _graph = _graphFactory.Create((AnimationRootNode) AnimationTree.TreeRoot, Context);
        }

        public AnimationNode GetAnimationNode(string name) => _graph?.GetAnimationNode(name);

        public IAnimationGraph GetGraph(string name) => _graph?.GetGraph(name);

        public IAnimationControl GetControl(string name) => _graph?.GetControl(name);

        protected override void ProcessFrames(float delta) => AnimationTree.Advance(delta);
    }
}
