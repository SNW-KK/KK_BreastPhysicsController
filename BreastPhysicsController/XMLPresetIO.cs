using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using BreastPhysicsController.Compatibility;

namespace BreastPhysicsController
{
    public static class XMLPresetIO
    {
        public static void SaveXMLBust(ParamBustCustom bust,string path)
        {
            XMLParamBust xml = new XMLParamBust(bust);
            using (FileStream fs = new FileStream(path, FileMode.Create,FileAccess.Write))
                using(StreamWriter sw=new StreamWriter(fs))
            {
                xml.Serialize(sw);
            }
                
        }

        public static bool LoadXMLBust(ParamBustCustom bust, string path)
        {
            if (LoadXMLBust_ver1(bust, path)) return true;
            if (LoadXMLBust_ver2(bust, path)) return true;
            return false;
        }

        private static bool LoadXMLBust_ver1(ParamBustCustom bust, string path)
        {
            XMLDynamicBoneParameter xml_ver1 = new XMLDynamicBoneParameter();
            using (StreamReader reader = new System.IO.StreamReader(path))
            {
                if (!xml_ver1.Deserialize(reader))
                {
                    return false;
                }
            }
            xml_ver1.CopyParamsTo(bust);
            return true;
        }

        private static bool LoadXMLBust_ver2(ParamBustCustom bust, string path)
        {
            XMLParamBust xml_ver2 = new XMLParamBust();
            using (StreamReader reader = new System.IO.StreamReader(path))
            {
                if (!xml_ver2.Deserialize(reader))
                {
                    return false;
                }
            }
            xml_ver2.CopyParams(bust);
            return true;
        }

        public static void SaveXMLHip(ParamHipCustom hip, string path)
        {
            XMLParamHip xml = new XMLParamHip(hip);
            using (FileStream fs = new FileStream(path, FileMode.Create, FileAccess.Write))
            using (StreamWriter sw = new StreamWriter(fs))
            {
                xml.Serialize(sw);
            }

        }
        public static bool LoadXMLHip(ParamHipCustom hip,string path)
        {
            XMLParamHip xml = new XMLParamHip();
            using (StreamReader reader = new StreamReader(path))
            {
                if (!xml.Deserialize(reader)) return false;
            }
            xml.CopyParams(hip);
            return true;
        }
    }
}
