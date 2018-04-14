using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using KS3P.Shaders;
using System.IO;

namespace KS3P
{
    namespace Keys
    {
        /// <summary>
        /// The base interface for all keybind managers
        /// </summary>
        public interface IKeybind
        {
            bool GetKey();
        }
        /// <summary>
        /// This derivative of IKeybind is only assigned when there is a single key bound.
        /// </summary>
        public struct KS3PSingleKeybind : IKeybind
        {
            public KeyCode keybind;
            public KS3PSingleKeybind(string text)
            {
                keybind = (KeyCode)Enum.Parse(typeof(KeyCode), text, false);
            }
            public bool GetKey()
            {
                return Input.GetKeyDown(keybind);
            }
        }
        /// <summary>
        /// This derivative of IKeybind is only assigned when there is more than one key bound
        /// </summary>
        public struct KS3PMultiKeybind : IKeybind
        {
            public KeyCode[] keybinds;
            public KS3PMultiKeybind(string[] text)
            {
                keybinds = new KeyCode[1];
                List<KeyCode> codes = new List<KeyCode>();
                foreach (string s in text)
                {
                    KeyCode parsed;
                    //Check if it is a valid parse
                    if (Data.Parser.TryParseEnum<KeyCode>(s, out parsed))
                    {
                        codes.Add(parsed);
                    }
                }
                keybinds = codes.ToArray();
            }
            public bool GetKey()
            {
                //This function assumes there is more than one bound key
                //This is safe because the keybind manager automatically assigns an instance of KS3PSingleKeybind should only one key be bound.
                //Therefore this struct, and in turn this code, is only executed when there is more than one key bound.

                bool status = Input.GetKeyDown(keybinds[0]); //Check first for keydown (so that we don't get a flip each frame). Thanks to using GetKeyDown here before entering the loop, this method will only return true for one frame.
                for (int i = 0; i < keybinds.Length; i++) //Start at one - we already checked the first keybind
                {
                    status = (status && Input.GetKey(keybinds[i])); //Status is true if the status was already true, and the key at index i is pressed this frame
                }
                return status; //This is true if all bound keys are true.
            }
        }

        public struct KS3PUnbound : IKeybind
        {
            public bool GetKey()
            {
                return false;
            }
        }
    }
    namespace Data
    {
        public struct Parser
        {
            /// <summary>
            /// Parses an enumerator. Returns true if the operation was successful.
            /// </summary>
            /// <typeparam name="T">The type of enum to parse as</typeparam>
            /// <param name="target">The string to parse into an enum</param>
            /// <param name="parsed">The parsed enumerator</param>
            /// <returns></returns>
            public static bool TryParseEnum<T>(string target, out T parsed)
            {
                try
                {
                    parsed = (T)Enum.Parse(typeof(T), target, true);
                    return true;
                }
                catch(Exception e)
                {
                    Core.KS3P.LogException(e);
                    throw e;
                }
            }
            static Vector2 Flip(Vector2 input)
            {
                return new Vector2(input.y, input.x);
            }

            public static AmbientOcclusionModel ParseAO(ConfigNode AONode, string scenename, string bundleName)
            {
                AmbientOcclusionModel model = new AmbientOcclusionModel();
                model.settings = AmbientOcclusionModel.Settings.defaultSettings;
                if (AONode == null)
                {
                    model.enabled = false;
                }
                else
                {
                    Core.KS3P.Log("Parsing Ambient Occlusion node for bundle named [" + bundleName + "], for scene [" + scenename + "].");

                    AmbientOcclusionModel.Settings settings = new AmbientOcclusionModel.Settings();

                    bool ambientOnly, downsampling, forceForwardCompatibility, highPrecision;
                    float intensity, radius;
                    int sampleCount;

                    //Ambient Only
                    if (!bool.TryParse(AONode.GetValue("Ambient_Only"), out ambientOnly))
                    {
                        Core.KS3P.LogException(new InvalidCastException("Error parsing [Ambient_Only] for module [Ambient Occlusion]! Disabling Ambient Occlusion!"));
                        model.enabled = false;
                        model.settings = AmbientOcclusionModel.Settings.defaultSettings;
                        return model;
                    }
                    else
                    {
                        settings.ambientOnly = ambientOnly;
                    }

                    //Downsampling
                    if (!bool.TryParse(AONode.GetValue("Downsampling"), out downsampling))
                    {
                        Core.KS3P.LogException(new InvalidCastException("Error parsing [Downsampling] for module [Ambient Occlusion]! Disabling Ambient Occlusion!"));
                        model.enabled = false;
                        model.settings = AmbientOcclusionModel.Settings.defaultSettings;
                        return model;
                    }
                    else
                    {
                        settings.downsampling = downsampling;
                    }

                    //Force Forward Compatibility
                    if (!bool.TryParse(AONode.GetValue("Force_Forward_Compatibility"), out forceForwardCompatibility))
                    {
                        Core.KS3P.LogException(new InvalidCastException("Error parsing [Force_Forward_Compatibility] for module [Ambient Occlusion]! Disabling Ambient Occlusion!"));
                        model.enabled = false;
                        model.settings = AmbientOcclusionModel.Settings.defaultSettings;
                        return model;
                    }
                    else
                    {
                        settings.forceForwardCompatibility = forceForwardCompatibility;
                    }

                    //highPrecision
                    if (!bool.TryParse(AONode.GetValue("High_Precision"), out highPrecision))
                    {
                        Core.KS3P.LogException(new InvalidCastException("Error parsing [High_Precision] for module [Ambient Occlusion]! Disabling Ambient Occlusion!"));
                        model.enabled = false;
                        model.settings = AmbientOcclusionModel.Settings.defaultSettings;
                        return model;
                    }
                    else
                    {
                        settings.highPrecision = highPrecision;
                    }

                    //Intensity
                    if (!float.TryParse(AONode.GetValue("Intensity"), out intensity))
                    {
                        Core.KS3P.LogException(new InvalidCastException("Error parsing [Intensity] for module [Ambient Occlusion]! Disabling Ambient Occlusion!"));
                        model.enabled = false;
                        model.settings = AmbientOcclusionModel.Settings.defaultSettings;
                        return model;
                    }
                    else
                    {
                        settings.intensity = intensity;
                    }
                    //Radius

                    if (!float.TryParse(AONode.GetValue("Radius"), out radius))
                    {
                        Core.KS3P.LogException(new InvalidCastException("Error parsing [Radius] for module [Ambient Occlusion]! Disabling Ambient Occlusion!"));
                        model.enabled = false;
                        model.settings = AmbientOcclusionModel.Settings.defaultSettings;
                        return model;
                    }
                    else
                    {
                        settings.radius = radius;
                    }

                    //Sample Count
                    if (!int.TryParse(AONode.GetValue("Sample_Count"), out sampleCount))
                    {
                        Core.KS3P.LogException(new InvalidCastException("Error parsing [Sample_Count] for module [Ambient Occlusion]! Disabling Ambient Occlusion!"));
                        model.enabled = false;
                        model.settings = AmbientOcclusionModel.Settings.defaultSettings;
                        return model;
                    }
                    else
                    {
                        //Clamp to avoid funny stuff
                        sampleCount = Mathf.Clamp(sampleCount, 0, 3);

                        //This enumerator is weird, so we gotta do this cleverly

                        if (sampleCount == 0)
                        {
                            settings.sampleCount = AmbientOcclusionModel.SampleCount.Lowest;
                        }
                        else if (sampleCount == 1)
                        {
                            settings.sampleCount = AmbientOcclusionModel.SampleCount.Low;
                        }
                        else if (sampleCount == 2)
                        {
                            settings.sampleCount = AmbientOcclusionModel.SampleCount.Medium;
                        }
                        else
                        {
                            settings.sampleCount = AmbientOcclusionModel.SampleCount.High;
                        }
                    }

                    //We're done here.
                    model.enabled = true;
                    model.settings = settings;
                }
                return model;
            }

            public static AntialiasingModel ParseAA(ConfigNode AANode, string scenename, string bundleName)
            {
                AntialiasingModel model = new AntialiasingModel();
                model.settings = AntialiasingModel.Settings.defaultSettings;
                if(AANode == null)
                {
                    model.enabled = false;
                    return model;
                }
                else
                {
                    Core.KS3P.Log("Parsing Anti-Aliasing node for bundle named [" + bundleName + "], for scene [" + scenename + "].");
                    AntialiasingModel.Settings settings = new AntialiasingModel.Settings();
                    //Because we can use either FXAA or TAA, we branch here
                    string mode = AANode.GetValue("Mode");
                    //Do we use FXAA?
                    if (mode == "FXAA")
                    {
                        settings.method = AntialiasingModel.Method.Fxaa;
                        int quality;

                        //The FXAA preset
                        if (!int.TryParse(AANode.GetValue("Quality"), out quality))
                        {
                            //If we reach this code, an exception occured during the conversion
                            Core.KS3P.LogException(new InvalidCastException("Error parsing [Quality] for module [Anti-Aliasing]! Disabling Anti-Aliasing!"));
                            model.enabled = false;
                            return model;
                        }
                        else
                        {
                            settings.fxaaSettings.preset = (AntialiasingModel.FxaaPreset)Mathf.Clamp(quality, 0, 4);
                        }

                        //We're done here.
                        model.settings = settings;
                        model.enabled = true;
                        return model;
                    }
                    //Do we use TAA?
                    else if (mode == "TAA")
                    {
                        Core.KS3P.Log("Warning! Temporal Anti-Aliasing may not work correctly with MSAA enabled!");
                        settings.method = AntialiasingModel.Method.Taa;
                        float JitterSpread, MotionBlending, StationaryBlending, Sharpen;

                        //Succeeded becomes true, if an exception occurs it will become false.
                        model.enabled = false;

                        //Jitter Spread
                        if (!float.TryParse(AANode.GetValue("Jitter"), out JitterSpread))
                        {
                            Core.KS3P.LogException(new InvalidCastException("Error parsing [Jitter] for module [Anti-Aliasing]! Disabling Anti-Aliasing!"));
                            model.enabled = false;
                            return model;
                        }
                        else
                        {
                            settings.taaSettings.jitterSpread = JitterSpread;
                        }

                        //Motion blending
                        if (!float.TryParse(AANode.GetValue("Blend_Motion"), out MotionBlending))
                        {
                            Core.KS3P.LogException(new InvalidCastException("Error parsing [Blend_Motion] for module [Anti-Aliasing]! Disabling Anti-Aliasing!"));
                            model.enabled = false;
                            return model;
                        }
                        else
                        {
                            settings.taaSettings.motionBlending = MotionBlending;
                        }

                        //Stationary blending
                        if (!float.TryParse(AANode.GetValue("Blend_Stationary"), out StationaryBlending))
                        {
                            Core.KS3P.LogException(new InvalidCastException("Error parsing [Blend_Stationary] for module [Anti-Aliasing]! Disabling Anti-Aliasing!"));
                            model.enabled = false;
                            return model;
                        }
                        else
                        {
                            settings.taaSettings.stationaryBlending = StationaryBlending;
                        }

                        //Sharpen
                        if (!float.TryParse(AANode.GetValue("Sharpen"), out Sharpen))
                        {
                            Core.KS3P.LogException(new InvalidCastException("Error parsing [Sharpen] for module [Anti-Aliasing]! Disabling Anti-Aliasing!"));
                            model.enabled = false;
                            return model;
                        }
                        else
                        {
                            settings.taaSettings.sharpen = Sharpen;
                        }

                        //We're done here.
                        model.settings = settings;
                        model.enabled = true;
                        return model;
                    }
                    //Error buffer
                    else
                    {
                        Core.KS3P.LogException(new ArgumentNullException("Could not load the Anti-Aliasing model! Use either 'FXAA' or 'TAA'!"));
                        model.enabled = false;
                        return model;
                    }
                }
            }

