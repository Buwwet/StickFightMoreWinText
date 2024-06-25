using System.IO;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using InControl;

// TODO: find a way to find out who won before Vicotory runs.

namespace MoreVictoryText {
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class WinTextPlugin : BaseUnityPlugin {
        public static ManualLogSource logger = BepInEx.Logging.Logger.CreateLogSource(PluginInfo.PLUGIN_NAME);

        // Keeps track if the player who just won.
        public static int playerWonID = 0;

        private void Awake() {
            // Make our logger work.
            BepInEx.Logging.Logger.Sources.Add(logger);

            var harmony = new Harmony("com.buwwet.stickfight.wintextplugin");
            harmony.PatchAll();

            // TODO: load em here.

            logger.LogInfo("Blah Blah loaded N number of wintexts!");

            //File.ReadLines()

        }

        // Gets the number of vanilla text
        public static int getVanillaTextCount() {
            return Traverse.Create(GameManager.Instance).Field("vicotory").Field("standard").GetValue<string[]>().Length;
        }
    }

    

    // Patch "Vicotory" so that we can return our own text.
    [HarmonyPatch(typeof(Vicotory), nameof(Vicotory.GetRandomWinText))]
    public class VicotoryPatch {
        static bool Prefix(out string __result) {
            __result = "waluigi #" + WinTextPlugin.playerWonID;;

            return false;
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