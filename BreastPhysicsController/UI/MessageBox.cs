using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using BreastPhysicsController.UI.Util;
using System.Reflection;

namespace BreastPhysicsController.UI
{
    public class MessageBox
    {
        int _windowId;
        Vector2 position;
        int _width;
        bool _show;
        string _message;

        public int width
        {
            get
            {
                return _width;
            }
            set
            {
                _width = value;
                ReCalcPosition();
            }
        }
        int _height;
        public int height
        {
            get
            {
                return _height;
            }
            set
            {
                _height = value;
                ReCalcPosition();
            }
        }


        public MessageBox(int windowId)
        {
            _width = 200;
            _height = 100;
            ReCalcPosition();
            _windowId = windowId;
            _show = false;

        }
        public void Show(string message)
        {
            _show = true;
            _message = message;
        }

        public void OnGUI()
        {
            if(_show)
            {

                GUI.ModalWindow(_windowId, new Rect(position, new Vector2(_width, _height)), this.Draw, "Message",Style.Window);
            }
        }

        public void Draw(int windowId)
        {
            GUILayout.BeginArea(new Rect(2, 2, _width - 4, _height - 4), Style.WindowInside);

            //Title
            GUILayout.BeginArea(new Rect(3, 3, _width - 10, 20), Style.WindowContents);
            GUILayout.Label("Message", Style.LabelWindowTitle);
            GUILayout.EndArea();

            //Contents
            int backupFontSize = GUI.skin.label.fontSize;
            GUILayout.BeginArea(new Rect(3, 26, _width-10, _height-33),Style.WindowContents);

            GUI.skin.label.alignment = TextAnchor.MiddleCenter;
            GUI.skin.label.fontSize = 13;
            GUILayout.Label(_message);
            GUI.skin.label.alignment = TextAnchor.MiddleLeft;
            if(GUILayout.Button("OK"))
            {
                _show = false;
            }
            GUILayout.EndArea();

            GUI.skin.label.fontSize = backupFontSize;

            GUILayout.EndArea();
        }

        private void ReCalcPosition()
        {
            position.Set(Screen.width / 2 - width / 2, Screen.height / 2 - height / 2);
        }
    }
}
