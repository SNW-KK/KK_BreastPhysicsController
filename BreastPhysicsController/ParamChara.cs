using System.Collections.Generic;
using System.Linq;
using MessagePack;
using System;

namespace BreastPhysicsController
{
    [MessagePackObject(keyAsPropertyName:true)]
    public class ParamChara
    {
        //define
        [IgnoreMember]
        static readonly ChaFileDefine.CoordinateType[] Coordinates = {
            ChaFileDefine.CoordinateType.School01,
            ChaFileDefine.CoordinateType.School02,
            ChaFileDefine.CoordinateType.Gym,
            ChaFileDefine.CoordinateType.Swim,
            ChaFileDefine.CoordinateType.Club,
            ChaFileDefine.CoordinateType.Plain,
            ChaFileDefine.CoordinateType.Pajamas
        };
        [IgnoreMember]
        static readonly ParamCharaController.ParamsKind[] WearStates = { ParamCharaController.ParamsKind.Bra, ParamCharaController.ParamsKind.Tops };

        //parameters
        public ParamBustCustom paramBustNaked;
        public Dictionary<ChaFileDefine.CoordinateType, Dictionary<ParamCharaController.ParamsKind, ParamBustCustom>> paramBust;
        public ParamHipCustom paramHip;

        public ParamChara()
        {
            paramBustNaked = new ParamBustCustom();
            //Create instance of breast's paramter per coordinates,wearStates.
            paramBust = new Dictionary<ChaFileDefine.CoordinateType, Dictionary<ParamCharaController.ParamsKind, ParamBustCustom>>();
            for(int coordinate=0;coordinate<Coordinates.Count();coordinate++)
            {
                Dictionary<ParamCharaController.ParamsKind, ParamBustCustom> parametersCoordinate = new Dictionary<ParamCharaController.ParamsKind, ParamBustCustom>();
                for (int wearState=0;wearState<WearStates.Count();wearState++)
                {
                    ParamBustCustom parameterBreast = new ParamBustCustom(Coordinates[coordinate], WearStates[wearState]);
                    parametersCoordinate.Add(WearStates[wearState], parameterBreast);
                }
                paramBust.Add(Coordinates[coordinate], parametersCoordinate);
            }

            //Hip Custom
            paramHip = new ParamHipCustom();

        }

        public void ResetAllParams()
        {
            paramBustNaked.ResetAllParams();
            foreach (Dictionary<ParamCharaController.ParamsKind, ParamBustCustom> paramPerCoordinate in paramBust.Values)
            {
                foreach(ParamBustCustom paramPerWearState in paramPerCoordinate.Values)
                {
                    paramPerWearState.ResetAllParams();
                }
            }
            paramHip.ResetAllParams();
        }

        public ParamChara Clone()
        {
            ParamChara result = new ParamChara();
            result.paramBustNaked = paramBustNaked.Clone();
            foreach (ChaFileDefine.CoordinateType coordinate in paramBust.Keys)
            {
                foreach(ParamCharaController.ParamsKind state in paramBust[coordinate].Keys)
                {
                    result.paramBust[coordinate][state] = paramBust[coordinate][state].Clone();
                }
            }
            result.paramHip = paramHip.Clone();
            return result;
        }

        public byte[] Serialize()
        {
            return LZ4MessagePackSerializer.Serialize(this);
        }

        public List<ParamBustCustom> GetAllBustParameters()
        {
            List<ParamBustCustom> value = new List<ParamBustCustom>();
            value.Add(paramBustNaked);
            foreach(ChaFileDefine.CoordinateType coordinate in Enum.GetValues(typeof(ChaFileDefine.CoordinateType)))
            {
                value.Add(paramBust[coordinate][ParamCharaController.ParamsKind.Bra]);
                value.Add(paramBust[coordinate][ParamCharaController.ParamsKind.Tops]);
            }
            return value;
        }
    }
}
