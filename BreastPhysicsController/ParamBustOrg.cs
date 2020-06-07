using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.IO;
using MessagePack;
using UnityEngine;

namespace BreastPhysicsController
{

    public class ParamBustOrg
    { 

        //parameters
        public  Vector3 gravity;
        public ParamBone[] parameters { get; private set; }
        public ParamBone[] particles { get; private set; }


        public ParamBustOrg()
        {
            gravity = new Vector3();
        }

        public override string ToString()
        {
            string value = "";
            value += "ParamBustOrg.ToString()\r\n";
            value += "Parameters:\r\n";
            value += $"gravity:{gravity.ToString()}\r\n";

            for (int i=0; i<parameters.Count();i++)
            {
                value+=parameters[i].ToString();
            }
            value += "Particles:\r\n";
            for (int i = 0; i < particles.Count(); i++)
            {
                value += particles[i].ToString();
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
                ParamBone param = new ParamBone("");
                param.IsRotationCalc = target.Patterns[0].Params[i].IsRotationCalc;
                param.Damping = target.Patterns[0].Params[i].Damping;
                param.Elasticity = target.Patterns[0].Params[i].Elasticity;
                param.Stiffness = target.Patterns[0].Params[i].Stiffness;
                param.Inert = target.Patterns[0].Params[i].Inert;
                parameters[i] = param;
            }
            

            particles = new ParamBone[target.Patterns[0].ParticlePtns.Count()];
            for (int i = 0; i < particles.Count(); i++)
            {
                particles[i] = new ParamBone("");
                particles[i].IsRotationCalc = target.Patterns[0].ParticlePtns[i].IsRotationCalc;
                particles[i].Damping = target.Patterns[0].ParticlePtns[i].Damping;
                particles[i].Elasticity = target.Patterns[0].ParticlePtns[i].Elasticity;
                particles[i].Stiffness = target.Patterns[0].ParticlePtns[i].Stiffness;
                particles[i].Inert = target.Patterns[0].ParticlePtns[i].Inert;
            }
        }

        public bool CopyParamTo(DynamicBone_Ver02 target)
        {
            if (parameters == null || particles == null) return false;
            if (parameters.Count() != 3 || particles.Count() != 4) return false;

            target.Gravity.Set(gravity.x, gravity.y, gravity.z);

            for (int i = 0; i < parameters.Count(); i++)
            {
                target.Patterns[0].Params[i].IsRotationCalc = parameters[i].IsRotationCalc;
                target.Patterns[0].Params[i].Damping = parameters[i].Damping;
                target.Patterns[0].Params[i].Elasticity = parameters[i].Elasticity;
                target.Patterns[0].Params[i].Stiffness = parameters[i].Stiffness;
                target.Patterns[0].Params[i].Inert = parameters[i].Inert;
            }

            for (int i = 0; i < particles.Count(); i++)
            {
                target.Patterns[0].ParticlePtns[i].IsRotationCalc = particles[i].IsRotationCalc;
                target.Patterns[0].ParticlePtns[i].Damping = particles[i].Damping;
                target.Patterns[0].ParticlePtns[i].Elasticity = particles[i].Elasticity;
                target.Patterns[0].ParticlePtns[i].Stiffness = particles[i].Stiffness;
                target.Patterns[0].ParticlePtns[i].Inert = particles[i].Inert;
            }
            return true;
        }
    }
}



