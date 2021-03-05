using BepInEx.Configuration;
using UnityEngine;

namespace ValheimSnapMod
{
    public class Settings
    {
        public static void Init(ConfigFile config)
        {
            SnapSettings.Init(config);
        }

        public class SnapSettings
        {
            public static ConfigEntry<KeyCode> EnableModKey { get; private set; }
            public static ConfigEntry<KeyCode> IterateSourceSnapPoints { get; private set; }
            public static ConfigEntry<KeyCode> IterateDestinationSnapPoints { get; private set; }
            public static void Init(ConfigFile config)
            {
                string name = "SnapSettings";
                EnableModKey = config.Bind(name, "EnableModKey", KeyCode.RightControl,
                    "What key to press to send map to target?");

                IterateSourceSnapPoints = config.Bind(name, "IterateSourceSnapPoints", KeyCode.LeftControl,
                    "What key to press to accept a sent map?");

                IterateDestinationSnapPoints = config.Bind(name, "IterateDestinationSnapPoints", KeyCode.LeftAlt,
                    "What key to press to reject a sent map?");

                Debug.Log($"Loaded settings!\nEnableModKey: {EnableModKey.Value}\nIterateSourceSnapPoints:{IterateSourceSnapPoints.Value}\nIterateDestinationSnapPoints:{IterateDestinationSnapPoints.Value}");
            }
        }
    }
}