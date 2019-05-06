using AlleyCat.Common;

namespace AlleyCat.UI.Menu
{
    public interface IMenuRenderer
    {
        bool CanRender(object item);

        INamed Render(object item);
    }
}
