using System;
using System.Diagnostics;
using Windows.Storage;

namespace HBO.UWP.Player.Helpers
{
    internal abstract class LocalSettingBase
    {
        /// <summary>
        /// Access to LocalSettings must be atomic.
        /// </summary>
        protected static readonly object SettingsLock = new object();
    }

    /// <summary>
    /// Encapsulates a key/value pair stored in Application Settings.
    /// Note limit for each object is about 4kB of data!
    /// </summary>
    /// <typeparam name="T">Type to store, must be serializable.</typeparam>
    [DebuggerDisplay("{name}: {value}")]
    internal class LocalSetting<T> : LocalSettingBase
    {
        private readonly string name;
        private T value;
        private readonly T defaultValue;
        private bool hasValue;

        public LocalSetting(string name)
            : this(name, default(T)) { }

        public LocalSetting(string name, T defaultValue)
        {
            this.name = name;
            this.defaultValue = defaultValue;
        }

        public T Value
        {
            get
            {
                lock (SettingsLock)
                {
                    // Check for the cached value
                    if (hasValue) return value;

                    ApplicationDataContainer settings = ApplicationData.Current.LocalSettings;
                    try
                    {
                        object rawValue;
                        // Try to get the value from Application Settings
                        if (settings.Values.TryGetValue(name, out rawValue))
                        {
                            value = (T)rawValue;
                        }
                        else
                        {
                            // It hasn’t been set yet
                            value = defaultValue;
                            settings.Values[name] = value;
                        }
                    }
                    catch (Exception e)
                    {
                        value = defaultValue;
                        settings.Values[name] = value;
                        Debug.WriteLine("Setting Get error, name: {0}, {1}", name, e.Message);
                    }
                    hasValue = true;
                    return value;
                }
            }
            set
            {
                lock (SettingsLock)
                {
                    try
                    {
                        // Save the value to Application Settings
                        ApplicationData.Current.LocalSettings.Values[name] = value;
                        this.value = value;
                        hasValue = true;
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine("Setting Set error, name: {0}, {1}", name, e.Message);
                    }
                }
            }
        }

        public T DefaultValue => defaultValue;
    }
}