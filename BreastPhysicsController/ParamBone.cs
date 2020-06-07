using System;
using MessagePack;

namespace BreastPhysicsController
{
    [MessagePackObject(keyAsPropertyName: true)]
    public class ParamBone
    {
        public string boneName;
        public bool IsRotationCalc;
        public float Damping;
        public float Elasticity;
        public float Stiffness;
        public float Inert;

        public ParamBone(string boneName,bool isRotationCalc=false,float damping=0,float elasticity=0,float stiffness=0,float inert=0)
        {
            this.boneName = boneName;
            IsRotationCalc = isRotationCalc;
            Damping = damping;
            Elasticity = elasticity;
            Stiffness = stiffness;
            Inert = inert;
        }

        public void SetParameter(bool isRotationCalc, float damping, float elasticity, float stiffness, float inert)
        {
            IsRotationCalc = isRotationCalc;
            Damping = damping;
            Elasticity = elasticity;
            Stiffness = stiffness;
            Inert = inert;
        }

        public bool CopyParameterFrom(ParamBone paramBone)
        {
            try
            {
                IsRotationCalc = paramBone.IsRotationCalc;
                Damping = paramBone.Damping;
                Elasticity = paramBone.Elasticity;
                Stiffness = paramBone.Stiffness;
                Inert = paramBone.Inert;
                return true;
            }
            catch (Exception e)
            {
                BreastPhysicsController.Logger.LogError("Failed copy DynamicBoneParam.");
                return false;
            }
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
                BreastPhysicsController.Logger.LogError("Failed copy DynamicBoneParam.");
                return false;
            }
            return true;
        }

        public bool CopyParameterFrom(DynamicBone_Ver02.Particle particle)
        {
            try
            {
                IsRotationCalc = particle.IsRotationCalc;
                Damping = particle.Damping;
                Elasticity = particle.Elasticity;
                Stiffness = particle.Stiffness;
                Inert = particle.Inert;
            }
            catch (Exception e)
            {
                BreastPhysicsController.Logger.LogError("Failed copy DynamicBoneParam.");
                return false;
            }
            return true;
        }

        public ParamBone Clone()
        {
            return new ParamBone(boneName, IsRotationCalc, Damping, Elasticity, Stiffness, Inert);
        }


        public void ResetAllParams()
        {
            IsRotationCalc = false;
            Damping = 0;
            Elasticity = 0;
            Stiffness = 0;
            Inert = 0;
        }

        public void CopyParameterTo(DynamicBone_Ver02.BoneParameter parameter)
        {
            parameter.IsRotationCalc = IsRotationCalc;
            parameter.Damping = Damping;
            parameter.Elasticity = Elasticity;
            parameter.Stiffness = Stiffness;
            parameter.Inert = Inert;
        }

        public override string ToString()
        {
            string value = "";
            value += $"Parameters of ParameterBone Class\r\n";
            value += $"BoneName:{boneName}\r\n";
            value += $"IsRotationCalc:{IsRotationCalc}\r\n";
            value += $"Damping:{Damping}\r\n";
            value += $"Elasiticity:{Elasticity}\r\n";
            value += $"Stiffness:{Stiffness}\r\n";
            value += $"Inert:{Inert}\r\n";
            return value;
        }
    }
}
