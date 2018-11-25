using System;
using AlleyCat.Common;
using AlleyCat.Logging;
using EnsureThat;
using Godot;
using LanguageExt;
using Microsoft.Extensions.Logging;
using static LanguageExt.Prelude;

namespace AlleyCat.Animation
{
    public abstract class AnimationGraph : IAnimationGraph, ILoggable, IDisposableCollector
    {
        public string Key { get; }

        public AnimationRootNode Root { get; }

        public ILogger Logger { get; }

        protected AnimationGraphContext Context { get; }

        private Map<string, IAnimationGraph> _children = Map<string, IAnimationGraph>();

        private Map<string, IAnimationControl> _controls = Map<string, IAnimationControl>();

        private Lst<IDisposable> _disposables = Lst<IDisposable>.Empty;

        protected AnimationGraph(string path, AnimationRootNode root, AnimationGraphContext context)
        {
            Ensure.That(path, nameof(path)).IsNotNull();
            Ensure.That(root, nameof(root)).IsNotNull();
            Ensure.That(context, nameof(context)).IsNotNull();

            Key = path;
            Root = root;
            Context = context;
            Logger = context.LoggerFactory.CreateLogger(this.GetLogCategory());

            this.LogDebug("Created animation graph.");
        }

        public abstract Option<AnimationNode> FindAnimationNode(string name);

        public Option<IAnimationGraph> FindGraph(string name)
        {
            var result = _children.Find(name);

            if (result) return result;

            result = Context.GraphFactory.TryCreate(name, this, Context);

            result.Iter(c => _children = _children.Add(name, c));

            return result;
        }

        public Option<IAnimationControl> FindControl(string name)
        {
            var result = _controls.Find(name);

            if (result) return result;

            result = Context.ControlFactory.TryCreate(name, this, Context);

            result.Iter(c =>
            {
                _controls = _controls.Add(name, c);
            });

            return result;
        }

        public void Collect(IDisposable disposable)
        {
            Ensure.That(disposable, nameof(disposable)).IsNotNull();

            _disposables += disposable;
        }

        public virtual void Dispose()
        {
            this.LogDebug("Disposing animation graph.");

            _children.Values.Iter(c => c.DisposeQuietly());
            _children = _children.Clear();

            _controls.Values.Iter(c => c.DisposeQuietly());
            _controls = _controls.Clear();

            _disposables.Iter(d => d.DisposeQuietly());
            _disposables = _disposables.Clear();
        }
    }
}
