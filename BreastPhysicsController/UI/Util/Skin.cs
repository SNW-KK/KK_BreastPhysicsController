using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using System.Reflection;

namespace BreastPhysicsController.UI.Util
{
    public static class Skin
    {
        public static GUISkin defaultSkin;

        static Skin()
        {
            defaultSkin = GetDefaultSkin();
        }

        private static GUISkin GetDefaultSkin()
        {
            MethodInfo methodInfo = typeof(GUIUtility).GetMethod("GetDefaultSkin", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Static);
            return (GUISkin)methodInfo.Invoke(null, null);
        }
    }


}
