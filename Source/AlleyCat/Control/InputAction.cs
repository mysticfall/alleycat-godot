using System;
using System.Reactive.Linq;
using AlleyCat.Action;
using AlleyCat.Autowire;
using AlleyCat.Common;
using LanguageExt;

namespace AlleyCat.Control
{
    public abstract class InputAction : Action.Action
    {
        public ITriggerInput Input => _input.Head();

        protected abstract Option<IActionContext> CreateActionContext();

        [Node] private Option<ITriggerInput> _input;

        [PostConstruct]
        protected virtual void OnInitialize()
        {
            Input
                .Where(v => v && Active)
                .Select(_ => CreateActionContext())
                .Where(c => c.Exists(AllowedFor))
                .Subscribe(c => c.Iter(Execute))
                .AddTo(this);
        }
    }
}
