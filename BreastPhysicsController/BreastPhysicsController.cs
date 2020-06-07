using System;
using BepInEx;
using BepInEx.Logging;
using BepInEx.Configuration;
using UnityEngine;
using System.IO;
using System.Reflection;
using KKAPI.Chara;
using BreastPhysicsController.UI;
using BreastPhysicsController.UI.Util;


namespace BreastPhysicsController
{
    [BepInPlugin(GUID : GUID , Name : Name, Version : Version)]
    [BepInDependency("marco.kkapi", BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency("KK_com.bepis.bepinex.configurationmanager", BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency("com.bepis.bepinex.extendedsave",BepInDependency.DependencyFlags.HardDependency)]
    public class BreastPhysicsController : BaseUnityPlugin
    {
        public const string GUID = "com.snw.bepinex.breastphysicscontroller";
        public const string Name = "BreastPhysicsController";
        public const string Version = "2.0";
        public static string PresetDir;

        internal static new ManualLogSource Logger;

        public static ControllerWindow window;
        //private const int windowID = 1192;
        //private const int s_dialogID = 1193;
        //private const int toolsWindowID = 1194;


        private void Awake()
        {
            Logger = base.Logger;
            Hooks.InstallHooks();
            BindConfig();
            PresetDir = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), Name);
        }

        private void Start()
        {
            
            if (!Directory.Exists(PresetDir))
            {
                try
                {
                    DirectoryInfo info = Directory.CreateDirectory(PresetDir);
                }
                catch(Exception e)
                {
                    Logger.LogError("Failed create directory for presets. \r\nException info:\r\n" + e.ToString());
                }
            }

            CharacterApi.RegisterExtraBehaviour<ParamCharaController>(GUID);
            window = new ControllerWindow(WindowID.GetNewID(), WindowID.GetNewID(), WindowID.GetNewID());
        }

        public void Update()
        {
            if (ConfigGlobal.showWindowKey.Value.IsDown())
            {
                if (window._showWindow) window._showWindow = false;
                else window._showWindow = true;
            }

            //if (Input.GetKeyDown(KeyCode.P))
            //{
            //    if (window._showWindow) window._showWindow = false;
            //    else window._showWindow = true;
            //}
            
        }

        public void OnGUI()
        {
            if (window._showWindow)
            {
                window.OnGUI();
            }
        }

        void BindConfig()
        {
            const string DESC_DEFAULTSTATUSMODE = "When loaded character, set parameters and states saved as default status and enable controller.\r\n" +
                "DontUse :  Don't load default status always.\r\n"+
                "UseDefaultStatus : Load default status into the character and enable controller if the character don't have plugin's data or controller is disabled in the data.\r\n"+
                "ForceDefaultStatus : always load default status and enable controller.(Overwrite loaded data from card by default status.)";
                

            ConfigGlobal.defaultStatusMode = Config.Bind<ConfigGlobal.DefalutStatusMode>("Options",
                "Using default status mode", ConfigGlobal.DefalutStatusMode.DontUse, DESC_DEFAULTSTATUSMODE);

            ConfigGlobal.showWindowKey = Config.Bind<KeyboardShortcut>("Shortcut",
                 "Show controller",
                 new KeyboardShortcut(KeyCode.P, new KeyCode[] { }),
                 "key for show controller.");
            ConfigGlobal.minGravity = Config.Bind<float>("Slider limits",
                             "Gravity: Minimum value",
                             -0.001f,
                             "Minimum value of glavity slider.");
            ConfigGlobal.minGravity.SettingChanged += ChangedSliderLimit;

            ConfigGlobal.maxGravity = Config.Bind<float>("Slider limits",
                             "Gravity: Maximum value",
                             0.001f,
                             "Maximum value of glavity slider.");
            ConfigGlobal.maxGravity.SettingChanged += ChangedSliderLimit;

            ConfigGlobal.minDamping = Config.Bind<float>("Slider limits",
                             "Damping: Minimum value",
                             0,
                             "Minimum value of damping slider.");
            ConfigGlobal.minDamping.SettingChanged += ChangedSliderLimit;

            ConfigGlobal.maxDamping = Config.Bind<float>("Slider limits",
                             "Damping: Maximum value",
                             1,
                             "Maximum value of damping slider.");
            ConfigGlobal.maxDamping.SettingChanged += ChangedSliderLimit;

            ConfigGlobal.minElasticity = Config.Bind<float>("Slider limits",
                             "Elasticity: Minimum value",
                             0,
                             "Minimum value of elasticity slider.");
            ConfigGlobal.minElasticity.SettingChanged += ChangedSliderLimit;

            ConfigGlobal.maxElasticity = Config.Bind<float>("Slider limits",
                             "Elasticity: Maximum value",
                             1,
                             "Maximum value of elasticity slider.");
            ConfigGlobal.maxElasticity.SettingChanged += ChangedSliderLimit;

            ConfigGlobal.minStiffness = Config.Bind<float>("Slider limits",
                             "Stiffness: Minimum value",
                             0,
                             "Minimum value of stiffness slider.");
            ConfigGlobal.minStiffness.SettingChanged += ChangedSliderLimit;

            ConfigGlobal.maxStiffness = Config.Bind<float>("Slider limits",
                             "Stiffness: Maximum value",
                             1,
                             "Maximum value of stiffness slider.");
            ConfigGlobal.maxStiffness.SettingChanged += ChangedSliderLimit;

            ConfigGlobal.minInert = Config.Bind<float>("Slider limits",
                             "Inert: Minimum value",
                             0,
                             "Minimum value of inert slider.");
            ConfigGlobal.minInert.SettingChanged += ChangedSliderLimit;

            ConfigGlobal.maxInert = Config.Bind<float>("Slider limits",
                             "Inert: Maximum value",
                             1,
                             "Maximum value of inert slider.");
            ConfigGlobal.maxInert.SettingChanged += ChangedSliderLimit;

        }

        private void ChangedSliderLimit(object sender, EventArgs e)
        {
            window.ReloadConfig();
        }

    }
}
