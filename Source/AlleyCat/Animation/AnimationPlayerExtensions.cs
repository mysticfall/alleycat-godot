using System;
using System.Diagnostics;
using AlleyCat.Common;
using EnsureThat;
using Godot;
using LanguageExt;
using static LanguageExt.Prelude;

namespace AlleyCat.Animation
{
    public static class AnimationPlayerExtensions
    {
        private const string NodeName = "AnimationPlayerEventTracker";

        public static string AddAnimation(this AnimationPlayer player, Godot.Animation animation)
        {
            Ensure.That(player, nameof(player)).IsNotNull();
            Ensure.That(animation, nameof(animation)).IsNotNull();

            var name = animation.GetKey();

            Debug.Assert(name != null, "name != null");

            if (!player.HasAnimation(name))
            {
                player.AddAnimation(name, animation).ThrowOnError();
            }

            return name;
        }

        public static Option<Godot.Animation> FindAnimation(this AnimationPlayer player, string name)
        {
            Ensure.That(player, nameof(player)).IsNotNull();
            Ensure.That(name, nameof(name)).IsNotNull();

            return Optional(player.GetAnimation(name));
        }

        public static IObservable<AnimationChangeEvent> OnAnimationChange(this AnimationPlayer player)
        {
            Ensure.That(player, nameof(player)).IsNotNull();

            return player.GetComponent(NodeName, _ => new AnimationPlayerEventTracker()).OnAnimationChange;
        }

        public static IObservable<AnimationStartEvent> OnAnimationStart(this AnimationPlayer player)
        {
            Ensure.That(player, nameof(player)).IsNotNull();

            return player.GetComponent(NodeName, _ => new AnimationPlayerEventTracker()).OnAnimationStart;
        }

        public static IObservable<AnimationFinishEvent> OnAnimationFinish(this AnimationPlayer player)
        {
            Ensure.That(player, nameof(player)).IsNotNull();

            return player.GetComponent(NodeName, _ => new AnimationPlayerEventTracker()).OnAnimationFinish;
        }
    }
}
