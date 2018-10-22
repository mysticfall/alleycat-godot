using System;
using AlleyCat.Common;
using Godot;

namespace AlleyCat.Animation
{
    public interface IAnimationManager : IActivatable
    {
        AnimationPlayer Player { get; }

        IObservable<float> OnAdvance { get; }

        IObservable<AnimationEvent> OnAnimationEvent { get; }

        void Advance(float delta);

        void Play(Godot.Animation animation);
    }
}
