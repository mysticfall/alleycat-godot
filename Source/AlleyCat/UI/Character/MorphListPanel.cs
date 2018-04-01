using System;
using System.Linq;
using AlleyCat.Autowire;
using AlleyCat.Character.Morph;
using AlleyCat.Common;
using Godot;
using JetBrains.Annotations;

namespace AlleyCat.UI.Character
{
    [Singleton(typeof(MorphListPanel))]
    public class MorphListPanel : Panel
    {
        [Node]
        public IMorphableCharacter Character { get; private set; }

        [Node]
        protected TabContainer TabContainer { get; private set; }

        [Export, UsedImplicitly] private PackedScene _groupPanelScene;

        public override void _Ready()
        {
            base._Ready();

            this.Autowire();
        }

        protected virtual void LoadMorphs(IMorphSet morphSet)
        {
            var index = 0;

            foreach (var group in morphSet.Groups)
            {
                var tab = (MorphGroupPanel) _groupPanelScene.Instance();

                tab.LoadGroup(group, morphSet.AsEnumerable().Where(m => m.Definition.Group == group));

                TabContainer.AddChild(tab);
                TabContainer.SetTabTitle(index, group.DisplayName);

                index++;
            }
        }

        [PostConstruct]
        protected virtual void OnInitialized()
        {
            Character
                .OnMorphsChange
                .Subscribe(LoadMorphs)
                .AddTo(this);
        }
    }
}
