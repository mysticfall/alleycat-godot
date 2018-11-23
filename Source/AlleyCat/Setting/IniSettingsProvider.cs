using EnsureThat;
using Godot;
using Microsoft.Extensions.Configuration;

namespace AlleyCat.Setting
{
    public class IniSettingsProvider : SettingsFileProvider
    {
        [Export(PropertyHint.File, "*.ini")]
        public override string File { get; set; }

        protected override void AddSettings(
            IConfigurationBuilder builder, string file, bool optional, bool reloadOnChange)
        {
            Ensure.That(builder, nameof(builder)).IsNotNull();

            builder.AddIniFile(file, optional, reloadOnChange);
        }
    }
}
