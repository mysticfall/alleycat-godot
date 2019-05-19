using System;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using AlleyCat.Event;
using AlleyCat.Logging;
using EnsureThat;
using Godot;
using LanguageExt;
using Microsoft.Extensions.Logging;
using static LanguageExt.Prelude;

namespace AlleyCat.UI.Menu
{
    public class MenuItem : UIControl
    {
        protected IMenu Parent { get; }

        protected Label Label { get; }

        protected Option<Label> ShortcutLabel { get; }

        public Option<IMenuModel> Model
        {
            get => _model.Value;
            set => _model.OnNext(value);
        }

        public IObservable<Option<IMenuModel>> OnModelChange => _model.AsObservable();

        public Option<char> Shortcut
        {
            get => _shortcut.Value;
            set => _shortcut.OnNext(value);
        }

        public IObservable<Option<char>> OnShortcutChange => _shortcut.AsObservable();

        public IObservable<Unit> OnAction { get; }

        private readonly BehaviorSubject<Option<IMenuModel>> _model;

        private readonly BehaviorSubject<Option<char>> _shortcut;

        public MenuItem(
            IMenu parent,
            Label label,
            Option<Label> shortcutLabel,
            Godot.Control node,
            ILoggerFactory loggerFactory) : base(node, loggerFactory)
        {
            Ensure.That(parent, nameof(parent)).IsNotNull();
            Ensure.That(label, nameof(label)).IsNotNull();

            Parent = parent;
            Label = label;
            ShortcutLabel = shortcutLabel;

            _model = CreateSubject<Option<IMenuModel>>(None);
            _shortcut = CreateSubject<Option<char>>(None);

            OnAction = Node.OnUnhandledInput()
                .OfType<InputEventKey>()
                .Where(e => e.Pressed && !e.IsEcho())
                .Where(e => Shortcut.Map(v => (int) v).Contains(e.Scancode))
                .Do(_ => Node.GetTree().SetInputAsHandled())
                .AsUnitObservable();
        }

        protected override void PostConstruct()
        {
            base.PostConstruct();

            var disposed = Disposed.Where(identity);

            OnModelChange
                .TakeUntil(disposed)
                .Subscribe(OnModelChanged, this);

            OnAction
                .TakeUntil(disposed)
                .Subscribe(_ => Parent.Navigate(Model), this);

            ShortcutLabel.Iter(label =>
            {
                OnShortcutChange
                    .Select(s => s.Map(v => v.ToString()).IfNone("?"))
                    .TakeUntil(disposed)
                    .Subscribe(label.SetText, this);
            });
        }

        protected virtual void OnModelChanged(Option<IMenuModel> item)
        {
            Label.Text = item.Map(i => i.DisplayName).IfNone("");
        }
    }
}
