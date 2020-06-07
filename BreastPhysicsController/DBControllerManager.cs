using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using UnityEngine;
using KKAPI.Chara;

namespace BreastPhysicsController
{
    class DBControllerManager : ScriptableObject
    {

        //public static event Action ControllerListChanged;

        public static bool updatedCharaList = false;

        public static Dictionary<int, ParamCharaController> _controllers { get; private set; } = new Dictionary<int, ParamCharaController>();

        public static bool AddController(ParamCharaController controller)
        {
            try
            {
                _controllers.Add(controller.controllerID, controller);
            }
            catch (ArgumentException)
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
            bool removed= _controllers.Remove(id);
            if(removed)
            {
                return true;
            }
            return false;

        }

        public static List<ParamCharaController> GetAllController()
        {
            return _controllers.Values.ToList();
        }

        public static ParamCharaController GetControllerByID(int id)
        {
            ParamCharaController controller;
            if (_controllers.TryGetValue(id, out controller)) return controller;
            else return null;

        }

        public static ParamCharaController GetControllerByBustSoft(BustSoft bustSoft)
        {

            foreach (ParamCharaController controller in _controllers.Values)
            {
                if (ReferenceEquals(controller.ChaControl.bustSoft, bustSoft))
                {
                    return controller;
                }
            }

            return null;
        }

        public static ParamCharaController GetControllerByBustGravity(BustGravity bustGravity)
        {

            foreach (ParamCharaController controller in _controllers.Values)
            {
                if (ReferenceEquals(controller.ChaControl.bustGravity, bustGravity))
                {
                    return controller;
                }
            }

            return null;
        }

        public static ParamCharaController GetControllerByChaControl(ChaControl chaControl)
        {
            return _controllers.Values.FirstOrDefault(x => ReferenceEquals(x.ChaControl, chaControl));
        }
    }
}
