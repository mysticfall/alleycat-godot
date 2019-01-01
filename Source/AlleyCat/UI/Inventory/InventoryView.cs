using System;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using AlleyCat.Action;
using AlleyCat.Autowire;
using AlleyCat.Character;
using AlleyCat.Event;
using AlleyCat.Item;
using AlleyCat.Item.Generic;
using AlleyCat.Logging;
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
            set => _character.OnNext(value);
        }

        public IObservable<Option<ICharacter>> OnCharacterChange => _character.AsObservable();

        public Option<Equipment> Item { get; private set; }

        public IObservable<Option<Equipment>> OnItemChange =>
            _item.MatchObservable(identity, Observable.Empty<Option<Equipment>>);

        [Node("Control/View", true)]
        protected InspectingView ViewControl { get; private set; }

        [Node("List Panel/Layout/Tree", true)]
        protected Tree Tree { get; private set; }

        [Node("List Panel/Layout/Buttons Panel", true)]
        protected Container Buttons { get; private set; }

        [Node("Content Panel/Viewport/Item Box/Item", true)]
        protected MeshInstance ItemStand { get; private set; }

        [Node("Content Panel/Info Panel", true)]
        protected Panel InfoPanel { get; private set; }

        [Node("Content Panel/Info Panel/Title", true)]
        protected Label Title { get; private set; }

        [Node("Content Panel/Info Panel/Type", true)]
        protected Label Type { get; private set; }

        [Node("Content Panel/Info Panel/Description", true)]
        protected RichTextLabel Description { get; private set; }

        [Export] private PackedScene _actionButton;

        private const string SlotKey = "Slot";

        private readonly BehaviorSubject<Option<ICharacter>> _character;

        private Option<IObservable<Option<Equipment>>> _item;

        public InventoryView()
        {
            _character = new BehaviorSubject<Option<ICharacter>>(None);
        }

        protected override void PostConstruct()
        {
            base.PostConstruct();

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

            var onDispose = this.OnDispose().Where(identity);

            items
                .Do(_ => RemoveAllNodes())
                .CombineLatest(container, (list, parent) => (list, parent))
                .TakeUntil(onDispose)
                .Subscribe(t => t.list.ToList().ForEach(item => CreateNode(item, t.parent)), this);

            _item = Some(
                Tree.OnItemSelect()
                    .Select(e => e.Source.GetSelected()?.GetMeta(SlotKey))
                    .OfType<string>()
                    .Select(Optional)
                    .Merge(items.Select(_ => Option<string>.None))
                    .CombineLatest(container, (slot, slots) => (slot, slots))
                    .Select(t => t.slot.SelectMany(s => t.slots.FindItem(s)).HeadOrNone())
                    .Do(current => Item = current));

            OnItemChange
                .TakeUntil(onDispose)
                .Subscribe(DisplayItem, this);
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
            node.SetText(3, $"{item.Node.Weight:F1}kg");

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

        protected override void Dispose(bool disposing)
        {
            _character.CompleteAndDispose();

            base.Dispose(disposing);
        }
    }
}
