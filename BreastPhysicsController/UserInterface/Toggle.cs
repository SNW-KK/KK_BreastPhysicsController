using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace BreastPhysicsController
{
    public class Toggle
    {
        string _text;
        public bool changed;
        bool _value;

        public Toggle(string text, bool value)
        {
            _text = text;
            _value = value;
            changed = false;

        }

        public void SetValue(bool value)
        {
            _value = value;
            changed = false;
        }

        public bool GetValue()
        {
            return _value;
        }

        public bool Draw()
        {
            changed = false;

            bool value = GUILayout.Toggle(_value, _text);
            if(value!=_value)
            {
                changed = true;
                _value = value;
            }
            return changed;
        }
    }
}
