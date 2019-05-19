using AlleyCat.Character;
using AlleyCat.Common;
using AlleyCat.Morph;
using EnsureThat;
using Godot;
using LanguageExt;
using Microsoft.Extensions.Logging;
using static LanguageExt.Prelude;

namespace AlleyCat.UI.Character
{
    public class MorphListPanel : UIControl
    {
        protected TabContainer TabContainer { get; }

        protected PackedScene GroupPanelScene { get; }

        public MorphListPanel(
            TabContainer tabContainer,
            PackedScene groupPanelScene,
            Godot.Control node,
            ILoggerFactory loggerFactory) : base(node, loggerFactory)
        {
            Ensure.That(tabContainer, nameof(tabContainer)).IsNotNull();
            Ensure.That(groupPanelScene, nameof(groupPanelScene)).IsNotNull();

            TabContainer = tabContainer;
            GroupPanelScene = groupPanelScene;
        }

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
                var node = GroupPanelScene.Instance();

                node.OfType<MorphGroupPanelFactory>().HeadOrNone().Match(
                    tab =>
                    {
                        tab.Group = Some(group);
                        tab.Morphs = Some(morphSet);

                        TabContainer.AddChild(node);
                        TabContainer.SetTabTitle(index, group.DisplayName);

                        index++;
                    },
                    () => Logger.LogWarning("Invalid group panel scene."));
            }
        }

        protected virtual void ClearMorphs() => TabContainer.GetChildComponents<Node>().Iter(TabContainer.FreeChild);
    }
}
