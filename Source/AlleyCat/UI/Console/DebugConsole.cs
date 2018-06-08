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

namespace AlleyCat.UI.Console
{
    [AutowireContext, Singleton(typeof(IConsole), typeof(ICommandConsole), typeof(DebugConsole))]
    public class DebugConsole : Panel, ICommandConsole, IHideableUI
    {
        public const string ThemeType = "Console";

        public const string ShowAnimation = "Show";

        public const string HideAnimation = "Hide";

        [Export]
        public int BufferSize { get; set; } = 300;

        [Export, NotNull] public string ToggleAction = "ui_console";

        public Color TextColor => GetColor("info", ThemeType);

        public Color HighlightColor => GetColor("highlight", ThemeType);

        public Color WarningColor => GetColor("warning", ThemeType);

        public Color ErrorColor => GetColor("error", ThemeType);

        public IEnumerable<IConsoleCommand> SupportedCommands => _commandMap.Values;

        [Node("Container/Content")]
        protected RichTextLabel Content { get; private set; }

        [Node("Container/InputPane/Input")]
        protected LineEdit InputField { get; private set; }

        [Node("AnimationPlayer")]
        protected AnimationPlayer Player { get; private set; }

        [Service] private IEnumerable<IConsoleCommandProvider> _providers;

        private readonly IDictionary<string, IConsoleCommand> _commandMap;

        private Input.MouseMode _mouseMode;

        public DebugConsole()
        {
            _commandMap = new SortedDictionary<string, IConsoleCommand>();
        }

        [PostConstruct]
        private void OnInitialize()
        {
            Visible = false;

            SetProcess(false);
            SetPhysicsProcess(false);

            var commands = _providers.SelectMany(p => p.CreateCommands(this));

            _commandMap.Clear();

            foreach (var command in commands)
            {
                _commandMap.Add(command.Key, command);
            }

            this.OnInput()
                .Where(e => e.IsActionPressed(ToggleAction) && !e.IsEcho())
                .Subscribe(_ =>
                {
                    GetTree().SetInputAsHandled();
                    this.Toggle();
                })
                .AddTo(this);

            this.OnUnhandledInput()
                .OfType<InputEventKey>()
                .Where(e => e.Scancode == (int) KeyList.Space && e.Control && e.Pressed && !e.IsEcho())
                .Select(_ => InputField.Text.Substring(0, InputField.CaretPosition))
                .Subscribe(AutoComplete)
                .AddTo(this);

            Player.OnAnimationFinish()
                .Where(e => e.Animation == ShowAnimation)
                .Subscribe(_ => OnShown())
                .AddTo(this);

            Player.OnAnimationFinish()
                .Where(e => e.Animation == HideAnimation)
                .Subscribe(_ => OnHidden())
                .AddTo(this);

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
            Ensure.Any.IsNotNull(text, nameof(text));

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

        public void Execute(string command, string[] arguments = null)
        {
            Ensure.String.IsNotNullOrWhiteSpace(command, nameof(command));

            if (_commandMap.TryGetValue(command, out var action))
            {
                action.Execute(arguments);
            }
            else
            {
                var message = string.Format(Tr("console.error.command.invalid"), command);

                this.Warning(message).NewLine();
            }

            NewLine();
        }

        private void AutoComplete(string text)
        {
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

                var suggestions = candidates.Select(c => c.Substring(normalized.Length).Trim()).ToList();

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
            if (string.IsNullOrWhiteSpace(text))
            {
                return SupportedCommands.Select(c => c.Key);
            }

            var segments = text
                .Split(' ')
                .Select(s => s.Trim())
                .ToList();

            var command = segments.FirstOrDefault();

            if (command == null || segments.Count < 1)
            {
                return Enumerable.Empty<string>();
            }

            if (!command.EndsWith(" ") && segments.Count == 1)
            {
                return SupportedCommands.Where(c => c.Key.StartsWith(command)).Select(c => c.Key);
            }

            var arguments = string.Join(" ", segments.Skip(1));

            return SupportedCommands
                .Where(c => c.Key == command)
                .OfType<IAutoCompletionSupport>()
                .SelectMany(c => c.SuggestCandidates(arguments))
                .Select(s => string.Join(" ", command, s));
        }

        [UsedImplicitly]
        protected void OnTextInput([NotNull] string line)
        {
            if (string.IsNullOrWhiteSpace(line))
            {
                return;
            }

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
