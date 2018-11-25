using EnsureThat;
using LanguageExt;
using static LanguageExt.Prelude;

namespace AlleyCat.Autowire
{
    public class NodeAttribute : InjectAttribute
    {
        public Option<string> Path { get; }

        public NodeAttribute(bool required = false) : base(required)
        {
        }

        public NodeAttribute(string path, bool required = false) : base(required)
        {
            Ensure.That(path, nameof(path)).IsNotNull();

            Path = Some(path);
        }

        public override string ToString() => "[Node]";
    }
}
