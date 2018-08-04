using System;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using AlleyCat.Autowire;
using AlleyCat.Character;
using AlleyCat.Common;
using AlleyCat.Event;
using AlleyCat.Item;
using AlleyCat.Item.Generic;
using AlleyCat.View;
using EnsureThat;
using Godot;
using JetBrains.Annotations;
using Array = Godot.Array;

namespace AlleyCat.UI.Inventory
{
    public class InventoryView : FullScreenModalPanel
    {
        [Node(required: false)]
        public virtual ICharacter Character
        {
            get => _character.Value;
            set => _character.Value = value;
        }

        public IObservable<IEquipmentHolder> OnHolderChange => _character;

        public IObservable<Equipment> OnItemChange => _item ?? Observable.Empty<Equipment>();

        [Node("Control/View")]
        protected InspectingView ViewControl { get; private set; }

        [Node("List Panel/Layout/Tree")]
        protected Tree Tree { get; private set; }

        [Node("List Panel/Layout/Buttons Panel")]
        protected Container Buttons { get; private set; }

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

        [Export, UsedImplicitly] private PackedScene _actionButton;

        private const string SlotKey = "Slot";

        private readonly ReactiveProperty<ICharacter> _character = new ReactiveProperty<ICharacter>();

        private ReactiveProperty<Equipment> _item;

        protected override void OnInitialize()
        {
            base.OnInitialize();

            Debug.Assert(_actionButton != null, "_actionButton != null");

            Character = Character ?? GetTree().GetNodesInGroup<ICharacter>(Tags.Player).FirstOrDefault();

            Tree.CreateItem();

            Tree.SetColumnTitle(0, "Name");
            Tree.SetColumnTitle(1, "Type");
            Tree.SetColumnTitle(2, "Slot");
            Tree.SetColumnTitle(3, "Weight");
            Tree.SetColumnTitlesVisible(true);

            var container = OnHolderChange.Select(c => c.Equipments);
            var items = container.SelectMany(c => c.OnItemsChange);

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

            _item = Tree.OnItemSelect()
                .Select(e => e.Source.GetSelected()?.GetMeta(SlotKey))
                .OfType<string>()
                .Merge(items.Select(_ => (string) null))
                .CombineLatest(container, (slot, slots) => (slot, slots))
                .Select(t => t.slot != null ? t.slots.FindItem(t.slot) : null)
                .ToReactiveProperty();

            OnItemChange
                .Subscribe(DisplayItem)
                .AddTo(this);
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
            foreach (var button in Buttons.GetChildren().OfType<Button>())
            {
                button.Disconnect("pressed", this, nameof(OnButtonPress));
                button.QueueFree();
            }

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

                ViewControl.Reset();

                var actions = item.Actions.Where(a => a.Active && a.Valid && a.AllowedFor(Character));

                foreach (var action in actions)
                {
                    var button = (Button) _actionButton.Instance();

                    button.Text = action.DisplayName;
                    button.Connect("pressed", this, nameof(OnButtonPress), new Array {action.Key});

                    Buttons.AddChild(button);
                }
            }
        }

        private void OnButtonPress(string key)
        {
            var action = _item.Value?.Actions.FirstOrDefault(a => a.Key == key);

            action?.Execute(Character);
        }

        public override void _Notification(int what)
        {
            base._Notification(what);

            if (what != NotificationPredelete) return;

            _character?.Dispose();
            _item?.Dispose();
        }
    }
}
