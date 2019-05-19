using System.Linq;
using System.Reactive.Linq;
using AlleyCat.Action;
using AlleyCat.Control;
using AlleyCat.Event;
using AlleyCat.Logging;
using EnsureThat;
using Godot;
using LanguageExt;
using Microsoft.Extensions.Logging;
using static LanguageExt.Prelude;

namespace AlleyCat.UI
{
    public class EntityLabel : UIControl
    {
        public const string DefaultKeyLabel = "?";

        public string InteractAction { get; }

        protected IPlayerControl PlayerControl { get; }

        protected Label TitleLabel { get; }

        protected Option<Label> ShortcutLabel { get; }

        protected Option<Label> ActionLabel { get; }

        protected Option<Godot.Control> ActionPanel { get; }

        protected ITimeSource TimeSource { get; }

        public EntityLabel(
            IPlayerControl playerControl,
            string interactAction,
            Label titleLabel,
            Option<Label> shortcutLabel,
            Option<Label> actionLabel,
            Option<Godot.Control> actionPanel,
            ITimeSource timeSource,
            Godot.Control node,
            ILoggerFactory loggerFactory) : base(node, loggerFactory)
        {
            Ensure.That(playerControl, nameof(playerControl)).IsNotNull();
            Ensure.That(interactAction, nameof(interactAction)).IsNotNull();
            Ensure.That(titleLabel, nameof(titleLabel)).IsNotNull();
            Ensure.That(timeSource, nameof(timeSource)).IsNotNull();

            PlayerControl = playerControl;
            InteractAction = interactAction;
            TitleLabel = titleLabel;
            ShortcutLabel = shortcutLabel;
            ActionLabel = actionLabel;
            ActionPanel = actionPanel;
            TimeSource = timeSource;
            InteractAction = interactAction;
        }

        protected override void PostConstruct()
        {
            base.PostConstruct();

            var onFocus = PlayerControl.OnFocusChange;
            var ticks = TimeSource.OnProcess;

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
                .Select(pos => new Vector2(pos.x - Node.RectSize.x / 2f, pos.y - Node.RectSize.y / 2f));

            var onDispose = Disposed.Where(identity);

            showTitle
                .TakeUntil(onDispose)
                .Subscribe(v => Node.Visible = v, this);

            ActionPanel.Iter(panel =>
            {
                showAction
                    .TakeUntil(onDispose)
                    .Subscribe(v => panel.Visible = v, this);
            });

            title
                .TakeUntil(onDispose)
                .Subscribe(v => TitleLabel.Text = v, this);

            ActionLabel.Iter(label =>
            {
                action
                    .TakeUntil(onDispose)
                    .Subscribe(a => a.Iter(v => label.Text = v), this);
            });

            position
                .TakeUntil(onDispose)
                .Subscribe(pos => Node.RectPosition = pos.Round(), this);

            var shortcut = InputMap
                .GetActionList(InteractAction)
                .OfType<InputEvent>()
                .Bind(e => e.FindKeyLabel())
                .HeadOrNone()
                .IfNone(DefaultKeyLabel);

            //TODO Handle key mapping changes.
            ShortcutLabel.Iter(l => l.Text = shortcut);
        }
    }
}
