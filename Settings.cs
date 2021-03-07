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
            public static ConfigEntry<bool> ResetSnapsOnChangePiece { get; private set; }
            
            public static void Init(ConfigFile config)
            {
                string name = "SnapSettings";
                EnableModKey = config.Bind(name, "EnableModKey", KeyCode.RightControl,
                    "This key will enable or disable the Snap mod.");

                IterateSourceSnapPoints = config.Bind(name, "IterateSourceSnapPoints", KeyCode.LeftControl,
                    "This key will cycle through the snap points on the piece you are placing.");

                IterateDestinationSnapPoints = config.Bind(name, "IterateDestinationSnapPoints", KeyCode.LeftAlt,
                    "This key will cycle through the snap points on the piece you are attaching to.");

                ResetSnapsOnChangePiece = config.Bind(name, "ResetSnapsOnChangePiece", false,
                    "Controls if the selected snap point is reset for each placement, default to not reset. This means your selections carry over between placements.");
                
                Debug.Log($"Loaded settings!\nEnableModKey: {EnableModKey.Value}\nIterateSourceSnapPoints:{IterateSourceSnapPoints.Value}\nIterateDestinationSnapPoints:{IterateDestinationSnapPoints.Value}");
            }
        }
    }
}