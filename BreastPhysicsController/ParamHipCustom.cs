using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MessagePack;
using System.Text.RegularExpressions;

namespace BreastPhysicsController
{
    [MessagePackObject(keyAsPropertyName:true)]
    public class ParamHipCustom
    {
        //Bones is RefTransform.name
        [IgnoreMember]
        public static readonly string[] Bones = { "cf_d_siri01_L", "cf_j_siri_L_01" };

        public bool enabled;
        public float gravity;
        public Dictionary<string, ParamBone> paramBones;

        public ParamHipCustom()
        {
            enabled = false;
            gravity = 0;
            paramBones = new Dictionary<string, ParamBone>();
            for (int i = 0; i < Bones.Count(); i++)
            {
                paramBones.Add(Bones[i], new ParamBone(Bones[i]));
            }
        }

        public ParamBone GetParameterBone(string boneName)
        {
            if (paramBones.ContainsKey(boneName)) return paramBones[boneName];
            else return null;
        }

        public void SetParameterBone(string targetBone, bool isRotationCalc, float damping, float elasticity, float stiffness, float inert)
        {
            paramBones[targetBone].SetParameter(isRotationCalc, damping, elasticity, stiffness, inert);
        }

        public void CopyTo(List<DynamicBone_Ver02.Particle> particles)
        {
            Regex regex = new Regex("_R");

            for (int i=0;i<particles.Count();i++)
            {
                if(particles[i].refTrans!=null)
                {
                    string refTransName = regex.Replace(particles[i].refTrans.name, "_L");
                    if (paramBones.ContainsKey(refTransName))
                    {
                        particles[i].IsRotationCalc = paramBones[refTransName].IsRotationCalc;
                        particles[i].Damping = paramBones[refTransName].Damping;
                        particles[i].Elasticity = paramBones[refTransName].Elasticity;
                        particles[i].Stiffness = paramBones[refTransName].Stiffness;
                        particles[i].Inert = paramBones[refTransName].Inert;
                    }
                }
            }
        }

        public ParamHipCustom Clone()
        {
            ParamHipCustom result = new ParamHipCustom();
            result.enabled = enabled;
            result.gravity = gravity;
            foreach(string bone in paramBones.Keys)
            {
                result.paramBones[bone] = paramBones[bone].Clone();
            }
            return result;
        }

        public void CopyParamsFrom(ParamHipCustom src)
        {
            gravity = src.gravity;
            foreach(string bone in Bones)
            {
                paramBones[bone].CopyParameterFrom(src.paramBones[bone]);
            }
        }

        public bool LoadParamsFromCharacter(ParamCharaController controller)
        {
            if (controller.ChaControl.dictDynamicBoneBust != null)
            {
                DynamicBone_Ver02 dynamicBoneChara;

                dynamicBoneChara = controller.ChaControl.getDynamicBoneBust(ChaInfo.DynamicBoneKind.HipL);

                gravity = dynamicBoneChara.Gravity.y;

                foreach (DynamicBone_Ver02.Particle particle in CharaParamControl.GetCharacterParticles(dynamicBoneChara))
                {
                    if(particle.refTrans!=null)
                    {
                        if (paramBones.ContainsKey(particle.refTrans.name))
                        {
                            paramBones[particle.refTrans.name].boneName = particle.refTrans.name;
                            paramBones[particle.refTrans.name].CopyParameterFrom(particle);
                        }
                    }
                }

                return true;
            }

            return false;
        }

        public override string ToString()
        {
            string value = "";
            value += "ParamHipCustom.ToString()\r\n";
            value += $"enabled:{enabled.ToString()}\r\n";
            value += $"gravity:{gravity.ToString()}\r\n";
            foreach (string bone in Bones)
            {
                paramBones[bone].ToString();
            }
            return value;
        }

        public void ResetAllParams()
        {
            foreach(ParamBone paramBone in paramBones.Values)
            {
                paramBone.ResetAllParams();
            }
        }
    }
}
