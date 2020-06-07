using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.IO;

namespace BreastPhysicsController
{
    [XmlRoot("XMLParamBust")]
    public class XMLParamBust
    {
        [XmlElement("Version")]
        public string Version = "2";
        [XmlElement("Gravity")]
        public float gravity;
        readonly static List<string> partNames = ParamBustCustom.Bones.ToList();
        [XmlArray("ParameterSets")]
        [XmlArrayItem("ParameterSet")]
        public List<XMLParamSet> paramSets;

        public XMLParamBust(ParamBustCustom param)
        {
            gravity = param.gravity;
            paramSets = new List<XMLParamSet>();
            foreach (string partName in partNames)
            {
                XMLParamSet set = new XMLParamSet();
                set.PartName = partName;
                paramSets.Add(set);
            }
            LoadParamFrom(param);
        }

        public XMLParamBust()
        {
        }

        public XMLParamSet GetParameterSet(string partName)
        {
            foreach (XMLParamSet set in paramSets)
            {
                if (set.PartName == partName) return set;
            }
            return null;
        }

        private void LoadParamFrom(ParamBustCustom param)
        {
            gravity = param.gravity;
            foreach (XMLParamSet set in paramSets)
            {
                set.CopyParameterFrom(param.paramBones[set.PartName]);
            }
        }

        public void CopyParams(ParamBustCustom target)
        {
            target.gravity = gravity;
            foreach(XMLParamSet set in paramSets)
            {
                set.CopyParameterTo(target.paramBones[set.PartName]);
                //switch(set.PartName)
                //{
                //    case "Bust01":
                //        set.CopyParameterTo(target.paramBones[ParamBustCustom.Bones[0]]);
                //        break;
                //    case "Bust02":
                //        set.CopyParameterTo(target.paramBones[ParamBustCustom.Bones[1]]);
                //        break;
                //    case "Bust03":
                //        set.CopyParameterTo(target.paramBones[ParamBustCustom.Bones[2]]);
                //        break;
                //    default:
                //        break;
                //}
            }
        }

        public void Serialize(StreamWriter writer)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(XMLParamBust));
            serializer.Serialize(writer, this);
        }

        public bool Deserialize(StreamReader reader)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(XMLParamBust));
            XMLParamBust xmlDynamicBoneParam;
            try
            {
                xmlDynamicBoneParam = serializer.Deserialize(reader) as XMLParamBust;
            }
            catch (Exception e)
            {
                return false;
            }
            if (xmlDynamicBoneParam == null) return false;
            Version = xmlDynamicBoneParam.Version;
            gravity = xmlDynamicBoneParam.gravity;
            paramSets = xmlDynamicBoneParam.paramSets;
            return true;
        }
    }
}
