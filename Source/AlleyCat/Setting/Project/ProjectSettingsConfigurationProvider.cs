using System.Collections.Generic;
using System.Linq;
using EnsureThat;
using Godot;
using JetBrains.Annotations;
using LanguageExt;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Primitives;
using static LanguageExt.Prelude;

namespace AlleyCat.Setting.Project
{
    public class ProjectSettingsConfigurationProvider : IConfigurationProvider
    {
        public IEnumerable<string> Keys { get; }

        private static readonly Map<string, string> Replacements = Map(
            ("two_d", "2d"), ("three_d", "3d"));

        public ProjectSettingsConfigurationProvider(IEnumerable<string> keys)
        {
            Ensure.That(keys, nameof(keys)).IsNotNull();

            Keys = keys.Freeze();
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

        public void Set(string key, [CanBeNull] string value) =>
            ProjectSettings.SetSetting(NormalizeKey(key), GD.Str2Var(value));

        public void Load()
        {
        }

        public IChangeToken GetReloadToken() => NullChangeToken.Singleton;

        public IEnumerable<string> GetChildKeys(IEnumerable<string> earlierKeys, string parentPath)
        {
            Ensure.That(earlierKeys, nameof(earlierKeys)).IsNotNull();

            return earlierKeys.Concat(Keys.Where(k => k.StartsWith(parentPath)));
        }

        private static string NormalizeKey(string key)
        {
            Ensure.That(key, nameof(key)).IsNotNull();

            var values = key.Split(':');

            if (values.Length < 2 || values[0] != ProjectSettingsProvider.Prefix) return key;

            string ToSnakeCase(string v) => string
                .Concat(v.Select((x, i) => i > 0 && char.IsUpper(x) ? $"_{x}" : x.ToString()))
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
