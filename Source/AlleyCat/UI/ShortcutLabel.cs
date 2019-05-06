using System;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using AlleyCat.Autowire;
using AlleyCat.Common;
using AlleyCat.Control;
using AlleyCat.Event;
using AlleyCat.Logging;
using Godot;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;

namespace AlleyCat.UI
{
    public class ShortcutLabel : HBoxContainer, IActivatable, ILoggable
    {
        [Export]
        public bool Active
        {
            get => _active.Value;
            set => _active.OnNext(value);
        }

        public IObservable<bool> OnActiveStateChange => _active.AsObservable();

        [Export]
        public string Label
        {
            get => _label.Value;
            set => _label.OnNext(value);
        }

        [Export]
        public string Action
        {
            get => _action.Value;
            set => _action.OnNext(value);
        }

        public IObservable<string> OnAction => _press.AsObservable();

        [Node("Label", true)]
        protected Label LabelText { get; private set; }

        [Node("Shortcut", true)]
        protected Label ShortcutText { get; private set; }

        [Service, CanBeNull]
        public ILogger Logger { get; private set; }

        private readonly BehaviorSubject<string> _label;

        private readonly BehaviorSubject<string> _action;

        private readonly BehaviorSubject<bool> _active;

        private readonly Subject<string> _press;

        public ShortcutLabel()
        {
            _label = new BehaviorSubject<string>("Label");
            _action = new BehaviorSubject<string>("ui_accept");
            _active = new BehaviorSubject<bool>(true);

            _press = new Subject<string>();
        }

        public override void _Ready()
        {
            base._Ready();

            this.Autowire();
        }

        [PostConstruct]
        private void PostConstruct()
        {
            OnActiveStateChange
                .Do(SetVisible)
                .Do(SetProcessUnhandledInput)
                .Subscribe(this);

            string FindShortcut(string action) => InputMap
                .GetActionList(action)
                .OfType<InputEvent>()
                .Bind(e => e.FindKeyLabel())
                .HeadOrNone()
                .IfNone("?");

            _action
                .Select(FindShortcut)
                .Subscribe(ShortcutText.SetText, this);
            _label
                .Select(Tr)
                .Subscribe(LabelText.SetText, this);
        }

        public override void _UnhandledInput(InputEvent @event)
        {
            base._UnhandledInput(@event);

            if (@event.IsActionPressed(Action))
            {
                _press.OnNext(Action);

                GetTree().SetInputAsHandled();
            }
        }

        protected override void Dispose(bool disposing)
        {
            _label.CompleteAndDispose();
            _action.CompleteAndDispose();
            _active.CompleteAndDispose();

            _press.CompleteAndDispose();

            base.Dispose(disposing);
        }
    }
}
