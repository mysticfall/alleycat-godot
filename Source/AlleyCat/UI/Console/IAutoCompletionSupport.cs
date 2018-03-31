using System.Collections.Generic;
using JetBrains.Annotations;

namespace AlleyCat.UI.Console
{
    public interface IAutoCompletionSupport
    {
        [NotNull]
        IEnumerable<string> SuggestCandidates([CanBeNull] string text);
    }
}
