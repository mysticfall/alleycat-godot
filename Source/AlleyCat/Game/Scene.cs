using System;
using AlleyCat.Autowire;
using AlleyCat.IO;
using EnsureThat;
using Godot;
using JetBrains.Annotations;

namespace AlleyCat.Game
{
    [AutowireContext]
    public class Scene : AutowiredNode, IScene
    {
        public string Key => _key ?? Name;

        [Export, UsedImplicitly] private string _key;

        public PackedScene Pack()
        {
            throw new NotImplementedException();
        }

        public void SaveState(IState state)
        {
            Ensure.Any.IsNotNull(state, nameof(state));

            throw new NotImplementedException();
        }

        public void RestoreState(IState state)
        {
            Ensure.Any.IsNotNull(state, nameof(state));

            throw new NotImplementedException();
        }
    }
}
