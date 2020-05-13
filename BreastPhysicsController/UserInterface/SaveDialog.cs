using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using System.IO;

namespace BreastPhysicsController
{
    public class SaveDialog
    {
        public ControllerWindow _parent;
        public string _windowTitle;
        public int _windowID;
        private string _saveDir = @".\BepInEx\plugins\BreastPhysicsController\";

        public string _fileName;

        //Flags
        public bool _show = false;

        //GUI Component
        public Rect WindowRect;


        public SaveDialog(ControllerWindow parent,int windowID, string windowTitle, Rect rect)
        {
            _parent = parent;
            _windowID = windowID;
            _windowTitle = windowTitle;
            WindowRect = rect;
            _fileName = "";
            InitGUI();
        }

        public void InitGUI()
        {

        }

        public void OnGUI()
        {
            if (_show)
            {
                WindowRect = GUI.Window(_windowID, WindowRect, Draw, _windowTitle);
            }
        }

        public void Draw(int windowID)
        {
            _fileName=GUILayout.TextField(_fileName);

            if (GUILayout.Button("Save"))
            {
                SaveFile(_fileName);
                _parent._showWindow = true;
                _show = false;
            }

            if (GUILayout.Button("Cancel"))
            {
                _parent._showWindow = true;
                _show = false;
            }

            Input.ResetInputAxes();
            
            GUI.DragWindow();
        }

        private void SaveFile(string filename)
        {
            XMLDynamicBoneParameter parameterXML = new XMLDynamicBoneParameter();
            List<ParameterSetXML> parameterSets = new List<ParameterSetXML>();

            ParameterSetXML bust01 = new ParameterSetXML();
            bust01.PartName = "Bust01";
            bust01.IsRotationCalc = _parent.irc01.GetValue();
            bust01.Damping = _parent.damping01.GetValue();
            bust01.Elasticity = _parent.elasticity01.GetValue();
            bust01.Stiffness = _parent.stiffness01.GetValue();
            bust01.Inert = _parent.inert01.GetValue();
            parameterSets.Add(bust01);

            ParameterSetXML bust02 = new ParameterSetXML();
            bust02.PartName = "Bust02";
            bust02.IsRotationCalc = _parent.irc02.GetValue();
            bust02.Damping = _parent.damping02.GetValue();
            bust02.Elasticity = _parent.elasticity02.GetValue();
            bust02.Stiffness = _parent.stiffness02.GetValue();
            bust02.Inert = _parent.inert02.GetValue();
            parameterSets.Add(bust02);

            ParameterSetXML bust03 = new ParameterSetXML();
            bust03.PartName = "Bust03";
            bust03.IsRotationCalc = _parent.irc03.GetValue();
            bust03.Damping = _parent.damping03.GetValue();
            bust03.Elasticity = _parent.elasticity03.GetValue();
            bust03.Stiffness = _parent.stiffness03.GetValue();
            bust03.Inert = _parent.inert03.GetValue();
            parameterSets.Add(bust03);

            parameterXML.parameterSets = parameterSets;

            string savePath = Path.Combine(_saveDir, filename) + ".xml";
            using (FileStream fs = new FileStream(savePath, FileMode.Create, FileAccess.Write))
            using (StreamWriter sw = new StreamWriter(fs, Encoding.UTF8))
            {
                parameterXML.Serialize(sw);
            }
                
        }
    }
}
