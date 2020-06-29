using System;
using KKAPI;
using KKAPI.Chara;


namespace BreastPhysicsController
{
    public class ParamCharaController : CharaCustomFunctionController
    {
        public readonly static string ExtendedDataKey = "DynamicBoneParameter";
        public readonly int ExtendedDataVersion = 2;

        public enum ParamsKind { Naked = 0, Bra = 1, Tops = 2, Hip = 3 }

        public int controllerID;
        public static ParamChara defaultParam = null;
        public ParamChara paramCustom;
        private ParamBackup paramBackup;
        private bool _enabled;
        public bool Enabled
        {
            set
            {
                if(_enabled!=value)changedControllerEnabled = true;
                _enabled = value;
            }
            get
            {
                return _enabled;
            }
        }
        public bool changedControllerEnabled;
        public ParamChangedInfo changedInfo;
        bool endInitLoad;

        private void Init()
        {
            controllerID = this.GetInstanceID();
            DBControllerManager.AddController(this);
            paramCustom = new ParamChara();
            paramBackup = new ParamBackup();
            Enabled = false;
            changedControllerEnabled = false;
            changedInfo = new ParamChangedInfo();
            endInitLoad = false;
        }

        private void LateUpdate()
        {
            if (paramCustom ==null || !ChaControl.loadEnd)
            {
                return;
            }

            //Load init parameters from chara. 
            //Dont load if parameters were loaded from extended data.(if endInitLoad=false)
            if (!endInitLoad)
            {
                InitialLoadParams();
            }

            //Controller enabled or diabled
            if (changedControllerEnabled)
            {
                changedControllerEnabled = false;
                //enabled
                if (Enabled)
                {
#if DEBUG
                    BreastPhysicsController.Logger.LogDebug("Controller is enabled.");
#endif
                    if (isEnabledNowBust())
                    {
                        paramBackup.BackupBust(ChaControl.getDynamicBoneBust(ChaInfo.DynamicBoneKind.BreastL));
                        ApplyParamBust();
                    }
                    if (isEnabledNowHip())
                    {
                        paramBackup.BackupHip(ChaControl.getDynamicBoneBust(ChaInfo.DynamicBoneKind.HipL));
                        ApplyParamHip();
                    }
                }
                else //disbaled
                {
#if DEBUG
                    BreastPhysicsController.Logger.LogDebug("Controller is disabled.");
#endif
                    paramBackup.RestoreAll(ChaControl);
                }
                changedInfo.Reset();
            }

            //Changed enabled or params per state
            if (changedInfo.forceChanged)
            {
                if (Enabled && isEnabledNowBust())
                {
                    paramBackup.BackupBust(ChaControl.getDynamicBoneBust(ChaInfo.DynamicBoneKind.BreastL));
                    ApplyParamBust();
                }
                else
                {
                    paramBackup.RestoreBust(ChaControl);
                }
                if (Enabled && isEnabledNowHip())
                {
                    paramBackup.BackupHip(ChaControl.getDynamicBoneBust(ChaInfo.DynamicBoneKind.HipL));
                    ApplyParamHip();
                }
                else
                {
                    paramBackup.RestoreHip(ChaControl);
                }
                changedInfo.Reset();
            }
            else if (changedInfo.changedEnabled)
            {
                if (IsMatchState(changedInfo.kind, changedInfo.coordinate))
                {
                    if (changedInfo.kind == ParamsKind.Naked)
                    {
                        if (Enabled && isEnabledNowBust())
                        {
                            paramBackup.BackupBust(ChaControl.getDynamicBoneBust(ChaInfo.DynamicBoneKind.BreastL));
                            ApplyParamBust();
                        }
                        else
                        {
                            paramBackup.RestoreBust(ChaControl);
                        }
                    }
                    else if ((changedInfo.kind == ParamsKind.Bra || changedInfo.kind == ParamsKind.Tops))
                    {
                        if (Enabled && isEnabledNowBust())
                        {
                            paramBackup.BackupBust(ChaControl.getDynamicBoneBust(ChaInfo.DynamicBoneKind.BreastL));
                            ApplyParamBust();
                        }
                        else
                        {
                            paramBackup.RestoreBust(ChaControl);
                        }
                    }
                    else if (changedInfo.kind == ParamsKind.Hip)
                    {
                        if (Enabled && isEnabledNowHip())
                        {
                            paramBackup.BackupHip(ChaControl.getDynamicBoneBust(ChaInfo.DynamicBoneKind.HipL));
                            ApplyParamHip();
                        }
                        else
                        {
                            paramBackup.RestoreHip(ChaControl);
                        }
                    }
                }
                changedInfo.Reset();
            }
            else if(changedInfo.changedParam) //changed paramters per state
            {
                if (IsMatchState(changedInfo.kind, changedInfo.coordinate) && Enabled)
                {
                    if(changedInfo.kind==ParamsKind.Naked || changedInfo.kind==ParamsKind.Bra || changedInfo.kind==ParamsKind.Tops)
                    {
                        if(isEnabledNowBust()) ApplyParamBust();
                    }
                    else if(changedInfo.kind==ParamsKind.Hip)
                    {
                        if (isEnabledNowHip()) ApplyParamHip();
                    }
                }
                changedInfo.Reset();
            }
        }

