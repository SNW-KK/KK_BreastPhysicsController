using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BreastPhysicsController
{
    public class ParamChangedInfo
    {
        public ChaFileDefine.CoordinateType coordinate { get; private set; }
        public ParamCharaController.ParamsKind kind { get; private set; }
        public bool changedEnabled { get; private set; } = false;
        public bool changedParam { get; private set; } = false;
        public bool forceChanged;

        public void SetInfo(ChaFileDefine.CoordinateType coordinate, ParamCharaController.ParamsKind kind, bool changedEnabled, bool changedParam,bool forceChanged=false)
        {
            this.coordinate = coordinate;
            this.kind = kind;
            this.changedEnabled = changedEnabled;
            this.changedParam = changedParam;
            this.forceChanged = forceChanged;
        }

        public void Reset()
        {
            this.coordinate = 0;
            this.kind = 0;
            this.changedEnabled = false;
            this.changedParam = false;
            this.forceChanged = false;
        }
    }
}
