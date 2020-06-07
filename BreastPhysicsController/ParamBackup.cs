using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BreastPhysicsController
{
    class ParamBackup
    {
        public ParamBustOrg bust;
        public bool backupedBust { get; private set; }
        public ParamHipOrg hip;
        public bool backupedHip { get; private set; }

        public ParamBackup()
        {
            bust = new ParamBustOrg();
            backupedBust = false;
            hip = new ParamHipOrg();
            backupedHip = false;
        }

        public void BackupAll(ChaControl control)
        {
            BackupBust(control.getDynamicBoneBust(ChaInfo.DynamicBoneKind.BreastL));
            BackupHip(control.getDynamicBoneBust(ChaInfo.DynamicBoneKind.HipL));
        }

        public bool BackupBust(DynamicBone_Ver02 target)
        {
            if (backupedBust)
            {
#if DEBUG
                BreastPhysicsController.Logger.LogDebug("BackuBust failed because i'm having backupdata already.");
#endif
                return false;
            }
            bust.CopyParamFrom(target);
            backupedBust = true;

#if DEBUG
            BreastPhysicsController.Logger.LogDebug("Call BackupBust");
            BreastPhysicsController.Logger.LogDebug(bust.ToString());
#endif
            return true;
        }

        public bool BackupHip(DynamicBone_Ver02 target)
        {
            if (backupedHip)
            {
#if DEBUG
                BreastPhysicsController.Logger.LogDebug("BackuHip failed because i'm having backupdata already.");
#endif
                return false;
            }
            hip.CopyParamFrom(target);
            backupedHip = true;

#if DEBUG
            BreastPhysicsController.Logger.LogDebug("Call BackupHip");
            BreastPhysicsController.Logger.LogDebug(hip.ToString());
#endif
            return true;
        }

        public void RestoreAll(ChaControl control)
        {
            RestoreBust(control);
            RestoreHip(control);
            control.ReSetupDynamicBoneBust();
        }

        public bool RestoreBust(ChaControl control)
        {
#if DEBUG
            BreastPhysicsController.Logger.LogDebug("Call RestoreBust");
#endif
            if (!backupedBust) return false;

            DynamicBone_Ver02 breastL = control.getDynamicBoneBust(ChaInfo.DynamicBoneKind.BreastL);
            DynamicBone_Ver02 breastR = control.getDynamicBoneBust(ChaInfo.DynamicBoneKind.BreastR);

            CharaParamControl.ApplyParamBust(bust, breastL);
            CharaParamControl.ApplyParamBust(bust, breastR);

            backupedBust = false;
            bust.Clear();

            return true;
        }

        public bool RestoreHip(ChaControl control)
        {
#if DEBUG
            BreastPhysicsController.Logger.LogDebug("Call RestoreHip");
#endif
            if (!backupedHip) return false;

            DynamicBone_Ver02 hipL = control.getDynamicBoneBust(ChaInfo.DynamicBoneKind.HipL);
            DynamicBone_Ver02 hipR = control.getDynamicBoneBust(ChaInfo.DynamicBoneKind.HipR);

            CharaParamControl.ApplyParamHip(hip, hipL);
            CharaParamControl.ApplyParamHip(hip, hipR);

            backupedHip = false;
            hip.Clear();
#if DEBUG
            BreastPhysicsController.Logger.LogDebug("Done RestoreHip");
#endif
            return false;
        }
    }
}
