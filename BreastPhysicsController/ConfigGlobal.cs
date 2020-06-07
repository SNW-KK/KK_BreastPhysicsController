using BepInEx.Configuration;

namespace BreastPhysicsController
{
    public static class ConfigGlobal
    {
        public enum DefalutStatusMode
        {
            DontUse,
            UseDefaultStatus,
            ForceDefalutStatus

        }

        public static ConfigEntry<DefalutStatusMode> defaultStatusMode;
        public static ConfigEntry<bool> forceDefaultStatus;
        public static ConfigEntry<KeyboardShortcut> showWindowKey;
        public static ConfigEntry<float> minGravity;
        public static ConfigEntry<float> maxGravity;
        public static ConfigEntry<float> minDamping;
        public static ConfigEntry<float> maxDamping;
        public static ConfigEntry<float> minElasticity;
        public static ConfigEntry<float> maxElasticity;
        public static ConfigEntry<float> minStiffness;
        public static ConfigEntry<float> maxStiffness;
        public static ConfigEntry<float> minInert;
        public static ConfigEntry<float> maxInert;

    }
}
