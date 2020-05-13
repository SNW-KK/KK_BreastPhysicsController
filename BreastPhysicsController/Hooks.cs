using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HarmonyLib;
using BepInEx.Harmony;
using ExtensibleSaveFormat;
using BepInEx.Logging;
using KKAPI;

namespace BreastPhysicsController
{
    public static class Hooks
    {
        public static void InstallHooks()
        {
            var harmony = HarmonyWrapper.PatchAll(typeof(Hooks));
        }

        //for performance imporovement. but it makes them less compatible with other logic.
        [HarmonyPrefix, HarmonyPatch(typeof(BustSoft), "ReCalc")]
        public static bool BustSoft_ReCalc_Pre(BustSoft __instance)
        {
            BreastDynamicBoneController controller = ControllerManager.GetControllerByBustSoft(__instance);
            if (controller != null && controller.enable)
            {
                return false;
            }
            return true;
        }

        //this is safe code, but It degrades performance especially when used with ABMX
        //[HarmonyPostfix, HarmonyPatch(typeof(BustSoft), "ReCalc")]
        //public static void BustSoft_ReCalc_Post(BustSoft __instance)
        //{
        //    BreastDynamicBoneController controller = ControllerManager.GetControllerByBustSoft(__instance);
        //    if(controller!=null && controller.Started)
        //    {
        //        controller.OnBustSoftRecalc();
        //    }
        //}
    }
}

