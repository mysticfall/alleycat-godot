using System.Collections.Generic;
using LanguageExt;

namespace AlleyCat.UI.Console
{
    public interface IAutoCompletionSupport
    {
        IEnumerable<string> SuggestCandidates(Option<string> text);
    }
}
