using AlleyCat.Autowire;

namespace AlleyCat.UI.Console
{
    public class ToggleDebugConsole : ToggleUIAction
    {
        public override IHideableUI UI => _console;

        [Service]
        private DebugConsole _console;
    }
}
