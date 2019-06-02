using System.Collections.Generic;

namespace AlleyCat.Control
{
    public interface IActionInput : IInput
    {
        IEnumerable<string> Actions { get; }
    }
}
