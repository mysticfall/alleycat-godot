using System;
using System.Reactive.Linq;
using AlleyCat.Action;
using AlleyCat.Autowire;
using AlleyCat.Common;

namespace AlleyCat.Control
{
    public abstract class InputAction : Action.Action
    {
        [Node]
        public ITriggerInput Input { get; private set; }

        protected abstract IActionContext CreateActionContext();

        [PostConstruct]
        protected virtual void OnInitialize()
        {
            Input
                .Where(v => v && Active)
                .Select(_ => CreateActionContext())
                .Where(AllowedFor)
                .Subscribe(Execute)
                .AddTo(this);
        }
    }
}
