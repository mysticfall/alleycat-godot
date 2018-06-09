using AlleyCat.Common;
using AlleyCat.Condition.Generic;
using JetBrains.Annotations;

namespace AlleyCat.Action
{
    public interface IAction : IIdentifiable, IActivatable, IValidatable, IRestricted<IActor>
    {
        void Execute([CanBeNull] IActor actor);
    }
}
