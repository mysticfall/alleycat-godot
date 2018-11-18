using AlleyCat.Common;
using LanguageExt;

namespace AlleyCat.Control
{
    public interface IInputBindings : IActivatable
    {
        Map<string, IInput> Inputs { get; }
    }
}
