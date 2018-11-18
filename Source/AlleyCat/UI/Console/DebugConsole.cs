using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text.RegularExpressions;
using AlleyCat.Animation;
using AlleyCat.Autowire;
using AlleyCat.Common;
using AlleyCat.Event;
using EnsureThat;
using Godot;
using JetBrains.Annotations;
using LanguageExt;
using static LanguageExt.Prelude;

namespace AlleyCat.UI.Console
{
    [AutowireContext, Singleton(typeof(IConsole), typeof(ICommandConsole), typeof(DebugConsole))]
    public class DebugConsole : Panel, ICommandConsole, IHideableUI
    {
        public const string ThemeType = "Console";

        public const string ShowAnimation = "Show";

        public const string HideAnimation = "Hide";

        [Export]
        public int BufferSize
        {
            get => _bufferSize;
            set => _bufferSize = Mathf.Max(1, value);
        }

        public Color TextColor => GetColor("info", ThemeType);

        public Color HighlightColor => GetColor("highlight", ThemeType);

        public Color WarningColor => GetColor("warning", ThemeType);

        public Color ErrorColor => GetColor("error", ThemeType);

        public IEnumerable<IConsoleCommand> SupportedCommands => _commands.Values;

        [Node("AnimationPlayer")]
        protected AnimationPlayer Player { get; private set; }

        [Node("Container/Content")]
        protected RichTextLabel Content { get; private set; }

        [Node("Container/InputPane/Input")]
        protected LineEdit InputField { get; private set; }

        [Service] private IEnumerable<IConsoleCommandProvider> _providers = Seq<IConsoleCommandProvider>();

        private Map<string, IConsoleCommand> _commands = Map<string, IConsoleCommand>();

        private int _bufferSize = 300;

        private Input.MouseMode _mouseMode;

        [PostConstruct]
        private void OnInitialize()
        {
            Visible = false;

            SetProcess(false);
            SetPhysicsProcess(false);

            _commands = _providers.Bind(p => p.CreateCommands(this)).ToMap();

            InputField.OnUnhandledInput()
                .OfType<InputEventKey>()
                .Where(_ => Visible)
                .Where(e => e.Scancode == (int) KeyList.Space && e.Control && e.Pressed && !e.IsEcho())
                .Select(_ => InputField.Text.Substring(0, InputField.CaretPosition))
                .Subscribe(AutoComplete)
                .AddTo(this.GetCollector());

            Player.OnAnimationFinish()
                .Where(e => e.Animation == ShowAnimation)
                .Subscribe(_ => OnShown())
                .AddTo(this.GetCollector());

            Player.OnAnimationFinish()
                .Where(e => e.Animation == HideAnimation)
                .Subscribe(_ => OnHidden())
                .AddTo(this.GetCollector());

            Content.AddColorOverride("default_color", TextColor);
        }

        public new void Show()
        {
            if (!Visible) PlayAnimation(ShowAnimation);
        }

        public new void Hide()
        {
            if (Visible) PlayAnimation(HideAnimation);
        }

        protected void OnShown()
        {
            GetTree().SetPause(true);

            _mouseMode = Input.GetMouseMode();

            Input.SetMouseMode(Input.MouseMode.Visible);
            InputField.GrabFocus();
        }

        protected void OnHidden()
        {
            Input.SetMouseMode(_mouseMode);

            GetTree().SetPause(false);
        }

        private void PlayAnimation(string name)
        {
            if (Player.IsPlaying()) return;

            InputField.Clear();
            Player.Play(name);
        }

        public IConsole Write(string text, TextStyle style)
        {
            Ensure.That(text, nameof(text)).IsNotNull();

            style.Write(text, Content);

            return this;
        }

        public IConsole NewLine()
        {
            Content.Newline();

            AdjustBuffer();

            return this;
        }

        public void Clear()
        {
            InputField.Clear();
            Content.Clear();
        }

        public void Execute(string command, params string[] arguments)
        {
            Ensure.That(command, nameof(command)).IsNotNull();

            _commands.Find(command).Match(
                action => action.Execute(arguments),
                () =>
                {
                    var message = string.Format(Tr("console.error.command.invalid"), command);

                    this.Warning(message).NewLine();
                }
            );
        }

        private void AutoComplete(string text)
        {
            Ensure.That(text, nameof(text)).IsNotNull();

            var candidates = SuggestCandidates(text).ToList();

            if (candidates.Count == 1)
            {
                var suggestion = candidates.First();

                InputField.Text = suggestion;
                InputField.CaretPosition = suggestion.Length;
            }
            else if (candidates.Count > 1)
            {
                var normalized = Regex.Replace(text, @"\s+", " ");

                InputField.Text = normalized;
                InputField.CaretPosition = normalized.Length;

                var lastSpace = normalized.LastIndexOf(' ');
                var prefix = lastSpace > 0 ? normalized.Right(lastSpace) : normalized;

                var suggestions = candidates.Select(c => c.Substring(normalized.Length).Trim()).Freeze();

                this.Text(Tr("console.suggestions")).Text(": ");

                var first = true;

                foreach (var suggestion in suggestions)
                {
                    if (first)
                    {
                        first = false;
                    }
                    else
                    {
                        this.Text(" ");
                    }

                    this.Text(prefix).Highlight(suggestion);
                }

                NewLine();
                NewLine();
            }
        }

        public IEnumerable<string> SuggestCandidates(string text)
        {
            Ensure.That(text, nameof(text)).IsNotNull();

            if (text.Empty())
            {
                return SupportedCommands.Select(c => c.Key);
            }

            var segments = text
                .Split(' ')
                .Select(s => s.Trim())
                .ToList();

            var command = segments.HeadOrNone();

            if (command.IsNone)
            {
                return Enumerable.Empty<string>();
            }

            if (!command.Exists(c => c.EndsWith(" ")) && segments.Count == 1)
            {
                return SupportedCommands
                    .Where(c => command.Exists(c.Key.StartsWith))
                    .Select(c => c.Key);
            }

            var arguments = string.Join(" ", segments.Tail());

            return SupportedCommands
                .Where(c => command.Contains(c.Key))
                .OfType<IAutoCompletionSupport>()
                .Bind(c => c.SuggestCandidates(arguments))
                .Map(s => string.Join(" ", command.Head(), s));
        }

        [UsedImplicitly]
        protected void OnTextInput(string line)
        {
            Ensure.That(line, nameof(line)).IsNotNull();

            if (line.Empty()) return;

            this.Highlight(line).NewLine().NewLine();

            InputField.Clear();

            var segments = line.Split().Select(w => w.Trim()).Where(w => !w.Empty()).ToList();

            Execute(segments.First(), segments.Skip(1).ToArray());
        }

        private void AdjustBuffer()
        {
            int count;

            while ((count = Content.GetLineCount() - BufferSize - 1) > 0)
            {
                Content.RemoveLine(count - 1);
            }
        }

        public override void _Ready() => this.Autowire();
    }
}
