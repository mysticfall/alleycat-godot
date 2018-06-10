using System;
using Godot;

namespace AlleyCat.UI
{
    public class GameMenu : Panel
    {
        [Export]
        public string CloseAction { get; set; } = "ui_cancel";

        public override void _Ready()
        {
            base._Ready();

            GetTree().Paused = true;

            Input.SetMouseMode(Input.MouseMode.Visible);
        }

        public void Resume()
        {
            GetTree().Paused = false;

            Hide();
            QueueFree();

            Input.SetMouseMode(Input.MouseMode.Captured);
        }

        public void ShowSettings()
        {
            throw new NotImplementedException();
        }

        public void Quit()
        {
            GetTree().Quit();
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
