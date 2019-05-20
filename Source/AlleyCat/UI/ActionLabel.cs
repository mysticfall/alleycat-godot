using System;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using AlleyCat.Common;
using AlleyCat.Control;
using AlleyCat.Event;
using AlleyCat.Logging;
using EnsureThat;
using Godot;
using LanguageExt;
using Microsoft.Extensions.Logging;
using static LanguageExt.Prelude;

namespace AlleyCat.UI
{
    public class ActionLabel : UIControl, IActivatable
    {
        public bool Active
        {
            get => _active.Value;
            set => _active.OnNext(value);
        }

        public IObservable<bool> OnActiveStateChange => _active.AsObservable();

        public string Label
        {
            get => _label.Value;
            set => _label.OnNext(value);
        }

        public string Action
        {
            get => _action.Value;
            set => _action.OnNext(value);
        }

        public IObservable<Unit> OnAction { get; }

        protected Label TextLabel { get; }

        protected Option<Label> ShortcutLabel { get; }

        private readonly BehaviorSubject<string> _label;

        private readonly BehaviorSubject<string> _action;

        private readonly BehaviorSubject<bool> _active;

        public ActionLabel(
            Label textLabel,
            Option<Label> shortcutLabel,
            Godot.Control node,
            ILoggerFactory loggerFactory) : base(node, loggerFactory)
        {
            Ensure.That(textLabel, nameof(textLabel)).IsNotNull();

            TextLabel = textLabel;
            ShortcutLabel = shortcutLabel;

            _label = CreateSubject("Label");
            _action = CreateSubject("ui_accept");
            _active = CreateSubject(true);

            OnAction = Node.OnUnhandledInput()
                .Where(_ => Active && Valid)
                .Where(e => e.IsActionPressed(Action) && !e.IsEcho())
                .Do(_ => Node.GetTree().SetInputAsHandled())
                .AsUnitObservable();
        }

        protected override void PostConstruct()
        {
            base.PostConstruct();

            var disposed = Disposed.Where(identity);

            OnActiveStateChange
                .Do(v => Node.Visible = v)
                .Do(Node.SetProcessUnhandledInput)
                .TakeUntil(disposed)
                .Subscribe(this);

            _label
                .Select(Translate)
                .TakeUntil(disposed)
                .Subscribe(v => TextLabel.Text = v, this);

            ShortcutLabel.Iter(label =>
            {
                string FindShortcut(string action) => InputMap
                    .GetActionList(action)
                    .OfType<InputEvent>()
                    .Bind(e => e.FindKeyLabel())
                    .HeadOrNone()
                    .IfNone("?");

                _action
                    .Select(FindShortcut)
                    .TakeUntil(disposed)
                    .Subscribe(v => label.Text = v, this);
            });
        }
    }
}
