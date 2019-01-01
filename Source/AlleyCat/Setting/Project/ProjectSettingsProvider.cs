using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AlleyCat.Autowire;
using EnsureThat;
using LanguageExt;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using static LanguageExt.Prelude;

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
            var keys = SettingsTypes.Bind(t => FindKeys(t));

            return new ProjectSettingsConfigurationProvider(keys);
        }

        public void AddSettings(IConfigurationBuilder builder)
        {
            Ensure.That(builder, nameof(builder)).IsNotNull();

            builder.Add(this);
        }

        public void BindSettings(IConfigurationRoot root, IServiceCollection collection)
        {
            Ensure.That(root, nameof(root)).IsNotNull();

            var parent = root.GetSection(Prefix);

            SettingsTypes.Iter(t => BindSettings(t, parent, collection));
        }

        protected void BindSettings<T>(
            IConfigurationSection section,
            IServiceCollection collection) => BindSettings(typeof(T), section, collection);

        protected void BindSettings(
            Type type,
            IConfigurationSection parent,
            IServiceCollection collection)
        {
            Ensure.That(parent, nameof(parent)).IsNotNull();

            FindKey(type).Select(parent.GetSection).Iter(section =>
            {
                ConfigurationHelper.Configure(collection, section, type);

                GetMembers(type)
                    .Select(t => (member: t.Item1, type: t.Item2))
                    .Where(m => m.type.GetCustomAttribute<SettingsAttribute>() != null)
                    .Select(m => m.type)
                    .Iter(t => BindSettings(t, section, collection));
            });
        }

        protected Option<string> FindKey<T>() => FindKey(typeof(T));

        protected Option<string> FindKey(Type type)
        {
            Ensure.That(type, nameof(type)).IsNotNull();

            var attribute = type.GetCustomAttribute<SettingsAttribute>();

            if (attribute == null) return None;

            return attribute.Key ?? Optional(type.Name.Replace("Settings", "")).Filter(v => v.Length > 0);
        }

        protected IEnumerable<string> FindKeys<T>(string prefix = Prefix) => FindKeys(typeof(T), prefix);

        protected IEnumerable<string> FindKeys(Type type, string prefix = Prefix)
        {
            return match(FindKey(type),
                key =>
                {
                    var path = string.Join(":", prefix, key);

                    var groups = GetMembers(type)
                        .Select(g => (member: g.Item1, type: g.Item2))
                        .GroupBy(m => m.type.GetCustomAttribute<SettingsAttribute>())
                        .ToList();

                    var keys = groups
                        .Where(g => g.Key == null)
                        .Bind(g => g.AsEnumerable())
                        .Select(m => string.Join(":", string.Join(":", prefix, key), m.member.Name));

                    var childKeys = groups
                        .Where(g => g.Key != null)
                        .Bind(g => g.AsEnumerable())
                        .Bind(g => FindKeys(g.type, path));

                    var children = keys.Concat(childKeys);

                    return new[] {path}.Concat(children);
                },
                Enumerable.Empty<string>);
        }

        private static IEnumerable<(MemberInfo, Type)> GetMembers(Type type)
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
