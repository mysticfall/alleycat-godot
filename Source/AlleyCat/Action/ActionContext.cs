using LanguageExt;

namespace AlleyCat.Action
{
    public struct ActionContext : IActionContext
    {
        public Option<IActor> Actor { get; }

        public ActionContext(Option<IActor> actor)
        {
            Actor = actor;
        }

        public override string ToString() => $"ActionContext[Actor = {Actor}]";
    }
}
