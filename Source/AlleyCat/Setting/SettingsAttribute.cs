using System;
using JetBrains.Annotations;

namespace AlleyCat.Setting
{
    [AttributeUsage(AttributeTargets.Class)]
    public class SettingsAttribute : System.Attribute
    {
        public string Key { get; }

        public SettingsAttribute([CanBeNull] string key = null)
        {
            Key = key;
        }
    }
}
