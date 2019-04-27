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

        private List<BreastDynamicBoneParameter.ParameterSet> originalL;
        private List<BreastDynamicBoneParameter.ParameterSet> originalR;
        public BreastDynamicBoneParameter DynamicBoneParameter;

        public bool enable;
        public int controllerID;
        public bool isInitialized;
        public bool onDisable;
        public bool needUpdate;


        protected override void Update()
        {
            if(!isInitialized && needUpdate)
            {
                if (BackupOriginalParameter())
                {
                    isInitialized = true;
                }
            }
            if(needUpdate && enable && isInitialized)
            {
                ApplyBreastDynamicBoneParams();
                needUpdate = false;
            }
            else if(onDisable)
            {
                enable = false;
                onDisable = false;
                OnDisable();
            }
            base.Update();
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

        protected override void OnReload(GameMode currentGameMode)
        {
            enable = false;
            if (ChaControl.sex == 1) //Female
            {
                BreastPhysicsController.w_NeedUpdateCharaList = true;

                DynamicBoneParameter = new BreastDynamicBoneParameter(this);
                onDisable = false;
                isInitialized = false;
                originalL = new List<BreastDynamicBoneParameter.ParameterSet>();
                originalR = new List<BreastDynamicBoneParameter.ParameterSet>();
                controllerID = this.GetInstanceID();
                needUpdate = false;
                InitialLoadParameter();
            }
            else //Male
            {
                DynamicBoneParameter = null;
                isInitialized = false;
                enable = false;
                onDisable = false;
                needUpdate = false;
            }
        }

        public void InitialLoadParameter()
        {
            //needInitialLoad = false;

            var data = GetExtendedData();
            var byteDBParams = new object();
            if (data != null && data.data.TryGetValue(ExtendedDataKey, out byteDBParams) && byteDBParams is byte[])
            {
                if (DynamicBoneParameter.SetParamByte((byte[])byteDBParams))
                {
                    var cardEnable = new object();
                    if (data.data.TryGetValue("Enable", out cardEnable) && cardEnable is bool)
                    {
                        Logger.LogFormatted(LogLevel.Debug, "DynamicBoneParameter was loaded from ExtendedData.");
                        enable = (bool)cardEnable;
                        needUpdate = true;
                        //Logger.LogFormatted(LogLevel.Debug, DynamicBoneParameter.ToString());
                    }
                    else
                    {
                        enable = false;
                        Logger.LogFormatted(LogLevel.Debug, "DynamicBoneParameter readed,but controller is disabled.");
                    }
                    //ApplyBreastDynamicBoneParams();
                }
                else
                {
                    //DynamicBoneParameter.LoadParamsFromCharacter(ChaControl);
                    Logger.LogFormatted(LogLevel.Debug, "Loaded parameters from ExtendedData is invalid. BrestDynamicBoneController is disabled.");
                }
            }
            else
            {
                Logger.LogFormatted(LogLevel.Debug, "DynamicBoneParameter is not saved in card.");
                //DynamicBoneParameter.LoadParamsFromCharacter(this);
            }
        }

        //protected override void OnReload(GameMode currentGameMode)
        //{
        //    enable = false;
        //    if (ChaControl.sex == 1) //Female
        //    {
        //        BreastPhysicsController.w_NeedUpdateCharaList = true;

        //        DynamicBoneParameter = new BreastDynamicBoneParameter(this);
        //        onDisable = false;
        //        save = false;
        //        controllerID = this.GetInstanceID();
        //        needUpdate = true;

        //        var data = GetExtendedData();
        //        var byteDBParams = new object();
        //        if (data != null && data.data.TryGetValue(ExtendedDataKey, out byteDBParams) && byteDBParams is byte[])
        //        {
        //            if (DynamicBoneParameter.SetParamByte((byte[])byteDBParams))
        //            {
        //                var cardEnable = new object();
        //                if (data.data.TryGetValue("Enable", out cardEnable) && cardEnable is bool)
        //                {
        //                    Logger.LogFormatted(LogLevel.Debug, "DynamicBoneParameter was loaded from ExtendedData.");
        //                    enable = (bool)cardEnable;
        //                }
        //                else
        //                {
        //                    Logger.LogFormatted(LogLevel.Debug, "DynamicBoneParameter readed,but controller is disabled.");
        //                }
        //            }
        //            else
        //            {
        //                DynamicBoneParameter.LoadParamsFromCharacter(ChaControl);
        //                Logger.LogFormatted(LogLevel.Debug, "Loaded parameters from ExtendedData is invalid. BrestDynamicBoneController is disabled.");
        //            }
        //        }
        //        else
        //        {
        //            Logger.LogFormatted(LogLevel.Debug, "DynamicBoneParameter is not saved in card.");
        //            //DynamicBoneParameter.LoadParamsFromCharacter(this);
        //            LoadParamsFromCharacter();
        //        }
        //    }
        //    else //Male
        //    {
        //        DynamicBoneParameter = null;
        //        save = false;
        //        needUpdate = false;
        //    }
        //}



        private void OnDisable()
        {
            RestoreOriginalParameter();
        }

        protected override void OnDestroy()
        {
            BreastPhysicsController.w_NeedUpdateCharaList = true;

            base.OnDestroy();
        }

        private bool BackupOriginalParameter()
        {

            DynamicBone_Ver02 breastL = ChaControl.getDynamicBoneBust(ChaInfo.DynamicBoneKind.BreastL);
            if (breastL != null)
            {
                originalL.Clear();
                for (int i = 0; i < breastL.Patterns[0].ParticlePtns.Count; i++)
                {
                    BreastDynamicBoneParameter.ParameterSet set = new BreastDynamicBoneParameter.ParameterSet();
                    set.IsRotationCalc = breastL.Patterns[0].ParticlePtns[i].IsRotationCalc;
                    set.Damping = breastL.Patterns[0].ParticlePtns[i].Damping;
                    set.Elasticity = breastL.Patterns[0].ParticlePtns[i].Elasticity;
                    set.Stiffness = breastL.Patterns[0].ParticlePtns[i].Stiffness;
                    set.Inert = breastL.Patterns[0].ParticlePtns[i].Inert;

                    originalL.Add(set);

                }
            }
            else
            {
                originalL.Clear();
                Logger.LogFormatted(LogLevel.Debug, "Failed backup original parameter of BreastL");
                return false;
            }

            DynamicBone_Ver02 breastR = ChaControl.getDynamicBoneBust(ChaInfo.DynamicBoneKind.BreastR);
            if (breastR != null)
            {
                originalR.Clear();
                for (int i = 0; i < breastR.Patterns[0].ParticlePtns.Count; i++)
                {

                    BreastDynamicBoneParameter.ParameterSet set = new BreastDynamicBoneParameter.ParameterSet();
                    set.IsRotationCalc = breastR.Patterns[0].ParticlePtns[i].IsRotationCalc;
                    set.Damping = breastR.Patterns[0].ParticlePtns[i].Damping;
                    set.Elasticity = breastR.Patterns[0].ParticlePtns[i].Elasticity;
                    set.Stiffness = breastR.Patterns[0].ParticlePtns[i].Stiffness;
                    set.Inert = breastR.Patterns[0].ParticlePtns[i].Inert;

                    originalR.Add(set);

                }
            }
            else
            {
                originalR.Clear();
                Logger.LogFormatted(LogLevel.Debug, "Failed backup original paramerter of BreastR");
                return false;
            }

            return true;
        }

        private void RestoreOriginalParameter()
        {
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
                    breastL.Patterns[0].ParticlePtns[1].IsRotationCalc = originalL[0].IsRotationCalc;
                    breastL.Patterns[0].ParticlePtns[2].IsRotationCalc = originalL[0].IsRotationCalc;

                    breastL.Patterns[0].ParticlePtns[0].Inert = originalL[0].Inert;
                    breastL.Patterns[0].ParticlePtns[1].Inert = originalL[1].Inert;
                    breastL.Patterns[0].ParticlePtns[2].Inert = originalL[2].Inert;
                }
                else Logger.LogFormatted(LogLevel.Debug, "Failed restore Inert of breastL. Backuped parameter is invalid");
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
                    breastR.Patterns[0].ParticlePtns[1].IsRotationCalc = originalR[0].IsRotationCalc;
                    breastR.Patterns[0].ParticlePtns[2].IsRotationCalc = originalR[0].IsRotationCalc;

                    breastR.Patterns[0].ParticlePtns[0].Inert = originalR[0].Inert;
                    breastR.Patterns[0].ParticlePtns[1].Inert = originalR[1].Inert;
                    breastR.Patterns[0].ParticlePtns[2].Inert = originalR[2].Inert;
                }
                else Logger.LogFormatted(LogLevel.Debug, "Failed restore Inert of breastR. Backuped parameter is invalid");
            }

            ChaControl.UpdateBustSoftness();
            ChaControl.reSetupDynamicBoneBust = true;
        }

        public bool LoadParamsFromCharacter()
        {
            //Logger.LogFormatted(LogLevel.Debug, "Call LoadParamsFromCharacter");
            if (!haveValidDynamicBoneParam())
            {
                Logger.LogFormatted(LogLevel.Debug, "LoadParamsFromCharacter:Failed haveValidDynamicBoneParam");
                return false;
            }
            bool success= DynamicBoneParameter.LoadParamsFromCharacter(this);
            if(success)
            {
                Logger.LogFormatted(LogLevel.Debug, "LoadParamsFromCharacter:Success LoadParamsFromCharacter");
                BreastPhysicsController.w_NeedUpdateValue = true;
                return true;
            }
            Logger.LogFormatted(LogLevel.Debug, "LoadParamsFromCharacter:Failed LoadParamsFromCharacter");
            return false;
        }
        


        public void ApplyBreastDynamicBoneParams()
        {
            
            DynamicBone_Ver02 breastL = ChaControl.getDynamicBoneBust(ChaInfo.DynamicBoneKind.BreastL);
            if (breastL != null)
            {
                //Logger.LogFormatted(LogLevel.Debug, "ApplyBreastDynamicBoneParams to BrestL");
                ApplyBreastDynamicBoneParams(breastL);
            }

            DynamicBone_Ver02 breastR = ChaControl.getDynamicBoneBust(ChaInfo.DynamicBoneKind.BreastR);
            if (breastR != null)
            {
                //Logger.LogFormatted(LogLevel.Debug, "ApplyBreastDynamicBoneParams to BrestR");
                ApplyBreastDynamicBoneParams(breastR);
            }

            //ChaControl.reSetupDynamicBoneBust = true;
        }

        public void ApplyBreastDynamicBoneParams(ChaControl chaControl)
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


        public void ApplyBreastDynamicBoneParams(DynamicBone_Ver02 targetDynamicBone)
        {
            //For Params
            //Parameter Patterns[0]="通常"
            for (int i = 0; i < targetDynamicBone.Patterns[0].Params.Count; i++)
            {
                string boneName = targetDynamicBone.Patterns[0].Params[i].Name;
                if (BreastDynamicBoneParameter.targetBoneNames.Contains(boneName))
                {
                    BreastDynamicBoneParameter.ParameterSet parameterSet = DynamicBoneParameter.GetParameterSet(boneName);
                    if(parameterSet!=null)
                    {
                        parameterSet.CopyParameterTo(targetDynamicBone.Patterns[0].Params[i]);
                    }
                        
                }
            }
            //For ParticlePtn
            //ParticlePtn Patterns[0]="通常"
            CopyParamsToParticlePtn(targetDynamicBone);

            //For Particle. Must set at last.
            targetDynamicBone.setPtn(0, true);
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
