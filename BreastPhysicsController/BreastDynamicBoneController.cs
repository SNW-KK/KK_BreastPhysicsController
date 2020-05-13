using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using BepInEx.Logging;
using ExtensibleSaveFormat;
using KKAPI;
using KKAPI.Chara;
using UnityEngine;

namespace BreastPhysicsController
{
    public class BreastDynamicBoneController : CharaCustomFunctionController
    {
        public readonly static string GUID = "BreastDynamicBoneController";
        public const string ExtendedDataKey = "BrestDynamicBoneParameter";

        private List<BreastDynamicBoneParameter.ParameterSet> orgParamL, orgParticlePtnL, orgParamR, orgParticlePtnR;
        public BreastDynamicBoneParameter DynamicBoneParameter;

        public int controllerID;

        public bool enable;

        public bool needBackup;
        public bool needRestore;
        public bool needApplyToChara;

        public bool haveBackup;
        public bool charaHaveOrgParam;

        protected override void Awake()
        {
            controllerID = this.GetInstanceID();
            ControllerManager.AddController(this);

            enable = false;
            needBackup = false;
            needRestore = false;
            needApplyToChara = false;
            haveBackup = false;
            charaHaveOrgParam = true;

            orgParamL = new List<BreastDynamicBoneParameter.ParameterSet>();
            orgParticlePtnL = new List<BreastDynamicBoneParameter.ParameterSet>();
            orgParamR = new List<BreastDynamicBoneParameter.ParameterSet>();
            orgParticlePtnR = new List<BreastDynamicBoneParameter.ParameterSet>();

            base.Awake();
        }

        private void LateUpdate()
        {
            //Backup
            if(needBackup && charaHaveOrgParam)
            {
                BackupOriginalParameter();
            }
            //Apply
            if(needApplyToChara && haveBackup)
            {
                ApplyBreastDynamicBoneParams();
            }
            //Restore
            if(needRestore && haveBackup)
            {
                RestoreOriginalParameter();
            }
        }

        protected override void OnReload(GameMode currentGameMode)
        {
#if DEBUG
            BreastPhysicsController.Logger.LogDebug("Call OnReload");
#endif

            if (ChaControl.sex == 1) //Female
            {
                
                enable = false;
                needBackup = false;
                needRestore = false;
                needApplyToChara = false;

                //isRotationCalc and Inert become previous character's parameters when load card in Maker if previous character's controller is enabled.
                //so if previous character's controller have backup, force restore original parameters at first.
                if (haveBackup)
                {
                    needRestore = true;
                    charaHaveOrgParam = false;
                }
                else
                {
                    charaHaveOrgParam = true;

                    orgParamL.Clear();
                    orgParticlePtnL.Clear();
                    orgParamR.Clear();
                    orgParticlePtnR.Clear();
                }

                DynamicBoneParameter = new BreastDynamicBoneParameter(this);
                BreastPhysicsController.w_NeedUpdateCharaList = true;

                if (!LoadExtendedData())
                {
                    ChaControl.ReSetupDynamicBoneBust();
                }
            }
            else //Male
            {
                DynamicBoneParameter = null;
            }
        }

        public void OnEnableController()
        {
#if DEBUG
            BreastPhysicsController.Logger.LogDebug("Call OnEnableController");
#endif
            enable = true;
            if (!haveBackup && charaHaveOrgParam) needBackup = true;
            needApplyToChara = true;

        }

        public void OnBustSoftRecalc()
        {
#if DEBUG
            BreastPhysicsController.Logger.LogDebug("Call OnBustSoftRecalc");
#endif
            if (!enable) return;

            if (!haveBackup && charaHaveOrgParam) needBackup = true;
            needApplyToChara = true;
        }

