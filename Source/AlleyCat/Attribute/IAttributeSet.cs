using System;
using System.Collections.Generic;

namespace AlleyCat.Attribute
{
    public interface IAttributeSet: IReadOnlyDictionary<string, IAttribute>, IDisposable
    {
        IObservable<IAttribute> OnChange { get; }
    }
}