            public static BloomModel ParseB(ConfigNode BNode, string scenename, string bundleName, out string dirtTexPath)
            {
                BloomModel model = new BloomModel();
                model.settings = BloomModel.Settings.defaultSettings;
                if(BNode == null)
                {
                    model.enabled = false;
                    dirtTexPath = "KS3P/Textures/Null";
                    return model;
                }
                else
                {
                    Core.KS3P.Log("Parsing Bloom node for bundle named [" + bundleName + "], for scene [" + scenename + "].");
                    BloomModel.Settings settings = new BloomModel.Settings(); //The base effect settings, basically combines bloomSettings and dirtSettings
                    BloomModel.BloomSettings bloomSettings = new BloomModel.BloomSettings(); //The bloom settings
                    BloomModel.LensDirtSettings dirtSettings = new BloomModel.LensDirtSettings(); //The lens dirt settings
                    
                    #region BloomSettings
                    bool antiFlicker;
                    float intensity, radius, softKnee, threshold;

                    //Anti-flicker
                    if (!bool.TryParse(BNode.GetValue("Anti_Flicker"), out antiFlicker))
                    {
                        Core.KS3P.LogException(new InvalidCastException("Error parsing [Anti_Flicker] for module [Bloom]! Disabling Bloom!"));
                        model.enabled = false;
                        dirtTexPath = "KS3P/Textures/Null";
                        return model;
                    }
                    else
                    {
                        bloomSettings.antiFlicker = antiFlicker;
                    }

                    //Intensity
                    if (!float.TryParse(BNode.GetValue("Intensity"), out intensity))
                    {
                        Core.KS3P.LogException(new InvalidCastException("Error parsing [Intensity] for module [Bloom]! Disabling Bloom!"));
                        model.enabled = false;
                        dirtTexPath = "KS3P/Textures/Null";
                        return model;
                    }
                    else
                    {
                        bloomSettings.intensity = intensity;
                    }

                    //Radius
                    if (!float.TryParse(BNode.GetValue("Radius"), out radius))
                    {
                        Core.KS3P.LogException(new InvalidCastException("Error parsing [Radius] for module [Bloom]! Disabling Bloom!"));
                        model.enabled = false;
                        dirtTexPath = "KS3P/Textures/Null";
                        return model;
                    }
                    else
                    {
                        bloomSettings.radius = radius;
                    }

                    //Soft_Knee
                    if (!float.TryParse(BNode.GetValue("Soft_Knee"), out softKnee))
                    {
                        Core.KS3P.LogException(new InvalidCastException("Error parsing [Soft_Knee] for module [Bloom]! Disabling Bloom!"));
                        model.enabled = false;
                        dirtTexPath = "KS3P/Textures/Null";
                        return model;
                    }
                    else
                    {
                        bloomSettings.softKnee = softKnee;
                    }

                    //Threshold
                    if (!float.TryParse(BNode.GetValue("Threshold"), out threshold))
                    {
                        Core.KS3P.LogException(new InvalidCastException("Error parsing [Threshold] for module [Bloom]! Disabling Bloom!"));
                        model.enabled = false;
                        dirtTexPath = "KS3P/Textures/Null";
                        return model;
                    }
                    else
                    {
                        bloomSettings.threshold = threshold;
                    }
                    #endregion

                    #region DirtSettings
                    float dirtIntensity;
                    bool dirtEnabled;

                    //Texture2D has no TryParse, we'll have to do this manually

                    dirtEnabled = bool.Parse(BNode.GetValue("Dirt_Enabled"));

                    if (dirtEnabled)
                    {
                        try
                        {
                            //Try parsing
                            dirtTexPath = BNode.GetValue("Dirt_Tex");
                        }
                        //If parse failed
                        catch
                        {
                            Core.KS3P.LogException(new InvalidCastException("Error parsing [Dirt_Tex] for module [Bloom]! Disabling Bloom!"));
                            model.enabled = false;
                            dirtTexPath = "KS3P/Textures/Null";
                            return model;
                        }
                        Core.KS3P.Log("Dirt texture set to [" + BNode.GetValue("Dirt_Tex") + "].");
                        //dirtIntensity
                        if (!float.TryParse(BNode.GetValue("Dirt_Intensity"), out dirtIntensity))
                        {
                            Core.KS3P.LogException(new InvalidCastException("Error parsing [Dirt_Intensity] for module [Bloom]! Disabling Bloom!"));
                            model.enabled = false;
                            dirtTexPath = "KS3P/Textures/Null";
                            return model;
                        }
                        else
                        {
                            dirtSettings.intensity = dirtIntensity;
                        }
                    }
                    else
                    {
                        Core.KS3P.Log("Lens dirt preference was disable. Disabling lens dirt...");
                        dirtSettings = BloomModel.LensDirtSettings.defaultSettings; //Else we set to default...
                        dirtTexPath = "KS3P/Textures/Null";
                        dirtSettings.intensity = 1f; //And disable the effect through intensity
                    }

                    #endregion

                    //We're done here, let's wrap it up.
                    settings.bloom = bloomSettings;
                    settings.lensDirt = dirtSettings;
                    model.settings = settings;
                    model.enabled = true;
                    return model;
                }
            }

            public static ChromaticAberrationModel ParseCA(ConfigNode CANode, string scenename, string bundleName, out string spectrumPath)
            {
                ChromaticAberrationModel model = new ChromaticAberrationModel();
                model.settings = ChromaticAberrationModel.Settings.defaultSettings;
                if(CANode == null)
                {
                    model.enabled = false;
                    spectrumPath = "KS3P/Textures/Null";
                    return model;
                }
                else
                {
                    Core.KS3P.Log("Parsing Chromatic Abberation node for bundle named [" + bundleName + "], for scene [" + scenename + "].");
                    //Make a new settings instance to edit
                    ChromaticAberrationModel.Settings settings = new ChromaticAberrationModel.Settings();
                    
                    float intensity;
                    //Texture2D has no TryParse, we'll have to do this manually
                    try
                    {
                        //Try parsing
                       spectrumPath = CANode.GetValue("Spectral_Tex");
                    }
                    //If parse failed
                    catch
                    {
                        Core.KS3P.LogException(new InvalidCastException("Error parsing [Spectral_Tex] for module [Chromatic Abberation]! Disabling Chromatic Abberation!"));
                        model.enabled = false;
                        spectrumPath = "KS3P/Textures/Null";
                        return model;
                    }

                    if (!float.TryParse(CANode.GetValue("Intensity"), out intensity))
                    {
                        Core.KS3P.LogException(new InvalidCastException("Error parsing [Intensity] for module [Chromatic Abberation]! Disabling Chromatic Abberation!"));
                        model.enabled = false;
                        spectrumPath = "KS3P/Textures/Null";
                        return model;
                    }
                    else
                    {
                        settings.intensity = intensity;
                    }

                    //We're done here.
                    model.settings = settings;
                    model.enabled = true;
                    return model;
                }
            }

            public static DepthOfFieldModel ParseDOF(ConfigNode DOFNode, string scenename, string bundleName)
            {
                DepthOfFieldModel model = new DepthOfFieldModel();
                model.settings = DepthOfFieldModel.Settings.defaultSettings;
                if(DOFNode == null)
                {
                    model.enabled = false;
                    return model;
                }
                else
                {
                    Core.KS3P.Log("Parsing Depth Of Field node for bundle named [" + bundleName + "], for scene [" + scenename + "].");
                    DepthOfFieldModel.Settings settings = new DepthOfFieldModel.Settings();
                    float FocusDistance, Aperture, FocalLength;
                    bool UseCam;
                    int size;

                    if (!float.TryParse(DOFNode.GetValue("Focus_Distance"), out FocusDistance))
                    {
                        Core.KS3P.LogException(new InvalidCastException("Error parsing [Focus_Distance] for module [Depth Of Field]! Disabling Depth Of Field!"));
                        model.enabled = false;
                        return model;
                    }
                    else
                    {
                        settings.focusDistance = FocusDistance;
                    }

                    if (!float.TryParse(DOFNode.GetValue("Aperture"), out Aperture))
                    {
                        Core.KS3P.LogException(new InvalidCastException("Error parsing [Aperture] for module [Depth Of Field]! Disabling Depth Of Field!"));
                        model.enabled = false;
                        return model;
                    }
                    else
                    {
                        settings.aperture = Mathf.Clamp(Aperture, 0.1f, 32);
                    }

                    if (!bool.TryParse(DOFNode.GetValue("Use_Camera_FOV"), out UseCam))
                    {
                        Core.KS3P.LogException(new InvalidCastException("Error parsing [Use_Camera_FOV] for module [Depth Of Field]! Disabling Depth Of Field!"));
                        model.enabled = false;
                        return model;
                    }
                    else
                    {
                        settings.useCameraFov = UseCam;
                    }

                    if (!float.TryParse(DOFNode.GetValue("Focal_Length"), out FocalLength))
                    {
                        Core.KS3P.LogException(new InvalidCastException("Error parsing [Focal_Length] for module [Depth Of Field]! Disabling Depth Of Field!"));
                        model.enabled = false;
                        return model;
                    }
                    else
                    {
                        settings.focalLength = Mathf.Clamp(FocalLength, 1f, 300f);
                    }

                    if (!int.TryParse(DOFNode.GetValue("Kernel_Size"), out size))
                    {
                        Core.KS3P.LogException(new InvalidCastException("Error parsing [Kernel_Size] for module [Depth Of Field]! Disabling Depth Of Field!"));
                        model.enabled = false;
                        return model;
                    }
                    else
                    {
                        settings.kernelSize = (DepthOfFieldModel.KernelSize)Mathf.Clamp(size, 0, 3);
                    }

                    //We're done here.
                    model.settings = settings;
                    model.enabled = true;
                    return model;
                }
            }

