using System;

namespace AlleyCat.UI
{
    public class GameMenu : FullScreenModalPanel
    {
        public void ShowSettings()
        {
            throw new NotImplementedException();
        }

        public void Quit()
        {
            GetTree().Quit();
        }
    }
}
