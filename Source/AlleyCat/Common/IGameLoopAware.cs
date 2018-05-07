using System;

namespace AlleyCat.Common
{
    public interface IGameLoopAware
    {
        ProcessMode ProcessMode { get; }

        IObservable<float> OnLoop { get; }
    }
}