        protected override void OnReload(GameMode currentGameMode)
        {
#if DEBUG
            BreastPhysicsController.Logger.LogDebug("Call OnReload");
#endif
            if (ChaControl.sex == 1 && HaveDynamicbone()) //Female and HighPoly
            {
                //Restore params from backup.
                if (paramBackup != null)
                {
                    if(paramBackup.backupedBust || paramBackup.backupedHip)
                    {
                        if (paramBackup.backupedBust)
                        {
                            paramBackup.RestoreBust(ChaControl);
                        }
                        if (paramBackup.backupedHip)
                        {
                            paramBackup.RestoreHip(ChaControl);
                        }
                        ChaControl?.ReSetupDynamicBoneBust();
                    }
                }

                //Initialize fields
                Init();
            }
            else //Male or LowPoly
            {
                paramCustom = null;
                paramBackup = null;
            }
        }

        private bool HaveDynamicbone()
        {
            //If not highPoly or dont have dynamicbone, ChaControl.getDynamicBoneBust() throw Null Reffrence Exception.
            DynamicBone_Ver02 dynamicBone;
            try
            {
                dynamicBone = ChaControl.getDynamicBoneBust(ChaInfo.DynamicBoneKind.BreastL);
            }
            catch(NullReferenceException e)
            {
                return false;
            }
            
            if (dynamicBone != null) return true;
            return false;
        }

        private void InitialLoadParams()
        {
            switch (ConfigGlobal.defaultStatusMode.Value)
            {
                case ConfigGlobal.DefalutStatusMode.DontUse:
                    if (ExtendedDataIO.LoadExtendedData(this))
                    {
                    }
                    else
                    {
                        LoadParamFromCharaAll();
                        ChaControl.ReSetupDynamicBoneBust();
                    }
                    break;
                case ConfigGlobal.DefalutStatusMode.UseDefaultStatus:
                    if (ExtendedDataIO.LoadExtendedData(this))
                    {
                        if (!Enabled)
                        {
                            LoadDefaultStatus(true);
                        }
                    }
                    else if (LoadDefaultStatus(true))
                    {
                    }
                    else
                    {
                        LoadParamFromCharaAll();
                        ChaControl.ReSetupDynamicBoneBust();
                    }
                    break;
                case ConfigGlobal.DefalutStatusMode.ForceDefalutStatus:
                    if (LoadDefaultStatus(true))
                    {
                    }
                    else
                    {
                        LoadParamFromCharaAll();
                        ChaControl.ReSetupDynamicBoneBust();
                    }
                    break;
                default:
                    break;
            }
            endInitLoad = true;
        }

        public void OnClothesStateChanged()
        {
            changedInfo.SetInfo(GetNowCoordinate(), GetNowBustWear(), true, false);
        }

        protected override void OnCardBeingSaved(GameMode currentGameMode)
        {
            if (ChaControl.sex == 1)
            {
                ExtendedDataIO.SaveExtendedData(this);
            }

        }

        protected override void OnDestroy()
        {
            DBControllerManager.RemoveController(controllerID);
            //BreastPhysicsController.w_NeedUpdateCharaList = true;
            base.OnDestroy();
        }

        public bool isEnabledNowBust()
        {
            if (paramCustom == null) return false;

            ChaFileDefine.CoordinateType coordinate = GetNowCoordinate();
            ParamsKind state = GetNowBustWear();
            if(state== ParamsKind.Naked)
            {
                return paramCustom.paramBustNaked.enabled;
            }
            else if(state== ParamsKind.Bra || state== ParamsKind.Tops)
            {
                return paramCustom.paramBust[coordinate][state].enabled;
            }

            return false;
        }