            public static MotionBlurModel ParseMB(ConfigNode MBNode, string scenename, string bundleName)
            {
                MotionBlurModel model = new MotionBlurModel();
                model.settings = MotionBlurModel.Settings.defaultSettings;
                if(MBNode == null)
                {
                    model.enabled = false;
                    return model;
                }
                else
                {
                    Core.KS3P.Log("Parsing Motion Blur node for bundle named [" + bundleName + "], for scene [" + scenename + "].");
                    MotionBlurModel.Settings settings = new MotionBlurModel.Settings();

                    float ShutterAngle, FrameBlending;
                    int SampleCount;

                    if (!float.TryParse(MBNode.GetValue("Shutter_Angle"), out ShutterAngle))
                    {
                        Core.KS3P.LogException(new InvalidCastException("Error parsing [Shutter_Angle] for module [Motion Blur]! Disabling Motion Blur!"));
                        model.enabled = false;
                        return model;
                    }
                    else
                    {
                        settings.shutterAngle = Mathf.Clamp(ShutterAngle, 0f, 360f);
                    }

                    if (!int.TryParse(MBNode.GetValue("Sample_Count"), out SampleCount))
                    {
                        Core.KS3P.LogException(new InvalidCastException("Error parsing [Sample_Count] for module [Motion Blur]! Disabling Motion Blur!"));
                        model.enabled = false;
                        return model;
                    }
                    else
                    {
                        settings.sampleCount = Mathf.Clamp(SampleCount, 4, 32);
                    }

                    if (!float.TryParse(MBNode.GetValue("Frame_Blending"), out FrameBlending))
                    {
                        Core.KS3P.LogException(new InvalidCastException("Error parsing [Frame_Blending] for module [Motion Blur]! Disabling Motion Blur!"));
                        model.enabled = false;
                        return model;
                    }
                    else
                    {
                        settings.frameBlending = Mathf.Clamp01(FrameBlending);
                    }

                    //We're done here
                    model.settings = settings;
                    model.enabled = true;
                    return model;
                }
            }

            public static EyeAdaptationModel ParseEA(ConfigNode EANode, string scenename, string bundleName)
            {
                EyeAdaptationModel model = new EyeAdaptationModel();
                model.settings = EyeAdaptationModel.Settings.defaultSettings;
                if(EANode == null)
                {
                    model.enabled = false;
                    return model;
                }
                else
                {
                    Core.KS3P.Log("Parsing Eye Adaptation node for bundle named [" + bundleName + "], for scene [" + scenename + "].");
                    EyeAdaptationModel.Settings settings = new EyeAdaptationModel.Settings();

                    int MinLum, MaxLum, type;
                    float Min, Max, speedUp, speedDown, keyValue;
                    bool DynaKey;
                    Vector2 range;
                    if (!int.TryParse(EANode.GetValue("Luminosity_Minimum"), out MinLum))
                    {
                        Core.KS3P.LogException(new InvalidCastException("Error parsing [Luminosity_Minimum] for module [Eye Adaptation]! Disabling Eye Adaptation!"));
                        model.enabled = false;
                        return model;
                    }
                    else
                    {
                        settings.logMin = Mathf.Clamp(MinLum, -16, -1);
                    }

                    if (!int.TryParse(EANode.GetValue("Luminosity_Maximum"), out MaxLum))
                    {
                        Core.KS3P.LogException(new InvalidCastException("Error parsing [Luminosity_Maximum] for module [Eye Adaptation]! Disabling Eye Adaptation!"));
                        model.enabled = false;
                        return model;
                    }
                    else
                    {
                        settings.logMax = Mathf.Clamp(MaxLum, 1, 16);
                    }

                    //Auto exposure stuff
                    if (!float.TryParse(EANode.GetValue("Maximum_EV"), out Max))
                    {
                        Core.KS3P.LogException(new InvalidCastException("Error parsing [Maximum_EV] for module [Eye Adaptation]! Disabling Eye Adaptation!"));
                        model.enabled = false;
                        return model;
                    }
                    else
                    {
                        settings.maxLuminance = Max;
                    }

                    if (!float.TryParse(EANode.GetValue("Minimum_EV"), out Min))
                    {
                        Core.KS3P.LogException(new InvalidCastException("Error parsing [Minimum_EV] for module [Eye Adaptation]! Disabling Eye Adaptation!"));
                        model.enabled = false;
                        return model;
                    }
                    else
                    {
                        settings.minLuminance = Min;
                    }

                    if (!bool.TryParse(EANode.GetValue("Dynamic_Key_Value"), out DynaKey))
                    {
                        Core.KS3P.LogException(new InvalidCastException("Error parsing [Dynamic_Key_Value] for module [Eye Adaptation]! Disabling Eye Adaptation!"));
                        model.enabled = false;
                        return model;
                    }
                    else
                    {
                        settings.dynamicKeyValue = DynaKey;
                    }

                    if (!int.TryParse(EANode.GetValue("Type"), out type))
                    {
                        Core.KS3P.LogException(new InvalidCastException("Error parsing [Type] for module [Eye Adaptation]! Disabling Eye Adaptation!"));
                        model.enabled = false;
                        return model;
                    }
                    else
                    {
                        settings.adaptationType = (EyeAdaptationModel.EyeAdaptationType)Mathf.Clamp01(type);
                    }

                    if (!float.TryParse(EANode.GetValue("Speed_Up"), out speedUp))
                    {
                        Core.KS3P.LogException(new InvalidCastException("Error parsing [Speed_Up] for module [Eye Adaptation]! Disabling Eye Adaptation!"));
                        model.enabled = false;
                        return model;
                    }
                    else
                    {
                        settings.speedUp = speedUp;
                    }

                    if (!float.TryParse(EANode.GetValue("Speed_Down"), out speedDown))
                    {
                        Core.KS3P.LogException(new InvalidCastException("Error parsing [Speed_Down] for module [Eye Adaptation]! Disabling Eye Adaptation!"));
                        model.enabled = false;
                        return model;
                    }
                    else
                    {
                        settings.speedDown = speedDown;
                    }


                    //The sliders
                    try
                    {
                        range = ConfigNode.ParseVector2(EANode.GetValue("Range"));
                        if (range.x > range.y)
                        {
                            Flip(range);
                        }
                        settings.lowPercent = range.x;
                        settings.highPercent = range.y;
                    }
                    catch
                    {
                        Core.KS3P.LogException(new InvalidCastException("Error parsing [Range] for module [Eye Adaptation]! Disabling Eye Adaptation!"));
                        model.enabled = false;
                        return model;
                    }

                    if (!float.TryParse(EANode.GetValue("Key_Value"), out keyValue))
                    {
                        Core.KS3P.LogException(new InvalidCastException("Error parsing [Key_Value] for module [Eye Adaptation]! Disabling Eye Adaptation!"));
                        model.enabled = false;
                        return model;
                    }
                    else
                    {
                        settings.keyValue = keyValue;
                    }

                    //We're done here

                    model.settings = settings;
                    model.enabled = true;
                    return model;
                }
            }

            public static GrainModel ParseG(ConfigNode GNode, string scenename, string bundleName)
            {
                GrainModel model = new GrainModel();
                model.settings = GrainModel.Settings.defaultSettings;
                if(GNode == null)
                {
                    model.enabled = false;
                    return model;
                }
                else
                {
                    Core.KS3P.Log("Parsing Grain node for bundle named [" + bundleName + "], for scene [" + scenename + "].");
                    GrainModel.Settings settings = new GrainModel.Settings();

                    bool colored;
                    float intensity, luminanceContribution, size;

                    if (!bool.TryParse(GNode.GetValue("Colored"), out colored))
                    {
                        Core.KS3P.LogException(new InvalidCastException("Error parsing [Colored] for module [Grain]! Disabling Grain!"));
                        model.enabled = false;
                        return model;
                    }
                    else
                    {
                        settings.colored = colored;
                    }

                    if (!float.TryParse(GNode.GetValue("Intensity"), out intensity))
                    {
                        Core.KS3P.LogException(new InvalidCastException("Error parsing [Intensity] for module [Grain]! Disabling Grain!"));
                        model.enabled = false;
                        return model;
                    }
                    else
                    {
                        settings.intensity = intensity;
                    }

                    if (!float.TryParse(GNode.GetValue("Luminance_Contribution"), out luminanceContribution))
                    {
                        Core.KS3P.LogException(new InvalidCastException("Error parsing [Luminance_Contribution] for module [Grain]! Disabling Grain!"));
                        model.enabled = false;
                        return model;
                    }
                    else
                    {
                        settings.luminanceContribution = luminanceContribution;
                    }

                    if (!float.TryParse(GNode.GetValue("Size"), out size))
                    {
                        Core.KS3P.LogException(new InvalidCastException("Error parsing [Size] for module [Grain]! Disabling Grain!"));
                        model.enabled = false;
                        return model;
                    }
                    else
                    {
                        settings.size = size;
                    }

                    //We're done here.
                    model.settings = settings;
                    model.enabled = true;
                    return model;
                }
            }

