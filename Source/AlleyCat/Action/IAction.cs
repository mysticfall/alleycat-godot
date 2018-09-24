using AlleyCat.Common;
using AlleyCat.Condition.Generic;
using JetBrains.Annotations;

namespace AlleyCat.Action
{
    public interface IAction : INamed, IActivatable, IValidatable, IRestricted<IActionContext>
    {
        void Execute([NotNull] IActionContext context);
    }
}
