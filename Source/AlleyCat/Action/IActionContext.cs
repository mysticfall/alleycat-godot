using LanguageExt;

namespace AlleyCat.Action
{
    public interface IActionContext
    {
        Option<IActor> Actor { get; }
    }
}
