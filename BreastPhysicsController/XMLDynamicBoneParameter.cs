using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.IO;

namespace BreastPhysicsController
{
    [XmlRoot("XMLDynamicBoneParameter")]
    public class XMLDynamicBoneParameter
    {
        [XmlElement("Version")]
        public string Version = "1";
        readonly static List<string> partNames = new List<string> { "Bust01","Bust02","Bust03" };
        [XmlArray("ParameterSets")]
        [XmlArrayItem("ParameterSet")]
        public List<ParameterSetXML> parameterSets;

        public XMLDynamicBoneParameter(BreastDynamicBoneParameter param)
        {
            parameterSets = new List<ParameterSetXML>();
            foreach(string partName in partNames)
            {
                ParameterSetXML set = new ParameterSetXML();
                set.PartName = partName;
                parameterSets.Add(set);
            }
            LoadParamFrom(param);
        }

        public XMLDynamicBoneParameter()
        {
        }

        public ParameterSetXML GetParameterSet(string partName)
        {
            foreach (ParameterSetXML set in parameterSets)
            {
                if (set.PartName == partName) return set;
            }
            return null;
        }

        private void LoadParamFrom(BreastDynamicBoneParameter parameter)
        {
            foreach(ParameterSetXML set in parameterSets)
            {
                switch(set.PartName)
                {
                    case "Bust01":
                        set.CopyParameterFrom(parameter.dictParameterSet["cf_j_bust01_L"]);
                        break;
                    case "Bust02":
                        set.CopyParameterFrom(parameter.dictParameterSet["cf_j_bust02_L"]);
                        break;
                    case "Bust03":
                        set.CopyParameterFrom(parameter.dictParameterSet["cf_j_bust03_L"]);
                        break;
                    default:
                        break;
                }
            }
        }

        public void Serialize(StreamWriter writer)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(XMLDynamicBoneParameter));
            serializer.Serialize(writer, this);
        }

        public bool Deserialize(StreamReader reader)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(XMLDynamicBoneParameter));
            XMLDynamicBoneParameter xmlDynamicBoneParam = serializer.Deserialize(reader) as XMLDynamicBoneParameter;
            if (xmlDynamicBoneParam == null) return false;
            Version = xmlDynamicBoneParam.Version;
            parameterSets = xmlDynamicBoneParam.parameterSets;
            return true;
        }
    }

    [Serializable]
    public class ParameterSetXML
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

        public ParameterSetXML() { }

        public bool CopyParameterFrom(BreastDynamicBoneParameter.ParameterSet set)
        {
            try
            {
                IsRotationCalc = set.IsRotationCalc;
                Damping = set.Damping;
                Elasticity = set.Elasticity;
                Stiffness = set.Stiffness;
                Inert = set.Inert;
            }
            catch (Exception e)
            {
                Logger.LogFormatted(BepInEx.Logging.LogLevel.Warning, "Failed copy DynamicBoneParam to ParameterSetXML.");
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
    }
}
