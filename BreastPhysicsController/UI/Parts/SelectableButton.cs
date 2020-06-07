using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Reflection;
using System.Text;

namespace BreastPhysicsController.UI.Parts
{
    public class SelectableButton<T> where T : IComparable
    {
        List<T> _values;
        string[] _buttonTexts;
        protected int _selectedIndex;
        GUIStyle _selectedButtonStyle;

        public SelectableButton(Color colorSelected)
        {
            _selectedIndex = 0;
            MethodInfo methodInfo = typeof(GUIUtility).GetMethod("GetDefaultSkin", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Static);
            GUISkin skin=(GUISkin)methodInfo.Invoke(null, null);
            _selectedButtonStyle = new GUIStyle(skin.button);
            _selectedButtonStyle.normal.textColor = colorSelected;
            _selectedButtonStyle.hover.textColor = colorSelected;

            _values = new List<T>();
            foreach (T value in Enum.GetValues(typeof(T)))
            {
                _values.Add(value);
            }
            List<string> labels = new List<string>();
            foreach (string name in Enum.GetNames(typeof(T)))
            {
                labels.Add(name);
            }
            _buttonTexts = labels.ToArray();
        }

        public int Draw()
        {
            GUILayout.BeginHorizontal();
            for (int i = 0; i < _buttonTexts.Count(); i++)
            {
                if (_selectedIndex == i)
                {
                    if(GUILayout.Button(_buttonTexts[i], _selectedButtonStyle))
                    {
                        _selectedIndex = i;
                    }
                }
                else
                {
                    if (GUILayout.Button(_buttonTexts[i]))
                    {
                        _selectedIndex = i;
                    }

                }
            }
            GUILayout.EndHorizontal();
            return _selectedIndex;
        }

        public T GetSelected()
        {
            return _values[_selectedIndex];
        }

        public void Select(T value)
        {
            int index=_values.FindIndex((x => x.CompareTo(value) == 0));
            if(index>=0 && index<_buttonTexts.Count())
            {
                _selectedIndex = index;
            }
        }

        public void Select(int index)
        {
            if(index<_values.Count() && index<_buttonTexts.Count())
            {
                _selectedIndex = index;
            }
        }
    }
}
