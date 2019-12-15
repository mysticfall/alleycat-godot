using System;
using System.Collections;
using System.Collections.Generic;
using System.Reactive.Linq;
using AlleyCat.Common;
using AlleyCat.Game;
using EnsureThat;
using LanguageExt;
using Microsoft.Extensions.Logging;

namespace AlleyCat.Attribute
{
    public class AttributeSet : GameNode, IAttributeSet
    {
        public IAttributeHolder Holder { get; }

        public IObservable<IAttribute> OnChange { get; }

        public IEnumerator<KeyValuePair<string, IAttribute>> GetEnumerator() =>
            _attributes.Map(p => new KeyValuePair<string, IAttribute>(p.Item1, p.Item2)).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public int Count => _attributes.Count;

        public bool ContainsKey(string key) => _attributes.ContainsKey(key);

        public bool TryGetValue(string key, out IAttribute value)
        {
            Ensure.That(key, nameof(key)).IsNotNull();

            if (!_attributes.ContainsKey(key))
            {
                value = null;
                return false;
            }

            value = _attributes[key];
            return true;
        }

        public IAttribute this[string key] => _attributes[key];

        public IEnumerable<string> Keys => _attributes.Keys;

        public IEnumerable<IAttribute> Values => _attributes.Values;

        private readonly Map<string, IAttribute> _attributes;

        public AttributeSet(
            IEnumerable<IAttribute> attributes,
            IAttributeHolder holder,
            ILoggerFactory loggerFactory) : base(loggerFactory)
        {
            Ensure.That(attributes, nameof(attributes)).IsNotNull();
            Ensure.That(holder, nameof(holder)).IsNotNull();

            Holder = holder;

            _attributes = attributes.ToMap();

            OnChange = _attributes.Values.Map(a => a.OnChange.Select(_ => a)).Merge();
        }

        protected override void PostConstruct()
        {
            base.PostConstruct();

            Values.Iter(m => m.Initialize(Holder));
        }

        protected override void PreDestroy()
        {
            base.PreDestroy();

            Values.Iter(a => a.DisposeQuietly());
        }
    }
}
