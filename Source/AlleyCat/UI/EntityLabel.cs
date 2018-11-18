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
        public string InteractAction { get; set; } = "interact";

        [Export]
        public string DefaultKeyLabel { get; set; } = "?";

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

            var showTitle = onFocus.Select(e => e.IsSome);
            var entity = onFocus.Where(e => e.IsSome).Select(e => e.First());
            var title = entity.Select(e => e.DisplayName);

            var action = ticks
                .CombineLatest(entity, (_, e) => e)
                .Select(target => PlayerControl.Character
                    .Bind(p => p.FindAction(new InteractionContext(p, target), a => a is Interaction))
                    .Map(a => a.DisplayName)
                    .HeadOrNone());

            var showAction = action.Select(a => a.IsSome);

            var position = ticks
                .CombineLatest(entity, (_, e) => e)
                .Select(e => PlayerControl.Camera.UnprojectPosition(e.LabelPosition))
                .Select(pos => new Vector2(pos.x - RectSize.x / 2f, pos.y - RectSize.y / 2f));

            showTitle
                .Subscribe(v => Visible = v)
                .AddTo(this.GetCollector());
            showAction
                .Subscribe(v => ActionPanel.Visible = v)
                .AddTo(this.GetCollector());

            title
                .Subscribe(v => Title.Text = v)
                .AddTo(this.GetCollector());
            action
                .Subscribe(a => a.Iter(ActionTitle.SetText))
                .AddTo(this.GetCollector());

            position
                .Subscribe(pos => RectPosition = pos.Round())
                .AddTo(this.GetCollector());

            var shortcut = InputMap
                .GetActionList(InteractAction)
                .OfType<InputEvent>()
                .Bind(e => e.FindKeyLabel())
                .HeadOrNone()
                .IfNone(DefaultKeyLabel);

            //TODO Handle key mapping changes.
            Shortcut.Text = shortcut;
        }
    }
}
