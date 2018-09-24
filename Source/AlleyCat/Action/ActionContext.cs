namespace AlleyCat.Action
{
    public struct ActionContext : IActionContext
    {
        public IActor Actor { get; }

        public ActionContext(IActor actor = null)
        {
            Actor = actor;
        }
    }
}
