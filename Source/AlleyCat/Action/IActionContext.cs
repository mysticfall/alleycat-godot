using JetBrains.Annotations;

namespace AlleyCat.Action
{
    public interface IActionContext
    {
        [CanBeNull]
        IActor Actor { get; }
    }
}
