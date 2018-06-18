using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using AlleyCat.Action;
using AlleyCat.Autowire;
using AlleyCat.Character;
using AlleyCat.Common;
using Godot;
using JetBrains.Annotations;
using static AlleyCat.Item.CommonEquipmentTags;

namespace AlleyCat.Item
{
    public class PickupAction : Action.Action
    {
        [Ancestor]
        public EquippableItem Item { get; private set; }

        [Export(PropertyHint.ExpRange, "0.1, 5")]
        public float PickupDistance { get; set; } = 1.2f;

        [Export]
        public Godot.Animation Animation { get; set; }

        public IEnumerable<string> Tags => _tags.TrimToEnumerable();

        [Export, UsedImplicitly] private string _tags = string.Join(",", Carry, Hand);

        private bool _deleted;

        protected override void DoExecute(IActor actor)
        {
            var character = (ICharacter) actor;
            var container = character.Equipments;

            var configuration = Item.Configurations.Values
                .TaggedAny(Tags.ToArray())
                .FirstOrDefault(c => container.AllowedFor(c));

            if (configuration == null) return;

            if (Animation == null)
            {
                container.Equip(Item, configuration);

                _deleted = true;

                return;
            }

            var animator = character.AnimationManager;

            var listener = animator.OnAnimationEvent
                .Where(e => e.Name == "action." + Key)
                .Subscribe(_ =>
                {
                    container.Equip(Item, configuration, false);

                    Item.Visible = false;
                });

            animator.Play(Animation, () =>
            {
                listener.Dispose();
                Item.QueueFree();

                _deleted = true;
            });
        }

        public override bool AllowedFor(IActor context) =>
            !_deleted &&
            Item.Visible &&
            context is ICharacter character &&
            character.DistanceTo(Item) <= PickupDistance;
    }
}
