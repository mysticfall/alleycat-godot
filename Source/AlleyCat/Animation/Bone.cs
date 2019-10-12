using System.Collections.Generic;
using AlleyCat.Common;
using EnsureThat;
using Godot;

namespace AlleyCat.Animation
{
    public struct Bone : IIdentifiable
    {
        public int Index { get; }

        public string Key { get; }

        public Bone(string key, int index)
        {
            Ensure.That(key, nameof(key)).IsNotNull();

            Key = key;
            Index = index;
        }
    }

    public static class BoneExtensions
    {
        public static IEnumerable<Bone> GetBones(this IRigged rig) => GetBones(rig?.Skeleton);

        public static IEnumerable<Bone> GetBones(this Skeleton skeleton)
        {
            Ensure.That(skeleton, nameof(skeleton)).IsNotNull();

            var count = skeleton.GetBoneCount();

            for (var i = 0; i < count; i++)
            {
                yield return new Bone(skeleton.GetBoneName(i), i);
            }
        }
    }
}