            public static UserLutModel ParseUL(ConfigNode ULNode, string scenename, string bundleName, out string lutTex)
            {
                UserLutModel model = new UserLutModel();
                model.settings = UserLutModel.Settings.defaultSettings;
                if (ULNode == null)
                {
                    model.enabled = false;
                    lutTex = "KS3P/Textures/Null";
                    return model;
                }
                else
                {
                    Core.KS3P.Log("Parsing User Lut node for bundle named [" + bundleName + "], for scene [" + scenename + "].");
                    UserLutModel.Settings settings = new UserLutModel.Settings();

                    float contribution;

                    try
                    {
                        lutTex = ULNode.GetValue("Lut_Texture");
                    }
                    //Check if exception
                    catch
                    {
                        Core.KS3P.LogException(new InvalidCastException("Error parsing [Lut_Texture] for module [User Lut]! Disabling User Lut!"));
                        model.enabled = false;
                        lutTex = "KS3P/Textures/Null";
                        return model;
                    }
                    //Check if null
                    if (lutTex == null)
                    {
                        Core.KS3P.LogException(new ArgumentNullException("Error finding texture [Lut_Texture] for module [User Lut]! Disabling User Lut!"));
                        model.enabled = false;
                        lutTex = "KS3P/Textures/Null";
                        return model;
                    }

                    if (!float.TryParse(ULNode.GetValue("Contribution"), out contribution))
                    {
                        Core.KS3P.LogException(new InvalidCastException("Error parsing [Contribution] for module [User Lut]! Disabling User Lut!"));
                        model.enabled = false;
                        lutTex = "KS3P/Textures/Null";
                        return model;
                    }
                    else
                    {
                        settings.contribution = contribution;
                    }

                    //We're done here.
                    model.settings = settings;
                    model.enabled = true;
                    return model;
                }
            }

            public static VignetteModel ParseV(ConfigNode VNode, string scenename, string bundleName, out string maskTex)
            {
                VignetteModel model = new VignetteModel();
                model.settings = VignetteModel.Settings.defaultSettings;
                if(VNode == null)
                {
                    model.enabled = false;
                    maskTex = "KS3P/Textures/Null";
                    return model;
                }
                else
                {
                    Core.KS3P.Log("Parsing Vignette node for bundle named [" + bundleName + "], for scene [" + scenename + "].");
                    VignetteModel.Settings settings = new VignetteModel.Settings();
                    Vector2 center;
                    Color color;
                    float intensity, opacity, roundness, smoothness;
                    int mode;
                    bool rounded;

                    try
                    {
                        center = ConfigNode.ParseVector2(VNode.GetValue("Center"));
                    }
                    catch
                    {
                        Core.KS3P.LogException(new InvalidCastException("Error parsing [Center] for module [Vignette]! Disabling Vignette!"));
                        model.enabled = false;
                        maskTex = "KS3P/Textures/Null";
                        return model;
                    }
                    //If we reach this code, we didn't return because exception
                    settings.center = center;

                    try
                    {
                        color = ConfigNode.ParseColor(VNode.GetValue("Color"));
                    }
                    catch
                    {
                        Core.KS3P.LogException(new InvalidCastException("Error parsing [Color] for module [Vignette]! Disabling Vignette!"));
                        model.enabled = false;
                        maskTex = "KS3P/Textures/Null";
                        return model;
                    }
                    settings.color = color;

                    if (!float.TryParse(VNode.GetValue("Intensity"), out intensity))
                    {
                        Core.KS3P.LogException(new InvalidCastException("Error parsing [Intensity] for module [Vignette]! Disabling Vignette!"));
                        model.enabled = false;
                        maskTex = "KS3P/Textures/Null";
                        return model;
                    }
                    else
                    {
                        settings.intensity = intensity;
                    }

                    if (!float.TryParse(VNode.GetValue("Opacity"), out opacity))
                    {
                        Core.KS3P.LogException(new InvalidCastException("Error parsing [Opacity] for module [Vignette]! Disabling Vignette!"));
                        model.enabled = false;
                        maskTex = "KS3P/Textures/Null";
                        return model;
                    }
                    else
                    {
                        settings.opacity = opacity;
                    }

                    if (!float.TryParse(VNode.GetValue("Roundness"), out roundness))
                    {
                        Core.KS3P.LogException(new InvalidCastException("Error parsing [Roundness] for module [Vignette]! Disabling Vignette!"));
                        model.enabled = false;
                        maskTex = "KS3P/Textures/Null";
                        return model;
                    }
                    else
                    {
                        settings.roundness = roundness;
                    }

                    if (!float.TryParse(VNode.GetValue("Smoothness"), out smoothness))
                    {
                        Core.KS3P.LogException(new InvalidCastException("Error parsing [Smoothness] for module [Vignette]! Disabling Vignette!"));
                        model.enabled = false;
                        maskTex = "KS3P/Textures/Null";
                        return model;
                    }
                    else
                    {
                        settings.smoothness = smoothness;
                    }

                    if (!bool.TryParse(VNode.GetValue("Rounded"), out rounded))
                    {
                        Core.KS3P.LogException(new InvalidCastException("Error parsing [Rounded] for module [Vignette]! Disabling Vignette!"));
                        model.enabled = false;
                        maskTex = "KS3P/Textures/Null";
                        return model;
                    }
                    else
                    {
                        settings.rounded = rounded;
                    }

                    try
                    {
                        maskTex = VNode.GetValue("Mask");
                    }
                    catch
                    {
                        Core.KS3P.LogException(new InvalidCastException("Error parsing [Mask] for module [Vignette]! Disabling Vignette!"));
                        model.enabled = false;
                        maskTex = "KS3P/Textures/Null";
                        return model;
                    }

                    if (!int.TryParse(VNode.GetValue("Mode"), out mode))
                    {
                        Debug.LogException(new InvalidCastException("[KSP_PostProcessing]: Error parsing [Mode] for module [Vignette]! Disabling Vignette!"));
                        model.enabled = false;
                        maskTex = "KS3P/Textures/Null";
                        return model;
                    }
                    else
                    {
                        //Use integer index to grab enumerator value
                        settings.mode = (VignetteModel.Mode)Mathf.Clamp01(mode);
                    }

                    //We're done here.
                    model.settings = settings;
                    model.enabled = true;
                    return model;
                }
            }

            public static DitheringModel ParseD(ConfigNode DNode, string scenename, string bundleName)
            {
                DitheringModel model = new DitheringModel();
                model.settings = DitheringModel.Settings.defaultSettings;
                if(DNode == null)
                {
                    model.enabled = false;
                }
                else
                {
                    Core.KS3P.Log("Parsing Dithering node for bundle named [" + bundleName + "], for scene [" + scenename + "].");
                    model.enabled = true;
                }
                return model;
            }

            public static ColorGradingModel ParseCG(ConfigNode CGNode, string scenename, string bundleName)
            {
                ColorGradingModel model = new ColorGradingModel();
                model.settings = ColorGradingModel.Settings.defaultSettings;
                if(CGNode == null)
                {
                    model.enabled = false;
                    return model;
                }
                else
                {
                    Core.KS3P.Log("Parsing Color Grading node for bundle named [" + bundleName + "], for scene [" + scenename + "].");
                    ColorGradingModel.Settings settings = new ColorGradingModel.Settings();

                    string preset = CGNode.GetValue("Preset");

                    //Are we going for ACES filmic?
                    if (preset == "ACES")
                    {
                        //ACES filmic tonemapper preset setup
                        Core.KS3P.Log("[ColorGrading]: Setting CG to ACES preset.");
                        settings.tonemapping.tonemapper = ColorGradingModel.Tonemapper.ACES;
                        settings.basic = ColorGradingModel.BasicSettings.defaultSettings;
                        settings.channelMixer = ColorGradingModel.ChannelMixerSettings.defaultSettings;
                        settings.colorWheels = ColorGradingModel.ColorWheelsSettings.defaultSettings;
                        settings.curves = ColorGradingModel.CurvesSettings.defaultSettings;
                        settings.tonemapping = ColorGradingModel.TonemappingSettings.defaultSettings;
                        settings.tonemapping.tonemapper = ColorGradingModel.Tonemapper.ACES;
                        model.settings = settings;
                        model.enabled = true;
                        return model;
                    }
                    //No? Neutral then maybe?
                    else if (preset == "Neutral")
                    {
                        //Neutral tonemapper preset setup
                        Core.KS3P.Log("[ColorGrading]: Setting CG to Neutral preset.");
                        settings.tonemapping.tonemapper = ColorGradingModel.Tonemapper.Neutral;
                        settings.basic = ColorGradingModel.BasicSettings.defaultSettings;
                        settings.channelMixer = ColorGradingModel.ChannelMixerSettings.defaultSettings;
                        settings.colorWheels = ColorGradingModel.ColorWheelsSettings.defaultSettings;
                        settings.curves = ColorGradingModel.CurvesSettings.defaultSettings;
                        settings.tonemapping = ColorGradingModel.TonemappingSettings.defaultSettings;
                        model.settings = settings;
                        model.enabled = true;
                        return model;
                    }
                    //We either ignored preset or want no preset. Noted.
                    else
                    {
                        settings.tonemapping.tonemapper = ColorGradingModel.Tonemapper.None;
                        ColorGradingModel.BasicSettings baseSettings;
                        ConfigNode BaseNode = CGNode.GetNode("Base");
                        bool succeeded;
                        baseSettings = CG_ProcessBaseSettings(BaseNode, out succeeded);
                        //If the previous operation failed, we're not continuing this.
                        if (!succeeded)
                        {
                            model.enabled = false;
                            return model;
                        }
                        ColorGradingModel.ChannelMixerSettings channelSettings = new ColorGradingModel.ChannelMixerSettings();
                        ConfigNode mixerNode = CGNode.GetNode("ColorMixer");
                        channelSettings = CG_ProcessMixerSettings(mixerNode, out succeeded);
                        if (!succeeded)
                        {
                            model.enabled = false;
                            return model;
                        }
                        ColorGradingModel.ColorWheelsSettings colorWheelSettings = new ColorGradingModel.ColorWheelsSettings();
                        ConfigNode wheelNode = CGNode.GetNode("ColorWheels");
                        colorWheelSettings = CG_ProcessColorWheels(wheelNode, out succeeded);
                        if (!succeeded) //Has the last operation thrown an exception?
                        {
                            model.enabled = false;
                            return model; //Then abort here.
                        }
                        ColorGradingModel.CurvesSettings curveSettings = new ColorGradingModel.CurvesSettings();
                        ConfigNode curveNode = CGNode.GetNode("ColorCurves");
                        curveSettings = CG_ProcessCurveSettings(curveNode, out succeeded);
                        if (!succeeded)
                        {
                            model.enabled = false;
                            return model; //Abort, abort!
                        }
                        ColorGradingModel.TonemappingSettings tonemapperSettings = new ColorGradingModel.TonemappingSettings();
                        ConfigNode mapperNode = CGNode.GetNode("Tonemapper");
                        tonemapperSettings = CG_ProcessTonemapperSettings(mapperNode, out succeeded);
                        if (!succeeded)
                        {
                            model.enabled = false;
                            return model;
                        }

                        settings.basic = baseSettings;
                        settings.channelMixer = channelSettings;
                        settings.colorWheels = colorWheelSettings;
                        settings.curves = curveSettings;
                        settings.tonemapping = tonemapperSettings;

                        model.settings = settings;
                        model.enabled = true;
                        return model;
                    }
                }
            }

