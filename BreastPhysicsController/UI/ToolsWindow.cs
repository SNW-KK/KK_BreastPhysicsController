using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using BreastPhysicsController.UI.Parts;
using BreastPhysicsController.UI.Util;

namespace BreastPhysicsController.UI
{
    public class ToolsWindow
    {
        ControllerWindow _parent;

        public bool _show { get; private set; }
        int _windowID;
        public Rect WindowRect;

        CoordinateSelect coordinateSelect;
        PresetSelect presetSelectBust, presetSelectHip;

        //DialogSave
        public int _s_dialogID;
        public DialogSavePreset s_dialog;
        public Rect SDRect = new Rect(Screen.width / 2 - 150, Screen.height / 2 - 50, 300, 120);

        //MessageBox
        public MessageBox msgBox;

        public ToolsWindow(int windowID, int s_dialogID,ControllerWindow parent)
        {
            WindowRect = new Rect(parent.WindowRect.x + parent.WindowRect.width,
                                parent.WindowRect.y,
                                280,
                                345);
            _show = false;
            _windowID = windowID;
            _parent = parent;
            _s_dialogID = s_dialogID;

            InitGUI();
        }

        private void InitGUI()
        {
            //Coordinate Select
            ChaFileDefine.CoordinateType[] coordinateList = new ChaFileDefine.CoordinateType[]
            {
                ChaFileDefine.CoordinateType.School01,
                ChaFileDefine.CoordinateType.School02,
                ChaFileDefine.CoordinateType.Gym,
                ChaFileDefine.CoordinateType.Swim,
                ChaFileDefine.CoordinateType.Club,
                ChaFileDefine.CoordinateType.Plain,
                ChaFileDefine.CoordinateType.Pajamas,
            };
            List<string> coordinateStringList = new List<string>();
            for (int i = 0; i < coordinateList.Count(); i++)
            {
                coordinateStringList.Add(coordinateList[i].ToString());
            }
            coordinateSelect = new CoordinateSelect(coordinateStringList.ToArray(), coordinateList, "");

            //Preset select
            presetSelectBust = new PresetSelect(PluginPath.presetDirBust, "Load Bust preset", 0, 20);
            presetSelectHip = new PresetSelect(PluginPath.presetDirHip, "Load Hip preset", 0, 20);

            //Save dialog
            s_dialog = new DialogSavePreset(_parent, _s_dialogID, "Save preset", SDRect);

            //message box
            msgBox = new MessageBox(WindowID.GetNewID());
        }

        public void OnGUI()
        {
            WindowRect.x = _parent.WindowRect.x + _parent.WindowRect.width;
            WindowRect.y = _parent.WindowRect.y;

            if (_show)
            {
                WindowRect = GUI.Window(_windowID, WindowRect, Draw, "Tools",Style.Window);
            }

            s_dialog.OnGUI();

            msgBox.OnGUI();
        }

