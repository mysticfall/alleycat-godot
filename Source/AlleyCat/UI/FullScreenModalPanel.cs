using AlleyCat.Autowire;
using AlleyCat.Control;
using Godot;

namespace AlleyCat.UI
{
    public class FullScreenModalPanel : Panel
    {
        [Export]
        public string CloseAction { get; set; } = "ui_cancel";

        [Service]
        protected IPlayerControl PlayerControl { get; private set; }

        [Export]
        public bool PauseWhenVisible { get; set; } = true;

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
        protected virtual void OnInitialize()
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

            if (CloseAction != null && @event.IsActionPressed(CloseAction))
            {
                Resume();
                GetTree().SetInputAsHandled();
            }
        }
    }
}
