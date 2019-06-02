using System;
using AlleyCat.Common;

namespace AlleyCat.Control
{
    public interface IInput : IIdentifiable, IActivatable
    {
        bool ConflictsWith(IInput other);
    }

    namespace Generic
    {
        public interface IInput<out T> : IInput, IObservable<T>
        {
        }
    }
}
