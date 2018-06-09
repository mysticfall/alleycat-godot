using System;

namespace AlleyCat.Game
{
    public interface ISceneManager
    {
        IScene CurrentScene { get; }

        IObservable<IScene> OnChange { get; }

        void Switch(string key);
    }
}
