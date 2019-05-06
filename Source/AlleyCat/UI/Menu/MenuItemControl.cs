using System;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using AlleyCat.Autowire;
using AlleyCat.Event;
using AlleyCat.Logging;
using Godot;
using JetBrains.Annotations;
using LanguageExt;
using Microsoft.Extensions.Logging;
using static LanguageExt.Prelude;

namespace AlleyCat.UI.Menu
{
    public class MenuItemControl : HBoxContainer, ILoggable
    {
        [Ancestor(true)]
        protected IMenu Parent { get; private set; }

        [Node("Label", true)]
        protected Label LabelText { get; private set; }

        [Node("Shortcut", true)]
        protected Label ShortcutText { get; private set; }

        public Option<IMenuItem> Model
        {
            get => _model.Value;
            set => _model.OnNext(value);
        }

        public IObservable<Option<IMenuItem>> OnModelChange => _model.AsObservable();

        public Option<char> Shortcut
        {
            get => _shortcut.Value;
            set => _shortcut.OnNext(value);
        }

        public IObservable<Option<char>> OnShortcutChange => _shortcut.AsObservable();

        [Service, CanBeNull]
        public ILogger Logger { get; private set; }

        private readonly BehaviorSubject<Option<IMenuItem>> _model;

        private readonly BehaviorSubject<Option<char>> _shortcut;

        public MenuItemControl()
        {
            _model = new BehaviorSubject<Option<IMenuItem>>(None);
            _shortcut = new BehaviorSubject<Option<char>>(None);
        }

        public override void _Ready()
        {
            base._Ready();

            this.Autowire();
        }

        [PostConstruct]
        private void PostConstruct()
        {
            OnModelChange
                .Subscribe(OnModelChanged, this);

            OnShortcutChange
                .Select(s => s.Map(v => v.ToString()).IfNone("?"))
                .Subscribe(ShortcutText.SetText, this);
        }

        protected virtual void OnModelChanged(Option<IMenuItem> item)
        {
            LabelText.Text = item.Map(i => i.DisplayName).IfNone("");
        }

        public override void _UnhandledKeyInput(InputEventKey @event)
        {
            base._UnhandledKeyInput(@event);

            if (Shortcut.Map(v => (int) v).Contains(@event.Scancode) && @event.Pressed && !@event.Echo)
            {
                Parent.Navigate(Model);
            }
        }

        protected override void Dispose(bool disposing)
        {
            _model.CompleteAndDispose();
            _shortcut.CompleteAndDispose();

            base.Dispose(disposing);
        }
    }
}
