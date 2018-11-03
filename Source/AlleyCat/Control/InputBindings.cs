using System;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using AlleyCat.Autowire;
using AlleyCat.Common;
using Godot;

namespace AlleyCat.Control
{
    public class InputBindings : IdentifiableDirectory<IInput>, IActivatable
    {
        [Export]
        public bool Active
        {
            get => _active.Value;
            set => _active.OnNext(value);
        }

        public IObservable<bool> OnActiveStateChange => _active.AsObservable();

        private readonly BehaviorSubject<bool> _active;

        public InputBindings()
        {
            _active = new BehaviorSubject<bool>(true).AddTo(this);
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
