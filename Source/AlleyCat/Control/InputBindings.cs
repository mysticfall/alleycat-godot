using System;
using System.Linq;
using AlleyCat.Autowire;
using AlleyCat.Common;
using AlleyCat.Event;
using Godot;

namespace AlleyCat.Control
{
    public class InputBindings : IdentifiableDirectory<IInput>, IActivatable
    {
        [Export]
        public bool Active
        {
            get => _active.Value;
            set => _active.Value = value;
        }

        public IObservable<bool> OnActiveStateChange => _active;

        private readonly ReactiveProperty<bool> _active;

        public InputBindings()
        {
            _active = new ReactiveProperty<bool>(true).AddTo(this);
        }

        [PostConstruct]
        protected virtual void OnInitialize()
        {
            OnActiveStateChange
                .Subscribe(v => Values.ToList().ForEach(i => i.Active = v))
                .AddTo(this);
        }
    }
}
