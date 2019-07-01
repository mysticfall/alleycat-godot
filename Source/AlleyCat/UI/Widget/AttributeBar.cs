using System;
using AlleyCat.Attribute;
using AlleyCat.Logging;
using EnsureThat;
using Godot;
using LanguageExt;
using static LanguageExt.Prelude;
using Microsoft.Extensions.Logging;
using static System.Reactive.Linq.Observable;

namespace AlleyCat.UI.Widget
{
    public class AttributeBar : AttributeWidget
    {
        protected ProgressBar ProgressBar { get; }

        protected virtual IObservable<float> OnMaxChange =>
            OnAttributeChange
                .Select(a => a.Bind(v => v.Max).Map(v => v.OnChange).ToObservable().Switch())
                .Switch();

        protected virtual IObservable<float> OnMinChange =>
            OnAttributeChange
                .Select(a => a.Bind(v => v.Min).Map(v => v.OnChange).ToObservable().Switch())
                .Switch();

        public AttributeBar(
            Option<IAttribute> attribute,
            Option<Label> label,
            Option<Label> valueLabel,
            Option<string> valueFormat,
            Option<TextureRect> icon,
            ProgressBar progressBar,
            Godot.Control node,
            ILoggerFactory loggerFactory) : base(attribute, label, valueLabel, valueFormat, icon, node, loggerFactory)
        {
            Ensure.That(progressBar, nameof(progressBar)).IsNotNull();

            ProgressBar = progressBar;
        }

        protected override void PostConstruct()
        {
            base.PostConstruct();

            var disposed = Disposed.Where(identity);

            OnValueChange
                .Where(_ => Visible)
                .TakeUntil(disposed)
                .Subscribe(v => ProgressBar.Value = v, this);

            OnMinChange
                .Where(_ => Visible)
                .TakeUntil(disposed)
                .Subscribe(v => ProgressBar.MinValue = v, this);

            OnMaxChange
                .Where(_ => Visible)
                .TakeUntil(disposed)
                .Subscribe(v => ProgressBar.MaxValue = v, this);
        }
    }
}
