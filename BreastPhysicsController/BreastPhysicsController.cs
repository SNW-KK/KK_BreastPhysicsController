using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BepInEx;
using BepInEx.Logging;
using UnityEngine;
using System.IO;
using System.Reflection;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using KKAPI.Chara;


namespace BreastPhysicsController
{
    [BepInPlugin(GUID : GUID , Name : Name, Version : Version)]
    [BepInDependency("com.bepis.bepinex.extendedsave",BepInDependency.DependencyFlags.HardDependency)]
    public class BreastPhysicsController : BaseUnityPlugin
    {
        public const string GUID = "com.snw.bepinex.breastphysicscontroller";
        public const string Name = "BreastPhysicsController";
        public const string Version = "1.2";
        public static string PresetDir;

        internal static new ManualLogSource Logger;

        public static ControllerWindow window;
        private const int windowID = 1192;
        private const int s_dialogID = 1193;

        //Flags for Window
        public static bool w_NeedUpdateCharaList;
        public static bool w_NeedUpdateValue;


        private void Awake()
        {
            Logger = base.Logger;
            Hooks.InstallHooks();
            PresetDir = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), Name);
            w_NeedUpdateCharaList = false;
            w_NeedUpdateValue = false;
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

            CharacterApi.RegisterExtraBehaviour<BreastDynamicBoneController>(BreastDynamicBoneController.GUID);
            window = new ControllerWindow(windowID, s_dialogID);
        }

        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.P))
            {
                if (window._showWindow) window._showWindow = false;
                else window._showWindow = true;
            }
        }

        public void OnGUI()
        {
            if (window._showWindow)
            {
                window.WindowRect = GUI.Window(1192, window.WindowRect, window.Draw, window._windowTitle);
            }
            UpdateWindow();

            if (window.s_dialog._show)
            {
                window.s_dialog.OnGUI();
            }
        }

        public void UpdateWindow()
        {
            //For ControllerWindow
            //Changed selected character
            if (window.charaSelect.changed)
            {
                window.charaSelect.changed = false;
                //Logger.LogFormatted(LogLevel.Debug, "Changed selected character");
                BreastDynamicBoneController controller = ControllerManager.GetControllerByID(window.charaSelect.GetSelectedId());
                if (controller != null)
                {
                    window.RefreshValue();
                }
                else
                {
                    window.ResetWindowValue();
                }
            }

            //Need update charalist
            if (w_NeedUpdateCharaList)
            {
                w_NeedUpdateCharaList = false;
                w_NeedUpdateValue = true;
                window.RefreshCharaList();
            }

            //Need update window parameters
            if (w_NeedUpdateValue)
            {
                w_NeedUpdateValue = false;
                window.RefreshValue();
            }

            //Enabled or Disabled
            if (window.controllEnable.changed)
            {
                BreastDynamicBoneController controller = ControllerManager.GetControllerByID(window.charaSelect.GetSelectedId());
                if(controller!=null)
                {
                    if (window.controllEnable.GetValue())
                    {
                        //window.ApplyParameterToController(controller);
                        controller.enable = true;
                        controller.OnEnableController();
                    }
                    else
                    {
                        controller.enable = false;
                        controller.OnDisableController();
                        //controller.onDisable = true;
                    }
                }
            }

            //Changed Parameter
            if (window.CheckParameterChanged())
            {
                window._parameterChanged = false;
                BreastDynamicBoneController controller = ControllerManager.GetControllerByID(window.charaSelect.GetSelectedId());
                //if(controller!=null) window.ApplyParameterToController(controller);//original
                controller.OnWindowValueChanged(window);
            }

            //Preset was Loaded
            if(window.presetSelect.changed)
            {
                window.presetSelect.changed = false;
                string xmlPath = window.presetSelect.GetSelectedFilePath();
                if(xmlPath!=null)
                {
                    BreastDynamicBoneController controller = ControllerManager.GetControllerByID(window.charaSelect.GetSelectedId());
                    if (controller != null)
                    {
                        controller.DynamicBoneParameter.LoadFile(xmlPath);
                        w_NeedUpdateValue = true;
                    }
                    
                }

                    
            }


        }

    }
}
