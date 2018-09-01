using System;
using System.Diagnostics;
using System.Linq;
using AlleyCat.Autowire;
using AlleyCat.Common;
using EnsureThat;
using Godot;
using JetBrains.Annotations;
using Gen = System.Collections.Generic;

namespace AlleyCat.Animation
{
    [Singleton(typeof(IAnimationManager), typeof(IAnimationStateManager))]
    public class AnimationStateManager : AnimationManager, IAnimationStateManager
    {
        [Service]
        public AnimationTree AnimationTree { get; private set; }

        public AnimationNodeStateMachine States
        {
            get
            {
                var current = TransitionNode.GetInputConnection(TransitionNode.Current);

                return RootNode?.GetNode(current) as AnimationNodeStateMachine;
            }
        }

        public Gen.IReadOnlyDictionary<string, AnimationBlender> Blenders =>
            _blenderMap ?? Enumerable.Empty<AnimationBlender>().ToDictionary(i => i.Key);

        protected AnimationNodeBlendTree RootNode { get; private set; }

        protected AnimationNodeTransition TransitionNode { get; private set; }

        protected AnimationNodeAnimation ActionNode =>
            _actionNode != null ? States.GetNode(_actionNode) as AnimationNodeAnimation : null;

        [Export, UsedImplicitly] private string _actionNode = "Act";

        [Export, UsedImplicitly] private string _transitionNode = "Transition";

        private Gen.IEnumerable<AnimationBlender> _blenders;

        private Gen.IReadOnlyDictionary<string, AnimationBlender> _blenderMap;

        protected override void OnInitialize()
        {
            base.OnInitialize();

            RootNode = (AnimationNodeBlendTree) AnimationTree?.TreeRoot;

            Debug.Assert(RootNode != null, "RootNode != null");

            TransitionNode = RootNode.GetNode(_transitionNode) as AnimationNodeTransition;

            Debug.Assert(TransitionNode != null, "TransitionNode != null");

            var output = RootNode.GetNode("output");

            Debug.Assert(output != null, "output != null");

            _blenders = CreateBlenders(output);
            _blenderMap = _blenders.ToDictionary();

            OnActiveStateChange
                .Subscribe(AnimationTree.SetActive)
                .AddTo(this);
        }

        public override void Play(Godot.Animation animation)
        {
            Ensure.Any.IsNotNull(animation, nameof(animation));

            var name = animation.GetName();

            if (!Player.HasAnimation(name))
            {
                Player.AddAnimation(name, animation).ThrowIfNecessary();

                AnimationTree.InvalidateCaches();
            }

            ActionNode?.SetAnimation(name);

            States.Travel(_actionNode);
        }

        protected override void ProcessLoop(float delta)
        {
            base.ProcessLoop(delta);

            if (!Active || _blenders == null) return;

            foreach (var blender in _blenders)
            {
                blender.Process(delta);
            }
        }

        protected virtual Gen.IEnumerable<AnimationBlender> CreateBlenders(AnimationNode output)
        {
            var sources = new Gen.List<AnimationBlender>();
            var parent = (AnimationNodeBlendTree) output.GetParent();

            var node = output;

            while (true)
            {
                var name = node.GetInputConnection(0);
                var input = parent.GetNode(name);

                if (input is AnimationNodeBlend2 next)
                {
                    node = next;
                    sources.Add(new AnimationBlender(name, next));
                }
                else
                {
                    break;
                }
            }

            return sources;
        }
    }
}
