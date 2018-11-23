using System;
using System.Linq;
using AlleyCat.Action;
using Microsoft.Extensions.Logging;
using static LanguageExt.Prelude;

namespace AlleyCat.Item
{
    public abstract class EquipmentAction : Interaction
    {
        protected EquipmentAction(
            string key,
            string displayName,
            bool active,
            ILogger logger) : base(key, displayName, active, logger)
        {
        }

        protected override void DoExecute(InteractionContext context)
        {
            var arguments = (
                from holder in context.Actor.OfType<IEquipmentHolder>()
                from equipment in Optional(context.Target).OfType<Equipment>()
                select (holder, equipment)).HeadOrNone();

            if (arguments.IsNone)
            {
                throw new ArgumentException(
                    "The specified context does not provide information for item interaction.");
            }

            arguments.Iter(t => DoExecute(t.holder, t.equipment, context));
        }

        protected abstract void DoExecute(
            IEquipmentHolder holder,
            Equipment equipment,
            InteractionContext context);

        protected override bool AllowedFor(InteractionContext context)
        {
            var arguments = (
                from holder in context.Actor.OfType<IEquipmentHolder>()
                from equipment in Optional(context.Target).OfType<Equipment>()
                select (holder, equipment)).HeadOrNone();

            return arguments.Exists(t => AllowedFor(t.holder, t.equipment, context));
        }

        protected abstract bool AllowedFor(
            IEquipmentHolder holder,
            Equipment equipment,
            InteractionContext context);
    }
}