        private bool isEnabledNowHip()
        {
            if (paramCustom == null) return false;

            return paramCustom.paramHip.enabled;
        }

        private bool IsMatchState(ParamsKind paramsKind,ChaFileDefine.CoordinateType coordinate=ChaFileDefine.CoordinateType.School01)
        {
            switch(paramsKind)
            {
                case ParamsKind.Naked:
                    if (GetNowBustWear() == ParamsKind.Naked) return true;
                    else return false;
                case ParamsKind.Bra:
                    if (coordinate == GetNowCoordinate() && paramsKind == GetNowBustWear()) return true;
                    else return false;
                case ParamsKind.Tops:
                    if (coordinate == GetNowCoordinate() && paramsKind == GetNowBustWear()) return true;
                    else return false;
                case ParamsKind.Hip:
                    return true;
                default:
                    return false;
            }
        }

        public ChaFileDefine.CoordinateType GetNowCoordinate()
        {
            return (ChaFileDefine.CoordinateType)ChaControl.fileStatus.coordinateType;
        }

        public ParamsKind GetNowBustWear()
        {
            int topsState = ChaControl.fileStatus.clothesState[(int)ChaFileDefine.ClothesKind.top];
            if (topsState == 0) return ParamsKind.Tops;

            int braState = ChaControl.fileStatus.clothesState[(int)ChaFileDefine.ClothesKind.bra];
            if (braState == 0) return ParamsKind.Bra;

            return ParamsKind.Naked;
        }

        private ParamBustCustom GetParamBustCustomNow()
        { 
            ParamCharaController.ParamsKind state = GetNowBustWear();
            if(state==ParamCharaController.ParamsKind.Naked)
            {
                return paramCustom.paramBustNaked;
            }

            if (state == ParamCharaController.ParamsKind.Tops || state == ParamCharaController.ParamsKind.Bra)
            {
                return paramCustom.paramBust[GetNowCoordinate()][state];
            }

            return null;
        }

        private bool ApplyParamBust()
        {
#if DEBUG
            BreastPhysicsController.Logger.LogDebug("Call ApplyParamBust");
#endif

            DynamicBone_Ver02 breastL = ChaControl.getDynamicBoneBust(ChaInfo.DynamicBoneKind.BreastL);
            if (breastL != null)
            {
#if DEBUG
                BreastPhysicsController.Logger.LogDebug("ApplyParamBust to BrestL");
#endif
                if (!CharaParamControl.ApplyParamBust(GetParamBustCustomNow(),breastL)) return false;
            }
            else return false;

            DynamicBone_Ver02 breastR = ChaControl.getDynamicBoneBust(ChaInfo.DynamicBoneKind.BreastR);
            if (breastR != null)
            {
#if DEBUG
                BreastPhysicsController.Logger.LogDebug("ApplyParamBust to BrestR");
                BreastPhysicsController.Logger.LogDebug(GetParamBustCustomNow().ToString());
#endif
                if (!CharaParamControl.ApplyParamBust(GetParamBustCustomNow(), breastR)) return false;
            }
            else return false;
#if DEBUG
            BreastPhysicsController.Logger.LogDebug("Success ApplyParamBust");
#endif
            ChaControl.ReSetupDynamicBoneBust();

            return true;
        }

        private bool ApplyParamHip()
        {
#if DEBUG
            BreastPhysicsController.Logger.LogDebug("Call ApplyParamHip");
#endif

            DynamicBone_Ver02 hipL = ChaControl.getDynamicBoneBust(ChaInfo.DynamicBoneKind.HipL);
            if (hipL != null)
            {
#if DEBUG
                BreastPhysicsController.Logger.LogDebug("ApplyParamHip to HipL");
#endif
                if (!CharaParamControl.ApplyParamHip(paramCustom.paramHip,hipL)) return false;
            }
            else return false;

            DynamicBone_Ver02 hipR = ChaControl.getDynamicBoneBust(ChaInfo.DynamicBoneKind.HipR);
            if (hipR != null)
            {
#if DEBUG
                BreastPhysicsController.Logger.LogDebug("ApplyParamHip to HipR");
#endif
                if (!CharaParamControl.ApplyParamHip(paramCustom.paramHip, hipR)) return false;
            }
            else return false;

            return true;
        }

