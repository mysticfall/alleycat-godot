using AlleyCat.Autowire;
using Godot;
using LanguageExt;

namespace AlleyCat.Common
{
    [NonInjectable]
    public interface IIconSource
    {
        Option<Texture> FindIcon(int sizeHint);
    }
}
