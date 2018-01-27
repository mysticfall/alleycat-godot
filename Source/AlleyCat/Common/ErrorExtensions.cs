using System;
using Godot;
using JetBrains.Annotations;

namespace AlleyCat.Common
{
    public static class ErrorExtensions
    {
        public static void ThrowIfNecessary(
            this Error error, [CanBeNull] Func<string, Exception> handler = null)
        {
            if (error == Error.Ok) return;

            var code = Enum.GetName(typeof(Error), error);
            var message = $"Operation failed with code: '{code}(error)'";

            throw handler?.Invoke(message) ?? new InvalidOperationException(message);
        }
    }
}