        public bool LoadParamFromChara(ChaFileDefine.CoordinateType coordinate, ParamsKind paramsKind)
        {
            if (paramCustom == null) return false;

            changedInfo.SetInfo(coordinate,
                                paramsKind,
                                false, true, true);

            switch (paramsKind)
            {
                case ParamsKind.Naked:
                    return paramCustom.paramBustNaked.LoadParamsFromCharacter(this);
                case ParamsKind.Bra:
                    return paramCustom.paramBust[coordinate][ParamsKind.Bra].LoadParamsFromCharacter(this);
                case ParamsKind.Tops:
                    return paramCustom.paramBust[coordinate][ParamsKind.Tops].LoadParamsFromCharacter(this);
                case ParamsKind.Hip:
                    return paramCustom.paramHip.LoadParamsFromCharacter(this);
                default:
                    return false;
            }

        }

        public bool LoadParamFromCharaAll()
        {
            if (!LoadParamFromChara(ChaFileDefine.CoordinateType.School01, ParamsKind.Naked)) return false;

            foreach(ChaFileDefine.CoordinateType coordinate in Enum.GetValues(typeof(ChaFileDefine.CoordinateType)))
            {
                if (!LoadParamFromChara(coordinate, ParamsKind.Bra)) return false;
                if (!LoadParamFromChara(coordinate, ParamsKind.Tops)) return false;
            }

            if (!LoadParamFromChara(ChaFileDefine.CoordinateType.School01, ParamsKind.Hip))return false;

            changedInfo.SetInfo(ChaFileDefine.CoordinateType.School01,
                    ParamsKind.Naked,
                    true, true, true);

            return true;
        }

        public void ChangeEnabledAll(bool enabled)
        {
            foreach(ParamBustCustom bust in paramCustom.GetAllBustParameters())
            {
                bust.enabled = enabled;
            }
            paramCustom.paramHip.enabled = enabled;

            changedInfo.SetInfo(ChaFileDefine.CoordinateType.School01,
                                ParamsKind.Naked,
                                true, false, true);
        }

        public void CopyParamAllCoordinate(ChaFileDefine.CoordinateType srcCoordinate)
        {
            foreach(ChaFileDefine.CoordinateType coordinate in Enum.GetValues(typeof(ChaFileDefine.CoordinateType)))
            {
                paramCustom.paramBust[coordinate][ParamsKind.Bra].CopyParamsFrom(paramCustom.paramBust[srcCoordinate][ParamsKind.Bra]);
                paramCustom.paramBust[coordinate][ParamsKind.Tops].CopyParamsFrom(paramCustom.paramBust[srcCoordinate][ParamsKind.Tops]);
            }
            changedInfo.SetInfo(ChaFileDefine.CoordinateType.School01, ParamsKind.Naked, false, true, true);
        }

        public void CopyParamCoordinate(ChaFileDefine.CoordinateType srcCoordinate, ChaFileDefine.CoordinateType dstCoordinate)
        {
            paramCustom.paramBust[dstCoordinate][ParamsKind.Bra].CopyParamsFrom(paramCustom.paramBust[srcCoordinate][ParamsKind.Bra]);
            paramCustom.paramBust[dstCoordinate][ParamsKind.Tops].CopyParamsFrom(paramCustom.paramBust[srcCoordinate][ParamsKind.Tops]);
            changedInfo.SetInfo(ChaFileDefine.CoordinateType.School01, ParamsKind.Naked, false, true, true);
        }

        public bool SaveDefaultStatus()
        {
            if(ExtendedDataIO.SaveParamChara(this, PluginPath.defaultParamPath))
            {
                defaultParam = paramCustom.Clone();
                return true;
            }
            return false;

        }

        public bool LoadDefaultStatus(bool forceControllerEnabled)
        {
            if (defaultParam == null)
            {
                if (!ExtendedDataIO.LoadExtendedData(out defaultParam, PluginPath.defaultParamPath))
                {
                    return false;
                }
            }
            paramCustom = defaultParam.Clone();
            if(forceControllerEnabled)
            {
                Enabled = true;
            }
            changedInfo.SetInfo(ChaFileDefine.CoordinateType.School01,
                                                            ParamsKind.Naked, true, true, true);


            return true;
        }

    }
}
