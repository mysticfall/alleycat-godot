using System.Linq;
using AlleyCat.Autowire;
using AlleyCat.Character;
using AlleyCat.Common;
using AlleyCat.Morph;
using EnsureThat;
using Godot;
using LanguageExt;

namespace AlleyCat.UI.Character
{
    [Singleton(typeof(MorphListPanel))]
    public class MorphListPanel : Panel
    {
        [Node("Tab Container", true)]
        protected TabContainer TabContainer { get; private set; }

        [Export] private PackedScene _groupPanelScene;

        public virtual void Load(Option<IHumanoid> character)
        {
            character.Map(c => c.Morphs).BiIter(LoadMorphs, ClearMorphs);
        }

        protected virtual void LoadMorphs(IMorphSet morphSet)
        {
            Ensure.That(morphSet, nameof(morphSet)).IsNotNull();

            ClearMorphs();

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

        protected virtual void ClearMorphs()
        {
            var children = TabContainer.GetChildComponents<Node>().Reverse();

            children.Iter(child => TabContainer.RemoveChild(child));
        }

        public override void _Ready()
        {
            base._Ready();

            this.Autowire();
        }
    }
}
