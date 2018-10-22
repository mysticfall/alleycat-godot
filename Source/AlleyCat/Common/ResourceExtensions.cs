using System;
using EnsureThat;
using Godot;

namespace AlleyCat.Common
{
    public static class ResourceExtensions
    {
        public static string GetKey(this Resource resource)
        {
            Ensure.That(resource, nameof(resource)).IsNotNull();

            return resource.ResourceName.TrimToOption().IfNone(() =>
            {
                var path = resource.ResourcePath;

                if (path == null)
                {
                    throw new ArgumentException("The specified resource doesn't have a name or path.");
                }

                var start = path.LastIndexOf('/') + 1;
                var end = path.LastIndexOf('.');

                return end != -1 ? path.Substring(start, end - start) : path.Substring(start);
            });
        }
    }
}
