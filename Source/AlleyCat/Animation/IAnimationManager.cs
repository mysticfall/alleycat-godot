using System;
using System.Reactive;
using AlleyCat.Common;
using Godot;

namespace AlleyCat.Animation
{
    public interface IAnimationManager : IActivatable, IGameLoopAware
    {
        AnimationPlayer Player { get; }

        IObservable<Unit> OnBeforeAdvance { get; }

        IObservable<float> OnAdvance { get; }

        void Advance(float delta);
    }
}
