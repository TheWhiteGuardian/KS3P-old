using System;
using System.IO;
using UnityEngine;
using KSP.UI.Screens;
using System.Collections.Generic;
using KS3P.Shaders;

[KSPAddon(KSPAddon.Startup.Instantly, true)]
public class KS3P_Editor : MonoBehaviour
{
    private KS3P.Core.KS3P core;
    private List<GameScenes> scenes;
    private bool initialized = false;

    private PostProcessingProfile profile;

    private AntialiasingModel.FxaaSettings fxaaSettings;
    private AntialiasingModel.TaaSettings taaSettings;

    private AmbientOcclusionModel.Settings occlusionSettings;

    private BloomModel.BloomSettings bloomSettings;
    private BloomModel.LensDirtSettings dirtSettings;

    private ChromaticAberrationModel.Settings abberationSettings;

    private ColorGradingModel.BasicSettings cgBasicSettings;
    private ColorGradingModel.ChannelMixerSettings cgMixerSettings;
    private ColorGradingModel.CurvesSettings cgCurvesSettings;
    private ColorGradingModel.LinearWheelsSettings cgLinearSettings;
    private ColorGradingModel.LogWheelsSettings cgLogSettings;
    private ColorGradingModel.TonemappingSettings cgTonemapperSettings;

    private DepthOfFieldModel.Settings dofSettings;

    private EyeAdaptationModel.Settings eaSettings;

    private GrainModel.Settings grainSettings;

    private MotionBlurModel.Settings mbSettings;

    private UserLutModel.Settings ulSettings;

    private VignetteModel.Settings vSettings;

    private void Awake()
    {
        DontDestroyOnLoad(this);
    }
    private void Start()
    {
        if(!initialized)
        {
            core = KS3P.Core.KS3P.Main;
            scenes = core.guiButtonScenes;
            GameEvents.onGUIApplicationLauncherReady.Add(AddButton);
            GameEvents.onGUIApplicationLauncherUnreadifying.Add(RemoveButton);
            initialized = true;
        }
    }

    private void RemoveButton(GameScenes data)
    {
        
    }

    private void AddButton()
    {

    }

    void Build()
    {
        profile.ambientOcclusion.settings = occlusionSettings;
        
    }
}