using System;
using System.Reactive.Linq;
using AlleyCat.Autowire;
using AlleyCat.Common;
using AlleyCat.Event;
using Godot;
using Microsoft.Extensions.Logging;

namespace AlleyCat.UI.Console
{
    public class Console : Panel
    {
        [Node("Container/ScrollPane")] private ScrollContainer _scrollPane;

        [Node("Container/ScrollPane/Content")] private RichTextLabel _content;

        [Node("AnimationPlayer")] private AnimationPlayer _player;

        [Service] private ILogger _logger;

        public override void _Ready()
        {
            base._Ready();

            this.Autowire();
           
            this.OnInput()
                .Where(e => e.IsActionPressed("ui_console"))
                .Select(_ => Visible ? "Hide" : "Show")
                .Subscribe(a => _player.Play(a))
                .AddTo(this);

            Visible = false;
        }

        protected void OnCloseButtonPressed() => _player.Play("Hide");
    }
}
