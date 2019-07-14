using System;
using System.Reactive.Disposables;
using System.Reactive.Subjects;
using AlleyCat.Common;
using AlleyCat.Game;
using AlleyCat.Logging;
using EnsureThat;
using Godot;
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

        public virtual IObservable<float> OnChange => _onChange.StartWith(Value);

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

        public Map<string, IAttribute> Children { get; }

        private readonly BehaviorSubject<bool> _active;

        private readonly BehaviorSubject<IObservable<float>> _source;

        private readonly CompositeDisposable _subscriptions = new CompositeDisposable();

        private readonly Option<Texture> _icon;

        private readonly IObservable<float> _onChange;

        protected Attribute(
            string key,
            string displayName,
            Option<string> description,
            Option<Texture> icon,
            Map<string, IAttribute> children,
            bool active,
            ILoggerFactory loggerFactory) : base(loggerFactory)
        {
            Ensure.That(key, nameof(key)).IsNotNull();
            Ensure.That(displayName, nameof(displayName)).IsNotNull();

            Key = key;
            DisplayName = displayName;
            Description = description;
            Children = children;

            if (Logger.IsEnabled(LogLevel.Debug))
            {
                Logger.LogDebug("Found children: {}", string.Join(", ", Children.Values));
            }

            Min = Children.Find("Min");
            Max = Children.Find("Max");
            Modifier = Children.Find("Modifier");

            _icon = icon;

            _active = CreateSubject(active);
            _source = CreateSubject(Empty<float>());

            _onChange = _source
                .Switch()
                .Publish()
                .AutoConnect(onConnect: _subscriptions.Add)
                .AsObservable();
        }

        public virtual void Initialize(IAttributeHolder holder)
        {
            Ensure.That(holder, nameof(holder)).IsNotNull();

            InitializeChildren(holder);

            var source = CreateObservable(holder).Where(_ => Active);

            _source.OnNext(source);
        }

        protected virtual void InitializeChildren(IAttributeHolder holder) => Children.Iter(d => d.Initialize(holder));

        public Option<Texture> FindIcon(int sizeHint) => _icon;

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

        protected abstract IObservable<float> CreateObservable(IAttributeHolder holder);
    }
}
