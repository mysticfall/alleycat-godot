using EnsureThat;
using LanguageExt;
using static LanguageExt.Prelude;

namespace AlleyCat.Autowire
{
    public class NodeAttribute : InjectAttribute
    {
        public Option<string> Path { get; }

        public NodeAttribute(bool required = true) : base(required)
        {
            Path = None;
        }

        public NodeAttribute(string path, bool required = true) : base(required)
        {
            Ensure.That(path, nameof(path)).IsNotNull();

            Path = Some(path);
        }
    }
}
