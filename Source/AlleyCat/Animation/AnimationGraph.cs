using System;
using AlleyCat.Common;
using EnsureThat;
using Godot;
using LanguageExt;
using static LanguageExt.Prelude;

namespace AlleyCat.Animation
{
    public abstract class AnimationGraph : IAnimationGraph, IDisposableCollector
    {
        public string Path { get; }

        public AnimationRootNode Root { get; }

        protected AnimationGraphContext Context { get; }

        private Map<string, IAnimationGraph> _children = Map<string, IAnimationGraph>();

        private Map<string, IAnimationControl> _controls = Map<string, IAnimationControl>();

        private Lst<IDisposable> _disposables = Lst<IDisposable>.Empty;

        protected AnimationGraph(string path, AnimationRootNode root, AnimationGraphContext context)
        {
            Ensure.That(path, nameof(path)).IsNotNull();
            Ensure.That(root, nameof(root)).IsNotNull();
            Ensure.That(context, nameof(context)).IsNotNull();

            Path = path;
            Root = root;
            Context = context;
        }

        public abstract Option<AnimationNode> FindAnimationNode(string name);

        public Option<IAnimationGraph> FindGraph(string name)
        {
            Ensure.That(name, nameof(name)).IsNotNull();

            var result = _children.Find(name);

            if (result) return result;

            result = Context.GraphFactory.TryCreate(name, this, Context);

            result.Iter(c => _children = _children.Add(name, c));

            return result;
        }

        public Option<IAnimationControl> FindControl(string name)
        {
            Ensure.That(name, nameof(name)).IsNotNull();

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
            _children.Values.Iter(c => c.DisposeQuietly());
            _children = _children.Clear();

            _controls.Values.Iter(c => c.DisposeQuietly());
            _controls = _controls.Clear();

            _disposables.Iter(d => d.DisposeQuietly());
            _disposables = _disposables.Clear();
        }
    }
}
