using System.Diagnostics;
using AlleyCat.Autowire;
using AlleyCat.Character.Morph;
using EnsureThat;
using Godot;
using LanguageExt;

namespace AlleyCat.UI.Character
{
    [Singleton(typeof(MorphListPanel))]
    public class MorphListPanel : Panel
    {
        public IMorphable Character => _character.Head();

        protected TabContainer TabContainer => _tabContainer.Head();

        [Service] private Option<IMorphable> _character;

        [Node("Tab Container")] private Option<TabContainer> _tabContainer;

        [Export] private PackedScene _groupPanelScene;

        [PostConstruct(true)]
        protected virtual void OnInitialized()
        {
            LoadMorphs(Character.Morphs);
        }

        protected virtual void LoadMorphs(IMorphSet morphSet)
        {
            Ensure.That(morphSet, nameof(morphSet)).IsNotNull();

            Debug.Assert(_tabContainer != null, "_tabContainer != null");

            var index = 0;

            foreach (var group in morphSet.Groups)
            {
                var tab = (MorphGroupPanel) _groupPanelScene.Instance();

                tab.LoadGroup(group, morphSet);

                TabContainer.AddChild(tab);
                TabContainer.SetTabTitle(index, group.DisplayName);

                index++;
            }
        }

        public override void _Ready()
        {
            base._Ready();

            this.Autowire();
        }
    }
}