            #region ColorGradingProcessors
            //Processes the basic settings of a color grading model
            static ColorGradingModel.BasicSettings CG_ProcessBaseSettings(ConfigNode baseNode, out bool succeeded)
            {
                ColorGradingModel.BasicSettings settings = new ColorGradingModel.BasicSettings();
                settings = ColorGradingModel.BasicSettings.defaultSettings; //Init default settings as a framework

                if (baseNode == null)
                {
                    //No node? Return detault.
                    succeeded = true;
                    return settings;
                }

                float contrast, hueShift, postExposure, saturation, temperature, tint;
                succeeded = true;
                if (!float.TryParse(baseNode.GetValue("Contrast"), out contrast))
                {
                    Core.KS3P.LogException(new InvalidCastException("Error parsing [Contrast] for module [ColorGrading_Base]! Disabling Color Grading!"));
                    succeeded = false;
                    return settings;
                }
                else
                {
                    settings.contrast = Mathf.Clamp(contrast, 0f, 2f);
                }

                if (!float.TryParse(baseNode.GetValue("Hue_Shift"), out hueShift))
                {
                    Core.KS3P.LogException(new InvalidCastException("Error parsing [Hue_Shift] for module [ColorGrading_Base]! Disabling Color Grading!"));
                    succeeded = false;
                    return settings;
                }
                else
                {
                    settings.hueShift = Mathf.Clamp(hueShift, -180f, 180f);
                }

                if (!float.TryParse(baseNode.GetValue("Post_Exposure"), out postExposure))
                {
                    Core.KS3P.LogException(new InvalidCastException("Error parsing [Post_Exposure] for module [ColorGrading_Base]! Disabling Color Grading!"));
                    succeeded = false;
                    return settings;
                }
                else
                {
                    settings.postExposure = postExposure;
                }

                if (!float.TryParse(baseNode.GetValue("Saturation"), out saturation))
                {
                    Core.KS3P.LogException(new InvalidCastException("Error parsing [Saturation] for module [ColorGrading_Base]! Disabling Color Grading!"));
                    succeeded = false;
                    return settings;
                }
                else
                {
                    settings.saturation = Mathf.Clamp(saturation, 0f, 2f);
                }

                if (!float.TryParse(baseNode.GetValue("Tint"), out tint))
                {
                    Core.KS3P.LogException(new InvalidCastException("Error parsing [Tint] for module [ColorGrading_Base]! Disabling Color Grading!"));
                    succeeded = false;
                    return settings;
                }
                else
                {
                    settings.tint = Mathf.Clamp(tint, -100f, 100f);
                }

                if (!float.TryParse(baseNode.GetValue("Temperature"), out temperature))
                {
                    Core.KS3P.LogException(new InvalidCastException("Error parsing [Temperature] for module [ColorGrading_Base]! Disabling Color Grading!"));
                    succeeded = false;
                    return settings;
                }
                else
                {
                    settings.temperature = Mathf.Clamp(temperature, -100f, 100f);
                }

                //We're done with this part.
                return settings;
            }

            //Processes the channel mixer settings of a color grading model
            static ColorGradingModel.ChannelMixerSettings CG_ProcessMixerSettings(ConfigNode mixNode, out bool succeeded)
            {
                ColorGradingModel.ChannelMixerSettings settings = new ColorGradingModel.ChannelMixerSettings();
                settings = ColorGradingModel.ChannelMixerSettings.defaultSettings; //Init default settings as a framework
                succeeded = true;

                if (mixNode == null)
                {
                    return settings;
                }

                Vector3 r, g, b;


                try
                {
                    r = ConfigNode.ParseVector3(mixNode.GetValue("Red"));
                    settings.red = r;
                }
                catch (InvalidCastException)
                {
                    Core.KS3P.LogException(new InvalidCastException("Error parsing [Red] for module [ColorGrading_Mixer]! Disabling Color Grading!"));
                    succeeded = false;
                    return settings;
                }

                try
                {
                    g = ConfigNode.ParseVector3(mixNode.GetValue("Green"));
                    settings.green = g;
                }
                catch (InvalidCastException)
                {
                    Core.KS3P.LogException(new InvalidCastException("Error parsing [Green] for module [ColorGrading_Mixer]! Disabling Color Grading!"));
                    succeeded = false;
                    return settings;
                }

                try
                {
                    b = ConfigNode.ParseVector3(mixNode.GetValue("Blue"));
                    settings.blue = b;
                }
                catch (InvalidCastException)
                {
                    Core.KS3P.LogException(new InvalidCastException("Error parsing [Blue] for module [ColorGrading_Mixer]! Disabling Color Grading!"));
                    succeeded = false;
                    return settings;
                }
                return settings;
            }

            //Processes the color wheel settings of a color grading model
            static ColorGradingModel.ColorWheelsSettings CG_ProcessColorWheels(ConfigNode wNode, out bool succeeded)
            {
                ColorGradingModel.ColorWheelsSettings settings = new ColorGradingModel.ColorWheelsSettings();
                settings = ColorGradingModel.ColorWheelsSettings.defaultSettings;

                if (wNode == null)
                {
                    //if there is no node, return default.
                    succeeded = true;
                    return settings;
                }
                string mode = wNode.GetValue("WheelMode");
                succeeded = true; //Overridden if exception occurs
                if (mode == "Linear")
                {
                    settings.mode = ColorGradingModel.ColorWheelMode.Linear;
                    #region LinearProcessor
                    ColorGradingModel.LinearWheelsSettings wheelSettings = new ColorGradingModel.LinearWheelsSettings();
                    try
                    {
                        wheelSettings.gain = ConfigNode.ParseColor(wNode.GetValue("Gain"));
                    }
                    catch (InvalidCastException)
                    {
                        Core.KS3P.LogException(new InvalidCastException("Error parsing [Gain_Linear] for module [ColorGrading_ColorWheels]! Disabling Color Grading!"));
                        succeeded = false;
                        return settings;
                    }
                    try
                    {
                        wheelSettings.gamma = ConfigNode.ParseColor(wNode.GetValue("Gamma"));
                    }
                    catch (InvalidCastException)
                    {
                        Core.KS3P.LogException(new InvalidCastException("Error parsing [Gamma_Linear] for module [ColorGrading_ColorWheels]! Disabling Color Grading!"));
                        succeeded = false;
                        return settings;
                    }
                    try
                    {
                        wheelSettings.lift = ConfigNode.ParseColor(wNode.GetValue("Lift"));
                    }
                    catch (InvalidCastException)
                    {
                        Core.KS3P.LogException(new InvalidCastException("Error parsing [] for module [ColorGrading_ColorWheels]! Disabling Color Grading!"));
                        succeeded = false;
                        return settings;
                    }
                    #endregion
                    settings.linear = wheelSettings;
                    settings.log = new ColorGradingModel.LogWheelsSettings(); //This one is empty.
                    return settings;
                }
                else if (mode == "Logarithmic")
                {
                    settings.mode = ColorGradingModel.ColorWheelMode.Log;
                    #region LogProcessor
                    ColorGradingModel.LogWheelsSettings wheelSettings = new ColorGradingModel.LogWheelsSettings();

                    try
                    {
                        wheelSettings.offset = ConfigNode.ParseColor(wNode.GetValue("Offset"));
                    }
                    catch (InvalidCastException)
                    {
                        Core.KS3P.LogException(new InvalidCastException("Error parsing [Offset_Logarithmic] for module [ColorGrading_ColorWheels]! Disabling Color Grading!"));
                        succeeded = false;
                        return settings;
                    }

                    try
                    {
                        wheelSettings.power = ConfigNode.ParseColor(wNode.GetValue("Power"));
                    }
                    catch (InvalidCastException)
                    {
                        Core.KS3P.LogException(new InvalidCastException("Error parsing [Power_Logarithmic] for module [ColorGrading_ColorWheels]! Disabling Color Grading!"));
                        succeeded = false;
                        return settings;
                    }

                    try
                    {
                        wheelSettings.slope = ConfigNode.ParseColor(wNode.GetValue("Slope"));
                    }
                    catch (InvalidCastException)
                    {
                        Core.KS3P.LogException(new InvalidCastException("Error parsing [Slope_Logarithmic] for module [ColorGrading_ColorWheels]! Disabling Color Grading!"));
                        succeeded = false;
                        return settings;
                    }
                    #endregion
                    settings.log = wheelSettings;
                    settings.linear = new ColorGradingModel.LinearWheelsSettings(); //This one is empty.
                    return settings;
                }
                else
                {
                    Core.KS3P.LogException(new InvalidCastException("Error parsing [WheelMode] for module [ColorGrading_ColorWheels]! Disabling Color Grading!"));
                    succeeded = false;
                    return settings;
                }
            }

