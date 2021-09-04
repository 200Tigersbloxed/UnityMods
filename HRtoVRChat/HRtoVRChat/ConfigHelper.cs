using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MelonLoader;

namespace HRtoVRChat
{
    public static class ConfigHelper
    {
        public static Config LoadedConfig;
        private static string myCategory = "HRtoVRChat";

        public static bool DoesCategoryExist(string category) => MelonPreferences.GetCategory(category) != null;

        private static void CreateConfig()
        {
            MelonPreferences.CreateCategory(myCategory);
            try { MelonPreferences.CreateEntry(myCategory, "hrtype", "unknown", "hrtype", "Displays the HRType to use; see documentation."); } catch (Exception) { }
            try { MelonPreferences.CreateEntry(myCategory, "fitbiturl", "ws://localhost:8080/", "fitbiturl", "Websocket URL to Connect to for FitbitHRtoWS."); } catch (Exception) { }
            try { MelonPreferences.CreateEntry(myCategory, "hyperatesessionid", String.Empty, "hyperatesessionid", "SessionId for HypeRate."); } catch (Exception) { }
            LogHelper.Log("ConfigHelper", "Created Config!");
        }

        public static Config LoadConfig()
        {
            CreateConfig();
            Config newConfig = new Config();

            foreach (MelonPreferences_Entry mpe in MelonPreferences.GetCategory(myCategory).Entries)
            {
                switch (mpe.DisplayName.ToLower())
                {
                    case "hrtype":
                        newConfig.hrType = mpe.GetValueAsString();
                        break;
                    case "fitbiturl":
                        newConfig.fitbitURL = mpe.GetValueAsString();
                        break;
                    case "hyperatesessionid":
                        newConfig.hyperateSessionId = mpe.GetValueAsString();
                        break;

                }
            }

            LoadedConfig = newConfig;
            LogHelper.Log("ConfigHelper", "Loaded Config!");
            return newConfig;
        }

        public class Config
        {
            public string hrType = "unknown";
            public string fitbitURL = "ws://localhost:8080/";
            public string hyperateSessionId = String.Empty;
        }
    }
}
