using AlleyCat.Control;
using Godot;

namespace AlleyCat.UI
{
    public abstract class UIActionFactory<T> : InputActionFactory<T> where T : UIAction
    {
        [Export]
        public bool Modal { get; set; }
    }
}
