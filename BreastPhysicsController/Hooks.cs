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

        [HarmonyPostfix, HarmonyPatch(typeof(BustSoft), "ReCalc")]
        public static void BustSoftReCalc(BustSoft __instance)
        {
            BreastDynamicBoneController controller = ControllerManager.GetControllerByBustSoft(__instance);
            if(controller!=null && controller.Started)
            {
                controller.OnBustSoftRecalc();
            }

        }
    }
}

