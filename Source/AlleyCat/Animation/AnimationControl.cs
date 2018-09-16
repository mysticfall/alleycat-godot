using System.Collections.Generic;
using System.Linq;
using EnsureThat;
using Godot;
using JetBrains.Annotations;

namespace AlleyCat.Animation
{
    public abstract class AnimationControl : IAnimationControl
    {
        protected AnimationGraphContext Context { get; }

        protected AnimationControl([NotNull] AnimationGraphContext context)
        {
            Ensure.Any.IsNotNull(context, nameof(context));

            Context = context;
        }

        public abstract void Dispose();

        protected static IEnumerable<NodePath> FindTransformTracks(Godot.Animation animation)
        {
            var tracks = animation.GetTrackCount();

            return Enumerable
                .Range(0, tracks)
                .Select(i => (path: animation.TrackGetPath(i), type: animation.TrackGetType(i)))
                .Where(t => t.type == Godot.Animation.TrackType.Transform)
                .Select(t => t.path);
        }
    }
}
