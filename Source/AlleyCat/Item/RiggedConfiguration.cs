using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using AlleyCat.Animation;
using AlleyCat.Common;
using Godot;
using Godot.Collections;
using JetBrains.Annotations;
using Object = Godot.Object;

namespace AlleyCat.Item
{
    public class RiggedConfiguration : EquipmentConfiguration
    {
        public ISet<string> MeshesToSync
        {
            get
            {
                if (_meshesToSyncSet != null) return _meshesToSyncSet;

                var names = _meshesToSync.TrimToEnumerable();

                Debug.Assert(names != null, nameof(names) + " != null");

                _meshesToSyncSet = new HashSet<string>(names);

                return _meshesToSyncSet;
            }
        }

        [Export, UsedImplicitly] private string _meshesToSync;

        private ISet<string> _meshesToSyncSet;

        private IDisposable _blendShapeListener;

        private IEnumerable<(string key, MeshInstance source, MeshInstance target)> _blendShapeMappings;

        public override void OnEquip(IEquipmentHolder holder, Equipment equipment)
        {
            base.OnEquip(holder, equipment);

            foreach (var mesh in equipment.Meshes)
            {
                equipment.Transform = new Transform(Basis.Identity, Vector3.Zero);

                mesh.Skeleton = mesh.GetPathTo(holder.Skeleton);
            }

            UnregisterBlendShapeListeners();

            if (!(holder is IMeshObject obj) || !(holder is IAnimatable animatable)) return;

            IEnumerable<string> FindBlendShapes(Object m) =>
                m.GetPropertyList()
                    .OfType<Dictionary>()
                    .Select(d => d["name"])
                    .OfType<string>()
                    .Where(k => k.StartsWith("blend_shapes/"));

            IDictionary<string, MeshInstance> GetBlendShapeMap(IEnumerable<MeshInstance> meshes) =>
                meshes
                    .Select(m => (mesh: m, blendshapes: FindBlendShapes(m)))
                    .Where(i => i.blendshapes.Any())
                    .SelectMany(i => i.blendshapes.Select(b => (blendshape: b, i.mesh)))
                    .ToDictionary(i => i.blendshape, i => i.mesh);

            var sources = GetBlendShapeMap(obj.Meshes.Where(m => MeshesToSync.Contains(m.Name)));

            if (!sources.Any()) return;

            var targets = GetBlendShapeMap(equipment.Meshes);

            _blendShapeMappings = sources
                .Where(i => targets.ContainsKey(i.Key))
                .Select(i => (key: i.Key, source: i.Value, target: targets[i.Key]));

            if (!_blendShapeMappings.Any()) return;

            _blendShapeListener = animatable.AnimationManager.OnAdvance
                .Subscribe(_ =>
                {
                    foreach (var mapping in _blendShapeMappings)
                    {
                        var value = mapping.source.Get(mapping.key);

                        mapping.target.Set(mapping.key, value);
                    }
                });
        }

        public override void OnUnequip(IEquipmentHolder holder, Equipment equipment)
        {
            base.OnUnequip(holder, equipment);

            UnregisterBlendShapeListeners();
        }

        protected override void Dispose(bool disposing)
        {
            UnregisterBlendShapeListeners();

            base.Dispose(disposing);
        }

        private void UnregisterBlendShapeListeners()
        {
            _blendShapeListener?.Dispose();

            _blendShapeListener = null;
            _blendShapeMappings = null;
        }
    }
}
