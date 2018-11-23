using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using AlleyCat.Character;
using AlleyCat.Common;
using AlleyCat.Control;
using AlleyCat.Event;
using AlleyCat.Game;
using AlleyCat.Physics;
using Godot;
using LanguageExt;
using Microsoft.Extensions.Logging;
using Array = Godot.Collections.Array;

namespace AlleyCat.View
{
    public class OrbitingCharacterView : OrbitingView, IThirdPersonView
    {
        public virtual Option<IHumanoid> Character
        {
            get => _character.Value;
            set => _character.OnNext(value);
        }

        public IObservable<Option<IHumanoid>> OnCharacterChange => _character.AsObservable();

        public Option<IEntity> FocusedObject { get; private set; }

        public IObservable<Option<IEntity>> OnFocusChange { get; }

        public float MaxFocalDistance
        {
            get => _maxFocalDistance;
            set => _maxFocalDistance = Mathf.Max(value, 0);
        }

        public override Spatial Target => Camera;

        public override bool Valid => base.Valid && Character.IsSome;

        public bool AutoActivate => true;

        public override Vector3 Origin => Character.Map(c => c.Vision.Head.origin).IfNone(Vector3.Zero);

        public override Vector3 Up => Vector3.Up;

        public override Vector3 Forward => Character
            .Map(c => c.GetGlobalTransform().Forward())
            .Map(new Plane(Vector3.Up, 0f).Project)
            .IfNone(Vector3.Forward);

        private float _maxFocalDistance = 2f;

        private readonly BehaviorSubject<Option<IHumanoid>> _character;

        public OrbitingCharacterView(
            Camera camera,
            Option<IHumanoid> character,
            Option<IInputBindings> rotationInput,
            Option<IInputBindings> zoomInput,
            Range<float> yawRange,
            Range<float> pitchRange,
            Range<float> distanceRange,
            float initialDistance,
            Vector3 initialOffset,
            ProcessMode processMode,
            ITimeSource timeSource,
            bool active,
            ILogger logger) : base(
            camera,
            rotationInput,
            zoomInput,
            yawRange,
            pitchRange,
            distanceRange,
            initialDistance,
            initialOffset,
            processMode,
            timeSource,
            active,
            logger)
        {
            OnFocusChange = timeSource.OnPhysicsProcess
                .Where(_ => Active && Valid)
                .Select(_ => (Origin - Camera.GlobalTransform.origin).Normalized())
                .Select(direction => Origin + direction * MaxFocalDistance)
                .Select(to => Character
                    .Map(c => new Array {c.Spatial})
                    .Bind(v => Camera.GetWorld().IntersectRay(Origin, to, v)))
                .Select(hit => hit.Bind(h => h.Collider.FindEntity()))
                .Select(e => e.Filter(v => v.Valid && v.Visible))
                .DistinctUntilChanged()
                .Do(current => FocusedObject = current);

            _character = new BehaviorSubject<Option<IHumanoid>>(character).DisposeWith(this);
        }

        protected override void PostConstruct()
        {
            base.PostConstruct();

            ZoomInput
                .Where(_ => Distance <= DistanceRange.Min)
                .Where(v => v > 0)
                .Subscribe(_ => this.Deactivate())
                .DisposeWith(this);
        }
    }
}
