using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using BreastPhysicsController.UI.Util;

namespace BreastPhysicsController.UI.Parts
{
    class ToggleEnabled:Toggle
    {
        public ToggleEnabled(string text,bool value):base(text,value)
        {

        }

        public bool Draw(ParamCharaController controller,GUIStyle style=null)
        {
            if (style == null) style = Skin.defaultSkin.toggle;

            bool changed = false;

            bool newValue = GUILayout.Toggle(controller.Enabled, _text, style);
            if (newValue != controller.Enabled)
            {
                changed = true;
                controller.Enabled = newValue;
            }
            return changed;
        }
    }
}
