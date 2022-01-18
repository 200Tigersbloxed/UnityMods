﻿using System;
using System.Reflection;
using MelonLoader;

namespace CustomLabratNPC
{
    public static class ConfigHelper
    {
        public static Config LoadedConfig;
        private static readonly string myCategory = "CustomLabratNPC";

        private static MelonPreferences_Category category;

        public static bool DoesEntryExist(string key) => MelonPreferences.HasEntry(category.Identifier, key);
        
        private static void CreateEntryInConfig<T>(string key, string description, T defaultValue) =>
            MelonPreferences.CreateEntry(category.Identifier, key, defaultValue, key, description);

        private static T GetEntryInConfig<T>(string key) =>
            MelonPreferences.GetEntry<T>(category.Identifier, key).Value;

        private static void CreateConfig()
        {
            category = MelonPreferences.CreateCategory(myCategory);
            LogHelper.Debug("Created Config!");
        }

        public static Config LoadConfig()
        {
            CreateConfig();
            Config newConfig = new Config();
            foreach (FieldInfo field in newConfig.GetType().GetFields())
            {
                // Try and get it's method
                MethodInfo reload =
                    field.FieldType.GetMethod("ReloadValues", BindingFlags.Public | BindingFlags.Instance);
                object fieldValue = field.GetValue(newConfig);
                if (reload != null)
                    reload.Invoke(fieldValue, null);
                else
                    LogHelper.Warn("Could not find ReloadValues method for Field " + field.Name);
            }

            LoadedConfig = newConfig;
            LogHelper.Debug("Loaded Config!");
            return newConfig;
        }

        public class Config
        {
            public ConfigValue<bool> loadUnsafeCode = new ConfigValue<bool>("loadUnsafeCode",
                () => CreateEntryInConfig("loadUnsafeCode", "Loads Libraries. **WARNING**: " +
                                                            "THIS WILL RUN ANY CODE THAT MAY CAUSE DAMAGE TO YOUR " +
                                                            "MACHINE! NEVER EXCEPT ANY UAC PROMPTS FROM LABRAT, AND " +
                                                            "ALWAYS MAKE SURE THAT LIBRARIES ARE SAFE BEFORE RUNNING! " +
                                                            "YOU. HAVE. BEEN. WARNED.", 
                    false), () => GetEntryInConfig<bool>("loadUnsafeCode"));
        }
        
        public class ConfigValue<T>
        {
            public string Key { get; }
            public T Value { get; set; }
            private Func<T> GetEntry;

            public ConfigValue(string Key, Action CreateEntry, Func<T> GetEntry)
            {
                this.Key = Key;
                if (!DoesEntryExist(Key))
                {
                    CreateEntry.Invoke();
                    LogHelper.Debug($"Created entry {Key}");
                }
                this.GetEntry = GetEntry;
                GetEntry.Invoke();
            }

            public T ReloadValues()
            {
                T value = GetEntry.Invoke();
                Value = value;
                LogHelper.Debug($"Reloaded Config Value {Key} and got Value {value}");
                return value;
            }
        }
    }
}