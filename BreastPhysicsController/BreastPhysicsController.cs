using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BepInEx;
using BepInEx.Logging;
using UnityEngine;
using UnityEngine.SceneManagement;
using BreastPhysicsController.UserInterface;
using KKAPI.Chara;

namespace BreastPhysicsController
{
    [BepInPlugin(GUID : GUID , Name : Name, Version : Version)]
    public class BreastPhysicsController : BaseUnityPlugin
    {
        public const string GUID = "com.snw.bepinex.breastphysicscontoller";
        public const string Name = "BreastPhysicsController";
        public const string Version = "1.0";

        private ControllerWindow window;
        private const int windowID = 1192;

        private void Start()
        {
            CharacterApi.RegisterExtraBehaviour<BreastDynamicBoneController>(BreastDynamicBoneController.GUID);
            window = new ControllerWindow(windowID);
            Hooks.InstallHooks();
        }

        public void Update()
        {
            if(Input.GetKeyDown(KeyCode.P))
            {
                if (window.showWindow) window.showWindow = false;
                else window.showWindow = true;
            }
        }

        public void OnGUI()
        {
            window.OnGUI();
            
        }


    }
}
