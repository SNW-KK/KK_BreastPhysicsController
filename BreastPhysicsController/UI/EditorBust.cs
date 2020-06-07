using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using BreastPhysicsController.UI.Parts;
using BepInEx.Configuration;
using BreastPhysicsController.UI.Util;

namespace BreastPhysicsController.UI
{
    public class EditorBust
    {

        //GUI Component
        public ToggleRef enabled, irc01, irc02, irc03;
        public SliderTextRef gravity,damping01, damping02, damping03, elasticity01, elasticity02, elasticity03,
            stiffness01, stiffness02, stiffness03, inert01, inert02, inert03;

        public EditorBust()
        {
            InitGUI();
        }

        private void InitGUI()
        {
            enabled = new ToggleRef("Enabled");

            gravity= new SliderTextRef("Gravity", ConfigGlobal.minGravity.Value, ConfigGlobal.maxGravity.Value, 200, 75);

            irc01 = new ToggleRef("isRotationCalc");
            damping01 = new SliderTextRef("Damping", ConfigGlobal.minDamping.Value, ConfigGlobal.maxDamping.Value, 200, 75);
            elasticity01 = new SliderTextRef("Elasticity", ConfigGlobal.minElasticity.Value, ConfigGlobal.maxElasticity.Value, 200, 75);
            stiffness01 = new SliderTextRef("Stiffness", ConfigGlobal.minStiffness.Value, ConfigGlobal.maxStiffness.Value, 200, 75);
            inert01 = new SliderTextRef("Inert", ConfigGlobal.minInert.Value, ConfigGlobal.maxInert.Value, 200, 75);

            irc02 = new ToggleRef("isRotationCalc");
            damping02 = new SliderTextRef("Damping", ConfigGlobal.minDamping.Value, ConfigGlobal.maxDamping.Value, 200, 75);
            elasticity02 = new SliderTextRef("Elasticity", ConfigGlobal.minElasticity.Value, ConfigGlobal.maxElasticity.Value, 200, 75);
            stiffness02 = new SliderTextRef("Stiffness", ConfigGlobal.minStiffness.Value, ConfigGlobal.maxStiffness.Value, 200, 75);
            inert02 = new SliderTextRef("Inert", ConfigGlobal.minInert.Value, ConfigGlobal.maxInert.Value, 200, 75);

            irc03 = new ToggleRef("isRotationCalc");
            damping03 = new SliderTextRef("Damping", ConfigGlobal.minDamping.Value, ConfigGlobal.maxDamping.Value, 200, 75);
            elasticity03 = new SliderTextRef("Elasticity", ConfigGlobal.minElasticity.Value, ConfigGlobal.maxElasticity.Value, 200, 75);
            stiffness03 = new SliderTextRef("Stiffness", ConfigGlobal.minStiffness.Value, ConfigGlobal.maxStiffness.Value, 200, 75);
            inert03 = new SliderTextRef("Inert", ConfigGlobal.minInert.Value, ConfigGlobal.maxInert.Value, 200, 75);


        }

