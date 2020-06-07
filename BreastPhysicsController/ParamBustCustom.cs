using System.Collections.Generic;
using System.Linq;
using MessagePack;


namespace BreastPhysicsController
{
    [MessagePackObject(keyAsPropertyName:true)]
    public class ParamBustCustom
    {
        //define
        [IgnoreMember]
        public static readonly string[] Bones = { "cf_j_bust01_L", "cf_j_bust02_L", "cf_j_bust03_L"};
        public enum WearState { Naked = 0, Bra = 1, Tops =2 }

        //parameters
        public bool enabled;
        public float gravity;
        public ChaFileDefine.CoordinateType _coordinate { get; private set; }
        public ParamCharaController.ParamsKind _wearState { get; private set; }
        public Dictionary<string, ParamBone> paramBones;

        public ParamBustCustom()
        {
            enabled = false;
            _wearState = ParamCharaController.ParamsKind.Naked;
            paramBones = new Dictionary<string, ParamBone>();
            for (int i = 0; i < Bones.Count(); i++)
            {
                paramBones.Add(Bones[i], new ParamBone(Bones[i]));
            }
            gravity = 0;
        }

        public ParamBustCustom(ChaFileDefine.CoordinateType coordinate, ParamCharaController.ParamsKind wearState)
        {
            enabled = false;
            gravity = 0;
            _coordinate = coordinate;
            _wearState = wearState;
            paramBones = new Dictionary<string, ParamBone>();
            for (int i = 0; i < Bones.Count(); i++)
            {
                paramBones.Add(Bones[i], new ParamBone(Bones[i]));
            }
        }

        public ParamBone GetParameterBone(string boneName)
        {
            if (paramBones.ContainsKey(boneName)) return paramBones[boneName];
            else return null;
        }

        public void SetParameterBone(string targetBone, bool isRotationCalc, float damping, float elasticity, float stiffness, float inert)
        {
            paramBones[targetBone].SetParameter(isRotationCalc, damping, elasticity, stiffness, inert);
        }

        public ParamBustCustom Clone()
        {
            ParamBustCustom result;
            if (_wearState == ParamCharaController.ParamsKind.Naked)
                result = new ParamBustCustom();
            else
                result = new ParamBustCustom(_coordinate, _wearState);

            result.enabled = enabled;
            foreach(string bone in paramBones.Keys)
            {
                result.paramBones[bone] = paramBones[bone].Clone();
            }
            result.gravity = gravity;
            return result;
        }

        public void CopyParamsFrom(ParamBustCustom source)
        {
            gravity = source.gravity;

            foreach(string bone in Bones)
            {
                paramBones[bone].CopyParameterFrom(source.paramBones[bone]);
            }
        }

        public void ResetAllParams()
        {
            foreach(ParamBone paramBone in paramBones.Values)
            {
                paramBone.ResetAllParams();
            }
        }

        public override string ToString()
        {
            string value = "";
            value += "ParamBustCustom.ToString()\r\n";
            value += $"_coordinate:{_coordinate.ToString()},_wearState:{_wearState.ToString()}\r\n";
            value += $"enabled:{enabled.ToString()}\r\n";
            value += $"gravity:{gravity.ToString()}\r\n";
            foreach (string bone in Bones)
            {
                value += paramBones[bone].ToString();
            }
            return value;
        }

        public bool LoadParamsFromCharacter(ParamCharaController controller)
        {
            if (controller.ChaControl.dictDynamicBoneBust != null)
            {
                DynamicBone_Ver02 dynamicBoneChara;

                dynamicBoneChara = controller.ChaControl.getDynamicBoneBust(ChaInfo.DynamicBoneKind.BreastL);

                gravity = dynamicBoneChara.Gravity.y;

                foreach (DynamicBone_Ver02.BoneParameter param in dynamicBoneChara.Patterns[0].Params)
                {
                    if (paramBones.ContainsKey(param.Name))
                    {
                        paramBones[param.Name].boneName = param.Name;
                        paramBones[param.Name].CopyParameterFrom(param);
                    }
                }

                return true;
            }

            return false;
        }

    }
}

