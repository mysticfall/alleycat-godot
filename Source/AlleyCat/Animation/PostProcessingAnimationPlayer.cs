using System.Collections.Generic;
using Godot;
using JetBrains.Annotations;

namespace AlleyCat.Animation
{
    public class PostProcessingAnimationPlayer : AnimationPlayer
    {
        [NotNull]
        public ICollection<IAnimationPostProcessor> Processors { get; }

        private bool _runInPhysicsLoop;

        public PostProcessingAnimationPlayer()
        {
            Processors = new List<IAnimationPostProcessor>();
        }

        public override void _Ready()
        {
            base._Ready();

            PlaybackActive = false;

            _runInPhysicsLoop = PlaybackProcessMode == AnimationProcessMode.Physics;
        }

        public override void _ExitTree()
        {
            base._ExitTree();

            Processors.Clear();
        }

        public override void _Process(float delta)
        {
            base._Process(delta);

            if (!_runInPhysicsLoop)
            {
                ProcessFrame(delta);
            }
        }

        public override void _PhysicsProcess(float delta)
        {
            base._PhysicsProcess(delta);

            if (_runInPhysicsLoop)
            {
                ProcessFrame(delta);
            }
        }

        private void ProcessFrame(float delta)
        {
            BeforeFrame();

            Advance(delta);

            AfterFrame(delta);
        }

        public void BeforeFrame()
        {
            foreach (var processor in Processors)
            {
                processor.BeforeFrame(this);
            }
        }

        public void AfterFrame(float delta)
        {
            foreach (var processor in Processors)
            {
                processor.AfterFrame(this, delta);
            }
        }
    }
}