        public void OnWindowValueChanged(ControllerWindow window)
        {
#if DEBUG
            BreastPhysicsController.Logger.LogDebug("Call OnWindowValueChanged");
#endif
            //enable = window.controllEnable.GetValue();

            DynamicBoneParameter.dictParameterSet["cf_j_bust01_L"].IsRotationCalc = window.irc01.GetValue();
            DynamicBoneParameter.dictParameterSet["cf_j_bust01_L"].Damping = window.damping01.GetValue();
            DynamicBoneParameter.dictParameterSet["cf_j_bust01_L"].Elasticity = window.elasticity01.GetValue();
            DynamicBoneParameter.dictParameterSet["cf_j_bust01_L"].Stiffness = window.stiffness01.GetValue();
            DynamicBoneParameter.dictParameterSet["cf_j_bust01_L"].Inert = window.inert01.GetValue();

            DynamicBoneParameter.dictParameterSet["cf_j_bust02_L"].IsRotationCalc = window.irc02.GetValue();
            DynamicBoneParameter.dictParameterSet["cf_j_bust02_L"].Damping = window.damping02.GetValue();
            DynamicBoneParameter.dictParameterSet["cf_j_bust02_L"].Elasticity = window.elasticity02.GetValue();
            DynamicBoneParameter.dictParameterSet["cf_j_bust02_L"].Stiffness = window.stiffness02.GetValue();
            DynamicBoneParameter.dictParameterSet["cf_j_bust02_L"].Inert = window.inert02.GetValue();

            DynamicBoneParameter.dictParameterSet["cf_j_bust03_L"].IsRotationCalc = window.irc03.GetValue();
            DynamicBoneParameter.dictParameterSet["cf_j_bust03_L"].Damping = window.damping03.GetValue();
            DynamicBoneParameter.dictParameterSet["cf_j_bust03_L"].Elasticity = window.elasticity03.GetValue();
            DynamicBoneParameter.dictParameterSet["cf_j_bust03_L"].Stiffness = window.stiffness03.GetValue();
            DynamicBoneParameter.dictParameterSet["cf_j_bust03_L"].Inert = window.inert03.GetValue();

            DynamicBoneParameter.dictParameterSet["cf_j_bust01_R"].CopyParameterFrom(DynamicBoneParameter.dictParameterSet["cf_j_bust01_L"]);
            DynamicBoneParameter.dictParameterSet["cf_j_bust02_R"].CopyParameterFrom(DynamicBoneParameter.dictParameterSet["cf_j_bust02_L"]);
            DynamicBoneParameter.dictParameterSet["cf_j_bust03_R"].CopyParameterFrom(DynamicBoneParameter.dictParameterSet["cf_j_bust03_L"]);


            if (!enable)
            {   
                return;
            }

            if (!haveBackup && charaHaveOrgParam) needBackup = true;
            needApplyToChara = true;
        }

        public void OnDisableController()
        {
#if DEBUG
            BreastPhysicsController.Logger.LogDebug("Call OnDisableController");
#endif
            enable = false;
            if (haveBackup) needRestore = true;
        }

        protected override void OnCardBeingSaved(GameMode currentGameMode)
        {
            if(ChaControl.sex==1 && enable)
            {
                var data = new PluginData();
                data.data.Add(ExtendedDataKey, DynamicBoneParameter.GetParamByte());
                data.data.Add("Enable", enable);
                data.version = 1;
                SetExtendedData(data);
            }

        }

        protected override void OnDestroy()
        {
            ControllerManager.RemoveController(controllerID);
            BreastPhysicsController.w_NeedUpdateCharaList = true;
            base.OnDestroy();
        }

        private bool LoadExtendedData()
        {

            var data = GetExtendedData();
            var byteDBParams = new object();
            if (data != null && data.data.TryGetValue(ExtendedDataKey, out byteDBParams) && byteDBParams is byte[])
            {
                if (DynamicBoneParameter.SetParamByte((byte[])byteDBParams))
                {
                    var cardEnable = new object();
                    if (data.data.TryGetValue("Enable", out cardEnable) && cardEnable is bool)
                    {
#if DEBUG
                        BreastPhysicsController.Logger.LogDebug("Read parameters from ExtendedData\r\n"+DynamicBoneParameter.ToString());
#endif
                        if ((bool)cardEnable)
                        {
                            BreastPhysicsController.Logger.Log(LogLevel.Info, "Loaded parameters from ExtendedData(Character Card).");
                            enable = true;
                            needBackup = true;
                            needApplyToChara = true;
                            return true;
                        }
                        else
                        {
                            BreastPhysicsController.Logger.Log(LogLevel.Info, "Loaded parameters from ExtendedData. but BrestDynamicBoneController is disabled.");
                        }
                    }
                    else
                    {
                        BreastPhysicsController.Logger.Log(LogLevel.Info, "Loaded parameters from ExtendedData is invalid. BrestDynamicBoneController is disabled.");
                    }
                }
                else
                {
                    BreastPhysicsController.Logger.Log(LogLevel.Warning, "Loaded parameters from ExtendedData is invalid. BrestDynamicBoneController is disabled.");
                }
            }
            else
            {
                BreastPhysicsController.Logger.Log(LogLevel.Info, "DynamicBoneParameter is not saved in card.");
            }

            return false;
        }

