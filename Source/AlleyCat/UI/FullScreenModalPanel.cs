using AlleyCat.Autowire;
using Godot;

namespace AlleyCat.UI
{
    public class FullScreenModalPanel : Panel
    {
        [Export]
        public string CloseAction { get; set; } = "ui_cancel";

        [Export]
        public bool PauseWhenVisible { get; set; } = true;

        public override void _Ready()
        {
            base._Ready();

            this.Autowire();

            if (PauseWhenVisible)
            {
                GetTree().Paused = true;
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
