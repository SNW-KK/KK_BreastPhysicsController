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
        //public Dictionary<int, BreastDynamicBoneController> dictController;
        public static bool updatedCharaList=false;

        public static List<BreastDynamicBoneController> GetAllController()
        {
            return CharacterApi.GetBehaviours().Where(x => x.ExtendedDataId == BreastDynamicBoneController.GUID).Cast<BreastDynamicBoneController>().ToList();
        }

        public static BreastDynamicBoneController GetControllerByID(int id)
        {
            return CharacterApi.GetBehaviours().Where(x => x.ExtendedDataId == BreastDynamicBoneController.GUID)
            .Cast<BreastDynamicBoneController>().Where( x=> x.controllerID == id).FirstOrDefault();
        }

        public static BreastDynamicBoneController GetControllerByBustSoft(BustSoft bustSoft)
        {
            List<BreastDynamicBoneController> controllers = GetAllController();
            foreach(BreastDynamicBoneController controller in controllers)
            {
                if(ReferenceEquals(controller.ChaControl.bustSoft, bustSoft))
                {
                    return controller;
                }
            }

            return null;
        }
    }
}
