using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using AlleyCat.Common;
using AlleyCat.Event;
using EnsureThat;
using Godot;
using LanguageExt;
using static LanguageExt.Prelude;

namespace AlleyCat.Animation
{
    public static class AnimationPlayerExtensions
    {
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

            return Optional(player.GetAnimation(name));
        }

        public static IObservable<AnimationChangeEvent> OnAnimationChange(this AnimationPlayer player)
        {
            Option<ValueTuple<Option<string>, string>> GetArguments(IEnumerable<object> args)
            {
                return args.Map(v => v?.ToString()).Freeze().Match(
                    () => None,
                    _ => None,
                    (head, tail) => tail.HeadOrNone().Map(v => (Optional(head), v)));
            }

            return player.FromSignal("animation_started")
                .SelectMany(args => GetArguments(args).ToObservable())
                .Select(v => new AnimationChangeEvent(v.Item1, v.Item2, player));
        }

        public static IObservable<AnimationStartEvent> OnAnimationStart(this AnimationPlayer player)
        {
            return player.FromSignal("animation_started")
                .SelectMany(args => args.HeadOrNone().OfType<string>().ToObservable())
                .Select(v => new AnimationStartEvent(v, player));
        }

        public static IObservable<AnimationFinishEvent> OnAnimationFinish(this AnimationPlayer player)
        {
            return player.FromSignal("animation_finished")
                .SelectMany(args => args.HeadOrNone().OfType<string>().ToObservable())
                .Select(v => new AnimationFinishEvent(v, player));
        }
    }
}
