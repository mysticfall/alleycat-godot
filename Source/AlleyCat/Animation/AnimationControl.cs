using System;
using System.Collections.Generic;
using System.Linq;
using AlleyCat.Common;
using EnsureThat;
using Godot;
using LanguageExt;

namespace AlleyCat.Animation
{
    public abstract class AnimationControl : IAnimationControl, IDisposableCollector
    {
        protected AnimationGraphContext Context { get; }

        private Lst<IDisposable> _disposables = Lst<IDisposable>.Empty;

        protected AnimationControl(AnimationGraphContext context)
        {
            Ensure.That(context, nameof(context)).IsNotNull();

            Context = context;
        }

        public void Collect(IDisposable disposable)
        {
            Ensure.That(disposable, nameof(disposable)).IsNotNull();

            _disposables += disposable;
        }

        public virtual void Dispose()
        {
            _disposables.Iter(d => d.DisposeQuietly());
            _disposables = _disposables.Clear();
        }

        protected static IEnumerable<NodePath> FindTransformTracks(Godot.Animation animation)
        {
            Ensure.That(animation, nameof(animation)).IsNotNull();

            return Enumerable
                .Range(0, animation.GetTrackCount())
                .Select(i => (path: animation.TrackGetPath(i), type: animation.TrackGetType(i)))
                .Where(t => t.type == Godot.Animation.TrackType.Transform)
                .Select(t => t.path);
        }
    }
}
