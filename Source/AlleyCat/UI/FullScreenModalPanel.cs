using AlleyCat.Autowire;
using AlleyCat.Common;
using AlleyCat.Control;
using AlleyCat.Logging;
using Godot;
using JetBrains.Annotations;
using LanguageExt;
using LanguageExt.UnsafeValueAccess;
using Microsoft.Extensions.Logging;

namespace AlleyCat.UI
{
    public class FullScreenModalPanel : Panel, ILoggable
    {
        public Option<string> CloseAction
        {
            get => _closeAction.TrimToOption();
            set => _closeAction = value.ValueUnsafe();
        }

        [Service, CanBeNull]
        public ILogger Logger { get; private set; }

        protected IPlayerControl PlayerControl => _playerControl.Head();

        [Export]
        public bool PauseWhenVisible { get; set; } = true;

        [Export] private string _closeAction = "ui_cancel";

        [Service] private Option<IPlayerControl> _playerControl;

        private bool _initialActiveState;

        public override void _Ready()
        {
            base._Ready();

            this.Autowire();

            if (PauseWhenVisible)
            {
                GetTree().Paused = true;
            }
            else
            {
                _initialActiveState = PlayerControl.Active;

                PlayerControl.Active = false;
            }

            Input.SetMouseMode(Input.MouseMode.Visible);
        }

        [PostConstruct]
        protected virtual void PostConstruct()
        {
        }

        public void Resume()
        {
            if (PauseWhenVisible)
            {
                GetTree().Paused = false;
            }
            else
            {
                PlayerControl.Active = _initialActiveState;
            }

            Hide();
            QueueFree();

            Input.SetMouseMode(Input.MouseMode.Captured);
        }

        public override void _UnhandledInput(InputEvent @event)
        {
            base._UnhandledInput(@event);

            if (CloseAction.Exists(@event.IsActionPressed))
            {
                Resume();
                GetTree().SetInputAsHandled();
            }
        }
    }
}
