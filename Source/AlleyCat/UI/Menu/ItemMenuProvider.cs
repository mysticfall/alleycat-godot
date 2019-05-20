using System.Collections.Generic;
using System.Linq;
using AlleyCat.Action;
using AlleyCat.Common;
using AlleyCat.Control;
using AlleyCat.Item;
using EnsureThat;
using Godot;
using LanguageExt;
using Microsoft.Extensions.Logging;
using static LanguageExt.Prelude;

namespace AlleyCat.UI.Menu
{
    public class ItemMenuProvider : PlayerMenuProvider, IMenuStructureProvider, IMenuRenderer, IMenuHandler
    {
        protected Option<IEquipmentContainer> Container => PlayerControl.Character.Map(c => c.Equipments);

        protected Node Node { get; }

        public ItemMenuProvider(
            string key,
            string displayName,
            Node node,
            PlayerControl playerControl,
            ILoggerFactory loggerFactory) : base(key, displayName, playerControl, loggerFactory)
        {
            Ensure.That(node, nameof(node)).IsNotNull();

            Node = node;
        }

        public bool HasChildren(object item) => item == this || item is Equipment;

        public IEnumerable<object> FindChildren(object item)
        {
            Ensure.That(item, nameof(item)).IsNotNull();

            switch (item)
            {
                case ItemMenuProvider provider when provider == this:
                    return Container.Bind(c => c.Items.Values);
                case Equipment equipment:
                    return
                        from actor in Actor
                        from context in Some(new InteractionContext(actor, equipment))
                        from actions in Actor
                            .Bind(a => a.Actions.Values)
                            .OfType<Interaction>()
                            .Where(a => a.Active && a.Valid && a.AllowedFor(context))
                        select actions;
                default:
                    return Enumerable.Empty<object>();
            }
        }

        public bool CanRender(object item) => item is Equipment;

        public INamed Render(object item)
        {
            Ensure.That(item, nameof(item)).IsNotNull();

            return new EquipmentLabel((Equipment) item, Node);
        }

        public bool CanExecute(IMenuModel item)
        {
            Ensure.That(item, nameof(item)).IsNotNull();

            var parent = item.Parent.Map(p => p.Model);

            if (item.Model is Interaction action && parent.Exists(p => p is Equipment))
            {
                var context =
                    from equipment in parent.OfType<Equipment>()
                    from ctx in Actor.Map(actor => new InteractionContext(actor, equipment))
                    select ctx;

                return context.OfType<IActionContext>().Exists(action.AllowedFor);
            }

            return false;
        }

        public void Execute(IMenuModel item)
        {
            Ensure.That(item, nameof(item)).IsNotNull();

            var parent = item.Parent.Map(p => p.Model);

            if (item.Model is Interaction action && parent.Exists(p => p is Equipment))
            {
                var context =
                    from equipment in parent.OfType<Equipment>()
                    from ctx in Actor.Map(actor => new InteractionContext(actor, equipment))
                    select ctx;

                context.OfType<IActionContext>().Iter(action.Execute);
            }
        }

        public struct EquipmentLabel : INamed
        {
            public string Key => _equipment.Key;

            public string DisplayName => $"{_equipment.DisplayName}{SlotName}";

            public string SlotName => _equipment.ActiveConfiguration
                .Map(c => "slot." + c.Slot)
                .Map(_node.Tr)
                .Map(s => $" ({s})")
                .IfNone("");

            private readonly Equipment _equipment;

            private readonly Node _node;

            public EquipmentLabel(Equipment equipment, Node context)
            {
                Ensure.That(equipment, nameof(equipment)).IsNotNull();
                Ensure.That(context, nameof(context)).IsNotNull();

                _equipment = equipment;
                _node = context;
            }
        }
    }
}
