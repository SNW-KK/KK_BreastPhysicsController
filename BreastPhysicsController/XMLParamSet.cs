using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace BreastPhysicsController
{
    [Serializable]
    public class XMLParamSet
    {
        [XmlAttribute("PartName")]
        public string PartName;
        [XmlElement("IsRotationCalc")]
        public bool IsRotationCalc;
        [XmlElement("Damping")]
        public float Damping;
        [XmlElement("Elasticity")]
        public float Elasticity;
        [XmlElement("Stiffness")]
        public float Stiffness;
        [XmlElement("Inert")]
        public float Inert;

        public XMLParamSet() { }

        public bool CopyParameterFrom(ParamBone paramBone)
        {
            try
            {
                IsRotationCalc = paramBone.IsRotationCalc;
                Damping = paramBone.Damping;
                Elasticity = paramBone.Elasticity;
                Stiffness = paramBone.Stiffness;
                Inert = paramBone.Inert;
            }
            catch (Exception e)
            {
                BreastPhysicsController.Logger.Log(BepInEx.Logging.LogLevel.Warning, "Failed copy DynamicBoneParam to XMLParamSet.");
                return false;
            }
            return true;
        }

        public bool CopyParameterFrom(DynamicBone_Ver02.BoneParameter boneParameter)
        {
            try
            {
                IsRotationCalc = boneParameter.IsRotationCalc;
                Damping = boneParameter.Damping;
                Elasticity = boneParameter.Elasticity;
                Stiffness = boneParameter.Stiffness;
                Inert = boneParameter.Inert;
            }
            catch (Exception e)
            {
                BreastPhysicsController.Logger.Log(BepInEx.Logging.LogLevel.Warning, "Failed copy DynamicBoneParam.");
                return false;
            }
            return true;
        }

        public void CopyParameterTo(DynamicBone_Ver02.BoneParameter parameter)
        {
            parameter.IsRotationCalc = IsRotationCalc;
            parameter.Damping = Damping;
            parameter.Elasticity = Elasticity;
            parameter.Stiffness = Stiffness;
            parameter.Inert = Inert;
        }

        public void CopyParameterTo(ParamBone target)
        {
            target.IsRotationCalc = IsRotationCalc;
            target.Damping = Damping;
            target.Elasticity = Elasticity;
            target.Stiffness = Stiffness;
            target.Inert = Inert;
        }
    }
}
