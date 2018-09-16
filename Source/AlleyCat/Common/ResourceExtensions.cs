using System;
using EnsureThat;
using Godot;
using JetBrains.Annotations;

namespace AlleyCat.Common
{
    public static class ResourceExtensions
    {
        [NotNull]
        public static String GetKey([NotNull] this Resource resource)
        {
            Ensure.Any.IsNotNull(resource, nameof(resource));

            var name = resource.ResourceName.TrimToNull();

            if (name != null) return name;

            var path = resource.ResourcePath;

            if (path == null)
            {
                throw new ArgumentException("The specified resource doesn't have a name or path.");
            }

            var start = path.LastIndexOf('/') + 1;
            var end = path.LastIndexOf('.');

            name = end != -1 ? path.Substring(start, end - start) : path.Substring(start);

            return name;
        }
    }
}
