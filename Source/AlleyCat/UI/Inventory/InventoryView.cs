using System;
using System.Linq;
using System.Reactive.Linq;
using AlleyCat.Autowire;
using AlleyCat.Common;
using AlleyCat.Event;
using AlleyCat.Item;
using AlleyCat.Item.Generic;
using EnsureThat;
using Godot;
using JetBrains.Annotations;

namespace AlleyCat.UI.Inventory
{
    public class InventoryView : FullScreenModalPanel
    {
        [Node(required: false)]
        public virtual IEquipmentHolder Holder
        {
            get => _holder.Value;
            set => _holder.Value = value;
        }

        public IObservable<IEquipmentHolder> OnHolderChange => _holder;

        [Node("List Panel/Tree")]
        protected Tree Tree { get; private set; }

        [Node("Content Panel/Viewport/Item Box/Item")]
        protected MeshInstance ItemStand { get; private set; }

        [Node("Content Panel/Info Panel")]
        protected Panel InfoPanel { get; private set; }

        [Node("Content Panel/Info Panel/Title")]
        protected Label Title { get; private set; }

        [Node("Content Panel/Info Panel/Type")]
        protected Label Type { get; private set; }

        [Node("Content Panel/Info Panel/Description")]
        protected RichTextLabel Description { get; private set; }

        private const string SlotKey = "Slot";

        private readonly ReactiveProperty<IEquipmentHolder> _holder = new ReactiveProperty<IEquipmentHolder>();

        protected override void OnInitialize()
        {
            base.OnInitialize();

            Holder = Holder ?? GetTree().GetNodesInGroup<IEquipmentHolder>(Tags.Player).FirstOrDefault();

            Tree.CreateItem();

            Tree.SetColumnTitle(0, "Name");
            Tree.SetColumnTitle(1, "Type");
            Tree.SetColumnTitle(2, "Slot");
            Tree.SetColumnTitle(3, "Weight");
            Tree.SetColumnTitlesVisible(true);

            var container = OnHolderChange.Select(c => c.Equipments);
            var items = container.SelectMany(c => c.OnItemsChange);

            var selected = Tree.OnItemSelect()
                .Select(e => e.Source.GetSelected()?.GetMeta(SlotKey))
                .OfType<string>()
                .CombineLatest(container, (slot, slots) => (slot, slots))
                .Select(t => t.slot != null ? t.slots.FindItem(t.slot) : null);

            void RemoveAllNodes()
            {
                var root = Tree.GetRoot();

                root.Children().Reverse().ToList().ForEach(root.RemoveChild);
            }

            items
                .Do(_ => RemoveAllNodes())
                .CombineLatest(container, (list, parent) => (list, parent))
                .Subscribe(t => t.list.ToList().ForEach(item => CreateNode(item, t.parent)))
                .AddTo(this);

            selected
                .Subscribe(DisplayItem)
                .AddTo(this);

            DisplayItem(null);
        }

        [NotNull]
        protected TreeItem CreateNode([NotNull] Equipment item, [NotNull] IEquipmentContainer parent)
        {
            Ensure.Any.IsNotNull(item, nameof(item));
            Ensure.Any.IsNotNull(parent, nameof(parent));

            var node = (TreeItem) Tree.CreateItem(Tree.GetRoot());

            node.SetMeta(SlotKey, item.Slot);

            node.SetCellMode(0, TreeItem.TreeCellMode.String);
            node.SetText(0, item.DisplayName);

            node.SetCellMode(1, TreeItem.TreeCellMode.String);
            node.SetText(1, item.EquipmentType.DisplayName(this));

            node.SetCellMode(2, TreeItem.TreeCellMode.String);
            node.SetText(2, parent.Slots[item.Slot].DisplayName);

            node.SetCellMode(3, TreeItem.TreeCellMode.String);
            node.SetText(3, $"{item.Weight:F1}kg");

            return node;
        }

        protected virtual void DisplayItem([CanBeNull] Equipment item)
        {
            if (item == null)
            {
                InfoPanel.Visible = false;
                ItemStand.Visible = false;

                ItemStand.Mesh = null;
            }
            else
            {
                InfoPanel.Visible = true;
                ItemStand.Visible = true;

                ItemStand.Mesh = item.Meshes.First().Mesh;

                Title.Text = item.DisplayName;
                Type.Text = item.EquipmentType.DisplayName(this);
                Description.Text = item.Description;
            }
        }

        protected override void Dispose(bool disposing)
        {
            _holder?.Dispose();

            base.Dispose(disposing);
        }
    }
}
