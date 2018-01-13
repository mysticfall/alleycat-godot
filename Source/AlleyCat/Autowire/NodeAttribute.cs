using JetBrains.Annotations;

namespace AlleyCat.Autowire
{
    public class NodeAttribute : InjectAttribute
    {
        [CanBeNull]
        public string Path { get; }

        public NodeAttribute([CanBeNull] string path = null)
        {
            Path = path;
        }
    }
}
