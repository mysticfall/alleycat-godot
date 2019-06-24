using System;
using AlleyCat.Common;
using AlleyCat.Game;
using LanguageExt;

namespace AlleyCat.Attribute
{
    public interface IAttribute : INamed, IDescribable, IActivatable, IGameObject
    {
        float Value { get; }

        Option<IAttribute> Min { get; }

        Option<IAttribute> Max { get; }

        Option<IAttribute> Modifier { get; }

        IObservable<float> OnChange { get; }

        void Initialize(IAttributeSet attributes);
    }
}
