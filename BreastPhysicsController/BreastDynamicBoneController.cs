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
        public bool enable;
        public BreastDynamicBoneParameter DynamicBoneParameter;
        public int controllerID;
        public bool save;
        public bool onDisable;
        public bool needUpdate;
        //public bool needInitialLoad;

        protected override void Update()
        {
            if(enable && needUpdate)
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
                save = false;
                controllerID = this.GetInstanceID();
                needUpdate = false;
                //needInitialLoad = true;
                InitialLoadParameter();
            }
            else //Male
            {
                DynamicBoneParameter = null;
                save = false;
                enable = false;
                onDisable = false;
                needUpdate = false;
                //needInitialLoad = false;
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

        private void RestoreOriginalParameter()
        {
            DynamicBone_Ver02 breastL=ChaControl.getDynamicBoneBust(ChaInfo.DynamicBoneKind.BreastL);
            if(breastL!=null)
            {
                breastL.Patterns[0].Params[0].IsRotationCalc = false;
                breastL.Patterns[0].Params[1].IsRotationCalc = true;
                breastL.Patterns[0].Params[2].IsRotationCalc = false;
                breastL.Patterns[0].ParticlePtns[0].IsRotationCalc = false;
                breastL.Patterns[0].ParticlePtns[1].IsRotationCalc = true;
                breastL.Patterns[0].ParticlePtns[2].IsRotationCalc = false;
            }

            DynamicBone_Ver02 breastR = ChaControl.getDynamicBoneBust(ChaInfo.DynamicBoneKind.BreastR);
            if(breastR!=null)
            {
                breastR.Patterns[0].Params[0].IsRotationCalc = false;
                breastR.Patterns[0].Params[1].IsRotationCalc = true;
                breastR.Patterns[0].Params[2].IsRotationCalc = false;
                breastR.Patterns[0].ParticlePtns[0].IsRotationCalc = false;
                breastR.Patterns[0].ParticlePtns[1].IsRotationCalc = true;
                breastR.Patterns[0].ParticlePtns[2].IsRotationCalc = false;
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
