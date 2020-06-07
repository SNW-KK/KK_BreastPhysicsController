using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Text.RegularExpressions;

namespace BreastPhysicsController
{
    public static class CharaParamControl
    {
        public static void CopyParamsToParticlePtn(DynamicBone_Ver02 target)
        {
            for (int i = 0; i < target.Patterns[0].Params.Count; i++)
            {
                if (i < target.Patterns[0].ParticlePtns.Count)
                {
                    target.Patterns[0].ParticlePtns[i].IsRotationCalc = target.Patterns[0].Params[i].IsRotationCalc;
                    target.Patterns[0].ParticlePtns[i].Damping = target.Patterns[0].Params[i].Damping;
                    target.Patterns[0].ParticlePtns[i].Elasticity = target.Patterns[0].Params[i].Elasticity;
                    target.Patterns[0].ParticlePtns[i].Stiffness = target.Patterns[0].Params[i].Stiffness;
                    target.Patterns[0].ParticlePtns[i].Inert = target.Patterns[0].Params[i].Inert;
                }
            }
        }

        public static bool ApplyParamBust(ParamBustCustom source,DynamicBone_Ver02 target)
        {
            Regex regex = new Regex("_R$");
            //For Params
            //Parameter Patterns[0]="通常"
            target.setGravity(0, new UnityEngine.Vector3(0, source.gravity, 0));

            for (int i = 0; i < target.Patterns[0].Params.Count; i++)
            {
                string boneName = regex.Replace(target.Patterns[0].Params[i].Name, "_L");
                if (source.paramBones.ContainsKey(boneName))
                {
                    ParamBone parameterSet = source.paramBones[boneName];
                    if (parameterSet != null)
                    {
                        parameterSet.CopyParameterTo(target.Patterns[0].Params[i]);
                    }
                    else return false;
                }
            }

            //For ParticlePtn
            //ParticlePtn Patterns[0]="通常"
            CopyParamsToParticlePtn(target);

            //For Particle. Must set at last.
            target.setPtn(0, true);

            return true;
        }

        public static bool ApplyParamBust(ParamBustOrg source, DynamicBone_Ver02 target)
        {
            //For Params
            //Parameter Patterns[0]="通常"
            //target.Gravity.Set(source.gravity.x,source.gravity.y,source.gravity.z);
            target.setGravity(0, source.gravity);
            for (int i = 0; i < target.Patterns[0].Params.Count; i++)
            {
                source.parameters[i].CopyParameterTo(target.Patterns[0].Params[i]);
            }

            //For ParticlePtn
            //ParticlePtn Patterns[0]="通常"
            CopyParamsToParticlePtn(target);

            //For Particle. Must set at last.
            target.setPtn(0, true);

            return true;
        }

        public static bool ApplyParamHip(ParamHipCustom source,DynamicBone_Ver02 target)
        {

            List<DynamicBone_Ver02.Particle> particles = GetCharacterParticles(target);
            if (particles != null)
            {
                target.setGravity(0, new UnityEngine.Vector3(0, source.gravity, 0));
                source.CopyTo(particles);
                return true;
            }
            else return false;
        }

        public static bool ApplyParamHip(ParamHipOrg source, DynamicBone_Ver02 target)
        {
            return source.CopyTo(target);
        }

        public static List<DynamicBone_Ver02.Particle> GetCharacterParticles(DynamicBone_Ver02 target)
        {
            FieldInfo particlesInfo = target.GetType().GetField("Particles", BindingFlags.Instance | BindingFlags.NonPublic);
            return particlesInfo.GetValue(target) as List<DynamicBone_Ver02.Particle>;
        }
    }
}