            //Processes the color curve settings of a color grading model
            static ColorGradingModel.CurvesSettings CG_ProcessCurveSettings(ConfigNode CNode, out bool succeeded)
            {
                ColorGradingModel.CurvesSettings settings = new ColorGradingModel.CurvesSettings(); //Clean slate.
                settings = ColorGradingModel.CurvesSettings.defaultSettings;

                if (CNode == null)
                {
                    succeeded = true;
                    return settings;
                }


                ColorGradingCurve blue, green, hueVhue, hueVsat, lumVsat, master, red, satVsat;
                ConfigNode Blue = CNode.GetNode("Blue");
                ConfigNode Green = CNode.GetNode("Green");
                ConfigNode hVh = CNode.GetNode("HueVersusHue");
                ConfigNode hVs = CNode.GetNode("HueVersusSaturation");
                ConfigNode lVs = CNode.GetNode("LuminosityVersusSaturation");
                ConfigNode Master = CNode.GetNode("Master");
                ConfigNode Red = CNode.GetNode("Red");
                ConfigNode sVs = CNode.GetNode("SaturationVersusSaturation");

                succeeded = true;

                if (!TryParseColorCurve(Blue, out blue))
                {
                    Core.KS3P.LogException(new InvalidCastException("Error parsing curve [Blue] for module [ColorGrading_Curves]! Disabling Color Grading!"));
                    succeeded = false;
                    return settings;
                }
                else
                {
                    settings.blue = blue;
                }

                if (!TryParseColorCurve(Green, out green))
                {
                    Core.KS3P.LogException(new InvalidCastException("Error parsing curve [Green] for module [ColorGrading_Curves]! Disabling Color Grading!"));
                    succeeded = false;
                    return settings;
                }
                else
                {
                    settings.green = green;
                }

                if (!TryParseColorCurve(hVh, out hueVhue))
                {
                    Core.KS3P.LogException(new InvalidCastException("Error parsing curve [Hue_Versus_Hue] for module [ColorGrading_Curves]! Disabling Color Grading!"));
                    succeeded = false;
                    return settings;
                }
                else
                {
                    settings.hueVShue = hueVhue;
                }

                if (!TryParseColorCurve(hVs, out hueVsat))
                {
                    Core.KS3P.LogException(new InvalidCastException("Error parsing curve [Hue_Versus_Saturation] for module [ColorGrading_Curves]! Disabling Color Grading!"));
                    succeeded = false;
                    return settings;
                }
                else
                {
                    settings.hueVSsat = hueVsat;
                }

                if (!TryParseColorCurve(lVs, out lumVsat))
                {
                    Core.KS3P.LogException(new InvalidCastException("Error parsing curve [Luminosity_Versus_Saturation] for module [ColorGrading_Curves]! Disabling Color Grading!"));
                    succeeded = false;
                    return settings;
                }
                else
                {
                    settings.lumVSsat = lumVsat;
                }

                if (!TryParseColorCurve(Master, out master))
                {
                    Core.KS3P.LogException(new InvalidCastException("Error parsing curve [Master] for module [ColorGrading_Curves]! Disabling Color Grading!"));
                    succeeded = false;
                    return settings;
                }
                else
                {
                    settings.master = master;
                }

                if (!TryParseColorCurve(Red, out red))
                {
                    Core.KS3P.LogException(new InvalidCastException("Error parsing curve [Red] for module [ColorGrading_Curves]! Disabling Color Grading!"));
                    succeeded = false;
                    return settings;
                }
                else
                {
                    settings.red = red;
                }

                if (!TryParseColorCurve(sVs, out satVsat))
                {
                    Core.KS3P.LogException(new InvalidCastException("Error parsing curve [Saturation_Versus_Saturation] for module [ColorGrading_Curves]! Disabling Color Grading!"));
                    succeeded = false;
                    return settings;
                }
                else
                {
                    settings.satVSsat = satVsat;
                }

                //We're done here.
                return settings;
            }

            //Processes the tone mapper settings of a color grading model
            static ColorGradingModel.TonemappingSettings CG_ProcessTonemapperSettings(ConfigNode TMNode, out bool succeeded)
            {
                ColorGradingModel.TonemappingSettings settings = new ColorGradingModel.TonemappingSettings();
                settings = ColorGradingModel.TonemappingSettings.defaultSettings;

                if (TMNode == null)
                {
                    succeeded = true;
                    return settings;
                }

                //Initializing
                float blackIn, blackOut, whiteIn, whiteOut, whiteClip, whiteLevel;
                string mapper = TMNode.GetValue("Tonemapper");

                if (mapper == "ACES") { settings.tonemapper = ColorGradingModel.Tonemapper.ACES; }
                else if (mapper == "Neutral") { settings.tonemapper = ColorGradingModel.Tonemapper.Neutral; }
                else { settings.tonemapper = ColorGradingModel.Tonemapper.None; }

                succeeded = true;

                if (!float.TryParse(TMNode.GetValue("Black_In"), out blackIn))
                {
                    Core.KS3P.LogException(new InvalidCastException("Error parsing [blackIn] for module [ColorGrading_Tonemapper]! Disabling Color Grading!"));
                    succeeded = false;
                    return settings;
                }
                else
                {
                    settings.neutralBlackIn = Mathf.Clamp(blackIn, -0.1f, 0.1f);
                }

                if (!float.TryParse(TMNode.GetValue("Black_Out"), out blackOut))
                {
                    Core.KS3P.LogException(new InvalidCastException("Error parsing [Black_Out] for module [ColorGrading_Tonemapper]! Disabling Color Grading!"));
                    succeeded = false;
                    return settings;
                }
                else
                {
                    settings.neutralBlackOut = Mathf.Clamp(blackOut, -0.09f, 0.1f);
                }

                if (!float.TryParse(TMNode.GetValue("White_In"), out whiteIn))
                {
                    Core.KS3P.LogException(new InvalidCastException("Error parsing [White_In] for module [ColorGrading_Tonemapper]! Disabling Color Grading!"));
                    succeeded = false;
                    return settings;
                }
                else
                {
                    settings.neutralWhiteIn = Mathf.Clamp(whiteIn, 1f, 20f);
                }

                if (!float.TryParse(TMNode.GetValue("White_Out"), out whiteOut))
                {
                    Core.KS3P.LogException(new InvalidCastException("Error parsing [White_Out] for module [ColorGrading_Tonemapper]! Disabling Color Grading!"));
                    succeeded = false;
                    return settings;
                }
                else
                {
                    settings.neutralWhiteOut = Mathf.Clamp(whiteOut, 1f, 19f);
                }

                if (!float.TryParse(TMNode.GetValue("White_Clip"), out whiteClip))
                {
                    Core.KS3P.LogException(new InvalidCastException("Error parsing [White_Clip] for module [ColorGrading_Tonemapper]! Disabling Color Grading!"));
                    succeeded = false;
                    return settings;
                }
                else
                {
                    settings.neutralWhiteClip = Mathf.Clamp(whiteClip, 1f, 10f);
                }

                if (!float.TryParse(TMNode.GetValue("White_Level"), out whiteLevel))
                {
                    Core.KS3P.LogException(new InvalidCastException("Error parsing [White_Level] for module [ColorGrading_Tonemapper]! Disabling Color Grading!"));
                    succeeded = false;
                    return settings;
                }
                else
                {
                    settings.neutralWhiteLevel = Mathf.Clamp(whiteLevel, 0.1f, 20f);
                }

                //Parsing

                return settings;
            }
            #endregion

            /// <summary>
            /// Parses a color grading curve node. Returns false if an exception occurred.
            /// </summary>
            /// <param name="target">The config node of this particular curve</param>
            /// <param name="curve">The resulting curve</param>
            /// <returns></returns>
            internal static bool TryParseColorCurve(ConfigNode target, out ColorGradingCurve curve)
            {
                AnimationCurve internalCurve = new AnimationCurve(); //The internal curve

                //Set up the curve's Zero value
                float zero;
                if (!float.TryParse(target.GetValue("Zero"), out zero))
                {
                    Core.KS3P.LogException(new InvalidCastException("[TryParseColorCurve]: Error parsing [Zero]!"));
                    curve = null;
                    return false;
                }

                //Set up the curve's Loop value
                bool isLoop;
                if (!bool.TryParse(target.GetValue("IsLooped"), out isLoop))
                {
                    Core.KS3P.LogException(new InvalidCastException("[TryParseColorCurve]: Error parsing [IsLooped]!"));
                    curve = null;
                    return false;
                }

                //Set up the curve's Bounds value
                Vector2 bounds;
                try
                {
                    bounds = ConfigNode.ParseVector2(target.GetValue("Bounds"));
                }
                catch
                {
                    Core.KS3P.LogException(new InvalidCastException("[TryParseColorCurve]: Error parsing [Bounds]!"));
                    curve = null;
                    return false;
                }

                //Set up the curve's CurveKeys
                ConfigNode keyNode = target.GetNode("Curve");
                string[] Keys = keyNode.GetValues("Key"); //Get the curve keys
                                                          //Make Vector2 list
                List<Vector4> KeyValues = new List<Vector4>();
                if (Keys.Length == 0) //Check if curve keys are specified
                {
                    //We have no array elements? Then we can't work.
                    curve = null;
                    return false;
                }
                else
                {
                    //We (try to) process all keys
                    for (int x = 0; x < Keys.Length; x++)
                    {
                        try
                        {
                            bool hasTangent;
                            var frame = ParseFromString(Keys[x], out hasTangent);
                            var final = (hasTangent) ? new Keyframe(frame.x, frame.y, frame.z, frame.w) : new Keyframe(frame.x, frame.y);
                            internalCurve.AddKey(final);
                        }
                        catch
                        {
                            Core.KS3P.LogException(new InvalidCastException("[TryParseColorCurve]: Error parsing curve key [" + Keys[x] + "]!"));
                            curve = null;
                            return false;
                        }
                    }
                }

                //Set up the AnimationCurve
                foreach (Vector2 key in KeyValues)
                {
                    internalCurve.AddKey(key.x, key.y);
                }


                //Finalize
                try
                {
                    curve = new ColorGradingCurve(internalCurve, zero, isLoop, bounds);
                    return true;
                    //Job well done.
                }
                catch
                {
                    Core.KS3P.LogException(new InvalidCastException("[TryParseColorCurve]: Error finalizing curve!"));
                    curve = null;
                    return false;
                }
            }

