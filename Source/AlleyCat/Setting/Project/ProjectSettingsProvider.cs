using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AlleyCat.Autowire;
using EnsureThat;
using JetBrains.Annotations;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AlleyCat.Setting.Project
{
    [Singleton(typeof(ISettingsProvider), typeof(IConfigurationSource))]
    public class ProjectSettingsProvider : AutowiredNode, ISettingsProvider, IConfigurationSource
    {
        public const string Prefix = "Project";

        private static readonly IMemoryCache Cache = new MemoryCache(new MemoryCacheOptions());

        public virtual IEnumerable<Type> SettingsTypes => new[] {typeof(PhysicsSettings)};

        public IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            var keys = SettingsTypes.SelectMany(t => FindKeys(t));

            return new ProjectSettingsConfigurationProvider(keys);
        }

        public void AddSettings(IConfigurationBuilder builder) => builder.Add(this);

        public void BindSettings(IConfigurationRoot root, IServiceCollection collection)
        {
            Ensure.Any.IsNotNull(root, nameof(root));
            Ensure.Any.IsNotNull(collection, nameof(collection));

            var parent = root.GetSection(Prefix);

            SettingsTypes.ToList().ForEach(t => BindSettings(t, parent, collection));
        }

        protected void BindSettings<T>(
            [NotNull] IConfigurationSection section,
            [NotNull] IServiceCollection collection) => BindSettings(typeof(T), section, collection);

        protected void BindSettings(
            [NotNull] Type type,
            [NotNull] IConfigurationSection parent,
            [NotNull] IServiceCollection collection)
        {
            Ensure.Any.IsNotNull(type, nameof(type));
            Ensure.Any.IsNotNull(parent, nameof(parent));
            Ensure.Any.IsNotNull(collection, nameof(collection));

            var key = FindKey(type);

            if (key == null) return;

            var section = parent.GetSection(key);

            ConfigurationHelper.Configure(collection, section, type);

            GetMembers(type)
                .Where(m => m.Item2.GetCustomAttribute<SettingsAttribute>() != null)
                .Select(m => m.Item2)
                .ToList()
                .ForEach(t => BindSettings(t, section, collection));
        }

        [CanBeNull]
        protected string FindKey<T>() => FindKey(typeof(T));

        [CanBeNull]
        protected string FindKey([NotNull] Type type)
        {
            Ensure.Any.IsNotNull(type, nameof(type));

            var attribute = type.GetCustomAttribute<SettingsAttribute>();

            if (attribute == null) return null;

            return attribute.Key ??
                   new[] {type.Name.Replace("Settings", "")}.FirstOrDefault(v => v.Length > 0);
        }

        [NotNull]
        protected IEnumerable<string> FindKeys<T>([NotNull] string prefix = Prefix) => FindKeys(typeof(T), prefix);

        [NotNull]
        protected IEnumerable<string> FindKeys([NotNull] Type type, [NotNull] string prefix = Prefix)
        {
            Ensure.Any.IsNotNull(type, nameof(type));
            Ensure.Any.IsNotNull(prefix, nameof(prefix));

            var key = FindKey(type);

            if (key == null) return Enumerable.Empty<string>();

            var path = string.Join(":", prefix, key);

            var groups = GetMembers(type)
                .GroupBy(m => m.Item2.GetCustomAttribute<SettingsAttribute>())
                .ToList();

            var keys = groups
                .Where(g => g.Key == null)
                .SelectMany(g => g.AsEnumerable())
                .Select(m => string.Join(":", string.Join(":", prefix, key), m.Item1.Name));

            var childKeys = groups
                .Where(g => g.Key != null)
                .SelectMany(g => g.AsEnumerable())
                .SelectMany(g => FindKeys(g.Item2, path));

            var children = keys.Concat(childKeys);

            return new[] {path}.Concat(children);
        }

        private IEnumerable<(MemberInfo, Type)> GetMembers(Type type)
        {
            return Cache.GetOrCreate(type, _ =>
            {
                var members = type
                    .GetMembers(BindingFlags.Instance | BindingFlags.Public)
                    .Where(m => (m.MemberType & (MemberTypes.Property | MemberTypes.Field)) != 0)
                    .ToList();

                var properties = members
                    .OfType<PropertyInfo>()
                    .Select(p => ((MemberInfo) p, p.PropertyType));

                var fields = members
                    .OfType<FieldInfo>()
                    .Select(f => ((MemberInfo) f, f.FieldType));

                return properties.Concat(fields).ToList();
            });
        }
    }
}
