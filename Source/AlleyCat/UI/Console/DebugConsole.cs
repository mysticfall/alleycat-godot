using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text.RegularExpressions;
using AlleyCat.Animation;
using AlleyCat.Common;
using AlleyCat.Event;
using EnsureThat;
using Godot;
using JetBrains.Annotations;
using LanguageExt;
using Microsoft.Extensions.Logging;
using static LanguageExt.Prelude;

namespace AlleyCat.UI.Console
{
    public class DebugConsole : UIControl, ICommandConsole
    {
        public const string ThemeType = "Message";

        public const string ShowAnimation = "Show";

        public const string HideAnimation = "Hide";

        public override bool Visible
        {
            get => base.Visible;
            set
            {
                if (Player.IsSome)
                {
                    PlayAnimation(Visible ? HideAnimation : ShowAnimation);
                }
                else
                {
                    Node.Visible = value;
                }
            }
        }

        public int BufferSize { get; }

        public Color TextColor => Node.GetColor("info", ThemeType);

        public Color HighlightColor => Node.GetColor("highlight", ThemeType);

        public Color WarningColor => Node.GetColor("warning", ThemeType);

        public Color ErrorColor => Node.GetColor("error", ThemeType);

        public IEnumerable<IConsoleCommand> SupportedCommands => _commands.Values;

        protected Option<AnimationPlayer> Player { get; }

        protected RichTextLabel Content { get; }

        protected LineEdit InputField { get; }

        private readonly Map<string, IConsoleCommand> _commands;

        private Option<Input.MouseMode> _mouseMode;

        public DebugConsole(
            IEnumerable<IConsoleCommandProvider> providers,
            Option<AnimationPlayer> player,
            int bufferSize,
            RichTextLabel content,
            LineEdit inputField,
            Godot.Control node,
            ILoggerFactory loggerFactory) : base(node, loggerFactory)
        {
            Ensure.That(providers, nameof(providers)).IsNotNull();
            Ensure.That(content, nameof(content)).IsNotNull();
            Ensure.That(inputField, nameof(inputField)).IsNotNull();

            _commands = providers.Bind(p => p.CreateCommands(this)).ToMap();

            Player = player;
            BufferSize = Math.Max(1, bufferSize);
            Content = content;
            InputField = inputField;
        }

        protected override void PostConstruct()
        {
            base.PostConstruct();

            Visible = false;

            Node.SetProcess(false);
            Node.SetPhysicsProcess(false);

            var disposed = Disposed.Where(identity);

            // Can't use ILoggable extension for Subscribe here, since ConsoleLogger depends on DebugConsole.
            void LogError(Exception e) => GD.PrintErr(e);

            InputField.OnUnhandledInput()
                .OfType<InputEventKey>()
                .Where(_ => Visible)
                .Where(e => e.Scancode == (int) KeyList.Space && e.Control && e.Pressed && !e.IsEcho())
                .Select(_ => InputField.Text.Substring(0, InputField.CaretPosition))
                .TakeUntil(disposed)
                .Subscribe(AutoComplete, LogError);

            InputField.OnTextEntered()
                .TakeUntil(disposed)
                .Subscribe(OnTextInput, LogError);

            var onAnimationFinish = Player.Map(p => p.OnAnimationFinish())
                .ToObservable()
                .Switch();

            onAnimationFinish
                .Where(a => a == ShowAnimation)
                .TakeUntil(disposed)
                .Subscribe(_ => OnShown(), LogError);

            onAnimationFinish
                .Where(a => a == HideAnimation)
                .TakeUntil(disposed)
                .Subscribe(_ => OnHidden(), LogError);

            Node.OnVisibilityChange()
                .Select(v => v ? HideAnimation : ShowAnimation)
                .TakeUntil(disposed)
                .Subscribe(PlayAnimation, LogError);

            Content.AddColorOverride("default_color", TextColor);
        }

        protected void OnShown()
        {
            Node.GetTree().Paused = true;

            _mouseMode = Input.GetMouseMode();

            Input.SetMouseMode(Input.MouseMode.Visible);
            InputField.GrabFocus();
        }

        protected void OnHidden()
        {
            _mouseMode.Iter(Input.SetMouseMode);
            _mouseMode = None;

            Node.GetTree().Paused = false;
        }

        private void PlayAnimation(string name)
        {
            Player.Filter(p => !p.IsPlaying()).Iter(player =>
            {
                InputField.Clear();
                player.Play(name);
            });
        }

        public IConsole Write(string text, TextStyle style)
        {
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
            _commands.Find(command).Match(
                action => action.Execute(arguments),
                () =>
                {
                    var message = string.Format(Translate("console.error.command.invalid"), command);

                    this.Warning(message).NewLine();
                }
            );
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

                var suggestions = candidates.Select(c => c.Substring(normalized.Length).Trim()).Freeze();

                this.Text(Translate("console.suggestions")).Text(": ");

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
    }
}
