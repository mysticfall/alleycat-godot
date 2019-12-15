using System;
using System.Reactive.Linq;
using AlleyCat.Autowire;
using AlleyCat.Control;
using AlleyCat.Game;
using Godot;
using JetBrains.Annotations;
using LanguageExt;
using static LanguageExt.Prelude;

namespace AlleyCat.UI.Widget
{
    public abstract class AttributeWidgetFactory<T> : DelegateNodeFactory<T, Godot.Control> where T : AttributeWidget
    {
        [Export]
        public string Attribute { get; set; }

        [Node]
        public Option<Label> Label { get; set; }

        [Node]
        public Option<TextureRect> Icon { get; set; }

        [Node]
        public Option<Label> ValueLabel { get; set; }

        [Export]
        public string ValueFormat { get; set; } = "0";

        [Service]
        public Option<IPlayerControl> PlayerControl { get; set; }

        [Export, UsedImplicitly] private NodePath _label = "../Container/Label";

        [Export, UsedImplicitly] private NodePath _icon = "../Container/Icon";

        [Export, UsedImplicitly] private NodePath _valueLabel = "../Container/Value";

        protected override void PostConstruct()
        {
            base.PostConstruct();

            Unit OnAttributeChange(IPlayerControl control, T service)
            {
                var attribute = control.OnCharacterChange
                    .Select(c => c.Bind(v => v.Attributes.TryGetValue(Attribute)));

                attribute
                    .TakeUntil(Disposed.Where(identity))
                    .Subscribe(a => Service.Iter(s => s.Attribute = a));

                return Unit.Default;
            }

            PlayerControl.SelectMany(_ => Service.ToOption(), OnAttributeChange);
        }
    }
}
