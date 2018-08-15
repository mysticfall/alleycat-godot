using System;
using AlleyCat.Common;
using Godot;
using JetBrains.Annotations;

namespace AlleyCat.Animation
{
    public interface IAnimationManager : IActivatable
    {
        AnimationPlayer Player { get; }

        IObservable<float> OnAdvance { get; }

        IObservable<AnimationEvent> OnAnimationEvent { get; }

        void Play([NotNull] Godot.Animation animation);
    }
}
