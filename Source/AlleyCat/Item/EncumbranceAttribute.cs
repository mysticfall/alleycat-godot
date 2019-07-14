using System;
using System.Linq;
using System.Reactive.Linq;
using AlleyCat.Attribute;
using EnsureThat;
using Godot;
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
            Option<Texture> icon,
            Map<string, IAttribute> children,
            bool active,
            ILoggerFactory loggerFactory) : base(
            key,
            displayName,
            description,
            icon,
            children,
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
