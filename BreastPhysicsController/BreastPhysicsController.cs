using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BepInEx;
using BepInEx.Logging;
using UnityEngine;
using UnityEngine.SceneManagement;
using KKAPI.Chara;

namespace BreastPhysicsController
{
    [BepInPlugin(GUID : GUID , Name : Name, Version : Version)]
    public class BreastPhysicsController : BaseUnityPlugin
    {
        public const string GUID = "com.snw.bepinex.breastphysicscontoller";
        public const string Name = "BreastPhysicsController";
        public const string Version = "1.0";

        public ControllerWindow window;
        private const int windowID = 1192;

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
            window = new ControllerWindow(windowID);
            Hooks.InstallHooks();
        }

        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.P))
            {
                if (window.showWindow) window.showWindow = false;
                else window.showWindow = true;
            }
        }

        public void OnGUI()
        {
            if (window.showWindow)
            {
                window.WindowRect = GUI.Window(1192, window.WindowRect, window.Draw, window.windowTitle);
            }
            UpdateWindow();

        }

        public void UpdateWindow()
        {
            if (w_NeedUpdateCharaList)
            {
                w_NeedUpdateCharaList = false;
                w_NeedUpdateValue = true;
                window.RefreshCharaList();
            }

            if (w_NeedUpdateValue)
            {
                w_NeedUpdateValue = false;
                window.RefreshValue();
            }

            if (window.controllEnable.changed)
            {
                BreastDynamicBoneController controller = ControllerManager.GetControllerByID(window.charaSelect.GetSelectedId());
                if (window.controllEnable.GetValue())
                {
                    window.ApplyParameterToController(controller);
                }
                else
                {
                    controller.onDisable = true;
                }
            }
            
            if (window.CheckParameterChanged())
            {
                window.parameterChanged = false;
                BreastDynamicBoneController controller = ControllerManager.GetControllerByID(window.charaSelect.GetSelectedId());
                window.ApplyParameterToController(controller);
            }
        }


    }
}
