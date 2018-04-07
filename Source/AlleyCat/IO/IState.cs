using System.Collections.Generic;
using JetBrains.Annotations;

namespace AlleyCat.IO
{
    public interface IState : IDictionary<string, object>
    {
        [NotNull]
        IState GetSection([NotNull] string key);

        [NotNull]
        IEnumerable<IState> GetChildren();
    }
}
