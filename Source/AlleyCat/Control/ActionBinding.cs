using System;
using System.Reactive.Linq;
using AlleyCat.Action;
using AlleyCat.Autowire;
using AlleyCat.Common;
using AlleyCat.Event;
using Godot;
using JetBrains.Annotations;

namespace AlleyCat.Control
{
    public class ActionBinding : AutowiredNode, IIdentifiable, IActivatable
    {
        public string Key => Action.Key;

        [Export]
        public bool Active
        {
            get => _active.Value;
            set => _active.Value = value;
        }

        public IObservable<bool> OnActiveStateChange => _active;

        [Node]
        public IAction Action { get; private set; }

        [Node]
        public ITriggerInput Input { get; private set; }

        [Node(required: false)]
        public IActor Actor { get; set; }

        [Export, UsedImplicitly] private NodePath _actor;

        private readonly ReactiveProperty<bool> _active = new ReactiveProperty<bool>(true);

        [PostConstruct]
        protected void OnInitialize()
        {
            Input
                .Where(v => v && Active)
                .Subscribe(_ => Action.Execute(Actor))
                .AddTo(this);

            OnActiveStateChange
                .Do(v => Action.Active = v)
                .Do(v => Input.Active = v)
                .Subscribe()
                .AddTo(this);
        }

        protected override void Dispose(bool disposing)
        {
            _active?.Dispose();

            base.Dispose(disposing);
        }
    }
}
