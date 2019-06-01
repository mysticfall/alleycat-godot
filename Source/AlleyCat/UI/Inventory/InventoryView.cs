using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using AlleyCat.Action;
using AlleyCat.Control;
using AlleyCat.Item;
using AlleyCat.Item.Generic;
using AlleyCat.Logging;
using AlleyCat.View;
using EnsureThat;
using Godot;
using LanguageExt;
using Microsoft.Extensions.Logging;
using static LanguageExt.Prelude;

namespace AlleyCat.UI.Inventory
{
    public class InventoryView : FullScreenModalPanel
    {
        public Option<IEquipment> Selected { get; private set; }

        public IObservable<Option<IEquipment>> OnSelectionChange { get; }

        public IObservable<IEnumerable<IEquipment>> OnItemsChange { get; }

        protected IObservable<IEquipmentContainer> OnEquipmentContainerChange { get; }

        protected InspectingView ViewControl { get; }

        protected Tree Tree { get; }

        protected Container ButtonContainer { get; }

        protected MeshInstance ItemStand { get; }

        protected Panel InfoPanel { get; }

        protected Label TitleLabel { get; }

        protected Option<Label> TypeLabel { get; }

        protected Option<RichTextLabel> DescriptionLabel { get; }

        protected PackedScene ActionButton { get; }

        private const string SlotKey = "Slot";

        // ReSharper disable once CollectionNeverQueried.Local
        private readonly CompositeDisposable _buttonListeners;

        public InventoryView(
            IPlayerControl playerControl,
            InspectingView viewControl,
            MeshInstance itemStand,
            bool pauseWhenVisible,
            Option<string> closeAction,
            Godot.Control node,
            Tree tree,
            Container buttonContainer,
            Panel infoPanel,
            Label titleLabel,
            Option<Label> typeLabel,
            Option<RichTextLabel> descriptionLabel,
            PackedScene actionButton,
            ILoggerFactory loggerFactory) : base(pauseWhenVisible, closeAction, playerControl, node, loggerFactory)
        {
            Ensure.That(viewControl, nameof(viewControl)).IsNotNull();
            Ensure.That(tree, nameof(tree)).IsNotNull();
            Ensure.That(itemStand, nameof(itemStand)).IsNotNull();
            Ensure.That(buttonContainer, nameof(buttonContainer)).IsNotNull();
            Ensure.That(infoPanel, nameof(infoPanel)).IsNotNull();
            Ensure.That(titleLabel, nameof(titleLabel)).IsNotNull();
            Ensure.That(actionButton, nameof(actionButton)).IsNotNull();

            ViewControl = viewControl;
            Tree = tree;
            ButtonContainer = buttonContainer;
            ItemStand = itemStand;
            InfoPanel = infoPanel;
            TitleLabel = titleLabel;
            TypeLabel = typeLabel;
            DescriptionLabel = descriptionLabel;
            ActionButton = actionButton;

            _buttonListeners = new CompositeDisposable();

            OnEquipmentContainerChange = PlayerControl.OnCharacterChange
                .Select(c => c.Select(v => v.Equipments).ToObservable())
                .Switch();

            OnItemsChange = OnEquipmentContainerChange
                .Select(c => c.OnItemsChange)
                .Switch();

            OnSelectionChange = Tree.OnItemSelect()
                .Select(v => v.Bind(i => Optional(i.GetMeta(SlotKey) as string)))
                .Merge(OnItemsChange.Select(_ => Option<string>.None))
                .CombineLatest(OnEquipmentContainerChange, (slot, slots) => (slot, slots))
                .Select(t => t.slot.Bind(s => t.slots.FindItemInSlot(s)).HeadOrNone())
                .Do(current => Selected = current);
        }

        protected override void PostConstruct()
        {
            base.PostConstruct();

            Tree.CreateItem();

            Tree.SetColumnTitle(0, Translate("ui.InventoryView.name"));
            Tree.SetColumnTitle(1, Translate("ui.InventoryView.type"));
            Tree.SetColumnTitle(2, Translate("ui.InventoryView.slot"));
            Tree.SetColumnTitle(3, Translate("ui.InventoryView.weight"));

            Tree.SetColumnTitlesVisible(true);

            void RemoveAllNodes() => Tree.GetRoot().Children().Iter(c => c.Free());

            var onDispose = Disposed.Where(identity);

            OnItemsChange
                .Do(_ => RemoveAllNodes())
                .CombineLatest(OnEquipmentContainerChange, (list, parent) => (list, parent))
                .TakeUntil(onDispose)
                .Subscribe(t => t.list.ToList().ForEach(item => CreateNode(item, t.parent)), this);

            OnSelectionChange
                .TakeUntil(onDispose)
                .Subscribe(DisplayItem, this);
        }

        protected TreeItem CreateNode(IEquipment item, IEquipmentContainer parent)
        {
            Ensure.That(item, nameof(item)).IsNotNull();
            Ensure.That(parent, nameof(parent)).IsNotNull();

            var node = Tree.CreateItem(Tree.GetRoot());

            node.SetMeta(SlotKey, item.Slot);

            node.SetCellMode(0, TreeItem.TreeCellMode.String);
            node.SetText(0, item.DisplayName);

            node.SetCellMode(1, TreeItem.TreeCellMode.String);
            node.SetText(1, item.EquipmentType.DisplayName(Node));

            node.SetCellMode(2, TreeItem.TreeCellMode.String);
            node.SetText(2, parent.Slots[item.Slot].DisplayName);

            node.SetCellMode(3, TreeItem.TreeCellMode.String);
            node.SetText(3, $"{item.Node.Weight:F1}kg");

            return node;
        }

        protected virtual void DisplayItem(Option<IEquipment> item)
        {
            _buttonListeners.Clear();

            foreach (var button in ButtonContainer.GetChildren().OfType<Button>())
            {
                button.QueueFree();
            }

            match(from i in item from c in PlayerControl.Character select (item: i, character: c),
                v =>
                {
                    InfoPanel.Visible = true;
                    ItemStand.Visible = true;

                    ItemStand.Mesh = v.item.Meshes.First().Mesh;

                    TitleLabel.Text = v.item.DisplayName;

                    TypeLabel.Iter(label => label.Text = v.item.EquipmentType.DisplayName(Node));
                    DescriptionLabel.Iter(label => label.Text = v.item.Description.IfNone(string.Empty));

                    ViewControl.Reset();

                    var context = new InteractionContext(v.character, v.item);
                    var actions = v.character.Actions.Values
                        .OfType<Interaction>()
                        .Where(a => a.Active && a.Valid && a.AllowedFor(context));

                    foreach (var action in actions)
                    {
                        var button = (Button) ActionButton.Instance();

                        button.Text = action.DisplayName;

                        ButtonContainer.AddChild(button);

                        var listener = button.OnPress().Subscribe(_ => OnButtonPress(action.Key), this);

                        _buttonListeners.Add(listener);
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
            Selected.SelectMany(i => PlayerControl.Character, (i, c) => (item: i, character: c)).Iter(v =>
            {
                var context = new InteractionContext(v.character, v.item);

                v.character.Actions[key].Execute(context);
            });
        }

        protected override void PreDestroy()
        {
            _buttonListeners.Clear();

            base.PreDestroy();
        }
    }
}
