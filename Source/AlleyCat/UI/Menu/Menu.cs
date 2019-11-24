using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using AlleyCat.Common;
using AlleyCat.Event;
using AlleyCat.Logging;
using EnsureThat;
using Godot;
using LanguageExt;
using Microsoft.Extensions.Logging;
using static LanguageExt.Prelude;

namespace AlleyCat.UI.Menu
{
    public class Menu : UIControl, IMenu
    {
        public Option<IMenuModel> Current => _current.Value;

        public IObservable<Option<IMenuModel>> OnNavigate => _current.AsObservable();

        public IEnumerable<IMenuModel> Items { get; private set; }

        public IObservable<IEnumerable<IMenuModel>> OnItemsChange => OnNavigate.Select(CreateChildren);

        protected IEnumerable<IMenuModel> RootItems { get; }

        protected IEnumerable<IMenuRenderer> Renderers { get; }

        protected IEnumerable<IMenuStructureProvider> StructureProviders { get; }

        protected IEnumerable<IMenuHandler> MenuHandlers { get; }

        protected Option<Label> Breadcrumb { get; }

        protected Option<ActionLabel> UpLabel { get; }

        protected Option<ActionLabel> CloseLabel { get; }

        protected Option<Label> EmptyLabel { get; }

        protected Node ItemsContainer { get; }

        protected PackedScene ItemScene { get; }

        protected Option<string> BackAction { get; }

        private readonly BehaviorSubject<Option<IMenuModel>> _current;

        public Menu(
            IEnumerable<IMenuModel> rootItems,
            IEnumerable<IMenuHandler> menuHandlers,
            IEnumerable<IMenuStructureProvider> structureProviders,
            IEnumerable<IMenuRenderer> renderers,
            Option<string> backAction,
            Godot.Control node,
            Node itemsContainer,
            Option<ActionLabel> closeLabel,
            Option<ActionLabel> upLabel,
            Option<Label> emptyLabel,
            Option<Label> breadcrumb,
            PackedScene itemScene,
            ILoggerFactory loggerFactory) : base(node, loggerFactory)
        {
            Ensure.That(renderers, nameof(renderers)).IsNotNull();

            RootItems = rootItems;
            MenuHandlers = menuHandlers;
            StructureProviders = structureProviders;
            Renderers = renderers.Append(new FallbackRenderer());

            Ensure.Enumerable.HasItems(RootItems, nameof(rootItems));
            Ensure.Enumerable.HasItems(MenuHandlers, nameof(menuHandlers));
            Ensure.Enumerable.HasItems(StructureProviders, nameof(structureProviders));

            Ensure.That(itemsContainer, nameof(itemsContainer)).IsNotNull();
            Ensure.That(itemScene, nameof(itemScene)).IsNotNull();

            BackAction = backAction;
            ItemsContainer = itemsContainer;
            CloseLabel = closeLabel;
            UpLabel = upLabel;
            EmptyLabel = emptyLabel;
            Breadcrumb = breadcrumb;
            ItemScene = itemScene;

            _current = CreateSubject<Option<IMenuModel>>(None);
        }

        protected override void PostConstruct()
        {
            base.PostConstruct();

            var disposed = Disposed.Where(identity);

            OnItemsChange
                .Do(items => Items = items)
                .TakeUntil(disposed)
                .Subscribe(HandleItemsChange, this);

            OnNavigate
                .Do(_ => UpLabel.Iter(l => l.Active = this.CanGoUp()))
                .Select(i => i.Bind(v => v.GetPath()).Reverse())
                .Select(p => string.Join(" > ", p.Map(v => v.DisplayName)))
                .TakeUntil(disposed)
                .Subscribe(v => Breadcrumb.Iter(b => b.Text = v), this);

            UpLabel
                .Map(l => l.OnAction)
                .ToObservable()
                .Switch()
                .TakeUntil(disposed)
                .Subscribe(_ => this.GoUp());
            CloseLabel
                .Map(l => l.OnAction)
                .ToObservable()
                .Switch()
                .TakeUntil(disposed)
                .Subscribe(_ => this.Hide());

            Node.OnVisibilityChange()
                .StartWith(Visible)
                .TakeUntil(disposed)
                .Subscribe(OnVisibilityChanged, this);

            if (BackAction.IsSome)
            {
                Node.OnUnhandledInput()
                    .Where(e => BackAction.Exists(v => e.IsActionPressed(v)) && this.CanGoUp())
                    .TakeUntil(disposed)
                    .Do(_ => Node.GetTree().SetInputAsHandled())
                    .Subscribe(_ => this.GoUp(), this);
            }
        }