        private bool BackupOriginalParameter()
        {
#if DEBUG
            BreastPhysicsController.Logger.LogDebug("Call BackupOriginalParameter");
#endif
            DynamicBone_Ver02 breastL = ChaControl.getDynamicBoneBust(ChaInfo.DynamicBoneKind.BreastL);
            if (breastL != null)
            {
                orgParamL.Clear();
                for (int i = 0; i < breastL.Patterns[0].Params.Count; i++)
                {
                    BreastDynamicBoneParameter.ParameterSet set = new BreastDynamicBoneParameter.ParameterSet();
                    set.IsRotationCalc = breastL.Patterns[0].Params[i].IsRotationCalc;
                    set.Damping = breastL.Patterns[0].Params[i].Damping;
                    set.Elasticity = breastL.Patterns[0].Params[i].Elasticity;
                    set.Stiffness = breastL.Patterns[0].Params[i].Stiffness;
                    set.Inert = breastL.Patterns[0].Params[i].Inert;

                    orgParamL.Add(set);
                }

                orgParticlePtnL.Clear();
                for (int i = 0; i < breastL.Patterns[0].ParticlePtns.Count; i++)
                {
                    BreastDynamicBoneParameter.ParameterSet set = new BreastDynamicBoneParameter.ParameterSet();
                    set.IsRotationCalc = breastL.Patterns[0].ParticlePtns[i].IsRotationCalc;
                    set.Damping = breastL.Patterns[0].ParticlePtns[i].Damping;
                    set.Elasticity = breastL.Patterns[0].ParticlePtns[i].Elasticity;
                    set.Stiffness = breastL.Patterns[0].ParticlePtns[i].Stiffness;
                    set.Inert = breastL.Patterns[0].ParticlePtns[i].Inert;

                    orgParticlePtnL.Add(set);
                }

            }
            else
            {
                BreastPhysicsController.Logger.Log(LogLevel.Warning, "Failed backup original Params and ParticlePtns of BreastL");
                return false;
            }

            DynamicBone_Ver02 breastR = ChaControl.getDynamicBoneBust(ChaInfo.DynamicBoneKind.BreastR);
            if (breastR != null)
            {
                orgParamR.Clear();
                for (int i = 0; i < breastR.Patterns[0].Params.Count; i++)
                {

                    BreastDynamicBoneParameter.ParameterSet set = new BreastDynamicBoneParameter.ParameterSet();
                    set.IsRotationCalc = breastR.Patterns[0].Params[i].IsRotationCalc;
                    set.Damping = breastR.Patterns[0].Params[i].Damping;
                    set.Elasticity = breastR.Patterns[0].Params[i].Elasticity;
                    set.Stiffness = breastR.Patterns[0].Params[i].Stiffness;
                    set.Inert = breastR.Patterns[0].Params[i].Inert;

                    orgParamR.Add(set);
                }

                orgParticlePtnR.Clear();
                for (int i = 0; i < breastR.Patterns[0].ParticlePtns.Count; i++)
                {

                    BreastDynamicBoneParameter.ParameterSet set = new BreastDynamicBoneParameter.ParameterSet();
                    set.IsRotationCalc = breastR.Patterns[0].ParticlePtns[i].IsRotationCalc;
                    set.Damping = breastR.Patterns[0].ParticlePtns[i].Damping;
                    set.Elasticity = breastR.Patterns[0].ParticlePtns[i].Elasticity;
                    set.Stiffness = breastR.Patterns[0].ParticlePtns[i].Stiffness;
                    set.Inert = breastR.Patterns[0].ParticlePtns[i].Inert;

                    orgParticlePtnR.Add(set);
                }
            }
            else
            {
                BreastPhysicsController.Logger.Log(LogLevel.Warning, "Failed backup original Params and ParticlePtns of BreastR");
                return false;
            }
#if DEBUG
            BreastPhysicsController.Logger.LogDebug("Backuped Bust L Params");
            foreach (BreastDynamicBoneParameter.ParameterSet set in orgParamL)
            {
                BreastPhysicsController.Logger.LogDebug("\r\n"+set.ToString());
            }

            BreastPhysicsController.Logger.LogDebug("Backuped Bust R Params");
            foreach (BreastDynamicBoneParameter.ParameterSet set in orgParamR)
            {
                BreastPhysicsController.Logger.LogDebug("\r\n" + set.ToString());
            }

            BreastPhysicsController.Logger.LogDebug("Backuped Bust L ParticlePtns");
            foreach (BreastDynamicBoneParameter.ParameterSet set in orgParticlePtnL)
            {
                BreastPhysicsController.Logger.LogDebug("\r\n" + set.ToString());
            }

            BreastPhysicsController.Logger.LogDebug("Backuped Bust R ParticlePtns");
            foreach (BreastDynamicBoneParameter.ParameterSet set in orgParticlePtnR)
            {
                BreastPhysicsController.Logger.LogDebug("\r\n" + set.ToString());
            }
#endif

            haveBackup = true;
            needBackup = false;
            return true;
        }

