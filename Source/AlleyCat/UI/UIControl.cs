using AlleyCat.Autowire;
using AlleyCat.Common;
using AlleyCat.Game;
using Microsoft.Extensions.Logging;

namespace AlleyCat.UI
{
    [NonInjectable]
    public abstract class UIControl : DelegateObject<Godot.Control>, IHideable
    {
        public virtual bool Visible
        {
            get => Node.Visible;
            set => Node.Visible = value;
        }

        protected UIControl(Godot.Control node, ILoggerFactory loggerFactory) : base(node, loggerFactory)
        {
        }

        public string Translate(string key) => Node.Tr(key);

        public static implicit operator Godot.Control(UIControl from) => from?.Node;
    }
}
