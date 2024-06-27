using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using InControl;
using UnityEngine;

// TODO: find a way to find out who won before Vicotory runs.

namespace MoreVictoryText {
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class WinTextPlugin : BaseUnityPlugin {
        public static ManualLogSource logger = BepInEx.Logging.Logger.CreateLogSource(PluginInfo.PLUGIN_NAME);

        // Keeps track if the player who just won.
        public static int playerWonID = 0;

        // All win texts that belong to all players.
        public static List<string> generalWinText = new List<string>();
        // All win texts that belong to only one player
        public static Dictionary<int, List<string>> specificWinText = [];

        public static ConfigEntry<bool> configOverrideVanillaWinTexts;

        private void Awake() {
            // Make our logger work.
            BepInEx.Logging.Logger.Sources.Add(logger);

            var harmony = new Harmony("com.buwwet.stickfight.wintextplugin");
            harmony.PatchAll();

            //TODO: log correctly
            logger.LogInfo("Blah Blah loaded N number of wintexts!");
            
            var configFile = new ConfigFile("BepInEx/plugins/MoreWinText/config.cfg", true);

            configOverrideVanillaWinTexts = configFile.Bind("General",
                "OvverrideVanillaWinTexts", true, "Set to true if you want to disable vanilla texts from appearing"
            );


            // Read all of the win texts and store them in memory.
            generalWinText = LoadFromFileWinTexts("general.txt");
            specificWinText.Add(0, LoadFromFileWinTexts("yellow.txt"));
            specificWinText.Add(1, LoadFromFileWinTexts("blue.txt"));
            specificWinText.Add(2, LoadFromFileWinTexts("red.txt"));
            specificWinText.Add(3, LoadFromFileWinTexts("green.txt"));

        }

        public static List<string> LoadFromFileWinTexts(string filename) {
            string filepath = "BepInEx/plugins/MoreWinText/texts/" + filename;
            if (!File.Exists(filepath)) {
                logger.LogError("Requested wintext file doesn't exist! Check " + filepath);
            }

            List<string> winTexts = [];
            foreach(string text in File.ReadAllLines("BepInEx/plugins/MoreWinText/texts/" + filename)) {
                winTexts.Add(text);
            }
            return winTexts;
        }

        // Gets the number of vanilla text
        public static int GetVanillaTextCount() {
            return Traverse.Create(GameManager.Instance).Field("vicotory").Field("standard").GetValue<string[]>().Length;
        }

        public static int GetTotalCustomTextCount(int id) {
            return generalWinText.Count + specificWinText[id].Count;
        }
    }

    

    // Patch "Vicotory" so that we can return our own text.
    [HarmonyPatch(typeof(Vicotory), nameof(Vicotory.GetRandomWinText))]
    public class VicotoryPatch {
        static bool Prefix(out string __result) {

            int totalCustom = WinTextPlugin.GetTotalCustomTextCount(WinTextPlugin.playerWonID);

            int total = totalCustom + WinTextPlugin.GetVanillaTextCount();

            // Check if we should generate a custom one.
            float customToTotalRatio = (float) totalCustom / (float) total;

            float customOverride = 0.0f;
            if (WinTextPlugin.configOverrideVanillaWinTexts.Value) {
                customOverride = 1.0f;
            }

            if (customToTotalRatio + customOverride > UnityEngine.Random.Range(0.0f, 1.0f)) {
                // Time to use our custom ones!

                // We'll use the same trick to decide if we should use the generalist ones or the special ones.
                int totalCustomSpecialized = WinTextPlugin.specificWinText[WinTextPlugin.playerWonID].Count;
                float specializedToTotalCustomRatio = (float) totalCustomSpecialized / (float) totalCustom;

                if (specializedToTotalCustomRatio > UnityEngine.Random.Range(0.0f, 1.0f)) {
                    // Let's use one of our specialized WinTexts
                    int winTextIndex = UnityEngine.Random.RandomRangeInt(0, totalCustomSpecialized - 1);
                    __result = WinTextPlugin.specificWinText[WinTextPlugin.playerWonID][winTextIndex];

                } else {
                    // Use a generic one.
                    int winTextIndex = UnityEngine.Random.RandomRangeInt(0, WinTextPlugin.generalWinText.Count - 1);
                    __result = WinTextPlugin.generalWinText[winTextIndex];
                }

                return false;
            }



            __result = "This shouldn't change anything. I hope";
            return true;
        }
    }

    // Patch GameManager's AllButOnePlayersDied so we can check for who died.
    [HarmonyPatch(typeof(GameManager), "AllButOnePlayersDied")]
    public class GameManagerAllDeadPatch {
        static bool Prefix() {
            // Iterate through all players to get the ID of the last living one.
            Controller controller = null;
            foreach (Controller item in GameManager.Instance.playersAlive) {
                // We found our alive player!
                if (item != null) {
                    controller = item;
                    break;
                }
            }

            WinTextPlugin.playerWonID = controller.playerID;

            return true;
        }
    }
}