using System;
using System.Reactive.Disposables;
using System.Reactive.Subjects;
using AlleyCat.Common;
using AlleyCat.Game;
using AlleyCat.Logging;
using EnsureThat;
using LanguageExt;
using LanguageExt.ClassInstances;
using Microsoft.Extensions.Logging;
using static System.Reactive.Linq.Observable;
using static LanguageExt.Prelude;

namespace AlleyCat.Attribute
{
    public abstract class Attribute : GameObject, IAttribute
    {
        public bool Active
        {
            get => _active.Value;
            set => _active.OnNext(value);
        }

        public IObservable<bool> OnActiveStateChange => _active.AsObservable();

        public string Key { get; }

        public string DisplayName { get; }

        public Option<string> Description { get; }

        public float Value { get; private set; }

        public Option<IAttribute> Min { get; }

        public Option<IAttribute> Max { get; }

        public Option<IAttribute> Modifier { get; }

        public virtual IObservable<float> OnChange { get; }

        public virtual IObservable<float> OnModifierChange =>
            Modifier.Select(m => m.OnChange.StartWith(m.Value)).IfNone(Return(1f));

        public virtual IObservable<Range<float>> OnRangeChange
        {
            get
            {
                var onMinChange = Min.Select(a => a.OnChange.StartWith(a.Value)).IfNone(Return(float.MinValue));
                var onMaxChange = Max.Select(a => a.OnChange.StartWith(a.Value)).IfNone(Return(float.MaxValue));

                return onMinChange.CombineLatest(onMaxChange,
                    (ms, mx) => new Range<float>(ms, mx, TFloat.Inst));
            }
        }

        private readonly BehaviorSubject<bool> _active;

        private readonly BehaviorSubject<IObservable<float>> _source;

        private readonly CompositeDisposable _subscriptions = new CompositeDisposable();

        protected Attribute(
            string key,
            string displayName,
            Option<string> description,
            Option<IAttribute> min,
            Option<IAttribute> max,
            Option<IAttribute> modifier,
            bool active,
            ILoggerFactory loggerFactory) : base(loggerFactory)
        {
            Ensure.That(key, nameof(key)).IsNotNull();
            Ensure.That(displayName, nameof(displayName)).IsNotNull();

            Key = key;
            DisplayName = displayName;
            Description = description;

            Min = min;
            Max = max;
            Modifier = modifier;

            _active = CreateSubject(active);
            _source = CreateSubject(Empty<float>());

            var publisher = _source.Switch().Publish().AutoConnect(onConnect: _subscriptions.Add);

            OnChange = publisher.AsObservable();
        }

        public virtual void Initialize(IAttributeSet attributes)
        {
            Ensure.That(attributes, nameof(attributes)).IsNotNull();

            Modifier.Iter(m => m.Initialize(attributes));

            Min.Iter(m => m.Initialize(attributes));
            Max.Iter(m => m.Initialize(attributes));

            var source = CreateObservable(attributes).Where(_ => Active).DistinctUntilChanged();

            _source.OnNext(source);
        }

        protected override void PostConstruct()
        {
            base.PostConstruct();

            OnChange
                .TakeUntil(Disposed.Where(identity))
                .Subscribe(v => Value = v, this);
        }

        protected override void PreDestroy()
        {
            base.PreDestroy();

            _subscriptions.DisposeQuietly();
        }

        protected abstract IObservable<float> CreateObservable(IAttributeSet attributes);
    }
}
