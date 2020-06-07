using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.IO;
using System.Text.RegularExpressions;
using MessagePack;

namespace BreastPhysicsController.Compatibility
{
    //Class For reading ExtendadDate saved by this plugin version 1.2 or earlier.
    public class BreastDynamicBoneParameter
    {

        public static readonly string[] targetBoneNames = { "cf_j_bust01_L", "cf_j_bust02_L", "cf_j_bust03_L", "cf_j_bust01_R", "cf_j_bust02_R", "cf_j_bust03_R" };

        public Dictionary<string, ParameterSet> dictParameterSet = new Dictionary<string, ParameterSet>();

        public BreastDynamicBoneParameter()
        {
            for (int i = 0; i < targetBoneNames.Count(); i++)
            {
                ParameterSet set = new ParameterSet();
                set.BoneName = targetBoneNames[i];
                dictParameterSet.Add(targetBoneNames[i], set);
            }

        }

        public ParameterSet GetParameterSet(string boneName)
        {
            if (!dictParameterSet.ContainsKey(boneName))
            {
                return null;
            }
            return dictParameterSet[boneName];
        }

        public byte[] GetParamByte()
        {
            return LZ4MessagePackSerializer.Serialize(dictParameterSet);
        }

        public bool SetParamByte(byte[] byteParams)
        {
            Dictionary<string, ParameterSet> readParams = LZ4MessagePackSerializer.Deserialize<Dictionary<string, ParameterSet>>(byteParams);

            if (!CheckDictParam(readParams))
            {
                return false;
            }
            dictParameterSet = readParams;

            return true;
        }

        public bool CopyParamsFrom(BreastDynamicBoneParameter dbParams)
        {
            for (int i = 0; i < targetBoneNames.Count(); i++)
            {
                string boneName = targetBoneNames[i];
                if (!dictParameterSet[boneName].CopyParameterFrom(dbParams.dictParameterSet[boneName]))
                {
                    return false;
                }
            }
            return true;
        }

        private bool CheckDictParam(Dictionary<string, ParameterSet> dictParameterSet)
        {
            foreach (string boneName in targetBoneNames)
            {
                if (!dictParameterSet.ContainsKey(boneName))
                {
                    BreastPhysicsController.Logger.LogError("CheckDictParam:dictParameterSet not contain requiered Key");
                    return false;
                }
            }
            return true;
        }


        public override string ToString()
        {
            string value = "";
            foreach (KeyValuePair<string, ParameterSet> kvp in dictParameterSet)
            {
                value += kvp.Value.ToString();
            }
            return value;
        }

        public bool CopyParamsTo(ParamChara paramChara)
        {
            Regex regex = new Regex("_R$");
            foreach(ParameterSet paramSet in dictParameterSet.Values)
            {
                string boneName = regex.Replace(paramSet.BoneName, "_L");
                //Bust Naked
                if (!paramChara.paramBustNaked.paramBones.ContainsKey(boneName)) return false;
                paramSet.CopyParameterTo(paramChara.paramBustNaked.paramBones[boneName]);

                //Bust per coordinate,wearState
                foreach (ChaFileDefine.CoordinateType coordinate in paramChara.paramBust.Keys)
                {
                    foreach (ParamCharaController.ParamsKind state in paramChara.paramBust[coordinate].Keys)
                    {
                        if (!paramChara.paramBust[coordinate][state].paramBones.ContainsKey(boneName)) return false;
                        paramSet.CopyParameterTo(paramChara.paramBust[coordinate][state].paramBones[boneName]);
                    }
                }

            }

            return true;
            
        }

        [MessagePackObject(keyAsPropertyName: true)]
        public class ParameterSet
        {
            public string BoneName;
            public bool IsRotationCalc;
            public float Damping;
            public float Elasticity;
            public float Stiffness;
            public float Inert;

            public bool CopyParameterFrom(ParameterSet parameterSet)
            {
                try
                {
                    IsRotationCalc = parameterSet.IsRotationCalc;
                    Damping = parameterSet.Damping;
                    Elasticity = parameterSet.Elasticity;
                    Stiffness = parameterSet.Stiffness;
                    Inert = parameterSet.Inert;
                    return true;
                }
                catch (Exception e)
                {
                    BreastPhysicsController.Logger.LogError("Failed copy DynamicBoneParam.");
                    return false;
                }
            }

            public void CopyParameterTo(DynamicBone_Ver02.BoneParameter parameter)
            {
                parameter.IsRotationCalc = IsRotationCalc;
                parameter.Damping = Damping;
                parameter.Elasticity = Elasticity;
                parameter.Stiffness = Stiffness;
                parameter.Inert = Inert;
            }

            public void CopyParameterTo(ParamBone paramBone)
            {
                paramBone.boneName = BoneName;
                paramBone.IsRotationCalc = IsRotationCalc;
                paramBone.Damping = Damping;
                paramBone.Elasticity = Elasticity;
                paramBone.Stiffness = Stiffness;
                paramBone.Inert = Inert;
            }

            public override string ToString()
            {
                string value = "";
                value += $"BoneName:{BoneName}\r\n";
                value += $"IsRotationCalc:{IsRotationCalc}\r\n";
                value += $"Damping:{Damping}\r\n";
                value += $"Elasiticity:{Elasticity}\r\n";
                value += $"Stiffness:{Stiffness}\r\n";
                value += $"Inert:{Inert}\r\n";
                return value;
            }
        }


    }


}