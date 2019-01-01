using System;
using EnsureThat;

namespace AlleyCat.Common
{
    public static class DisposableExtensions
    {
        public static void DisposeQuietly(this IDisposable disposable)
        {
            Ensure.That(disposable, nameof(disposable)).IsNotNull();

            try
            {
                disposable.Dispose();
            }
            catch (Exception)
            {
                // ignored
            }
        }
    }
}