            static Vector4 ParseFromString(string input, out bool hasTangents)
            {
                //Split input into 2
                char[] separator = { ',' };
                string[] parts = input.Split(separator, 4);
                try
                {
                    if (parts.Length == 2)
                    {
                        hasTangents = false;
                        return new Vector2(float.Parse(parts[0]), float.Parse(parts[1]));
                    }
                    else
                    {
                        hasTangents = true;
                        return new Vector4(float.Parse(parts[0]), float.Parse(parts[1]), float.Parse(parts[2]), float.Parse(parts[3]));
                    }
                }
                catch
                {
                    Core.KS3P.LogException(new InvalidOperationException("[ParseFromString]: Error parsing string [" + input + "]! Returning empty Vector2!"));
                    hasTangents = false;
                    return Vector2.zero;
                }
            }
        }
        public enum KS3PScene
        {
            MainMenu = 0,
            KSC = 1,
            VAB = 2,
            SPH = 3,
            Scaled = 4,
            Flight = 5,
            IVA = 6,
            EVA = 7
        }
        /// <summary>
        /// Stores all data of a PP-profile. When requested, generates a PP-profile on the spot from the loaded data.
        /// </summary>
        public struct KS3PProfile
        {
            string lensDirtTex, caSpectrumTex, userLutTex, vignetteMaskTex;
            public KS3PProfile(ConfigNode sceneData, string scenename, string bundlename)
            {
                AO = Parser.ParseAO(sceneData.GetNode("Ambient_Occlusion"), scenename, bundlename);
                AA = Parser.ParseAA(sceneData.GetNode("Anti_Aliasing"), scenename, bundlename);
                B = Parser.ParseB(sceneData.GetNode("Bloom"), scenename, bundlename, out lensDirtTex);
                CA = Parser.ParseCA(sceneData.GetNode("Chromatic_Abberation"), scenename, bundlename, out caSpectrumTex);
                CG = Parser.ParseCG(sceneData.GetNode("Color_Grading"), scenename, bundlename);
                DOF = Parser.ParseDOF(sceneData.GetNode("Depth_Of_Field"), scenename, bundlename);
                D = Parser.ParseD(sceneData.GetNode("Dithering"), scenename, bundlename);
                EA = Parser.ParseEA(sceneData.GetNode("Eye_Adaptation"), scenename, bundlename);
                G = Parser.ParseG(sceneData.GetNode("Grain"), scenename, bundlename);
                MB = Parser.ParseMB(sceneData.GetNode("Motion_Blur"), scenename, bundlename);
                UL = Parser.ParseUL(sceneData.GetNode("User_Lut"), scenename, bundlename, out userLutTex);
                V = Parser.ParseV(sceneData.GetNode("Vignette"), scenename, bundlename, out vignetteMaskTex);
            }

            AmbientOcclusionModel AO;
            AntialiasingModel AA;
            BloomModel B;
            ChromaticAberrationModel CA;
            ColorGradingModel CG;
            DepthOfFieldModel DOF;
            DitheringModel D;
            EyeAdaptationModel EA;
            GrainModel G;
            MotionBlurModel MB;
            UserLutModel UL;
            VignetteModel V;

            Texture2D Parse(string tex, GameDatabase database)
            {
                var t = database.GetTexture(tex, false);
                t.name = tex;
                return t;
            }

            /// <summary>
            /// Generates a PostProcessingProfile from the data gathered.
            /// </summary>
            /// <returns></returns>
            public PostProcessingProfile Generate()
            {
                GameDatabase database = GameDatabase.Instance; //Cached singleton grab
                var prof = ScriptableObject.CreateInstance<PostProcessingProfile>(); //Create profile

                //These need no textures
                prof.ambientOcclusion = AO;
                prof.antialiasing = AA;

                //Grab dirt tex
                var dirtSettings = B.settings.lensDirt;
                dirtSettings.texture = Parse(lensDirtTex, database);
                var settings = B.settings;
                settings.lensDirt = dirtSettings;
                B.settings = settings;
                prof.bloom = B;

                //Grab gradient tex
                var cas = CA.settings;
                cas.spectralTexture = Parse(caSpectrumTex, database);
                CA.settings = cas;
                prof.chromaticAberration = CA;
                
                //These need no textures
                prof.colorGrading = CG;
                prof.depthOfField = DOF;
                prof.eyeAdaptation = EA;
                prof.grain = G;
                prof.motionBlur = MB;

                //Grab lut tex
                var LS = UL.settings;
                LS.lut = Parse(userLutTex, database);
                UL.settings = LS;
                prof.userLut = UL;

                //Grab vignette mask
                var VS = V.settings;
                VS.mask = Parse(vignetteMaskTex, database);
                V.settings = VS;
                prof.vignette = V;

                return prof;
            }

            /// <summary>
            /// Implicit version of KS3PProfile.Generate();
            /// </summary>
            /// <param name="profile"></param>
            public static implicit operator PostProcessingProfile(KS3PProfile profile)
            {
                return profile.Generate();
            }
        }
        /// <summary>
        /// A named collection of scene-specific post processing profiles
        /// </summary>
        public struct KS3PBundle
        {
            /// <summary>
            /// The name of this profile bundle
            /// </summary>
            public string bundleName;
            Dictionary<KS3PScene, KS3PProfile> profiles;
            public KS3PBundle(ConfigNode bundleNode, string name)
            {
                bundleName = name;
                ConfigNode[] scenes = bundleNode.GetNodes("SETUP"); //Stores all scene nodes

                profiles = new Dictionary<KS3PScene, KS3PProfile>();
                foreach(ConfigNode scene in scenes)
                {
                    string sceneName = scene.GetValue("Scene");
                    Core.KS3P.Log("Processing data for scene [" + sceneName + "].");
                    profiles.Add((KS3PScene)Enum.Parse(typeof(KS3PScene), sceneName, true), new KS3PProfile(scene, sceneName, bundleName));
                }
            }
            public PostProcessingProfile GetProfile(KS3PScene scene)
            {
                return profiles[scene];
            }
        }
    }
    namespace Core
    {
        [KSPAddon(KSPAddon.Startup.Instantly, true)]
        public sealed class KS3P : MonoBehaviour
        {
            public static KS3P Main { get; private set; }
            internal static void Log(string message)
            {
                Debug.Log("[KS3P]: " + message);
                messages.Add("[KS3P]: " + message);
            }
            internal static void LogError(string message)
            {
                Debug.LogError("[KS3P]: " + message);
                messages.Add("[KS3P_ERR]: " + message);
            }
            internal static void LogException(Exception e)
            {
                Debug.LogException(e);
                LogError("Critical error occurred! StackTrace[" + e.StackTrace + "] Source[" + e.Source + "] Message[" + e.Message + "].");
            }
            static List<string> messages = new List<string>();
            
            /// <summary>
            /// The time in seconds that KS3P waits to patch the camera.
            /// </summary>
            public float delayTime = 0;

            /// <summary>
            /// All KS3P bundles are stored here.
            /// </summary>
            Dictionary<string, Data.KS3PBundle> bundleDictionary;

            /// <summary>
            /// The list of scenes for which a GUI button should be added
            /// </summary>
            public List<GameScenes> guiButtonScenes;

            /// <summary>
            /// This list contains all valid profile names.
            /// </summary>
            string[] names;
            

            /// <summary>
            /// The ID of this name
            /// </summary>
            int nameID;

            /// <summary>
            /// From the settings file: the name of the last loaded bundle.
            /// </summary>
            public string targetName;

            /// <summary>
            /// This is where KS3P will generate a log file
            /// </summary>
            public string logLocation;

            public bool launchDebugCamCheck = false;

            /// <summary>
            /// KS3P command keybinds
            /// </summary>
            public Keys.IKeybind disableKey, enableKey, nextProfKey, prevProfKey, toggleEditor;

            private void Awake()
            {
                Log("Alright, I'm up! Anyone made bacon yet?");
                Main = this;
                DontDestroyOnLoad(this); //Make persistent
                ShaderLoader.LoadAssetBundle("KS3P/Shaders", "postprocessingshaders"); //Load shaders

                LoadSettings();

                bundleDictionary = new Dictionary<string, Data.KS3PBundle>();

                if (LoadKS3P()) //Load KS3P. The code within this if-statement is executed only if no error occurred.
                {
                    //Subscribe to gameEvents
                    GameEvents.onGameSceneLoadRequested.Add(PatchCamera);
                }
                //If an error occurred, we do not subscribe to onGameSceneLoadRequested.
                else
                {
                    LogError("Critical loading error occurred - shutting down...");
                }
                nameID = GetNameID();
                PrintLog();
            }

            /// <summary>
            /// Gets the ID of the bundle corresponding to targetBundle
            /// </summary>
            /// <returns></returns>
            int GetNameID()
            {
                for(int i = 0; i < names.Length; i++)
                {
                    if(names[i] == targetName)
                    {
                        return i;
                    }
                }
                return 0;
            }

            /// <summary>
            /// Returns an IKeybind solution from an input string. Also checks what IKeybind derivative should be assigned - single or multi.
            /// </summary>
            /// <param name="name"></param>
            /// <returns></returns>
            Keys.IKeybind ParseKeyCode(string name)
            {
                if(name == null)
                {
                    return new Keys.KS3PUnbound();
                }
                char[] separator = { '+' };
                string[] parts = name.Split(separator);
                
                if(parts.Length == 1)
                {
                    return new Keys.KS3PSingleKeybind(parts[0]); //One key assigned - use SingleKeybind
                }
                else
                {
                    return new Keys.KS3PMultiKeybind(parts); //Multiple keys assigned - ues MultiKeybind
                }
            }

            void PrintLog()
            {
                Log("Finishing up! Generating logfile...");
                string location = (logLocation == null) ? KSPUtil.ApplicationRootPath + "/GameData/KS3P/Log.txt" : KSPUtil.ApplicationRootPath + logLocation + "/Log.txt";
                Log("Logfile created at location [" + location + "].");
                File.WriteAllLines(location, messages.ToArray());
            }

