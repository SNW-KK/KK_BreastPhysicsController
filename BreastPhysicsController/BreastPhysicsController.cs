using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BepInEx;
using BepInEx.Logging;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using KKAPI.Chara;
using System.IO;

namespace BreastPhysicsController
{
    [BepInPlugin(GUID : GUID , Name : Name, Version : Version)]
    public class BreastPhysicsController : BaseUnityPlugin
    {
        public const string GUID = "com.snw.bepinex.breastphysicscontroller";
        public const string Name = "BreastPhysicsController";
        public const string Version = "1.1";

        public ControllerWindow window;
        private const int windowID = 1192;
        private const int s_dialogID = 1193;

        //Flags for Window
        public static bool w_NeedUpdateCharaList;
        public static bool w_NeedUpdateValue;



        private void Awake()
        {
            w_NeedUpdateCharaList = false;
            w_NeedUpdateValue = false;
        }

        private void Start()
        {
            CharacterApi.RegisterExtraBehaviour<BreastDynamicBoneController>(BreastDynamicBoneController.GUID);
            window = new ControllerWindow(windowID, s_dialogID);
            Hooks.InstallHooks();
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

            if(window.s_dialog._show)
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
                Logger.LogFormatted(LogLevel.Debug, "Changed selected character");
                BreastDynamicBoneController controller = ControllerManager.GetControllerByID(window.charaSelect.GetSelectedId());
                if (controller != null)
                {
                    //controller.LoadParamsFromCharacter();
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
                        window.ApplyParameterToController(controller);
                    }
                    else
                    {
                        controller.onDisable = true;
                    }
                }
            }

            //Changed Parameter
            if (window.CheckParameterChanged())
            {
                window._parameterChanged = false;
                BreastDynamicBoneController controller = ControllerManager.GetControllerByID(window.charaSelect.GetSelectedId());
                if(controller!=null) window.ApplyParameterToController(controller);
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
