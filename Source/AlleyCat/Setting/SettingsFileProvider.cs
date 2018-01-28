using AlleyCat.Autowire;
using EnsureThat;
using Godot;
using JetBrains.Annotations;
using Microsoft.Extensions.Configuration;

namespace AlleyCat.Setting
{
    [Singleton(typeof(ISettingsProvider))]
    public abstract class SettingsFileProvider : AutowiredNode, ISettingsProvider
    {
        public abstract string File { get; set; }

        [Export(hintString: "Ignore this settings file if it doesn't exist.")]
        public bool Optional { get; set; } = false;

        [Export(hintString: "Reload this settings file if it has changed.")]
        public bool ReloadOnChange { get; set; } = false;

        public void AddSettings(IConfigurationBuilder builder)
        {
            Ensure.Any.IsNotNull(builder, nameof(builder));
            Ensure.String.IsNotNullOrWhiteSpace(File, nameof(File));

            AddSettings(builder, File, Optional, ReloadOnChange);
        }

        protected abstract void AddSettings(
            [NotNull] IConfigurationBuilder builder, 
            [NotNull] string file,
            bool optional,
            bool reloadOnChange);
    }
}
