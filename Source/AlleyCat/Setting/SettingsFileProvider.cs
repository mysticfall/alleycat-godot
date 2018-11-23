using AlleyCat.Autowire;
using Godot;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

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

        public void AddSettings(IConfigurationBuilder builder) => AddSettings(builder, File, Optional, ReloadOnChange);

        public virtual void BindSettings(IConfigurationRoot root, IServiceCollection collection)
        {
        }

        protected abstract void AddSettings(
            IConfigurationBuilder builder,
            string file,
            bool optional,
            bool reloadOnChange);
    }
}
