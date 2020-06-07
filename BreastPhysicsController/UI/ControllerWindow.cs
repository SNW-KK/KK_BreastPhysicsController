using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using BreastPhysicsController.UI.Parts;
using BreastPhysicsController.UI.Util;

namespace BreastPhysicsController.UI
{
    public class ControllerWindow
    {

        public readonly string _windowTitle = "BreastPhysicsController";
        public int _windowID;
        ParamCharaController _controller;
        public Rect WindowRect = new Rect(10, 10, 300, 810);

        //Flags
        public bool _showWindow = false;

        //GUI Component(MainWindow)
        ToggleEnabled controllEnable;
        CharaSelect charaSelect;
        CoordinateSelect coordinateSelect;
        SelectableButton<ParamCharaController.ParamsKind> kindSelect;
        
        //GUI Component(per WearState)
        EditorBust editorBust;
        EditorHip editorHip;

        //DialogSave
        public int _s_dialogID;

        //ToolsWindow
        public ToolsWindow tools;
        int _toolsWindowId;

        //GUI Style
        GUIStyle styleMatchState;

        public ControllerWindow(int windowID,int s_dialogID,int toolsWindowId)
        {
            _controller = null;
            _s_dialogID = s_dialogID;
            _windowID = windowID;
            _toolsWindowId = toolsWindowId;
            InitGUI();
        }

        private void InitGUI()
        {
            //Character Select
            List<ParamCharaController> controllers = DBControllerManager.GetAllController();
            charaSelect = new CharaSelect(DBControllerManager._controllers, "No character loaded", WindowRect.width-20, 25, WindowRect.width - 35, WindowRect.height-50);

            //Enbale Checkbox
            controllEnable = new ToggleEnabled("Enable controller", false);

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
            for (int i=0;i<coordinateList.Count();i++)
            {
                coordinateStringList.Add(coordinateList[i].ToString());
            }
            coordinateSelect=new CoordinateSelect(coordinateStringList.ToArray(),coordinateList,"", 175, 25, WindowRect.width - 35, WindowRect.height - 50);

            //State Select
            kindSelect = new SelectableButton<ParamCharaController.ParamsKind>(Color.cyan);

            //Parameter editors
            editorBust = new EditorBust();
            editorHip = new EditorHip();
            
            //Tools Window
            tools = new ToolsWindow(_toolsWindowId,_s_dialogID,this);

            //GUI Style
            styleMatchState = new GUIStyle(Skin.defaultSkin.button);
            styleMatchState.fixedHeight = 25;
        }

        public void OnGUI()
        {
            if(_showWindow)
            {
                WindowRect.x = tools.WindowRect.x - WindowRect.width;
                WindowRect.y = tools.WindowRect.y;
                WindowRect = GUI.Window(_windowID, WindowRect, Draw, _windowTitle,Style.Window);

                tools.OnGUI();
            }

        }

        public void Draw(int windowID)
        {

            bool changedParam = false;
            //Outline
            GUILayout.BeginArea(new Rect(2, 2, WindowRect.width-4, WindowRect.height-4), Style.WindowInside);

            //Title
            GUILayout.BeginArea(new Rect(3, 3, WindowRect.width - 10, 20), Style.WindowContents);
            GUILayout.Label("BreastPhysicsController", Style.LabelWindowTitle);
            GUILayout.EndArea();

            //Contents
            GUILayout.BeginArea(new Rect(3, 26, WindowRect.width-10, WindowRect.height-33),Style.WindowContents);

            //ListBox for CharacterSelect
            if(charaSelect._show)
            {

                GUILayout.Label("Select one character to control.",Style.LabelLarge);
                charaSelect.Draw();
            }
            else
            {

                //Chara Select
                bool charaChanged=charaSelect.Draw();
                GUILayout.Space(Style.defaultSpace);

                if (charaChanged || _controller == null)
                {
                    _controller = charaSelect.GetSelectedController();
                }

                if (_controller!=null)
                {
                    GUILayout.BeginHorizontal();
                    //Enable
                    controllEnable.Draw(_controller,Style.ToggleLarge);
                    GUILayout.EndHorizontal();

                    GUILayout.Space(Style.defaultSpace);

                    //Coordinate Select
                    GUILayout.Label("Select a coordinate and state.", Style.LabedMiddleSubject);
                    GUILayout.BeginHorizontal();
                    coordinateSelect.Draw();
                    if (GUILayout.Button("Match state", styleMatchState))
                    {
                        MatchWindowState(_controller);
                    }
                    GUILayout.EndHorizontal();
                    GUILayout.Space(Style.defaultSpace);

                    //State Select
                    kindSelect.Draw();
                    GUILayout.Space(Style.defaultSpace);

                    //Parameter Editor
                    switch (kindSelect.GetSelected())
                    {
                        case ParamCharaController.ParamsKind.Naked:
                            changedParam = changedParam | editorBust.Draw(_controller,coordinateSelect.GetSelectedCoordinate(),ParamCharaController.ParamsKind.Naked);
                            break;
                        case ParamCharaController.ParamsKind.Bra:
                            changedParam = changedParam | editorBust.Draw(_controller, coordinateSelect.GetSelectedCoordinate(), ParamCharaController.ParamsKind.Bra);
                            break;
                        case ParamCharaController.ParamsKind.Tops:
                            changedParam = changedParam | editorBust.Draw(_controller, coordinateSelect.GetSelectedCoordinate(), ParamCharaController.ParamsKind.Tops);
                            break;
                        case ParamCharaController.ParamsKind.Hip:
                            changedParam = changedParam | editorHip.Draw(_controller);
                            break;
                        default:
                            break;
                    }

                    //Button Load from chara
                    if (GUILayout.Button("Load parameters from chara"))
                    {
                        _controller.LoadParamFromChara(coordinateSelect.GetSelectedCoordinate(),kindSelect.GetSelected());
                    }

                    //Tools
                    if (GUILayout.Button("Tools"))
                    {
                        if (tools._show) tools.Hide();
                        else tools.Show(new Vector2(WindowRect.position.x+300,WindowRect.position.y+700));
                    }
                }

            }
            GUILayout.EndArea();

            GUILayout.EndArea();

            if (WindowRect.Contains(new Vector2(Input.mousePosition.x, Screen.height - Input.mousePosition.y)) &&
                Event.current.type != EventType.KeyDown && Event.current.type != EventType.KeyUp)
            {
                Input.ResetInputAxes();
            }
            GUI.DragWindow();

        }

        public ChaFileDefine.CoordinateType GetSelectedCoordinate()
        {
            return coordinateSelect.GetSelectedCoordinate();
        }

        public ParamCharaController.ParamsKind GetSelectedParamsKind()
        {
            return kindSelect.GetSelected();
        }

        public ParamCharaController GetSelectedController()
        {
            return _controller;
        }

        public void ReloadConfig()
        {
            editorBust.ReloadSliderLimits();
            editorHip.ReloadSliderLimits();
        }

        public void MatchWindowState(ParamCharaController controller)
        {
            ChaFileDefine.CoordinateType coordinate = controller.GetNowCoordinate();
            ParamCharaController.ParamsKind kind = controller.GetNowBustWear();
            coordinateSelect.Select(coordinate);
            kindSelect.Select(kind);
        }
    }
}
