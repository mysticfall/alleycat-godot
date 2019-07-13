using AlleyCat.Common;
using EnsureThat;
using LanguageExt;

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

            Path = path.TrimToOption();
        }

        public override string ToString() => "[Node]";
    }
}
