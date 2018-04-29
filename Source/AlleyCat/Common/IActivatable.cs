using System;

namespace AlleyCat.Common
{
    public interface IActivatable
    {
        bool Active { get; set; }

        IObservable<bool> OnActiveStateChange { get; }
    }
}
