using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using AlleyCat.Animation;
using AlleyCat.Autowire;
using AlleyCat.Common;
using AlleyCat.Event;
using EnsureThat;
using Godot;
using JetBrains.Annotations;

namespace AlleyCat.UI.Console
{
    [AutowireContext, Singleton(typeof(IConsole))]
    public class Console : Panel, IConsole
    {
        public const string ShowAnimation = "Show";

        public const string HideAnimation = "Hide";

        [Export]
        public int BufferSize { get; set; } = 300;

        [Export, NotNull] public string ToggleAction = "ui_console";

        public Color TextColor => GetColor("info", GetType().Name);

        public Color HighlightColor => GetColor("highlight", GetType().Name);

        public Color WarningColor => GetColor("warning", GetType().Name);

        public Color ErrorColor => GetColor("error", GetType().Name);

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

        public Console()
        {
            _commandMap = new Dictionary<string, IConsoleCommand>();
        }

        [PostConstruct]
        private void OnInitialize()
        {
            Visible = false;

            SetProcess(false);
            SetPhysicsProcess(false);

            var commands = _providers.SelectMany(p => p.Commands);

            _commandMap.Clear();

            foreach (var command in commands)
            {
                _commandMap.Add(command.Key, command);
            }

            this.OnInput()
                .Where(e => e.IsActionPressed(ToggleAction))
                .Subscribe(_ => Toggle())
                .AddTo(this);

            Player.OnAnimationFinish()
                .Where(e => e.Animation == ShowAnimation)
                .AsUnitObservable()
                .Subscribe(_ => OnShown())
                .AddTo(this);

            Player.OnAnimationFinish()
                .Where(e => e.Animation == HideAnimation)
                .AsUnitObservable()
                .Subscribe(_ => OnHidden())
                .AddTo(this);

            Content.AddColorOverride("default_color", TextColor);
        }

        public void Open()
        {
            if (!Visible) PlayAnimation(ShowAnimation);
        }

        public void Close()
        {
            if (Visible) PlayAnimation(HideAnimation);
        }

        public void Toggle()
        {
            if (Visible)
            {
                Close();
            }
            else
            {
                Open();
            }
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

        public IConsole Write(string text) => Write(text, new TextStyle());

        public IConsole Write(string text, TextStyle style)
        {
            Ensure.Any.IsNotNull(text, nameof(text));

            style.Write(text, Content);

            return this;
        }

        public IConsole WriteLine(string text) => WriteLine(text, new TextStyle());

        public IConsole WriteLine(string text, TextStyle style)
        {
            Ensure.Any.IsNotNull(text, nameof(text));

            style.Write(text, Content);

            Content.Newline();

            AdjustBuffer();

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
                action.Execute(arguments, this);
            }
            else
            {
                WriteLine(
                    string.Format(Tr("console.error.command.invalid"), command),
                    new TextStyle(WarningColor));
            }

            NewLine();
        }

        [UsedImplicitly]
        protected void OnTextInput([NotNull] string line)
        {
            if (string.IsNullOrWhiteSpace(line))
            {
                return;
            }

            WriteLine(line, new TextStyle(HighlightColor));

            NewLine();

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
