using System;
using System.Linq;
using System.Reactive.Linq;
using AlleyCat.Action;
using AlleyCat.Autowire;
using AlleyCat.Character;
using AlleyCat.Common;
using AlleyCat.Control;
using AlleyCat.Event;
using Godot;
using LanguageExt;

namespace AlleyCat.UI
{
    public class EntityLabel : Panel
    {
        public string InteractAction => _interactAction.TrimToOption().Head();

        public string DefaultKeyLabel => _defaultKeyLabel.TrimToOption().Head();

        protected Label Title => _title.Head();

        protected Container ActionPanel => _actionPanel.Head();

        protected Label Shortcut => _shortcut.Head();

        protected Label ActionTitle => _actionTitle.Head();

        protected Option<IHumanoid> Player => _playerControl.Bind(p => p.Character);

        protected IPlayerControl PlayerControl => _playerControl.Head();

        [Export] private string _interactAction = "interact";

        [Export] private string _defaultKeyLabel = "?";

        [Node("Container/Title")] private Option<Label> _title;

        [Node("Container/Action")] private Option<Container> _actionPanel;

        [Node("Container/Action/Shortcut")] private Option<Label> _shortcut;

        [Node("Container/Action/Action Title")]
        private Option<Label> _actionTitle;

        [Service] private Option<IPlayerControl> _playerControl;

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
                .Select(target => Player
                    .SelectMany(p => p.FindAction(new InteractionContext(p, target), a => a is Interaction))
                    .Select(a => a.DisplayName)
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
