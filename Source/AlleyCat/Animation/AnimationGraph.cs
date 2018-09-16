using System.Collections.Generic;
using EnsureThat;
using Godot;
using JetBrains.Annotations;

namespace AlleyCat.Animation
{
    public abstract class AnimationGraph : IAnimationGraph
    {
        public string Path { get; }

        public AnimationRootNode Root { get; }

        protected AnimationGraphContext Context { get; }

        private readonly IDictionary<string, IAnimationGraph> _children;

        private readonly IDictionary<string, IAnimationControl> _controls;

        protected AnimationGraph(
            [NotNull] string path,
            [NotNull] AnimationRootNode root,
            [NotNull] AnimationGraphContext context)
        {
            Ensure.Any.IsNotNull(path, nameof(path));
            Ensure.Any.IsNotNull(root, nameof(root));
            Ensure.Any.IsNotNull(context, nameof(context));

            Path = path;
            Root = root;
            Context = context;

            _children = new Dictionary<string, IAnimationGraph>();
            _controls = new Dictionary<string, IAnimationControl>();
        }

        public abstract AnimationNode GetAnimationNode(string name);

        public IAnimationGraph GetGraph(string name)
        {
            IAnimationGraph child;

            if (!_children.ContainsKey(name))
            {
                child = Context.GraphFactory.Create(name, this, Context);

                _children.Add(name, child);
            }
            else
            {
                _children.TryGetValue(name, out child);
            }

            return child;
        }

        public IAnimationControl GetControl(string name)
        {
            IAnimationControl control;

            if (!_controls.ContainsKey(name))
            {
                control = Context.ControlFactory.Create(name, this, Context);

                _controls.Add(name, control);
            }
            else
            {
                _controls.TryGetValue(name, out control);
            }

            return control;
        }

        public virtual void Dispose()
        {
            foreach (var child in _children.Values)
            {
                child?.Dispose();
            }

            foreach (var control in _controls.Values)
            {
                control?.Dispose();
            }
        }
    }
}
