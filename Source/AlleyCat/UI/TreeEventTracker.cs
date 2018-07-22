using System;
using System.Reactive.Subjects;
using AlleyCat.Event;
using EnsureThat;
using Godot;
using JetBrains.Annotations;

namespace AlleyCat.UI
{
    public class TreeEventTracker : EventTracker<Tree>
    {
        private const string SignalItemSelect = "item_selected";

        [NotNull]
        public IObservable<TreeItemSelectedEvent> OnItemSelect
        {
            get
            {
                if (_onItemSelect == null)
                {
                    Parent.Connect(SignalItemSelect, this, nameof(FireOnItemSelect));

                    _onItemSelect = new Subject<TreeItemSelectedEvent>();
                }

                return _onItemSelect;
            }
        }

        private Subject<TreeItemSelectedEvent> _onItemSelect;

        [UsedImplicitly]
        private void FireOnItemSelect() => _onItemSelect?.OnNext(new TreeItemSelectedEvent(Parent));

        protected override void Disconnect(Tree parent)
        {
            base.Disconnect(parent);

            Ensure.Any.IsNotNull(parent, nameof(parent));

            if (_onItemSelect != null)
            {
                parent.Disconnect(SignalItemSelect, this, nameof(FireOnItemSelect));

                _onItemSelect.Dispose();
                _onItemSelect = null;
            }
        }
    }
}
