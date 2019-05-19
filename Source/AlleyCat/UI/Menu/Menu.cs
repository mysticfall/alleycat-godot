using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using AlleyCat.Autowire;
using AlleyCat.Common;
using AlleyCat.Event;
using AlleyCat.Logging;
using EnsureThat;
using Godot;
using JetBrains.Annotations;
using LanguageExt;
using Microsoft.Extensions.Logging;
using static LanguageExt.Prelude;

namespace AlleyCat.UI.Menu
{
    [AutowireContext]
    public class Menu : Godot.Control, IMenu, IHideable, ILoggable
    {
        public Option<IMenuItem> Current => _current.Value;

        public IObservable<Option<IMenuItem>> OnNavigate => _current.AsObservable();

        public IEnumerable<IMenuItem> Items { get; private set; }

        public IObservable<IEnumerable<IMenuItem>> OnItemsChange => OnNavigate.Select(CreateChildren);

        [Service, CanBeNull]
        public ILogger Logger { get; private set; }

        [Service]
        protected IEnumerable<IMenuRenderer> Renderers { get; private set; } = Seq<IMenuRenderer>();

        [Service]
        protected IEnumerable<IMenuStructureProvider> StructureProviders { get; private set; } =
            Seq<IMenuStructureProvider>();

        [Service]
        protected IEnumerable<IMenuHandler> MenuHandlers { get; private set; } = Seq<IMenuHandler>();

        [Node]
        protected Option<Label> Breadcrumb { get; set; }

        [Node]
        protected Option<ShortcutLabel> UpLabel { get; set; }

        [Node]
        protected Option<ShortcutLabel> CloseLabel { get; set; }

        [Node(true)]
        protected Node ItemsContainer { get; private set; }

        [Export, UsedImplicitly]
        protected PackedScene ItemScene { get; private set; }

        [Export]
        protected string BackAction { get; private set; } = "ui_back";

        private readonly BehaviorSubject<Option<IMenuItem>> _current;

        [Service(local: true)] private IEnumerable<IMenuItem> _rootItems = Seq<IMenuItem>();

        [Export, UsedImplicitly] private NodePath _breadcrumb = "Breadcrumb";

        [Export, UsedImplicitly] private NodePath _itemsContainer = "Items";

        [Export, UsedImplicitly] private NodePath _upLabel = "Up";

        [Export, UsedImplicitly] private NodePath _closeLabel = "Close";

        public Menu()
        {
            _current = new BehaviorSubject<Option<IMenuItem>>(None);
        }

        public override void _Ready()
        {
            base._Ready();

            this.Autowire();
        }

        [PostConstruct]
        protected virtual void PostConstruct()
        {
            OnItemsChange
                .Do(items => Items = items)
                .Subscribe(HandleItemsChange, this);

            OnNavigate
                .Do(_ => UpLabel.Iter(l => l.Active = this.CanGoUp()))
                .Select(i => i.Bind(v => v.GetPath()).Reverse())
                .Select(p => string.Join(" > ", p.Map(v => v.DisplayName)))
                .Subscribe(v => Breadcrumb.Iter(b => b.Text = v), this);

            UpLabel
                .Map(l => l.OnAction)
                .ToObservable()
                .Switch()
                .Subscribe(_ => this.GoUp());
            CloseLabel
                .Map(l => l.OnAction)
                .ToObservable()
                .Switch()
                .Subscribe(_ => Hide());

            this.OnVisibilityChange()
                .StartWith(Visible)
                .Subscribe(OnVisibilityChanged, this);
        }

        private void HandleItemsChange(IEnumerable<IMenuItem> items)
        {
            ItemsContainer.GetChildren().OfType<Node>().Iter(ItemsContainer.FreeChild);

            bool IsValid(IMenuItem item)
            {
                var hasChildren = StructureProviders.Exists(p => p.HasChildren(item.Model));
                var executable = MenuHandlers.Exists(h => h.CanExecute(item));

                return hasChildren || executable;
            }

            items.Filter(IsValid).Iter((index, item) =>
            {
                var control = CreateItemControl(item, index);

                ItemsContainer.AddChild(control);
            });
        }

        private void OnVisibilityChanged(bool visible)
        {
            this.ToTop();

            SetProcessUnhandledInput(visible);

            UpLabel.Iter(l => l.Active = visible && this.CanGoUp());
            CloseLabel.Iter(l => l.Active = visible);

            if (!visible)
            {
                ItemsContainer.GetChildren().OfType<Node>().Iter(ItemsContainer.FreeChild);
            }
        }

        public virtual void Navigate(Option<IMenuItem> item)
        {
            void ExecuteAndHide(IMenuHandler h)
            {
                item.Iter(h.Execute);
                Hide();
            }

            this.LogDebug("Navigating to {}.", item.Map(v => v.ToString()).IfNone("(Root)"));

            MenuHandlers
                .Find(h => item.Exists(h.CanExecute))
                .BiIter(ExecuteAndHide, () => _current.OnNext(item));
        }

        protected virtual Option<IMenuItem> CreateItem(object item, Option<IMenuItem> parent)
        {
            Ensure.That(item, nameof(item)).IsNotNull();

            var label = Optional(item)
                .Bind(c => Renderers.Find(r => r.CanRender(c)).Map(r => r.Render(c)))
                .HeadOrNone();

            return label.Map<IMenuItem>(v => new MenuItem(v.Key, v.DisplayName, item, parent));
        }

        protected virtual IEnumerable<IMenuItem> CreateChildren(Option<IMenuItem> parent)
        {
            if (parent.IsNone) return _rootItems;

            var model = parent.Map(p => p.Model);
            var provider = StructureProviders.Find(p => model.Exists(p.HasChildren));

            this.LogDebug("Loading children for {}.", parent.Head());
            this.LogDebug("Using structure provider: {}.", provider);

            var children =
                from m in model
                from p in provider
                select p.FindChildren(m);

            return children.Flatten().Bind(child => CreateItem(child, parent));
        }

        public override void _UnhandledInput(InputEvent @event)
        {
            base._UnhandledInput(@event);

            if (@event.IsActionPressed(BackAction) && this.CanGoUp())
            {
                this.GoUp();

                GetTree().SetInputAsHandled();
            }
        }

        protected virtual MenuItemControl CreateItemControl(IMenuItem item, int index)
        {
            Ensure.That(item, nameof(item)).IsNotNull();

            var control = (MenuItemControl) ItemScene.Instance();
            var shortcut = (index + 1).ToString().Head();

            control.Model = Some(item);
            control.Shortcut = Some(shortcut);

            return control;
        }

        protected override void Dispose(bool disposing)
        {
            _current.CompleteAndDispose();

            base.Dispose(disposing);
        }
    }
}
