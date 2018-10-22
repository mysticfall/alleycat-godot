using System.Collections.Generic;
using AlleyCat.Common;
using Godot;

namespace AlleyCat.UI.Inventory
{
    public class ItemStand : MeshInstance, IMeshObject
    {
        public Spatial Spatial => this;

        public AABB Bounds => this.CalculateBounds();

        public IEnumerable<MeshInstance> Meshes => new [] {this};
    }
}
