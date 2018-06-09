using AlleyCat.Common;
using AlleyCat.Condition.Generic;
using JetBrains.Annotations;

namespace AlleyCat.Action
{
    public interface IAction : INamed, IActivatable, IValidatable, IRestricted<IActor>
    {
        void Execute([CanBeNull] IActor actor);
    }
}
