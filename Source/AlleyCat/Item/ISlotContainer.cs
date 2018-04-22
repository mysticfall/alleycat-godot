using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace AlleyCat.Item
{
    public interface ISlotContainer : IEnumerable
    {
    }

    namespace Generic
    {
        public interface ISlotContainer<TSlot, TItem> : ISlotContainer, IReadOnlyDictionary<string, TItem>
            where TSlot : ISlot
            where TItem : ISlotItem
        {
            [NotNull]
            IReadOnlyDictionary<string, TSlot> Slots { get; }

            IObservable<TItem> OnAdd { get; }

            IObservable<TItem> OnRemove { get; }

            [CanBeNull]
            TItem Add([NotNull] TItem item);

            void Remove([NotNull] TItem item);

            [CanBeNull]
            TItem Clear([NotNull] string slot);
        }
    }
}
