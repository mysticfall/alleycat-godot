using AlleyCat.Common;
using JetBrains.Annotations;

namespace AlleyCat.IO
{
    public interface IStateHolder : IIdentifiable
    {
        void SaveState([NotNull] IState state);

        void RestoreState([NotNull] IState state);
    }
}
