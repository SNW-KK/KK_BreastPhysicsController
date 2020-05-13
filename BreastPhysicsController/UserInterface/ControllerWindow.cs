using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace BreastPhysicsController
{
    public class ControllerWindow
    {

        public readonly string _windowTitle = "BreastPhysicsController";
        public int _windowID;

        //Flags
        public bool _onDisable = false;
        public bool _parameterChanged = false;
        public bool _showWindow = false;

        //GUI Component(MainWindow)
        public Rect WindowRect = new Rect(10, 10, 300, 920);
        public Toggle controllEnable,irc01, irc02, irc03;
        public SliderAndTextBox damping01, damping02, damping03, elasticity01, elasticity02, elasticity03,
            stiffness01, stiffness02, stiffness03, inert01, inert02, inert03;
        public CharaSelect charaSelect;
        public PresetSelect presetSelect;

        //SaveDialog
        public int _s_dialogID;
        public SaveDialog s_dialog;
        public Rect SDRect = new Rect(Screen.width / 2 - 150, Screen.height / 2 - 50, 300, 100);

        public ControllerWindow(int windowID,int s_dialogID)
        {
            _s_dialogID = s_dialogID;
            _windowID = windowID;
            InitGUI();
        }

        private void InitGUI()
        {

            charaSelect = new CharaSelect(ControllerManager.GetAllController().Select(x => x.ChaFileControl.parameter.fullname).ToArray(),
                ControllerManager.GetAllController().Select(x => x.controllerID).ToArray(), "No character loaded", WindowRect.width-20, 25, WindowRect.width - 35, WindowRect.height-50);

            controllEnable = new Toggle("Enable controller", false);

            irc01 = new Toggle("isRotationCalc", false);
            damping01 = new SliderAndTextBox("Damping",0,1,200,75);
            elasticity01 = new SliderAndTextBox("Elasticity", 0, 1, 200, 75);
            stiffness01 = new SliderAndTextBox("Stiffness", 0, 1, 200, 75);
            inert01 = new SliderAndTextBox("Inert", 0, 1, 200, 75);

            irc02 = new Toggle("isRotationCalc", false);
            damping02 = new SliderAndTextBox("Damping", 0, 1, 200, 75);
            elasticity02 = new SliderAndTextBox("Elasticity", 0, 1, 200, 75);
            stiffness02 = new SliderAndTextBox("Stiffness", 0, 1, 200, 75);
            inert02 = new SliderAndTextBox("Inert", 0, 1, 200, 75);

            irc03 = new Toggle("isRotationCalc", false);
            damping03 = new SliderAndTextBox("Damping", 0, 1, 200, 75);
            elasticity03 = new SliderAndTextBox("Elasticity", 0, 1, 200, 75);
            stiffness03 = new SliderAndTextBox("Stiffness", 0, 1, 200, 75);
            inert03 = new SliderAndTextBox("Inert", 0, 1, 200, 75);

            s_dialog = new SaveDialog(this,_s_dialogID, "Save Preset", SDRect);

            IEnumerable<string> xmls = System.IO.Directory.GetFiles(BreastPhysicsController.PresetDir, "*.xml").ToList();
            xmls = xmls.Select(x => System.IO.Path.GetFileNameWithoutExtension(x));
            
            presetSelect=new PresetSelect(BreastPhysicsController.PresetDir, "Load preset",137, 20, WindowRect.width - 45,WindowRect.height - 50);
        }

        public  void Draw(int windowID)
        {

            GUILayout.BeginArea(new Rect(10, 30, WindowRect.width-20, WindowRect.height-60));

            //DropDownBox for CharacterSelect
            if(charaSelect._show)
            {
                GUI.skin.label.fontSize = 16;
                GUILayout.Label("Select one character to control.");
                charaSelect.Draw();
            }
            else if(presetSelect._show)
            {
                GUI.skin.label.fontSize = 16;
                GUILayout.Label("Select one preset to load.");
                presetSelect.Draw();
            }
            else
            {
                //Chara Select
                charaSelect.Draw();
                GUILayout.Space(10);

                //Enable
                controllEnable.Draw();
                GUILayout.Space(10);

                //Bust01
                GUI.skin.label.fontSize = 16;
                GUILayout.Label("*bust01");
                GUI.skin.label.fontSize = 12;

                //irc01.Draw(); //If IsRotationCalc of Bust01 changed, bust animation is broken.
                damping01.Draw();
                elasticity01.Draw();
                stiffness01.Draw();
                inert01.Draw();
                GUILayout.Space(10);

                //Bust02
                GUI.skin.label.fontSize = 16;
                GUILayout.Label("*bust02");
                GUI.skin.label.fontSize = 12;

                irc02.Draw();
                damping02.Draw();
                elasticity02.Draw();
                stiffness02.Draw();
                inert02.Draw();
                GUILayout.Space(10);

                //Bust03
                GUI.skin.label.fontSize = 16;
                GUILayout.Label("*bust03");
                GUI.skin.label.fontSize = 12;

                irc03.Draw();
                damping03.Draw();
                elasticity03.Draw();
                stiffness03.Draw();
                inert03.Draw();
                GUILayout.Space(10);

                //Button Load from chara
                if(GUILayout.Button("Load parameters from chara"))
                {
                    BreastDynamicBoneController controller = ControllerManager.GetControllerByID(charaSelect.GetSelectedId());
                    if (controller != null) controller.LoadParamsFromGame();
                }

                GUILayout.BeginHorizontal();
                //Button Save
                if (GUILayout.Button("Save preset", GUILayout.Width(138)))
                {
                    s_dialog._show = true;
                    _showWindow = false;
                }

                //Button Load Preset
                presetSelect.Draw();

                GUILayout.EndHorizontal();
            }


            GUILayout.EndArea();

            if (WindowRect.Contains(new Vector2(Input.mousePosition.x, Screen.height - Input.mousePosition.y)) &&
                Event.current.type != EventType.KeyDown && Event.current.type != EventType.KeyUp)
            {
                Input.ResetInputAxes();
            }
            GUI.DragWindow();
            
        }

        public void RefreshCharaList()
        {
            List<BreastDynamicBoneController> controllers=ControllerManager.GetAllController();
            List<string> chaNameList = new List<string>();
            List<int> chaIdList = new List<int>();
            foreach (BreastDynamicBoneController controller in controllers)
            {
                if (controller.ChaControl.sex == 1)
                {
                    chaNameList.Add(controller.ChaFileControl.parameter.fullname);
                    chaIdList.Add(controller.controllerID);
                }
            }
            charaSelect.SetList(chaNameList.ToArray(),chaIdList.ToArray());

        }

        public void RefreshValue()
        {
            //if (!needRefreshValue) return;
            BreastDynamicBoneController controller=ControllerManager.GetControllerByID(charaSelect.GetSelectedId());
            if (controller == null) return;

            controllEnable.SetValue(controller.enable);

            BreastDynamicBoneParameter.ParameterSet parameterSet01 = controller.DynamicBoneParameter.GetParameterSet("cf_j_bust01_L");
            irc01.SetValue(parameterSet01.IsRotationCalc);
            damping01.SetValue(parameterSet01.Damping);
            elasticity01.SetValue(parameterSet01.Elasticity);
            stiffness01.SetValue(parameterSet01.Stiffness);
            inert01.SetValue(parameterSet01.Inert);

            BreastDynamicBoneParameter.ParameterSet parameterSet02 = controller.DynamicBoneParameter.GetParameterSet("cf_j_bust02_L");
            irc02.SetValue(parameterSet02.IsRotationCalc);
            damping02.SetValue(parameterSet02.Damping);
            elasticity02.SetValue(parameterSet02.Elasticity);
            stiffness02.SetValue(parameterSet02.Stiffness);
            inert02.SetValue(parameterSet02.Inert);

            BreastDynamicBoneParameter.ParameterSet parameterSet03 = controller.DynamicBoneParameter.GetParameterSet("cf_j_bust03_L");
            irc03.SetValue(parameterSet03.IsRotationCalc);
            damping03.SetValue(parameterSet03.Damping);
            elasticity03.SetValue(parameterSet03.Elasticity);
            stiffness03.SetValue(parameterSet03.Stiffness);
            inert03.SetValue(parameterSet03.Inert);
        }

        public void ResetWindowValue()
        {

            controllEnable.SetValue(false);

            irc01.SetValue(false);
            damping01.SetValue(0);
            elasticity01.SetValue(0);
            stiffness01.SetValue(0);
            inert01.SetValue(0);

            irc02.SetValue(false);
            damping02.SetValue(0);
            elasticity02.SetValue(0);
            stiffness02.SetValue(0);
            inert02.SetValue(0);

            irc03.SetValue(false);
            damping03.SetValue(0);
            elasticity03.SetValue(0);
            stiffness03.SetValue(0);
            inert03.SetValue(0);
        }

        /*
        public void ApplyParameterToController(BreastDynamicBoneController controller)
        {
            controller.enable = controllEnable.GetValue();

            controller.DynamicBoneParameter.dictParameterSet["cf_j_bust01_L"].IsRotationCalc = irc01.GetValue();
            controller.DynamicBoneParameter.dictParameterSet["cf_j_bust01_L"].Damping = damping01.GetValue();
            controller.DynamicBoneParameter.dictParameterSet["cf_j_bust01_L"].Elasticity = elasticity01.GetValue();
            controller.DynamicBoneParameter.dictParameterSet["cf_j_bust01_L"].Stiffness = stiffness01.GetValue();
            controller.DynamicBoneParameter.dictParameterSet["cf_j_bust01_L"].Inert = inert01.GetValue();

            controller.DynamicBoneParameter.dictParameterSet["cf_j_bust02_L"].IsRotationCalc = irc02.GetValue();
            controller.DynamicBoneParameter.dictParameterSet["cf_j_bust02_L"].Damping = damping02.GetValue();
            controller.DynamicBoneParameter.dictParameterSet["cf_j_bust02_L"].Elasticity = elasticity02.GetValue();
            controller.DynamicBoneParameter.dictParameterSet["cf_j_bust02_L"].Stiffness = stiffness02.GetValue();
            controller.DynamicBoneParameter.dictParameterSet["cf_j_bust02_L"].Inert = inert02.GetValue();

            controller.DynamicBoneParameter.dictParameterSet["cf_j_bust03_L"].IsRotationCalc = irc03.GetValue();
            controller.DynamicBoneParameter.dictParameterSet["cf_j_bust03_L"].Damping = damping03.GetValue();
            controller.DynamicBoneParameter.dictParameterSet["cf_j_bust03_L"].Elasticity = elasticity03.GetValue();
            controller.DynamicBoneParameter.dictParameterSet["cf_j_bust03_L"].Stiffness = stiffness03.GetValue();
            controller.DynamicBoneParameter.dictParameterSet["cf_j_bust03_L"].Inert = inert03.GetValue();

            controller.DynamicBoneParameter.dictParameterSet["cf_j_bust01_R"].CopyParameterFrom(controller.DynamicBoneParameter.dictParameterSet["cf_j_bust01_L"]);
            controller.DynamicBoneParameter.dictParameterSet["cf_j_bust02_R"].CopyParameterFrom(controller.DynamicBoneParameter.dictParameterSet["cf_j_bust02_L"]);
            controller.DynamicBoneParameter.dictParameterSet["cf_j_bust03_R"].CopyParameterFrom(controller.DynamicBoneParameter.dictParameterSet["cf_j_bust03_L"]);

            controller.needApplyToChara = true;
        }
        */

        public bool CheckParameterChanged()
        {
            _parameterChanged = irc01.changed | irc02.changed | irc03.changed |
                    damping01.changed | damping02.changed | damping03.changed |
                    elasticity01.changed | elasticity02.changed | elasticity03.changed |
                    stiffness01.changed | stiffness02.changed | stiffness03.changed |
                    inert01.changed | inert02.changed | inert03.changed;
            return _parameterChanged;
        }

    }
}
