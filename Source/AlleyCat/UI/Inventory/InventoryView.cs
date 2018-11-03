using System;
using System.Linq;
using System.Reactive.Linq;
using AlleyCat.Action;
using AlleyCat.Autowire;
using AlleyCat.Character;
using AlleyCat.Common;
using AlleyCat.Event;
using AlleyCat.Item;
using AlleyCat.Item.Generic;
using AlleyCat.View;
using EnsureThat;
using Godot;
using LanguageExt;
using static LanguageExt.Prelude;
using Array = Godot.Collections.Array;

namespace AlleyCat.UI.Inventory
{
    public class InventoryView : FullScreenModalPanel, ICharacterAware<ICharacter>
    {
        public Option<ICharacter> Character
        {
            get => _character.Value;
            set => _character.Value = value;
        }

        public IObservable<Option<ICharacter>> OnCharacterChange => _character.AsObservable();

        public Option<Equipment> Item
        {
            get => _item.Bind(i => i.Value);
            set => _item.Iter(i => i.Value = value);
        }

        public IObservable<Option<Equipment>> OnItemChange =>
            _item.MatchObservable(identity, Observable.Empty<Option<Equipment>>);

        protected InspectingView ViewControl => _viewControl.Head();

        protected Tree Tree => _tree.Head();

        protected Container Buttons => _buttons.Head();

        protected MeshInstance ItemStand => _itemStand.Head();

        protected Panel InfoPanel => _infoPanel.Head();

        protected Label Title => _title.Head();

        protected Label Type => _type.Head();

        protected RichTextLabel Description => _description.Head();

        [Export] private PackedScene _actionButton;

        [Node("Control/View")] private Option<InspectingView> _viewControl;

        [Node("List Panel/Layout/Tree")] private Option<Tree> _tree;

        [Node("List Panel/Layout/Buttons Panel")]
        private Option<Container> _buttons;

        [Node("Content Panel/Viewport/Item Box/Item")]
        private Option<MeshInstance> _itemStand;

        [Node("Content Panel/Info Panel")] private Option<Panel> _infoPanel;

        [Node("Content Panel/Info Panel/Title")]
        private Option<Label> _title;

        [Node("Content Panel/Info Panel/Type")]
        private Option<Label> _type;

        [Node("Content Panel/Info Panel/Description")]
        private Option<RichTextLabel> _description;

        private const string SlotKey = "Slot";

        private readonly ReactiveProperty<Option<ICharacter>> _character;

        private Option<ReactiveProperty<Option<Equipment>>> _item;

        public InventoryView()
        {
            _character = new ReactiveProperty<Option<ICharacter>>(None).AddTo(this.GetCollector());
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();

            Character |= this.FindPlayer<ICharacter>();

            Tree.CreateItem();

            Tree.SetColumnTitle(0, Tr("ui.InventoryView.name"));
            Tree.SetColumnTitle(1, Tr("ui.InventoryView.type"));
            Tree.SetColumnTitle(2, Tr("ui.InventoryView.slot"));
            Tree.SetColumnTitle(3, Tr("ui.InventoryView.weight"));
            Tree.SetColumnTitlesVisible(true);

            var container = OnCharacterChange.SelectMany(c => c.Select(v => v.Equipments));
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
                .AddTo(this.GetCollector());

            _item = Tree.OnItemSelect()
                .Select(e => e.Source.GetSelected()?.GetMeta(SlotKey))
                .OfType<string>()
                .Select(Optional)
                .Merge(items.Select(_ => Option<string>.None))
                .CombineLatest(container, (slot, slots) => (slot, slots))
                .Select(t => t.slot.SelectMany(s => t.slots.FindItem(s)).HeadOrNone())
                .ToReactiveProperty()
                .AddTo(this.GetCollector());

            OnItemChange
                .Subscribe(DisplayItem)
                .AddTo(this.GetCollector());
        }

        protected TreeItem CreateNode(Equipment item, IEquipmentContainer parent)
        {
            Ensure.That(item, nameof(item)).IsNotNull();
            Ensure.That(parent, nameof(parent)).IsNotNull();

            var node = Tree.CreateItem(Tree.GetRoot());

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

        protected virtual void DisplayItem(Option<Equipment> item)
        {
            foreach (var button in Buttons.GetChildren().OfType<Button>())
            {
                button.Disconnect("pressed", this, nameof(OnButtonPress));
                button.QueueFree();
            }

            match(from i in item from c in Character select (item: i, character: c),
                v =>
                {
                    InfoPanel.Visible = true;
                    ItemStand.Visible = true;

                    ItemStand.Mesh = v.item.Meshes.First().Mesh;

                    Title.Text = v.item.DisplayName;
                    Type.Text = v.item.EquipmentType.DisplayName(this);
                    Description.Text = v.item.Description.IfNone(string.Empty);

                    ViewControl.Reset();

                    var context = new InteractionContext(v.character, v.item);
                    var actions = v.character.Actions.Values
                        .OfType<Interaction>()
                        .Where(a => a.Active && a.Valid && a.AllowedFor(context));

                    foreach (var action in actions)
                    {
                        var button = (Button) _actionButton.Instance();

                        button.Text = action.DisplayName;
                        button.Connect("pressed", this, nameof(OnButtonPress), new Array {action.Key});

                        Buttons.AddChild(button);
                    }
                },
                () =>
                {
                    InfoPanel.Visible = false;
                    ItemStand.Visible = false;

                    ItemStand.Mesh = null;
                }
            );
        }

        private void OnButtonPress(string key)
        {
            Item.SelectMany(i => Character, (i, c) => (item: i, character: c)).Iter(v =>
            {
                var context = new InteractionContext(v.character, v.item);

                v.character.Actions[key].Execute(context);
            });
        }
    }
}
