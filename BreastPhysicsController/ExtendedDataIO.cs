using System;
using System.Collections.Generic;
using MessagePack;
using ExtensibleSaveFormat;
using KKAPI.Maker;
using System.IO;

namespace BreastPhysicsController
{
    internal static class ExtendedDataIO
    {
        static readonly string ExtendedDataID_ver1= "BreastDynamicBoneController";
        static readonly string ExtendedDataKey_ver1= "BrestDynamicBoneParameter";

        public static bool LoadExtendedData(ParamCharaController controller)
        {
#if DEBUG
            BreastPhysicsController.Logger.LogDebug("Call LoadExtendedData");
#endif

            if (LoadExtendedData_ver2(controller)) return true;
            if (LoadExtendedData_ver1(controller)) return true;

            return false;
        }

        private static bool LoadExtendedData_ver1(ParamCharaController controller)
        {
#if DEBUG
            BreastPhysicsController.Logger.LogDebug("Call LoadExtendedData_ver1");
#endif
            var chaFile = MakerAPI.LastLoadedChaFile ?? controller.ChaFileControl;
            var dataVer1= ExtendedSave.GetExtendedDataById(chaFile, ExtendedDataID_ver1);

            if (dataVer1 != null)
            {
#if DEBUG
                BreastPhysicsController.Logger.LogDebug("Found ver1 ExtendedData.");
#endif
                Compatibility.BreastDynamicBoneParameter paramVer1 = new Compatibility.BreastDynamicBoneParameter();
                var byteDBParams = new object();
                if (dataVer1.data.TryGetValue(ExtendedDataKey_ver1, out byteDBParams) && byteDBParams is byte[])
                {
                    if (paramVer1.SetParamByte((byte[])byteDBParams))
                    {
                        BreastPhysicsController.Logger.LogInfo("Loaded ver1 parameters from ExtendedData.");
                        if (paramVer1.CopyParamsTo(controller.paramCustom))
                        {
                            foreach(ParamBustCustom bust in controller.paramCustom.GetAllBustParameters())
                            {
                                bust.enabled = true;
                            }
                            controller.Enabled = true;

                            return true;
                        }
                        else
                        {
#if DEBUG
                            BreastPhysicsController.Logger.LogDebug("Failed copy ver1 parameters to controller");
#endif
                        }
                    }
                    else
                    {
                        BreastPhysicsController.Logger.LogError("Loaded ver1 parameters from ExtendedData is invalid.");
                    }
                }
            }
#if DEBUG
            BreastPhysicsController.Logger.LogDebug("Not found ver1 plugin data.");
#endif
            return false;
        }

        private static bool LoadExtendedData_ver2(ParamCharaController controller)
        {
#if DEBUG
            BreastPhysicsController.Logger.LogDebug("Call LoadExtendedData_ver2");
#endif
            var data = controller.GetExtendedData();

            if (data != null)
            {
#if DEBUG
                BreastPhysicsController.Logger.LogDebug("Found BodyPhysicsController plugin data");
#endif
                var byteCharaParam = new object();
                if (data.data.TryGetValue(ParamCharaController.ExtendedDataKey, out byteCharaParam) && byteCharaParam is byte[])
                {
#if DEBUG
                    BreastPhysicsController.Logger.LogDebug("Found ParamChara data");
#endif
                    controller.paramCustom = LZ4MessagePackSerializer.Deserialize<ParamChara>((byte[])byteCharaParam);
                    controller.Enabled = (bool)data.data["ControllerEnabled"];
                    return true;
                }
                else
                {
#if DEBUG
                    BreastPhysicsController.Logger.LogDebug("Not found ParamChara data");
#endif
                }
            }
#if DEBUG
            BreastPhysicsController.Logger.LogDebug("Not found ver2 plugin data.");
#endif
            return false;
        }

        public static bool LoadExtendedData(out ParamChara paramChara, string path)
        {
            try
            { 
                using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read))
                {
                    byte[] bytes = new byte[fs.Length];
                    fs.Read(bytes, 0, (int)fs.Length);
                    paramChara = LZ4MessagePackSerializer.Deserialize<ParamChara>(bytes);
                    return true;
                }
            }
            catch (FileNotFoundException e)
            {
                BreastPhysicsController.Logger.LogError("Not found default parameter file.");
                paramChara = null;
                return false;
            }
            catch (Exception e)
            {
                BreastPhysicsController.Logger.LogError("Failed opening default parameter file.\r\n" + e.ToString());
                paramChara = null;
                return false;
            }
        }

        public static void SaveExtendedData(ParamCharaController controller)
        {
            var data = new PluginData();
            data.version = controller.ExtendedDataVersion;
            data.data.Add("ControllerEnabled", controller.Enabled);
            data.data.Add(ParamCharaController.ExtendedDataKey, controller.paramCustom.Serialize());
            controller.SetExtendedData(data);
        }

        public static bool SaveParamChara(ParamCharaController controller,string path)
        {
            if (!Directory.Exists(Path.GetDirectoryName(path))) return false;

            byte[] value = controller.paramCustom.Serialize();
            if (value == null) return false;

            using (FileStream fs = new FileStream(path, FileMode.Create, FileAccess.Write))
            {
                fs.Write(value, 0, value.Length);
            }
            return true;
        }
    }
}
