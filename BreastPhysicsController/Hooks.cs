using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Harmony;
using BepInEx.Logging;
using KKAPI;

namespace BreastPhysicsController
{
    public static class Hooks
    {
        public static void InstallHooks()
        {
            var harmony = Harmony.HarmonyInstance.Create(BreastPhysicsController.GUID);
            harmony.PatchAll(typeof(Hooks));
        }

        [HarmonyPostfix, HarmonyPatch(typeof(BustSoft), "ReCalc")]
        public static void BustSoftReCalc(BustSoft __instance)
        {
            BreastDynamicBoneController controller = ControllerManager.GetControllerByBustSoft(__instance);
            if(controller!=null)
            {
                //if (controller.needInitialLoad)
                //{
                //    controller.InitialLoadParameter();
                //}
                if (controller.enable)
                {
                    controller.needUpdate = true;
                }
            }

        }
    }
}

