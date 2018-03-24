using System.Collections.Generic;
using Godot;

namespace AlleyCat.Common
{
    public interface IMeshObject : IBounded
    {
        IEnumerable<MeshInstance> Meshes { get; }
    }
}