        public void Draw(int windowID)
        {
            //Outline
            GUILayout.BeginArea(new Rect(2, 2, WindowRect.width - 4, WindowRect.height - 4), Style.WindowInside);

            //Title
            GUILayout.BeginArea(new Rect(3, 3, WindowRect.width - 10, 20), Style.WindowContents);
            GUILayout.Label("Tools", Style.LabelWindowTitle);
            GUILayout.EndArea();

            //Contents
            

            if (presetSelectBust._show) //Show preset select
            {
                GUILayout.Label("Select one preset to load.",Style.LabelLarge);
                if (presetSelectBust.Draw())
                {
                    OnPresetSelected(presetSelectBust);
                }
            }
            else if (presetSelectHip._show) //Show preset select
            {
                GUILayout.Label("Select one preset to load.", Style.LabelLarge);
                if (presetSelectHip.Draw())
                {
                    OnPresetSelected(presetSelectHip);
                }
            }
            else
            {

                //Enable or Disable all
                GUILayout.BeginArea(new Rect(3, 26, WindowRect.width - 10, 55), Style.WindowContents);
                GUILayout.Label("Enable or Disable",Style.LabedMiddleSubject);
                GUILayout.BeginHorizontal();
                if (GUILayout.Button("Disable all"))
                {
                    _parent.GetSelectedController().ChangeEnabledAll(false);

                }
                if (GUILayout.Button("Enable all"))
                {
                    _parent.GetSelectedController().ChangeEnabledAll(true);
                }
                GUILayout.EndHorizontal();
                GUILayout.EndArea();

                //if show destination coordinate for copy, dont draw area under Copy tools.
                //dont forget to expand area.
                if (coordinateSelect._show)
                {
                    //Copy tools
                    GUILayout.BeginArea(new Rect(3, 84, WindowRect.width - 10, 254), Style.WindowContents); //expand area than when coordinateSelect isnt shown.
                    GUILayout.Label("Copy", Style.LabedMiddleSubject);
                    if (GUILayout.Button("Copy parameter set to all coordinates"))
                    {
                        _parent.GetSelectedController().CopyParamAllCoordinate(_parent.GetSelectedCoordinate());
                        msgBox.Show("Copied parameter set to all coordinates.");
                    }
                    GUILayout.BeginHorizontal();
                    if (GUILayout.Button("Copy parameter set"))
                    {
                        _parent.GetSelectedController().CopyParamCoordinate(_parent.GetSelectedCoordinate(), coordinateSelect.GetSelectedCoordinate());
                        msgBox.Show("Copied parameter set\r\n" + _parent.GetSelectedCoordinate() + " to " + coordinateSelect.GetSelectedCoordinate());

                    }
                    GUILayout.Label("to", Style.LabedMiddleSubject);
                    coordinateSelect.Draw();
                    GUILayout.EndHorizontal();
                    GUILayout.EndArea();
                }
                else
                {
                    //Copy tools
                    GUILayout.BeginArea(new Rect(3, 84, WindowRect.width - 10, 80), Style.WindowContents);
                    GUILayout.Label("Copy", Style.LabedMiddleSubject);
                    if (GUILayout.Button("Copy parameter set to all coordinates"))
                    {
                        _parent.GetSelectedController().CopyParamAllCoordinate(_parent.GetSelectedCoordinate());
                        msgBox.Show("Copied parameter set to all coordinates.");
                    }
                    GUILayout.BeginHorizontal();
                    if (GUILayout.Button("Copy parameter set"))
                    {
                        _parent.GetSelectedController().CopyParamCoordinate(_parent.GetSelectedCoordinate(), coordinateSelect.GetSelectedCoordinate());
                        msgBox.Show("Copied parameter set\r\n" + _parent.GetSelectedCoordinate() + " to " + coordinateSelect.GetSelectedCoordinate());

                    }
                    GUILayout.Label("to", Style.LabedMiddleSubject);
                    coordinateSelect.Draw();
                    GUILayout.EndHorizontal();
                    GUILayout.EndArea();

                    //Preset Load
                    GUILayout.BeginArea(new Rect(3, 167, WindowRect.width - 10, 80), Style.WindowContents);
                    GUILayout.Label("Preset", Style.LabedMiddleSubject);
                    GUILayout.BeginHorizontal();
                    presetSelectBust.Draw();
                    presetSelectHip.Draw();
                    GUILayout.EndHorizontal();

                    //Preset Save
                    if (GUILayout.Button("Save preset"))
                    {
                        s_dialog._show = true;
                    }
                    GUILayout.EndArea();

                    //For default parameter
                    GUILayout.BeginArea(new Rect(3, 250, WindowRect.width - 10, 55), Style.WindowContents);
                    GUILayout.Label("Default Status", Style.LabedMiddleSubject);
                    GUILayout.BeginHorizontal();
                    if (GUILayout.Button("Save as default"))
                    {
                        if (_parent.GetSelectedController().SaveDefaultStatus())
                        {
                            msgBox.Show("Saved default status");
                        }
                    }
                    if (GUILayout.Button("Load default"))
                    {
                        if (_parent.GetSelectedController().LoadDefaultStatus(false))
                        {
                            msgBox.Show("Loaded default status");
                        }

                    }
                    GUILayout.EndHorizontal();
                    GUILayout.EndArea();

                    GUILayout.BeginArea(new Rect(3, 308, WindowRect.width - 10, 30), Style.WindowContents);
                    if (GUILayout.Button("Close this window"))
                    {
                        _show = false;
                    }
                    GUILayout.EndArea();
                }

            }

            GUILayout.EndArea();

            if (WindowRect.Contains(new Vector2(Input.mousePosition.x, Screen.height - Input.mousePosition.y)) &&
            Event.current.type != EventType.KeyDown && Event.current.type != EventType.KeyUp)
            {
                Input.ResetInputAxes();
            }

            GUI.DragWindow();
        }

        public void Show(Vector2 point)
        {
            _show = true;
        }

        public void Hide()
        {
            _show = false;
        }

        private bool OnPresetSelected(PresetSelect presetSelect)
        {
            ParamCharaController controller = _parent.GetSelectedController();
            ChaFileDefine.CoordinateType coordinate = _parent.GetSelectedCoordinate();
            ParamCharaController.ParamsKind kind = _parent.GetSelectedParamsKind();
            string xmlPath = presetSelect.GetSelectedFilePath();
            switch (_parent.GetSelectedParamsKind())
            {
                case ParamCharaController.ParamsKind.Naked:
                    if (!XMLPresetIO.LoadXMLBust(controller.paramCustom.paramBustNaked, xmlPath)) return false;
                    break;
                case ParamCharaController.ParamsKind.Bra:
                    if (!XMLPresetIO.LoadXMLBust(controller.paramCustom.paramBust[coordinate][kind], xmlPath)) return false;
                    break;
                case ParamCharaController.ParamsKind.Tops:
                    if (!XMLPresetIO.LoadXMLBust(controller.paramCustom.paramBust[coordinate][kind], xmlPath)) return false;
                    break;
                case ParamCharaController.ParamsKind.Hip:
                    if (!XMLPresetIO.LoadXMLHip(controller.paramCustom.paramHip, xmlPath)) return false;
                    break;
                default:
                    break;
            }
            controller.changedInfo.SetInfo(coordinate, kind, false, true, false);
            return true;
        }

    }
}
