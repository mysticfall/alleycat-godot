using System.Collections.Generic;
using System.Linq;
using EnsureThat;
using Godot;
using JetBrains.Annotations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Primitives;

namespace AlleyCat.Setting.Project
{
    public class ProjectSettingsConfigurationProvider : IConfigurationProvider
    {
        public IEnumerable<string> Keys { get; }

        private static readonly IDictionary<string, string> Replacements = new Dictionary<string, string>
        {
            ["two_d"] = "2d",
            ["three_d"] = "3d"
        };

        public ProjectSettingsConfigurationProvider([NotNull] IEnumerable<string> keys)
        {
            // ReSharper disable once ConstantConditionalAccessQualifier
            Keys = keys?.ToList();

            Ensure.Enumerable.HasItems(Keys, nameof(keys));
        }

        public bool TryGet(string key, out string value)
        {
            if (!ProjectSettings.HasSetting(NormalizeKey(key)))
            {
                value = null;
                return false;
            }

            value = GD.Var2Str(ProjectSettings.GetSetting(NormalizeKey(key)));

            return true;
        }

        public void Set(string key, string value) => ProjectSettings.SetSetting(NormalizeKey(key), GD.Str2Var(value));

        public void Load()
        {
        }

        public IChangeToken GetReloadToken() => NullChangeToken.Singleton;

        public IEnumerable<string> GetChildKeys(IEnumerable<string> earlierKeys, string parentPath) =>
            Keys.Where(k => k.StartsWith(parentPath));

        private static string NormalizeKey(string key)
        {
            Ensure.Any.IsNotNull(key, nameof(key));

            var values = key.Split(':');

            if (values.Length < 2 || values[0] != ProjectSettingsProvider.Prefix) return key;

            string ToSnakeCase(string v) => string
                .Concat(v.Select((x, i) => i > 0 && char.IsUpper(x) ? "_" + x.ToString() : x.ToString()))
                .ToLower();

            string Replace(string v) =>
                Replacements.Keys.Aggregate(v, (current, k) => current.Replace(k, Replacements[k]));

            var segments = values
                .Skip(1)
                .ToList()
                .Select(ToSnakeCase)
                .Select(Replace);

            return string.Join("/", segments);
        }
    }
}
