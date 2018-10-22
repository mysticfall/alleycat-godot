using System;
using System.Collections.Generic;
using System.Linq;
using AlleyCat.Animation;
using AlleyCat.Common;
using EnsureThat;
using Godot;
using Godot.Collections;
using JetBrains.Annotations;
using LanguageExt;
using static LanguageExt.Prelude;
using Object = Godot.Object;

namespace AlleyCat.Item
{
    public class RiggedConfiguration : EquipmentConfiguration
    {
        public Set<string> MeshesToSync => toSet(_meshesToSync);

        [Export, UsedImplicitly] private Array<string> _meshesToSync;

        private Option<IDisposable> _blendShapeListener = None;

        private Set<BlendShapeMapping> _blendShapeMappings = Set<BlendShapeMapping>();

        public override void OnEquip(IEquipmentHolder holder, Equipment equipment)
        {
            base.OnEquip(holder, equipment);

            foreach (var mesh in equipment.Meshes)
            {
                equipment.Transform = new Transform(Basis.Identity, Vector3.Zero);

                mesh.Skeleton = mesh.GetPathTo(holder.Skeleton);
            }

            UnregisterBlendShapeListeners();

            if (!(holder is IMeshObject obj && holder is IAnimatable animatable)) return;

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

            _blendShapeMappings = toSet(sources
                .Filter(i => targets.ContainsKey(i.Key))
                .Select(i => new BlendShapeMapping(i.Key, i.Value, targets[i.Key])));

            if (!_blendShapeMappings.Any()) return;

            _blendShapeListener = Some(
                animatable.AnimationManager.OnAdvance.Subscribe(_ =>
                {
                    foreach (var mapping in _blendShapeMappings)
                    {
                        var value = mapping.Source.Get(mapping.Key);

                        mapping.Target.Set(mapping.Key, value);
                    }
                }));
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
            _blendShapeListener.Iter(l => l.DisposeQuietly());
            _blendShapeListener = None;

            _blendShapeMappings = Set<BlendShapeMapping>();
        }
    }

    internal struct BlendShapeMapping : IComparable<BlendShapeMapping>
    {
        public string Key { get; }

        public MeshInstance Source { get; }

        public MeshInstance Target { get; }

        public BlendShapeMapping(string key, MeshInstance source, MeshInstance target)
        {
            Ensure.That(key, nameof(key)).IsNotNull();
            Ensure.That(source, nameof(source)).IsNotNull();
            Ensure.That(target, nameof(target)).IsNotNull();

            Key = key;
            Source = source;
            Target = target;
        }

        public int CompareTo(BlendShapeMapping other) => 
            string.Compare(Key, other.Key, StringComparison.Ordinal);
    }
}
