using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BepInEx.Logging;
using UnityEngine;
using UnityEngine.UI;

namespace BreastPhysicsController.UI.Parts
{
    public class SliderAndTextBox
    {
        string _text;
        float _sliderWidth;
        float _textWidth;
        float _height;

        public bool changed;

        float _maxValue, _minValue, _value;
        string _textValue;

        public SliderAndTextBox(string text, float minValue, float maxValue, float sliderWidth,float textWidth, float height = 0)
        {
            _text = text;
            _sliderWidth = sliderWidth;
            _textWidth = textWidth;
            _height = height;

            changed = false;

            _maxValue = maxValue;
            _minValue = minValue;
            _value = minValue;
            _textValue = _value.ToString();
        }

        public void SetValue(float value)
        {
            _value = value;
            _textValue = value.ToString();
            changed = false;
        }

        public float GetValue()
        {
            return _value;
        }

        public bool Draw()
        {
            changed = false;

            GUILayout.BeginVertical();

            GUILayout.Label(_text);

            GUILayout.BeginHorizontal();

            float value = GUILayout.HorizontalSlider(_value, _minValue, _maxValue,GUILayout.Width(_sliderWidth));

            if (value != _value)
            {
                changed = true;
                _value = value;
                _textValue = _value.ToString();
            }

            string textValue = GUILayout.TextField(_textValue,GUILayout.Width(_textWidth));
            if (textValue != _textValue && isPossibleParseFloat(textValue))
            {
                changed = true;
                _value = Mathf.Clamp(float.Parse(textValue), _minValue, _maxValue);
                _textValue = textValue;
            }

            GUILayout.EndHorizontal();

            GUILayout.EndVertical();

            return changed;
        }

        private bool isPossibleParseFloat(string text)
        {
            float buff;
            if (float.TryParse(text, out buff)) return true;
            return false;
        }
    }

}
