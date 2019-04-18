using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.IO;
using MessagePack;

namespace BreastPhysicsController
{

    public class BreastDynamicBoneParameter
    {

        public static readonly string[] targetBoneNames = { "cf_j_bust01_L", "cf_j_bust02_L", "cf_j_bust03_L", "cf_j_bust01_R", "cf_j_bust02_R", "cf_j_bust03_R" };

        public BreastDynamicBoneController _controller;

        public Dictionary<string, ParameterSet> dictParameterSet = new Dictionary<string, ParameterSet>();

        public BreastDynamicBoneParameter(BreastDynamicBoneController controller)
        {
            _controller = controller;
            for(int i=0;i< targetBoneNames.Count();i++)
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
        
            //return MessagePackSerializer.Serialize<Dictionary<string, ParameterSet>>(dictParameterSet);
        }

        public bool SetParamByte(byte[] byteParams)
        {
            Dictionary<string, ParameterSet> readedParams = LZ4MessagePackSerializer.Deserialize<Dictionary<string, ParameterSet>>(byteParams);

            if (!CheckDictParam(readedParams))
            {
                return false;
            }
            dictParameterSet = readedParams;

            return true;
        }

        public bool CopyParamsFrom(BreastDynamicBoneParameter dbParams)
        {
            for(int i=0;i< targetBoneNames.Count();i++)
            {
                string boneName = targetBoneNames[i];
                if (!dictParameterSet[boneName].CopyParameterFrom(dbParams.dictParameterSet[boneName]))
                {
                    return false;
                }
            }
            return true;
        }

        private bool CheckDictParam(Dictionary<string,ParameterSet> dictParameterSet)
        {
            foreach (string boneName in targetBoneNames)
            {
                if (!dictParameterSet.ContainsKey(boneName))
                {
                    Logger.LogFormatted(BepInEx.Logging.LogLevel.Warning, "CheckDictParam:dictParameterSet not contain requiered Key");
                    return false;
                }
            }
            return true;
        }



        public bool LoadParamsFromCharacter(BreastDynamicBoneController controller)
        {
            if (controller.ChaControl.dictDynamicBoneBust != null)
            {
                DynamicBone_Ver02 breastL, breastR;

                breastL = controller.ChaControl.getDynamicBoneBust(ChaInfo.DynamicBoneKind.BreastL);
                breastR = controller.ChaControl.getDynamicBoneBust(ChaInfo.DynamicBoneKind.BreastR);

                foreach (DynamicBone_Ver02.BoneParameter param in breastL.Patterns[0].Params)
                {
                    if (targetBoneNames.Contains(param.Name))
                    {
                        dictParameterSet[param.Name].BoneName = param.Name;
                        dictParameterSet[param.Name].CopyParameterFrom(param);
                    }
                }

                foreach (DynamicBone_Ver02.BoneParameter param in breastR.Patterns[0].Params)
                {
                    if (targetBoneNames.Contains(param.Name))
                    {
                        dictParameterSet[param.Name].BoneName = param.Name;
                        dictParameterSet[param.Name].CopyParameterFrom(param);
                    }
                }

                return true;
            }


            return false;
        }

        public bool LoadParamsFromCharacter(ChaControl chaCtrl)
        { 
            BreastDynamicBoneController dbc = chaCtrl.GetComponent<BreastDynamicBoneController>();
            if (dbc==null || !dbc.haveValidDynamicBoneParam()) return false;

            DynamicBone_Ver02 breastL, breastR;
            if (chaCtrl.dictDynamicBoneBust!=null)
            {
                breastL = chaCtrl.getDynamicBoneBust(ChaInfo.DynamicBoneKind.BreastL);
                breastR = chaCtrl.getDynamicBoneBust(ChaInfo.DynamicBoneKind.BreastR);

                if(breastL!=null && breastR!=null)
                {
                    foreach (DynamicBone_Ver02.BoneParameter param in breastL.Patterns[0].Params)
                    {
                        if (targetBoneNames.Contains(param.Name))
                        {
                            dictParameterSet[param.Name].BoneName = param.Name;
                            dictParameterSet[param.Name].CopyParameterFrom(param);
                        }
                    }

                    foreach (DynamicBone_Ver02.BoneParameter param in breastR.Patterns[0].Params)
                    {
                        if (targetBoneNames.Contains(param.Name))
                        {
                            dictParameterSet[param.Name].BoneName = param.Name;
                            dictParameterSet[param.Name].CopyParameterFrom(param);
                        }
                    }

                    return true;
                }
            }
            return false;
        }

        public bool SaveFile(string path,bool overwrite)
        {
            FileMode mode;
            if (overwrite) mode = FileMode.Create;
            else mode = FileMode.CreateNew;
            try
            {
                using (FileStream fs = new FileStream(path, mode, FileAccess.Write))
                using (StreamWriter sw = new StreamWriter(fs, Encoding.UTF8))
                {
                    XMLDynamicBoneParameter paramterXML = new XMLDynamicBoneParameter(this);
                    paramterXML.Serialize(sw);
                }

            }
            catch(Exception e)
            {

                Logger.LogFormatted(BepInEx.Logging.LogLevel.Warning, "Failed XMLSerialization,Cant save BreastDymamicBoneParameter to XML.");
                Logger.LogFormatted(BepInEx.Logging.LogLevel.Warning, e.ToString());
                return false;

            }
            return true;
        }