        private bool ApplyBreastDynamicBoneParams()
        {
#if DEBUG
            BreastPhysicsController.Logger.LogDebug("Call ApplyBreastDynamicBoneParams");
#endif
            DynamicBone_Ver02 breastL = ChaControl.getDynamicBoneBust(ChaInfo.DynamicBoneKind.BreastL);
            if (breastL != null)
            {
#if DEBUG
                BreastPhysicsController.Logger.LogDebug("ApplyBreastDynamicBoneParams to BrestL");
#endif
                if (!ApplyBreastDynamicBoneParams(breastL)) return false;
            }
            else return false;

            DynamicBone_Ver02 breastR = ChaControl.getDynamicBoneBust(ChaInfo.DynamicBoneKind.BreastR);
            if (breastR != null)
            {
#if DEBUG
                BreastPhysicsController.Logger.LogDebug("ApplyBreastDynamicBoneParams to BrestR");
#endif
                if (!ApplyBreastDynamicBoneParams(breastR)) return false;
            }
            else return false;

            //breastL.ResetPosition();
            //breastR.ResetPosition();
            ChaControl.ReSetupDynamicBoneBust();
            charaHaveOrgParam = false;
            needApplyToChara = false;
            return true;
        }

        private void ApplyBreastDynamicBoneParams(ChaControl chaControl)
        {

            DynamicBone_Ver02 breastL = chaControl.getDynamicBoneBust(ChaInfo.DynamicBoneKind.BreastL);
            if (breastL != null)
            {
                ApplyBreastDynamicBoneParams(breastL);
            }

            DynamicBone_Ver02 breastR = chaControl.getDynamicBoneBust(ChaInfo.DynamicBoneKind.BreastR);
            if (breastR != null)
            {
                ApplyBreastDynamicBoneParams(breastR);
            }

            chaControl.reSetupDynamicBoneBust = true;
        }

        private bool ApplyBreastDynamicBoneParams(DynamicBone_Ver02 targetDynamicBone)
        {

            //For Params
            //Parameter Patterns[0]="通常"
            for (int i = 0; i < targetDynamicBone.Patterns[0].Params.Count; i++)
            {
                string boneName = targetDynamicBone.Patterns[0].Params[i].Name;
                if (BreastDynamicBoneParameter.targetBoneNames.Contains(boneName))
                {
                    BreastDynamicBoneParameter.ParameterSet parameterSet = DynamicBoneParameter.GetParameterSet(boneName);
                    if (parameterSet != null)
                    {
                        parameterSet.CopyParameterTo(targetDynamicBone.Patterns[0].Params[i]);
                    }
                    else return false;

                }
            }

            //For ParticlePtn
            //ParticlePtn Patterns[0]="通常"
            CopyParamsToParticlePtn(targetDynamicBone);

            //For Particle. Must set at last.
            targetDynamicBone.setPtn(0, true);

            return true;
        }

