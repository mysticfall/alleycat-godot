using System;
using AlleyCat.Common;

namespace AlleyCat.Control
{
    public interface IInput : IIdentifiable, IActivatable
    {
    }

    namespace Generic
    {
        public interface IInput<out T> : IInput, IObservable<T>
        {
        }
    }
}
