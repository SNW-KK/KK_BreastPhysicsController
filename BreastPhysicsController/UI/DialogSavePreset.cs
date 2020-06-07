using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using System.IO;
using BreastPhysicsController.UI.Util;

namespace BreastPhysicsController.UI
{
    public class DialogSavePreset
    {
        public ControllerWindow _parent;
        public string _windowTitle;
        public int _windowID;
        public string _fileName;

        //Flags
        public bool _show = false;

        //GUI Component
        public Rect WindowRect;

        public DialogSavePreset(ControllerWindow parent,int windowID, string windowTitle, Rect rect)
        {
            _parent = parent;
            _windowID = windowID;
            _windowTitle = windowTitle;
            WindowRect = rect;
            _fileName = "Enter preset's filename.";
            InitGUI();
        }

        public void InitGUI()
        {

        }

        public void OnGUI()
        {
            if (_show)
            {
                WindowRect=GUI.ModalWindow(_windowID, WindowRect, Draw, _windowTitle,Style.Window);
            }
        }

        public void Draw(int windowID)
        {
            GUILayout.BeginArea(new Rect(2, 2, WindowRect.width - 4, WindowRect.height - 4), Style.WindowInside);


            //Title
            GUILayout.BeginArea(new Rect(3, 3, WindowRect.width - 10, 20), Style.WindowContents);
            GUILayout.Label(_windowTitle, Style.LabelWindowTitle);
            GUILayout.EndArea();

            //Contentes
            GUILayout.BeginArea(new Rect(3, 26, WindowRect.width - 10, WindowRect.height - 33),Style.WindowContents);
            _fileName =GUILayout.TextField(_fileName);

            if (GUILayout.Button("Save"))
            {
                if (SaveFile(_fileName))
                {
                    _parent._showWindow = true;
                    _show = false;
                }
            }

            if (GUILayout.Button("Cancel"))
            {
                _parent._showWindow = true;
                _show = false;
            }
            GUILayout.EndArea();

            GUILayout.EndArea();

            GUI.DragWindow();

            if (Event.current.type != EventType.KeyDown && Event.current.type != EventType.KeyUp)
            {
                Input.ResetInputAxes();
            }
        }

        private bool SaveFile(string filename)
        {

            if (filename.IsNullOrEmpty()) return false;

            if (!Directory.Exists(PluginPath.presetDirBust))
            {
                Directory.CreateDirectory(PluginPath.presetDirBust);
            }
            string pathBust = Path.Combine(PluginPath.presetDirBust, filename) + ".xml";

            if (!Directory.Exists(PluginPath.presetDirHip))
            {
                Directory.CreateDirectory(PluginPath.presetDirHip);
            }
            string pathHip = Path.Combine(PluginPath.presetDirHip, filename) + ".xml";

            ParamCharaController.ParamsKind paramsKind= _parent.GetSelectedParamsKind();
            switch(paramsKind)
            {
                case ParamCharaController.ParamsKind.Naked:
                    XMLPresetIO.SaveXMLBust(_parent.GetSelectedController().paramCustom.paramBustNaked, pathBust);
                    break;
                case ParamCharaController.ParamsKind.Bra:
                    XMLPresetIO.SaveXMLBust(_parent.GetSelectedController().paramCustom.paramBust[_parent.GetSelectedCoordinate()][paramsKind], pathBust);
                    break;
                case ParamCharaController.ParamsKind.Tops:
                    XMLPresetIO.SaveXMLBust(_parent.GetSelectedController().paramCustom.paramBust[_parent.GetSelectedCoordinate()][paramsKind], pathBust);
                    break;
                case ParamCharaController.ParamsKind.Hip:
                    XMLPresetIO.SaveXMLHip(_parent.GetSelectedController().paramCustom.paramHip, pathHip);
                    break;
                default:
                    break;
            }
            return true;
        }
    }
}