        private void RestoreOriginalParameter()
        {
#if DEBUG
            BreastPhysicsController.Logger.LogDebug("Call RestoreOriginalParameter");
#endif
            /*
            DynamicBone_Ver02 breastL=ChaControl.getDynamicBoneBust(ChaInfo.DynamicBoneKind.BreastL);
            if(breastL!=null)
            {
               
                if (originalL.Count >= 3)
                {
                    //Params
                    breastL.Patterns[0].Params[0].IsRotationCalc = originalL[0].IsRotationCalc;
                    breastL.Patterns[0].Params[1].IsRotationCalc = originalL[1].IsRotationCalc;
                    breastL.Patterns[0].Params[2].IsRotationCalc = originalL[2].IsRotationCalc;

                    breastL.Patterns[0].Params[0].Inert = originalL[0].Inert;
                    breastL.Patterns[0].Params[1].Inert = originalL[1].Inert;
                    breastL.Patterns[0].Params[2].Inert = originalL[2].Inert;

                    //Particle Ptns
                    breastL.Patterns[0].ParticlePtns[0].IsRotationCalc = originalL[0].IsRotationCalc;
                    breastL.Patterns[0].ParticlePtns[1].IsRotationCalc = originalL[1].IsRotationCalc;
                    breastL.Patterns[0].ParticlePtns[2].IsRotationCalc = originalL[2].IsRotationCalc;

                    breastL.Patterns[0].ParticlePtns[0].Inert = originalL[0].Inert;
                    breastL.Patterns[0].ParticlePtns[1].Inert = originalL[1].Inert;
                    breastL.Patterns[0].ParticlePtns[2].Inert = originalL[2].Inert;

                    breastL.setPtn(0, true);
                }
                else BreastPhysicsController.Logger.Log(LogLevel.Warning, "Failed restore original dynamic bone parameters of breastL. Backuped parameter is invalid");
            }

            DynamicBone_Ver02 breastR = ChaControl.getDynamicBoneBust(ChaInfo.DynamicBoneKind.BreastR);
            if(breastR!=null)
            {
                if (originalR.Count >= 3)
                {
                    //Params
                    breastR.Patterns[0].Params[0].IsRotationCalc = originalR[0].IsRotationCalc;
                    breastR.Patterns[0].Params[1].IsRotationCalc = originalR[1].IsRotationCalc;
                    breastR.Patterns[0].Params[2].IsRotationCalc = originalR[2].IsRotationCalc;

                    breastR.Patterns[0].Params[0].Inert = originalR[0].Inert;
                    breastR.Patterns[0].Params[1].Inert = originalR[1].Inert;
                    breastR.Patterns[0].Params[2].Inert = originalR[2].Inert;

                    //Particle Ptns
                    breastR.Patterns[0].ParticlePtns[0].IsRotationCalc = originalR[0].IsRotationCalc;
                    breastR.Patterns[0].ParticlePtns[1].IsRotationCalc = originalR[1].IsRotationCalc;
                    breastR.Patterns[0].ParticlePtns[2].IsRotationCalc = originalR[2].IsRotationCalc;

                    breastR.Patterns[0].ParticlePtns[0].Inert = originalR[0].Inert;
                    breastR.Patterns[0].ParticlePtns[1].Inert = originalR[1].Inert;
                    breastR.Patterns[0].ParticlePtns[2].Inert = originalR[2].Inert;

                    breastR.setPtn(0, true);
                }
                else BreastPhysicsController.Logger.Log(LogLevel.Warning, "Failed restore original dynamic bone parameters of breastR. Backuped parameter is invalid");
            }
            */

            DynamicBone_Ver02 breastL = ChaControl.getDynamicBoneBust(ChaInfo.DynamicBoneKind.BreastL);
            if(breastL!=null)
            {
                for (int i = 0; i < orgParamL.Count; i++)
                { 
                    breastL.Patterns[0].Params[i].IsRotationCalc = orgParamL[i].IsRotationCalc;
                    breastL.Patterns[0].Params[i].Inert = orgParamL[i].Inert;
                }
                orgParamL.Clear();

                for (int i = 0; i < orgParticlePtnL.Count; i++)
                {
                    breastL.Patterns[0].ParticlePtns[i].IsRotationCalc = orgParticlePtnL[i].IsRotationCalc;
                    breastL.Patterns[0].ParticlePtns[i].Inert = orgParticlePtnL[i].Inert;
                }
                orgParticlePtnL.Clear();

                breastL.setPtn(0, true);
            }

            DynamicBone_Ver02 breastR = ChaControl.getDynamicBoneBust(ChaInfo.DynamicBoneKind.BreastR);
            if (breastR != null)
            {
                for (int i = 0; i < orgParamR.Count; i++)
                {
                    breastR.Patterns[0].Params[i].IsRotationCalc = orgParamR[i].IsRotationCalc;
                    breastR.Patterns[0].Params[i].Inert = orgParamR[i].Inert;
                }
                orgParamR.Clear();

                for (int i = 0; i < orgParticlePtnR.Count; i++)
                {
                    breastR.Patterns[0].ParticlePtns[i].IsRotationCalc = orgParticlePtnR[i].IsRotationCalc;
                    breastR.Patterns[0].ParticlePtns[i].Inert = orgParticlePtnR[i].Inert;
                }
                orgParticlePtnR.Clear();

                breastR.setPtn(0, true);
            }

            charaHaveOrgParam = true;
            haveBackup = false;
            needRestore = false;

            ChaControl.reSetupDynamicBoneBust = true;
        }
        
