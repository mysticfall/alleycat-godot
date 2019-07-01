using System;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using AlleyCat.Attribute;
using AlleyCat.Logging;
using Godot;
using LanguageExt;
using Microsoft.Extensions.Logging;
using static LanguageExt.Prelude;

namespace AlleyCat.UI.Widget
{
    public abstract class AttributeWidget : UIControl, IWidget
    {
        public Option<IAttribute> Attribute
        {
            get => _attribute.Value;
            set => _attribute.OnNext(value);
        }

        public IObservable<Option<IAttribute>> OnAttributeChange => _attribute.AsObservable();

        protected Option<Label> Label { get; }

        protected Option<Label> ValueLabel { get; }

        protected string ValueFormat { get; }

        protected Option<TextureRect> Icon { get; }

        protected virtual IObservable<float> OnValueChange =>
            OnAttributeChange
                .Select(a => a.Map(v => v.OnChange).ToObservable().Switch())
                .Switch();

        protected virtual IObservable<Option<string>> OnDisplayNameChange =>
            OnAttributeChange.Select(a => a.Map(v => v.DisplayName));

        protected virtual IObservable<Option<string>> OnTooltipChange =>
            OnAttributeChange.Select(a => a.Bind(v => v.Description));

        protected virtual IObservable<Option<Texture>> OnIconChange =>
            OnAttributeChange.Select(a => a.Bind(v => v.FindIcon(0)));

        private readonly BehaviorSubject<Option<IAttribute>> _attribute;

        public AttributeWidget(
            Option<IAttribute> attribute,
            Option<Label> label,
            Option<Label> valueLabel,
            Option<string> valueFormat,
            Option<TextureRect> icon,
            Godot.Control node,
            ILoggerFactory loggerFactory) : base(node, loggerFactory)
        {
            Label = label;
            ValueLabel = valueLabel;
            ValueFormat = valueFormat.IfNone("0");
            Icon = icon;

            _attribute = CreateSubject(attribute);
        }

        protected override void PostConstruct()
        {
            base.PostConstruct();

            var disposed = Disposed.Where(identity);

            Label.Iter(label =>
            {
                OnDisplayNameChange
                    .Where(_ => Visible)
                    .TakeUntil(disposed)
                    .Subscribe(v => label.Text = v.FirstOrDefault(), this);
            });

            ValueLabel.Iter(label =>
            {
                OnValueChange
                    .Where(_ => Visible)
                    .TakeUntil(disposed)
                    .Subscribe(v => label.Text = v.ToString(ValueFormat), this);
            });

            OnTooltipChange
                .Where(_ => Visible)
                .TakeUntil(disposed)
                .Subscribe(v => Node.HintTooltip = v.FirstOrDefault(), this);

            Icon.Iter(icon =>
            {
                OnIconChange
                    .Where(_ => Visible)
                    .TakeUntil(disposed)
                    .Subscribe(v => icon.Texture = v.FirstOrDefault(), this);
            });
        }
    }
}
