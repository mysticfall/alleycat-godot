using System;
using System.Linq;
using System.Reactive.Linq;
using AlleyCat.Attribute;
using EnsureThat;
using LanguageExt;
using Microsoft.Extensions.Logging;
using static LanguageExt.Prelude;

namespace AlleyCat.Item
{
    public class EncumbranceAttribute : Attribute.Attribute
    {
        public EncumbranceAttribute(
            string key,
            string displayName,
            Option<string> description,
            Option<IAttribute> min,
            Option<IAttribute> max,
            Option<IAttribute> modifier,
            bool active,
            ILoggerFactory loggerFactory) : base(
            key,
            displayName,
            description,
            min,
            max,
            modifier,
            active,
            loggerFactory)
        {
        }

        protected override IObservable<float> CreateObservable(IAttributeHolder holder)
        {
            Ensure.That(holder, nameof(holder)).IsNotNull();

            return Optional(holder)
                .OfType<IEquipmentHolder>()
                .Map(h => h.Equipments.OnItemsChange)
                .ToObservable()
                .Switch()
                .Select(v => v.Map(e => e.Node.Weight).Sum());
        }
    }
}
