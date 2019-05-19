using System.Reactive.Linq;
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
    public abstract class FullScreenModalPanel : UIControl
    {
        public Option<string> CloseAction { get; }

        public bool PauseWhenVisible { get; }

        protected IPlayerControl PlayerControl { get; }

        private bool _initialActiveState;

        protected FullScreenModalPanel(
            bool pauseWhenVisible,
            Option<string> closeAction,
            IPlayerControl playerControl,
            Godot.Control node,
            ILoggerFactory loggerFactory) : base(node, loggerFactory)
        {
            Ensure.That(playerControl, nameof(playerControl)).IsNotNull();

            PauseWhenVisible = pauseWhenVisible;
            CloseAction = closeAction;
            PlayerControl = playerControl;
        }

        protected override void PostConstruct()
        {
            base.PostConstruct();

            if (PauseWhenVisible)
            {
                Node.GetTree().Paused = true;
            }
            else
            {
                _initialActiveState = PlayerControl.Active;

                PlayerControl.Active = false;
            }

            Input.SetMouseMode(Input.MouseMode.Visible);

            CloseAction.Iter(action =>
            {
                Node.OnUnhandledInput()
                    .Where(e => e.IsActionPressed(action) && !e.IsEcho())
                    .Do(_ => Node.GetTree().SetInputAsHandled())
                    .TakeUntil(Disposed.Where(identity))
                    .Subscribe(_ => Resume(), this);
            });
        }

        public void Resume()
        {
            if (PauseWhenVisible)
            {
                Node.GetTree().Paused = false;
            }
            else
            {
                PlayerControl.Active = _initialActiveState;
            }

            Node.Hide();
            Node.QueueFree();

            Input.SetMouseMode(Input.MouseMode.Captured);
        }
    }
}
