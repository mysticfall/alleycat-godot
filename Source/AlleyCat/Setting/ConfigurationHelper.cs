using System;
using System.Linq;
using System.Reflection;
using EnsureThat;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AlleyCat.Setting
{
    internal static class ConfigurationHelper
    {
        private static readonly MethodInfo ConfigureMethod =
            typeof(OptionsConfigurationServiceCollectionExtensions)
                .GetMethods(BindingFlags.Static | BindingFlags.Public)
                .Where(m => m.Name == "Configure")
                .Where(m =>
                {
                    var p = m.GetParameters();

                    return p.Length == 2 &&
                           p[0].ParameterType == typeof(IServiceCollection) &&
                           p[1].ParameterType == typeof(IConfiguration);
                }).Single();

        public static void Configure(object source, object target, Type type)
        {
            Ensure.That(source, nameof(source)).IsNotNull();
            Ensure.That(target, nameof(target)).IsNotNull();

            ConfigureMethod.MakeGenericMethod(type).Invoke(null, new[] {source, target});
        }
    }
}