        private void HandleItemsChange(IEnumerable<IMenuModel> items)
        {
            ItemsContainer.GetChildren()
                .OfType<Node>()
                .Filter(c => !EmptyLabel.Contains(c))
                .Iter(ItemsContainer.FreeChild);

            bool IsValid(IMenuModel item)
            {
                var hasChildren = StructureProviders.Exists(p => p.HasChildren(item.Model));
                var executable = MenuHandlers.Exists(h => h.CanExecute(item));

                return hasChildren || executable;
            }

            var list = items.Filter(IsValid).Freeze();

            list.Iter((index, item) => CreateItemControl(item, index, ItemsContainer));

            EmptyLabel.Iter(v => v.Visible = !list.Any());
        }

        private void OnVisibilityChanged(bool visible)
        {
            this.ToTop();

            Node.SetProcessUnhandledInput(visible);

            UpLabel.Iter(l => l.Active = visible && this.CanGoUp());
            CloseLabel.Iter(l => l.Active = visible);

            if (!visible)
            {
                ItemsContainer.GetChildren()
                    .OfType<Node>()
                    .Filter(c => !EmptyLabel.Contains(c))
                    .Iter(ItemsContainer.FreeChild);
            }
        }

        public virtual void Navigate(Option<IMenuModel> item)
        {
            void ExecuteAndHide(IMenuHandler h)
            {
                item.Iter(h.Execute);
                this.Hide();
            }

            this.LogDebug("Navigating to {}.", item.Map(v => v.ToString()).IfNone("(Root)"));

            MenuHandlers
                .Find(h => item.Exists(h.CanExecute))
                .BiIter(ExecuteAndHide, () => _current.OnNext(item));
        }

        protected virtual Option<IMenuModel> CreateItem(object item, Option<IMenuModel> parent)
        {
            Ensure.That(item, nameof(item)).IsNotNull();

            var label = Optional(item)
                .Bind(c => Renderers.Find(r => r.CanRender(c)).Map(r => r.Render(c)))
                .HeadOrNone();

            return label.Map<IMenuModel>(v => new MenuModel(v.Key, v.DisplayName, item, parent));
        }

        protected virtual IEnumerable<IMenuModel> CreateChildren(Option<IMenuModel> parent)
        {
            if (parent.IsNone) return RootItems;

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

        protected virtual Option<MenuItem> CreateItemControl(IMenuModel item, int index, Node parent)
        {
            Ensure.That(item, nameof(item)).IsNotNull();
            Ensure.That(parent, nameof(parent)).IsNotNull();

            var node = ItemScene.Instance();

            ItemsContainer.AddChild(node);

            var control = node.FindComponent<MenuItem>();

            control.Match(
                c =>
                {
                    var shortcut = (index + 1).ToString().Head();

                    c.Model = Some(item);
                    c.Shortcut = Some(shortcut);
                },
                () => Logger.LogWarning("Failed to create menu item instance.")
            );

            return control;
        }

        public class FallbackRenderer : IMenuRenderer
        {
            public bool CanRender(object item) => item is INamed;

            public INamed Render(object item) => (INamed) item;
        }
    }
}
