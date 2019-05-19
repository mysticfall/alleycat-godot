namespace AlleyCat.UI.Menu
{
    public interface IMenuHandler
    {
        bool CanExecute(IMenuModel item);

        void Execute(IMenuModel item);
    }
}
