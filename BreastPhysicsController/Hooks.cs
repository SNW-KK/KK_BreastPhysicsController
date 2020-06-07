using HarmonyLib;
using BepInEx.Harmony;


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
            ParamCharaController controller = DBControllerManager.GetControllerByBustSoft(__instance);
            if (controller != null && controller.Enabled && controller.isEnabledNowBust())
            {
                return false;
            }
            return true;
        }
        //for performance imporovement. but it makes them less compatible with other logic.
        [HarmonyPrefix, HarmonyPatch(typeof(BustGravity), "ReCalc")]
        public static bool BustGravity_ReCalc_Pre(BustGravity __instance)
        {
            ParamCharaController controller = DBControllerManager.GetControllerByBustGravity(__instance);
            if (controller != null && controller.Enabled && controller.isEnabledNowBust())
            {
                return false;
            }
            return true;
        }

        //called clohes state changed.
        //clothesKind=0(tops),2(bra)
        //state=0(wearing),3(stripped)
        [HarmonyPrefix,HarmonyPatch(typeof(ChaControl), "SetClothesState")]
        public static void ChaControl_SetClothesState(ChaControl __instance, int clothesKind, byte state, bool next = true)
        {
            DBControllerManager.GetControllerByChaControl(__instance)?.OnClothesStateChanged();
        }
    }
}

