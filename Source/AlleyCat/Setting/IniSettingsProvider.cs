using Godot;
using Microsoft.Extensions.Configuration;

namespace AlleyCat.Setting
{
    public class IniSettingsProvider : SettingsFileProvider
    {
        [Export(PropertyHint.File, "INI")]
        public override string File { get; set; }

        protected override void AddSettings(
            IConfigurationBuilder builder, string file, bool optional, bool reloadOnChange)
        {
            builder.AddIniFile(file, optional, reloadOnChange);
        }
    }
}
