using System;
using System.Reactive.Linq;
using AlleyCat.Autowire;
using AlleyCat.Common;
using AlleyCat.Control;
using AlleyCat.Event;
using Godot;

namespace AlleyCat.UI
{
    public class EntityLabel : Panel
    {
        [Node]
        public Label Title { get; private set; }

        [Service]
        public IFocusTracker FocusTracker { get; private set; }

        public override void _Ready()
        {
            base._Ready();

            this.Autowire();

            var entity = FocusTracker.OnFocusChange;

            entity
                .Where(e => e != null)
                .Select(e => e.DisplayName)
                .Subscribe(name => Title.Text = name)
                .AddTo(this);

            this.OnProcess()
                .CombineLatest(entity.Where(e => e != null), (_, e) => e.LabelPosition)
                .Select(FocusTracker.Camera.UnprojectPosition)
                .Select(pos => new Vector2(pos.x - RectSize.x / 2f, pos.y - RectSize.y / 2f))
                .Subscribe(pos => RectPosition = pos)
                .AddTo(this);

            FocusTracker.OnFocusChange
                .Subscribe(e => Visible = e != null)
                .AddTo(this);
        }
    }
}
