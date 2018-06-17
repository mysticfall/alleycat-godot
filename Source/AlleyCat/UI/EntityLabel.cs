using System;
using System.Linq;
using System.Reactive.Linq;
using AlleyCat.Action;
using AlleyCat.Autowire;
using AlleyCat.Common;
using AlleyCat.Control;
using AlleyCat.Event;
using Godot;

namespace AlleyCat.UI
{
    public class EntityLabel : Panel
    {
        [Export]
        public string InteractAction { get; private set; } = "interact";

        [Export]
        public string DefaultKeyLabel { get; private set; } = "?";

        [Node("Container/Title")]
        protected Label Title { get; private set; }

        [Node("Container/Action")]
        protected Container ActionPanel { get; private set; }

        [Node("Container/Action/Shortcut")]
        protected Label Shortcut { get; private set; }

        [Node("Container/Action/Action Title")]
        protected Label ActionTitle { get; private set; }

        [Service]
        protected IPlayerControl PlayerControl { get; private set; }

        public override void _Ready()
        {
            base._Ready();

            this.Autowire();
        }

        [PostConstruct]
        protected virtual void OnInitialize()
        {
            var onFocus = PlayerControl.OnFocusChange;
            var ticks = this.OnProcess();

            var showTitle = onFocus.Select(e => e != null);
            var entity = onFocus.Where(e => e != null);
            var title = entity.Select(e => e.DisplayName);

            var action = ticks
                .CombineLatest(entity, (_, e) => e as IInteractable)
                .Select(i => i?.Actions.FirstOrDefault(a => a.AllowedFor(PlayerControl.Character)))
                .Select(a => a?.DisplayName);

            var showAction = action.Select(a => a != null);

            var position = ticks
                .CombineLatest(onFocus, (_, e) => e)
                .Where(e => e != null)
                .Select(e => PlayerControl.Camera.UnprojectPosition(e.LabelPosition))
                .Select(pos => new Vector2(pos.x - RectSize.x / 2f, pos.y - RectSize.y / 2f));

            showTitle
                .Subscribe(v => Visible = v)
                .AddTo(this);
            showAction
                .Subscribe(v => ActionPanel.Visible = v)
                .AddTo(this);

            title
                .Subscribe(v => Title.Text = v)
                .AddTo(this);
            action
                .Subscribe(v => ActionTitle.Text = v)
                .AddTo(this);

            position
                .Subscribe(pos => RectPosition = pos.Round())
                .AddTo(this);

            var shortcut = InputMap
                               .GetActionList(InteractAction)
                               .OfType<InputEvent>()
                               .Select(e => e.GetKeyLabel())
                               .FirstOrDefault() ?? DefaultKeyLabel;

            //TODO Handle key mapping changes.
            Shortcut.Text = shortcut;
        }
    }
}
