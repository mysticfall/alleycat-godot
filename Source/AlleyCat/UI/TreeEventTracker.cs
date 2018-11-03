using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using AlleyCat.Common;
using AlleyCat.Event;
using Godot;
using JetBrains.Annotations;
using LanguageExt;
using static LanguageExt.Prelude;

namespace AlleyCat.UI
{
    public class TreeEventTracker : EventTracker<Tree>
    {
        private const string SignalItemSelect = "item_selected";

        public IObservable<TreeItemSelectedEvent> OnItemSelect
        {
            get
            {
                if (_onItemSelect.IsNone)
                {
                    Parent.Iter(p => p.Connect(SignalItemSelect, this, nameof(FireOnItemSelect)));

                    _onItemSelect = new Subject<TreeItemSelectedEvent>();
                }

                return _onItemSelect.Head().AsObservable();
            }
        }

        private Option<Subject<TreeItemSelectedEvent>> _onItemSelect;

        [UsedImplicitly]
        private void FireOnItemSelect() => _onItemSelect
            .SelectMany(o => Parent, (o, p) => (o, e: new TreeItemSelectedEvent(p)))
            .Iter(t => t.o.OnNext(t.e));

        protected override void Disconnect(Tree parent)
        {
            base.Disconnect(parent);

            _onItemSelect.Iter(p =>
            {
                parent.Disconnect(SignalItemSelect, this, nameof(FireOnItemSelect));
                p.DisposeQuietly();
            });

            _onItemSelect = None;
        }
    }
}
