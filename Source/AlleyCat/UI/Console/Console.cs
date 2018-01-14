using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using AlleyCat.Autowire;
using AlleyCat.Common;
using AlleyCat.Event;
using EnsureThat;
using Godot;
using JetBrains.Annotations;

namespace AlleyCat.UI.Console
{
    public class Console : Panel, IConsole
    {
        public const string ShowAnimation = "Show";

        public const string HideAnimation = "Hide";

        [Export] public int BufferSize = 300;

        [Export, NotNull] public string ToggleAction = "ui_console";

        public IEnumerable<IConsoleCommand> SupportedCommands => _commands.Values;

        public IObservable<Unit> OnShow { get; private set; }

        public IObservable<Unit> OnHide { get; private set; }

        public IObservable<bool> OnVisibilityChange { get; }

        [Node("Container/Content")]
        protected RichTextLabel Content { get; private set; }

        [Node("Container/InputPane/Input")]
        protected LineEdit Input { get; private set; }

        [Node("AnimationPlayer")]
        protected AnimationPlayer Player { get; private set; }

        private readonly IDictionary<string, IConsoleCommand> _commands;

        public Console()
        {
            _commands = new Dictionary<string, IConsoleCommand>();

            _commands.Add(HelpCommand.Key, new HelpCommand());
        }

        public override void _Ready()
        {
            base._Ready();

            Visible = false;

            SetProcess(false);
            SetPhysicsProcess(false);

            this.Autowire();

            this.OnInput()
                .Where(e => e.IsActionPressed(ToggleAction))
                .Where(_ => !Player.IsPlaying())
                .Select(_ => Visible ? HideAnimation : ShowAnimation)
                .Subscribe(PlayAnimation)
                .AddTo(this);

            var onAnimationFinish = Player.OnAnimationFinish();

            OnShow = onAnimationFinish
                .Where(e => e.Animation == ShowAnimation)
                .AsUnitObservable();
            OnHide = onAnimationFinish
                .Where(e => e.Animation == HideAnimation)
                .AsUnitObservable();

            OnShow
                .Subscribe(_ => Input.GrabFocus())
                .AddTo(this);
        }

        public new void Show() => PlayAnimation(ShowAnimation);

        public new void Hide() => PlayAnimation(HideAnimation);

        private void PlayAnimation(string name)
        {
            if (!Player.IsPlaying())
            {
                Input.Clear();

                Player.Play(name);
            }
        }

        public IConsole Write(string text)
        {
            Ensure.Any.IsNotNull(text, nameof(text));

            Content.AddText(text);

            return this;
        }

        public IConsole WriteLine(string text)
        {
            Ensure.Any.IsNotNull(text, nameof(text));

            Content.AddText(text);
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
            Input.Clear();
            Content.Clear();
        }

        public void Execute(string command, string[] arguments = null)
        {
            Ensure.String.IsNotNullOrWhiteSpace(command, nameof(command));

            if (_commands.TryGetValue(command, out var action))
            {
                action.Execute(arguments, this);
            }
            else
            {
                var color = Theme.GetColor("error", "Console");

                Content.PushColor(color);

                WriteLine($"Unknown command: '{command}'.");

                Content.Pop();
            }
        }

        [UsedImplicitly]
        protected void OnTextInput([NotNull] string line)
        {
            WriteLine(line);

            Input.Clear();

            var segments = line.Split();

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