        public bool LoadParamsFromGame()
        {

#if DEBUG
            BreastPhysicsController.Logger.Log(LogLevel.Debug, "Call LoadParamsFromGame");
#endif

            if (!haveValidDynamicBoneParam())
            {
                BreastPhysicsController.Logger.Log(LogLevel.Error, "LoadParamsFromCharacter:Failed haveValidDynamicBoneParam");
                return false;
            }
            bool success= DynamicBoneParameter.LoadParamsFromCharacter(this);
            if(success)
            {
                BreastPhysicsController.Logger.Log(LogLevel.Info, "LoadParamsFromCharacter:Success LoadParamsFromCharacter");
                BreastPhysicsController.w_NeedUpdateValue = true;
                return true;
            }
            BreastPhysicsController.Logger.Log(LogLevel.Error, "LoadParamsFromCharacter:Failed LoadParamsFromCharacter");
            return false;
        }

        private void CopyParamsToParticlePtn(DynamicBone_Ver02 target)
        {
            for(int i = 0; i < target.Patterns[0].Params.Count; i++)
            {
                if (i < target.Patterns[0].ParticlePtns.Count)
                {
                    target.Patterns[0].ParticlePtns[i].IsRotationCalc = target.Patterns[0].Params[i].IsRotationCalc;
                    target.Patterns[0].ParticlePtns[i].Damping = target.Patterns[0].Params[i].Damping;
                    target.Patterns[0].ParticlePtns[i].Elasticity = target.Patterns[0].Params[i].Elasticity;
                    target.Patterns[0].ParticlePtns[i].Stiffness = target.Patterns[0].Params[i].Stiffness;
                    target.Patterns[0].ParticlePtns[i].Inert = target.Patterns[0].Params[i].Inert;
                }
            }
        }

        public bool haveValidDynamicBoneParam()
        {
            DynamicBone_Ver02 breastL = ChaControl.getDynamicBoneBust(ChaInfo.DynamicBoneKind.BreastL);
            DynamicBone_Ver02 breastR = ChaControl.getDynamicBoneBust(ChaInfo.DynamicBoneKind.BreastR);
            foreach (string boneName in BreastDynamicBoneParameter.targetBoneNames)
            {
                if (!ContainBoneName(breastL, boneName) && !ContainBoneName(breastR, boneName)) return false;
            }
            return true;
        }

        private bool ContainBoneName(DynamicBone_Ver02 dynamicBone,string boneName)
        {
            foreach(DynamicBone_Ver02.BoneParameter paramter in dynamicBone.Patterns[0].Params)
            {
                if (paramter.Name == boneName) return true;
            }

            return false;
        }

    }
}
