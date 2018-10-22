using AlleyCat.Common;
using AlleyCat.Condition.Generic;

namespace AlleyCat.Action
{
    public interface IAction : INamed, IActivatable, IValidatable, IRestricted<IActionContext>
    {
        void Execute(IActionContext context);
    }
}
