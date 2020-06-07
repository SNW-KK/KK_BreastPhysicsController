using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using System.Reflection;

namespace BreastPhysicsController
{
    public class ParamHipOrg
    {
        public Vector3 gravity;
        ParamBone[] parameters;
        ParamBone[] particlesPtn;
        ParamBone[] particles;

        public ParamHipOrg()
        {
            gravity = new Vector3();
        }

        public override string ToString()
        {
            string value = "";
            value += "ParamHipOrg.ToString()\r\n";
            value += "Parameters:\r\n";
            value += $"gravity:{gravity.ToString()}\r\n";
            for (int i = 0; i < parameters.Count(); i++)
            {
                value+=parameters[i].ToString();
            }
            value += "Particles:\r\n";
            for (int i = 0; i < particles.Count(); i++)
            {
                value+=particles[i].ToString();
            }
            return value;
        }

        public void Clear()
        {
            gravity.Set(0, 0, 0);
            parameters = null;
            parameters = null;
        }

        public void CopyParamFrom(DynamicBone_Ver02 target)
        {
            gravity.Set(target.Gravity.x, target.Gravity.y, target.Gravity.z);
            parameters = new ParamBone[target.Patterns[0].Params.Count()];
            for (int i = 0; i < parameters.Count(); i++)
            {
                parameters[i] = new ParamBone("");
                parameters[i].IsRotationCalc = target.Patterns[0].Params[i].IsRotationCalc;
                parameters[i].Damping = target.Patterns[0].Params[i].Damping;
                parameters[i].Elasticity = target.Patterns[0].Params[i].Elasticity;
                parameters[i].Stiffness = target.Patterns[0].Params[i].Stiffness;
                parameters[i].Inert = target.Patterns[0].Params[i].Inert;
            }
            particlesPtn = new ParamBone[target.Patterns[0].ParticlePtns.Count()];
            for (int i = 0; i < particlesPtn.Count(); i++)
            {
                particlesPtn[i] = new ParamBone("");
                particlesPtn[i].IsRotationCalc = target.Patterns[0].ParticlePtns[i].IsRotationCalc;
                particlesPtn[i].Damping = target.Patterns[0].ParticlePtns[i].Damping;
                particlesPtn[i].Elasticity = target.Patterns[0].ParticlePtns[i].Elasticity;
                particlesPtn[i].Stiffness = target.Patterns[0].ParticlePtns[i].Stiffness;
                particlesPtn[i].Inert = target.Patterns[0].ParticlePtns[i].Inert;
            }

            FieldInfo fieldInfo = target.GetType().GetField("Particles", BindingFlags.Instance | BindingFlags.NonPublic);
            List<DynamicBone_Ver02.Particle> srcParticles = fieldInfo.GetValue(target) as List<DynamicBone_Ver02.Particle>;
            particles =new ParamBone[srcParticles.Count()];
            for (int i = 0; i < particles.Count(); i++)
            {
                particles[i] = new ParamBone("");
                particles[i].IsRotationCalc = srcParticles[i].IsRotationCalc;
                particles[i].Damping = srcParticles[i].Damping;
                particles[i].Elasticity = srcParticles[i].Elasticity;
                particles[i].Stiffness = srcParticles[i].Stiffness;
                particles[i].Inert = srcParticles[i].Inert;
            }
        }

        public bool CopyTo(DynamicBone_Ver02 target)
        {
            FieldInfo fieldInfo = target.GetType().GetField("Particles", BindingFlags.Instance | BindingFlags.NonPublic);
            List<DynamicBone_Ver02.Particle> dstParticles = fieldInfo.GetValue(target) as List<DynamicBone_Ver02.Particle>;

            if (parameters == null || particles == null) return false;

            if (parameters.Count() != target.Patterns[0].Params.Count() ||
                 particlesPtn.Count()!=target.Patterns[0].ParticlePtns.Count() ||
                particles.Count() != dstParticles.Count()) return false;

            target.setGravity(0, gravity);

            for (int i = 0; i < parameters.Count(); i++)
            {
                target.Patterns[0].Params[i].IsRotationCalc = parameters[i].IsRotationCalc;
                target.Patterns[0].Params[i].Damping = parameters[i].Damping;
                target.Patterns[0].Params[i].Elasticity = parameters[i].Elasticity;
                target.Patterns[0].Params[i].Stiffness = parameters[i].Stiffness;
                target.Patterns[0].Params[i].Inert = parameters[i].Inert;
            }

            for (int i = 0; i < particlesPtn.Count(); i++)
            {
                target.Patterns[0].ParticlePtns[i].IsRotationCalc = particlesPtn[i].IsRotationCalc;
                target.Patterns[0].ParticlePtns[i].Damping = particlesPtn[i].Damping;
                target.Patterns[0].ParticlePtns[i].Elasticity = particlesPtn[i].Elasticity;
                target.Patterns[0].ParticlePtns[i].Stiffness = particlesPtn[i].Stiffness;
                target.Patterns[0].ParticlePtns[i].Inert = particlesPtn[i].Inert;
            }

            for (int i = 0; i < particles.Count(); i++)
            {
                dstParticles[i].IsRotationCalc = particles[i].IsRotationCalc;
                dstParticles[i].Damping = particles[i].Damping;
                dstParticles[i].Elasticity = particles[i].Elasticity;
                dstParticles[i].Stiffness = particles[i].Stiffness;
                dstParticles[i].Inert = particles[i].Inert;
            }

            return true;
        }

    }
}
