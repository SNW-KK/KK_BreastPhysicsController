using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using UnityEngine;
using KKAPI.Chara;

namespace BreastPhysicsController
{
    class ControllerManager : ScriptableObject
    {

        public static bool updatedCharaList=false;

        private static Dictionary<int, BreastDynamicBoneController> controllers = new Dictionary<int, BreastDynamicBoneController>();

        public static bool AddController(BreastDynamicBoneController controller)
        {
            try
            {
                controllers.Add(controller.controllerID, controller);
            }
            catch(ArgumentException)
            {
#if DEBUG
                BreastPhysicsController.Logger.LogDebug("controllers.add failed. the controllerID already exists.");
#endif
                return false;
            }
            return true;
        }

        public static bool RemoveController(int id)
        {
            return controllers.Remove(id);
        }

        public static List<BreastDynamicBoneController> GetAllController()
        {
            return controllers.Values.ToList();
            //return CharacterApi.GetBehaviours().Where(x => x.ExtendedDataId == BreastDynamicBoneController.GUID).Cast<BreastDynamicBoneController>().ToList();
        }

        public static BreastDynamicBoneController GetControllerByID(int id)
        {
            BreastDynamicBoneController controller;
            if (controllers.TryGetValue(id, out controller)) return controller;
            else return null;

            //Using CharacterApi Version
            /*
            return CharacterApi.GetBehaviours().Where(x => x.ExtendedDataId == BreastDynamicBoneController.GUID)
            .Cast<BreastDynamicBoneController>().Where( x=> x.controllerID == id).FirstOrDefault();
            */
        }

        public static BreastDynamicBoneController GetControllerByBustSoft(BustSoft bustSoft)
        {

            foreach (BreastDynamicBoneController controller in controllers.Values)
            {
                if (ReferenceEquals(controller.ChaControl.bustSoft, bustSoft))
                {
                    return controller;
                }
            }

            //List<BreastDynamicBoneController> controllers = GetAllController();
            //foreach(BreastDynamicBoneController controller in controllers)
            //{
            //    if(ReferenceEquals(controller.ChaControl.bustSoft, bustSoft))
            //    {
            //        return controller;
            //    }
            //}

            return null;
        }
    }
}