        public bool Draw(ParamCharaController controller,ChaFileDefine.CoordinateType coordinate,ParamCharaController.ParamsKind kind)
        {

            if (controller.paramCustom == null) return false;

            //ParamBustCustom param
            ParamBustCustom param = null;
            if (kind == ParamCharaController.ParamsKind.Naked)
            {
                param = controller.paramCustom.paramBustNaked;
            }
            else if (kind == ParamCharaController.ParamsKind.Bra || kind == ParamCharaController.ParamsKind.Tops)
            {
                param = controller.paramCustom.paramBust[coordinate][kind];
            }
            else return false;

            bool changedEnabled = false;
            bool changedParam = false;

            //Enabled
            changedEnabled = enabled.Draw(ref param.enabled,Style.ToggleMiddle);

            //GUILayout.Label("Parameters", Style.LabedMiddleSubject);

            GUILayout.Space(Style.defaultSpace);

            changedParam = changedParam | gravity.Draw(ref param.gravity);

            /*Bust01
            GUILayout.Label(ParamBustCustom.Bones[0],Style.LabedMiddle);
            irc01.Draw(); //If IsRotationCalc of Bust01 changed, bust animation is broken.
            changedParam = changedParam | damping01.Draw(ref param.paramBones[ParamBustCustom.Bones[0]].Damping);
            changedParam = changedParam | elasticity01.Draw(ref param.paramBones[ParamBustCustom.Bones[0]].Elasticity);
            changedParam = changedParam | stiffness01.Draw(ref param.paramBones[ParamBustCustom.Bones[0]].Stiffness);
            changedParam = changedParam | inert01.Draw(ref param.paramBones[ParamBustCustom.Bones[0]].Inert);
            GUILayout.Space(Style.defaultSpace);
            */

            //Bust02
            GUILayout.Label(ParamBustCustom.Bones[1],Style.LabedMiddleSubject);
            GUILayout.Space(Style.defaultSpace);
            GUI.skin.label.fontSize = 12;
            changedParam = changedParam | irc02.Draw(ref param.paramBones[ParamBustCustom.Bones[1]].IsRotationCalc);
            changedParam = changedParam | damping02.Draw(ref param.paramBones[ParamBustCustom.Bones[1]].Damping);
            changedParam = changedParam | elasticity02.Draw(ref param.paramBones[ParamBustCustom.Bones[1]].Elasticity);
            changedParam = changedParam | stiffness02.Draw(ref param.paramBones[ParamBustCustom.Bones[1]].Stiffness);
            changedParam = changedParam | inert02.Draw(ref param.paramBones[ParamBustCustom.Bones[1]].Inert);
            GUILayout.Space(Style.defaultSpace);

            //Bust03
            GUILayout.Label(ParamBustCustom.Bones[2], Style.LabedMiddleSubject);
            GUILayout.Space(Style.defaultSpace);
            GUI.skin.label.fontSize = 12;
            changedParam = changedParam | irc03.Draw(ref param.paramBones[ParamBustCustom.Bones[2]].IsRotationCalc);
            changedParam = changedParam | damping03.Draw(ref param.paramBones[ParamBustCustom.Bones[2]].Damping);
            changedParam = changedParam | elasticity03.Draw(ref param.paramBones[ParamBustCustom.Bones[2]].Elasticity);
            changedParam = changedParam | stiffness03.Draw(ref param.paramBones[ParamBustCustom.Bones[2]].Stiffness);
            changedParam = changedParam | inert03.Draw(ref param.paramBones[ParamBustCustom.Bones[2]].Inert);
            GUILayout.Space(Style.defaultSpace);

            if (changedEnabled)
            {
                controller.changedInfo.SetInfo(coordinate, kind, true, false);
            }
            else if(changedParam)
            {
                controller.changedInfo.SetInfo(coordinate, kind, false, true);
            }
            return changedParam|changedEnabled;
        }

        public void ReloadSliderLimits()
        {
            gravity.SetLimit(ConfigGlobal.minGravity.Value, ConfigGlobal.maxGravity.Value);

            damping01.SetLimit(ConfigGlobal.minDamping.Value, ConfigGlobal.maxDamping.Value);
            damping02.SetLimit(ConfigGlobal.minDamping.Value, ConfigGlobal.maxDamping.Value);
            damping03.SetLimit(ConfigGlobal.minDamping.Value, ConfigGlobal.maxDamping.Value);

            elasticity01.SetLimit(ConfigGlobal.minElasticity.Value, ConfigGlobal.maxElasticity.Value);
            elasticity02.SetLimit(ConfigGlobal.minElasticity.Value, ConfigGlobal.maxElasticity.Value);
            elasticity03.SetLimit(ConfigGlobal.minElasticity.Value, ConfigGlobal.maxElasticity.Value);

            stiffness01.SetLimit(ConfigGlobal.minStiffness.Value, ConfigGlobal.maxStiffness.Value);
            stiffness02.SetLimit(ConfigGlobal.minStiffness.Value, ConfigGlobal.maxStiffness.Value);
            stiffness03.SetLimit(ConfigGlobal.minStiffness.Value, ConfigGlobal.maxStiffness.Value);

            inert01.SetLimit(ConfigGlobal.minInert.Value, ConfigGlobal.maxInert.Value);
            inert02.SetLimit(ConfigGlobal.minInert.Value, ConfigGlobal.maxInert.Value);
            inert03.SetLimit(ConfigGlobal.minInert.Value, ConfigGlobal.maxInert.Value);
        }

    }
}
