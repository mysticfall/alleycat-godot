using System;
using AlleyCat.Common;

namespace AlleyCat.Morph
{
    public interface IMorph : INamed, IInitializable, IDisposable
    {
        IMorphDefinition Definition { get; }

        IObservable<object> OnChange { get; }

        object Value { get; set; }

        void Reset();
    }

    namespace Generic
    {
        public interface IMorph<TVal, out TDef> : IMorph where TDef : IMorphDefinition
        {
            new TVal Value { get; set; }

            new TDef Definition { get; }

            new IObservable<TVal> OnChange { get; }
        }
    }
}
