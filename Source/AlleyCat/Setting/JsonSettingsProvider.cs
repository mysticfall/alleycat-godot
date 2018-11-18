using Godot;
using Microsoft.Extensions.Configuration;

namespace AlleyCat.Setting
{
    public class JsonSettingsProvider : SettingsFileProvider
    {
        [Export(PropertyHint.File, "*.json")]
        public override string File { get; set; }

        protected override void AddSettings(
            IConfigurationBuilder builder, string file, bool optional, bool reloadOnChange)
        {
            builder.AddJsonFile(file, optional, reloadOnChange);
        }
    }
}
