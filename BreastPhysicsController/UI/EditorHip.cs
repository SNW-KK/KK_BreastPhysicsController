using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using BreastPhysicsController.UI.Parts;
using BreastPhysicsController.UI.Util;

namespace BreastPhysicsController.UI
{
    public class EditorHip
    {

        //GUI Component
        public ToggleRef enabled, irc01, irc02;
        public SliderTextRef gravity, damping01, elasticity01, stiffness01, inert01,
            damping02, elasticity02, stiffness02, inert02;

        public EditorHip()
        {
            InitGUI();
        }

        private void InitGUI()
        {
            enabled = new ToggleRef("Enabled");

            gravity = new SliderTextRef("Gravity", ConfigGlobal.minGravity.Value, ConfigGlobal.maxGravity.Value, 200, 75);

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

        }

        public bool Draw(ParamCharaController controller)
        {

            bool changedEnabled = false;
            bool changedParams = false;
            ParamHipCustom param = controller.paramCustom.paramHip;

            //Enabled
            GUI.skin.label.fontSize = 12;
            changedEnabled = enabled.Draw(ref param.enabled,Style.ToggleMiddle);
            GUILayout.Space(Style.defaultSpace);

            //Gravity
            changedParams = changedParams | gravity.Draw(ref param.gravity);

            /*Hip1
            GUILayout.Label(ParamHipCustom.Bones[0], Style.LabedMiddle);
            changedParams = changedParams | irc01.Draw(ref param.paramBones[ParamHipCustom.Bones[0]].IsRotationCalc);
            changedParams = changedParams | damping01.Draw(ref param.paramBones[ParamHipCustom.Bones[0]].Damping);
            changedParams = changedParams | elasticity01.Draw(ref param.paramBones[ParamHipCustom.Bones[0]].Elasticity);
            changedParams = changedParams | stiffness01.Draw(ref param.paramBones[ParamHipCustom.Bones[0]].Stiffness);
            changedParams = changedParams | inert01.Draw(ref param.paramBones[ParamHipCustom.Bones[0]].Inert);
            */

            //Hip2
            GUILayout.Label(ParamHipCustom.Bones[1], Style.LabedMiddleSubject);
            GUILayout.Space(Style.defaultSpace);
            GUI.skin.label.fontSize = 12;

            changedParams = changedParams | irc02.Draw(ref param.paramBones[ParamHipCustom.Bones[1]].IsRotationCalc);
            changedParams = changedParams | damping02.Draw(ref param.paramBones[ParamHipCustom.Bones[1]].Damping);
            changedParams = changedParams | elasticity02.Draw(ref param.paramBones[ParamHipCustom.Bones[1]].Elasticity);
            changedParams = changedParams | stiffness02.Draw(ref param.paramBones[ParamHipCustom.Bones[1]].Stiffness);
            changedParams = changedParams | inert02.Draw(ref param.paramBones[ParamHipCustom.Bones[1]].Inert);
            GUILayout.Space(Style.defaultSpace);

            if(changedEnabled)
            {
                controller.changedInfo.SetInfo(ChaFileDefine.CoordinateType.School01, ParamCharaController.ParamsKind.Hip, true, false);
            }
            else if(changedParams)
            {
                controller.changedInfo.SetInfo(ChaFileDefine.CoordinateType.School01, ParamCharaController.ParamsKind.Hip, false, true);
            }
            return changedParams | changedEnabled;
        }

        public void ReloadSliderLimits()
        {
            gravity.SetLimit(ConfigGlobal.minGravity.Value, ConfigGlobal.maxGravity.Value);

            damping01.SetLimit(ConfigGlobal.minDamping.Value, ConfigGlobal.maxDamping.Value);
            elasticity01.SetLimit(ConfigGlobal.minElasticity.Value, ConfigGlobal.maxElasticity.Value);
            stiffness01.SetLimit(ConfigGlobal.minStiffness.Value, ConfigGlobal.maxStiffness.Value);
            inert01.SetLimit(ConfigGlobal.minInert.Value, ConfigGlobal.maxInert.Value);

            damping02.SetLimit(ConfigGlobal.minDamping.Value, ConfigGlobal.maxDamping.Value);
            elasticity02.SetLimit(ConfigGlobal.minElasticity.Value, ConfigGlobal.maxElasticity.Value);
            stiffness02.SetLimit(ConfigGlobal.minStiffness.Value, ConfigGlobal.maxStiffness.Value);
            inert02.SetLimit(ConfigGlobal.minInert.Value, ConfigGlobal.maxInert.Value);
        }

    }
}