        public bool LoadFile(string path)
        {
            try
            {
                using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read))
                using (StreamReader sr= new StreamReader(fs, Encoding.UTF8))
                {
                    XMLDynamicBoneParameter parameterXML = new XMLDynamicBoneParameter();
                    if (!parameterXML.Deserialize(sr))
                    {
                        Logger.LogFormatted(BepInEx.Logging.LogLevel.Warning, "Failed XML Deserialize");
                        return false;
                    }
                    
                    dictParameterSet["cf_j_bust01_L"].CopyParameterFrom(parameterXML.GetParameterSet("Bust01"));
                    dictParameterSet["cf_j_bust02_L"].CopyParameterFrom(parameterXML.GetParameterSet("Bust02"));
                    dictParameterSet["cf_j_bust03_L"].CopyParameterFrom(parameterXML.GetParameterSet("Bust03"));
                    dictParameterSet["cf_j_bust01_R"].CopyParameterFrom(parameterXML.GetParameterSet("Bust01"));
                    dictParameterSet["cf_j_bust02_R"].CopyParameterFrom(parameterXML.GetParameterSet("Bust02"));
                    dictParameterSet["cf_j_bust03_R"].CopyParameterFrom(parameterXML.GetParameterSet("Bust03"));

                    _controller.needUpdate = true;
                }
            }
            catch(Exception e)
            {
                Logger.LogFormatted(BepInEx.Logging.LogLevel.Warning, "Failed load parameter from file.");
                return false;
            }
            return true;
        }

        public override string ToString()
        {
            string value = "";
            foreach(KeyValuePair<string,ParameterSet> kvp in dictParameterSet)
            {
                value += kvp.Value.ToString();
            }
            return value;
        }

        //public void SetDefaultParams()
        //{
        //    dictParameterSet["cf_j_bust01_L"].BoneName = "cf_j_bust01_L";
        //    dictParameterSet["cf_j_bust01_L"].IsRotationCalc = false;
        //    dictParameterSet["cf_j_bust01_L"].Damping = 0.01f;
        //    dictParameterSet["cf_j_bust01_L"].Elasticity = 0.8f;
        //    dictParameterSet["cf_j_bust01_L"].Stiffness = 0.15f;
        //    dictParameterSet["cf_j_bust01_L"].Inert = 0.2f;

        //    dictParameterSet["cf_j_bust02_L"].BoneName = "cf_j_bust02_L";
        //    dictParameterSet["cf_j_bust02_L"].IsRotationCalc = true;
        //    dictParameterSet["cf_j_bust02_L"].Damping = 0.9f;
        //    dictParameterSet["cf_j_bust02_L"].Elasticity = 0.8f;
        //    dictParameterSet["cf_j_bust02_L"].Stiffness = 0.15f;
        //    dictParameterSet["cf_j_bust02_L"].Inert = 0.01f;

        //    dictParameterSet["cf_j_bust03_L"].BoneName = "cf_j_bust03_L";
        //    dictParameterSet["cf_j_bust03_L"].IsRotationCalc = true;
        //    dictParameterSet["cf_j_bust03_L"].Damping = 0.99f;
        //    dictParameterSet["cf_j_bust03_L"].Elasticity = 0.5f;
        //    dictParameterSet["cf_j_bust03_L"].Stiffness = 0.08f;
        //    dictParameterSet["cf_j_bust03_L"].Inert = 0.01f;

        //    dictParameterSet["cf_j_bust01_R"].BoneName = "cf_j_bust01_R";
        //    dictParameterSet["cf_j_bust01_R"].IsRotationCalc = false;
        //    dictParameterSet["cf_j_bust01_R"].Damping = 0.01f;
        //    dictParameterSet["cf_j_bust01_R"].Elasticity = 0.8f;
        //    dictParameterSet["cf_j_bust01_R"].Stiffness = 0.15f;
        //    dictParameterSet["cf_j_bust01_R"].Inert = 0.2f;

        //    dictParameterSet["cf_j_bust02_R"].BoneName = "cf_j_bust02_R";
        //    dictParameterSet["cf_j_bust02_R"].IsRotationCalc = true;
        //    dictParameterSet["cf_j_bust02_R"].Damping = 0.9f;
        //    dictParameterSet["cf_j_bust02_R"].Elasticity = 0.8f;
        //    dictParameterSet["cf_j_bust02_R"].Stiffness = 0.15f;
        //    dictParameterSet["cf_j_bust02_R"].Inert = 0.01f;

        //    dictParameterSet["cf_j_bust03_R"].BoneName = "cf_j_bust03_R";
        //    dictParameterSet["cf_j_bust03_R"].IsRotationCalc = true;
        //    dictParameterSet["cf_j_bust03_R"].Damping = 0.99f;
        //    dictParameterSet["cf_j_bust03_R"].Elasticity = 0.5f;
        //    dictParameterSet["cf_j_bust03_R"].Stiffness = 0.08f;
        //    dictParameterSet["cf_j_bust03_R"].Inert = 0.01f;

        //}

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
                    Logger.LogFormatted(BepInEx.Logging.LogLevel.Warning, "Failed copy DynamicBoneParam.");
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
                    Logger.LogFormatted(BepInEx.Logging.LogLevel.Warning, "Failed copy DynamicBoneParam.");
                    return false;
                }
                return true;
            }


            public bool CopyParameterFrom(ParameterSetXML paramterXML)
            {
                try
                {
                    IsRotationCalc = paramterXML.IsRotationCalc;
                    Damping = paramterXML.Damping;
                    Elasticity = paramterXML.Elasticity;
                    Stiffness = paramterXML.Stiffness;
                    Inert = paramterXML.Inert;
                }
                catch (Exception e)
                {
                    Logger.LogFormatted(BepInEx.Logging.LogLevel.Warning, "Failed copy DynamicBoneParam.");
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
