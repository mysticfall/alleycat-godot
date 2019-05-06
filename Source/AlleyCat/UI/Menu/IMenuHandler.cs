namespace AlleyCat.UI.Menu
{
    public interface IMenuHandler
    {
        bool CanExecute(IMenuItem item);

        void Execute(IMenuItem item);
    }
}