            private void LoadSettings()
            {
                ConfigNode settings = ConfigNode.Load(KSPUtil.ApplicationRootPath + "/GameData/KS3P/Settings.cfg");
                targetName = settings.GetValue("Target");
                logLocation = settings.GetValue("LogLocation");
                launchDebugCamCheck = (settings.GetValue("LaunchDebugCamCheck") == "true") ? true : false;
                //delayTime = float.Parse(settings.GetValue("DelayTime"));
                ConfigNode keyBinds = settings.GetNode("Keybinds");

                disableKey = ParseKeyCode(keyBinds.GetValue("DisableKey"));
                enableKey = ParseKeyCode(keyBinds.GetValue("EnableKey"));
                nextProfKey = ParseKeyCode(keyBinds.GetValue("NextProfileKey"));
                prevProfKey = ParseKeyCode(keyBinds.GetValue("PrevProfileKey"));
                toggleEditor = ParseKeyCode(keyBinds.GetValue("ToggleKey"));

                guiButtonScenes = GetGuiButtonScenes(settings.GetValue("GUIScenes"));
            }

            List<GameScenes> GetGuiButtonScenes(string v)
            {
                List<GameScenes> sceneList = new List<GameScenes>();
                char[] separator = { ',' };
                string[] entries = v.Split(separator);
                GameScenes scene;
                foreach (string s in entries)
                {
                    if (Data.Parser.TryParseEnum<GameScenes>(s, out scene))
                    {
                        sceneList.Add(scene);
                    }
                }
                return sceneList;
            }

            bool LoadKS3P()
            {
                int indexOfParseTarget = 0; //The index of the config file we are parsing
                int length = 0; //The total number of config files
                UrlDir.UrlConfig targetConfig = null; //The config file we are currently processing
                try
                {
                    //Grab all configs
                    UrlDir.UrlConfig[] urlFiles = GameDatabase.Instance.GetConfigs("KS3P");
                    length = urlFiles.Length; //Cache array length
                    names = new string[length];
                    Log("Beginning parsing operation for a total of [" + length + "] found files.");
                    for(int i = 0; i < length; i++) //Cycle through the array
                    {
                        indexOfParseTarget = i;
                        targetConfig = urlFiles[i];
                        Log("Parsing config file [" + i + "/" + length + "]." + " [Completion " + Mathf.FloorToInt((i * 100) / length) + "%]");
                        ProcessBundleNode(urlFiles[i].config, i);
                    }
                    Log("Loading operation complete.");
                    return true;
                }
                catch (Exception e)
                {
                    LogException(e);
                    LogError("[KS3P]: Error occured when parsing config [" + indexOfParseTarget + "] out of [" + length + "] configs. Error occurred in bundle located at [" + targetConfig.url +"].");
                    return false;
                }
            }

            /// <summary>
            /// Invoked on scene load, targets the right enumerator to handle this next scene.
            /// </summary>
            /// <param name="data"></param>
            public void PatchCamera(GameScenes data)
            {
                if (data == GameScenes.FLIGHT)
                {
                    StartCoroutine(FlightCameraManager());
                    StopCoroutine(CameraPatchManager(data));
                }
                else
                {
                    StartCoroutine(CameraPatchManager(data));
                    StopCoroutine(FlightCameraManager());
                }
                if(launchDebugCamCheck)
                {
                    StopCoroutine(CameraDebugCheck());
                    StartCoroutine(CameraDebugCheck());
                }
                else
                {
                    StopCoroutine(CameraDebugCheck());
                }
            }

            IEnumerator CameraDebugCheck()
            {
                Camera[] cams;
                while(true)
                {
                    cams = Resources.FindObjectsOfTypeAll<Camera>();
                    foreach(Camera cam in cams)
                    {
                        if(cam.targetTexture == null) //Is it not a RenderTexture cam?
                        {
                            Debug.Log("[KS3P_CamReporter]: Found camera [" + cam.gameObject.name + "].");
                        }
                    }
                    yield return new WaitForSecondsRealtime(10f);
                }
            }

            IEnumerator FlightCameraManager()
            {
                PostProcessingProfile main, eva, iva;
                PostProcessingProfile scaled = GetProfile(out iva, out eva, out main);
                yield return new WaitForSecondsRealtime(delayTime);

                var mainCam = Camera.main.gameObject;
                var scaledCam = GameObject.Find("Camera ScaledSpace");

                var mainComp = mainCam.AddOrGetComponent<PostProcessingBehaviour>();
                var scaledComp = mainCam.AddOrGetComponent<PostProcessingBehaviour>();

                mainComp.enabled = true;
                scaledComp.enabled = true;

                scaledComp.profile = scaled;
                InternalCamera ivaCam = InternalCamera.Instance; //Cache IVA cam singleton

                sbyte lastState = 0;
                //0: Initial, 1: Main, 2: IVA, 3: EVA

                while(true)
                {
                    if(ivaCam.isActive)
                    {
                        if(lastState != 2)
                        {
                            mainComp.profile = iva;
                            lastState = 2;
                        }
                    }
                    else
                    {
                        if(FlightGlobals.ready)
                        {
                            if(FlightGlobals.ActiveVessel.isEVA)
                            {
                                if (lastState != 3)
                                {
                                    mainComp.profile = eva;
                                    lastState = 3;
                                }
                            }
                            else
                            {
                                if (lastState != 1)
                                {
                                    mainComp.profile = main;
                                    lastState = 1;
                                }
                            }
                        }
                        //If not ready, fall back to main profile
                        if (lastState != 1)
                        {
                            mainComp.profile = main;
                            lastState = 1;
                        }
                    }
                    yield return null;
                }
            }

            IEnumerator CameraPatchManager(GameScenes data)
            {
                yield return new WaitForSecondsRealtime(delayTime);

                var mainCam = (data == GameScenes.TRACKSTATION) ? GameObject.Find("Camera ScaledSpace") : Camera.main.gameObject;
                if (mainCam != null)
                {
                    var component = mainCam.AddOrGetComponent<PostProcessingBehaviour>();
                    component.profile = GetProfile(data);
                    component.enabled = true;
                }
            }

            /// <summary>
            /// Returns a PostProcessingProfile generated from a profileBundle.
            /// </summary>
            /// <param name="data"></param>
            /// <returns></returns>
            PostProcessingProfile GetProfile(GameScenes data)
            {
                return bundleDictionary[names[nameID]].GetProfile(Convert(data));
            }
            
            public Data.KS3PScene Convert(GameScenes baseScene)
            {
                switch(baseScene)
                {
                    case GameScenes.EDITOR:
                        if (EditorDriver.editorFacility == EditorFacility.SPH) return Data.KS3PScene.SPH;
                        else return Data.KS3PScene.VAB;
                    case GameScenes.MAINMENU: return Data.KS3PScene.MainMenu;
                    case GameScenes.SPACECENTER: return Data.KS3PScene.KSC;
                    case GameScenes.TRACKSTATION: return Data.KS3PScene.Scaled;
                    default:
                        ArgumentNullException e = new ArgumentNullException("baseScene", "Could not link KS3PScene to baseScene!");
                        LogException(e);
                        throw e;
                }
            }

            /// <summary>
            /// For the flight profile: returns the scaledspace bundle, and all other flight-dependant bundles as out-parameters. Scene data is not needed - we know it's the flight scene.
            /// </summary>
            /// <param name="iva"></param>
            /// <param name="eva"></param>
            /// <param name="flight"></param>
            /// <returns></returns>
            PostProcessingProfile GetProfile(out PostProcessingProfile iva, out PostProcessingProfile eva, out PostProcessingProfile flight)
            {
                Data.KS3PBundle bundle = bundleDictionary[names[nameID]];
                iva = bundle.GetProfile(Data.KS3PScene.IVA);
                eva = bundle.GetProfile(Data.KS3PScene.EVA);
                flight = bundle.GetProfile(Data.KS3PScene.Flight);
                return bundle.GetProfile(Data.KS3PScene.Scaled);
            }

            void ProcessBundleNode(ConfigNode bundleNode, int index)
            {
                string name = bundleNode.GetValue("Name");
                Log("[KS3P]: Parsing post-processing bundle named [" + name + "], bundle loadID [" + index + "].");
                Data.KS3PBundle bundle = new Data.KS3PBundle(bundleNode, name);
                bundleDictionary.Add(name, bundle);
                names[index] = name;
            }

            void Update()
            {
                if(Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt))
                {
                    if(disableKey.GetKey())
                    {
                        SetKS3P(false);
                    }
                    if(enableKey.GetKey())
                    {
                        SetKS3P(true);
                    }
                    if(nextProfKey.GetKey())
                    {
                        ToNext();
                    }
                    if(prevProfKey.GetKey())
                    {
                        ToPrevious();
                    }
                }
            }

            void DisableOn(GameObject cam)
            {
                var behaviour = cam.GetComponent<PostProcessingBehaviour>();
                if (behaviour != null)
                {
                    behaviour.enabled = false;
                }
            }

            public void SetKS3P(bool enable)
            {
                if(enable)
                {
                    PatchCamera(HighLogic.LoadedScene);
                }
                else
                {
                    GameObject maincam = Camera.main.gameObject;
                    GameObject scaledCam = GameObject.Find("Camera ScaledSpace");
                    if (maincam != null)
                    {
                        DisableOn(maincam);
                    }
                    if (scaledCam != null)
                    {
                        DisableOn(scaledCam);
                    }
                }
            }
            public void ToNext()
            {
                if(nameID == names.Length - 1) //Is this the last name in the array?
                {
                    nameID = 0;
                    targetName = names[0];
                }
                else //This is not the last name in the array. Reaching for index + 1 is safe.
                {
                    targetName = names[nameID + 1];
                    nameID = GetNameID();
                }
                Debug.Log("[KS3P]: New profile bundle selected! Bundle name [" + targetName + "] is now in charge.");
                PatchCamera(HighLogic.LoadedScene); //Patch the camera dependant on the active scene.
            }
            public void ToPrevious()
            {
                if (nameID == 0) //Is this the first name in the array?
                {
                    targetName = names[names.Length - 1]; //Set to last array item
                    nameID = GetNameID();
                }
                else //This is not the first name in the array. Reaching for index - 1 is safe.
                {
                    targetName = names[nameID - 1]; //Set to previous array item
                    nameID = GetNameID();
                }
                Debug.Log("[KS3P]: New profile bundle selected! Bundle name [" + targetName + "] is now in charge.");
                PatchCamera(HighLogic.LoadedScene); //Patch the camera dependant on the active scene.
            }
        }
    }
}