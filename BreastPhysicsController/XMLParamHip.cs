using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.IO;

namespace BreastPhysicsController
{
    [XmlRoot("XMLParamHip")]
    public class XMLParamHip
    {
        [XmlElement("Version")]
        public string Version = "2";
        [XmlElement("Gravity")]
        public float gravity;
        readonly static List<string> partNames = ParamHipCustom.Bones.ToList();
        [XmlArray("ParameterSets")]
        [XmlArrayItem("ParameterSet")]
        public List<XMLParamSet> paramSets;

        public XMLParamHip(ParamHipCustom param)
        {
            paramSets = new List<XMLParamSet>();
            foreach (string partName in partNames)
            {
                XMLParamSet set = new XMLParamSet();
                set.PartName = partName;
                paramSets.Add(set);
            }
            LoadParamFrom(param);
        }

        public XMLParamHip()
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

        private void LoadParamFrom(ParamHipCustom source)
        {
            gravity = source.gravity;
            foreach (XMLParamSet set in paramSets)
            {
                set.CopyParameterFrom(source.paramBones[set.PartName]);
            }
        }

        public void CopyParams(ParamHipCustom target)
        {
            target.gravity = gravity;
            foreach (XMLParamSet set in paramSets)
            {
                set.CopyParameterTo(target.paramBones[set.PartName]);
            }
        }

        public void Serialize(StreamWriter writer)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(XMLParamHip));
            serializer.Serialize(writer, this);
        }

        public bool Deserialize(StreamReader reader)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(XMLParamHip));
            XMLParamHip xmlDynamicBoneParam;
            try
            {
                xmlDynamicBoneParam = serializer.Deserialize(reader) as XMLParamHip;
            }
            catch(Exception e)
            {
                return false;
            }
            if (xmlDynamicBoneParam == null) return false;
            CopyFrom(xmlDynamicBoneParam);
            return true;
        }

        public void CopyFrom(XMLParamHip source)
        {
            Version = source.Version;
            gravity = source.gravity;
            paramSets = source.paramSets;
        }
    }
}
