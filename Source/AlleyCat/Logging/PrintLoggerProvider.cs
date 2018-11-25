using Microsoft.Extensions.Logging;

namespace AlleyCat.Logging
{
    [ProviderAlias("Editor")]
    public class PrintLoggerProvider : LoggerProvider
    {
        protected override ILogger DoCreateLogger(string categoryName) =>
            new PrintLogger(categoryName, CategorySegments, ShowId);
    }
}
