using System;
using System.Collections.Generic;
using System.Linq;
using AlleyCat.Common;
using AlleyCat.Logging;
using EnsureThat;
using Godot;
using LanguageExt;
using Microsoft.Extensions.Logging;

namespace AlleyCat.Animation
{
    public abstract class AnimationControl : IAnimationControl, ILoggable, IDisposableCollector
    {
        public string Key { get; }

        public ILogger Logger { get; }

        protected AnimationGraphContext Context { get; }

        private Lst<IDisposable> _disposables = Lst<IDisposable>.Empty;

        protected AnimationControl(string key, AnimationGraphContext context)
        {
            Ensure.That(key, nameof(key)).IsNotNullOrEmpty();
            Ensure.That(context, nameof(context)).IsNotNull();

            Key = key;
            Context = context;
            Logger = context.LoggerFactory.CreateLogger(this.GetLogCategory());

            this.LogDebug("Created animation control.");
        }

        public void Collect(IDisposable disposable)
        {
            Ensure.That(disposable, nameof(disposable)).IsNotNull();

            _disposables += disposable;
        }

        public virtual void Dispose()
        {
            this.LogDebug("Disposing animation control.");

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
