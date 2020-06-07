using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using BreastPhysicsController.UI.Util;

namespace BreastPhysicsController.UI.Parts
{
    public class ToggleRef
    {
        string _text;

        public ToggleRef(string text)
        {
            _text = text;

        }

        public bool Draw(ref bool value,GUIStyle style=null)
        {
            if (style == null) style = new GUIStyle(Skin.defaultSkin.toggle);

            bool changed = false;

            bool newValue = GUILayout.Toggle(value, _text,style);
            if(newValue != value)
            {
                changed = true;
                value = newValue;
            }
            return changed;
        }
    }
}
