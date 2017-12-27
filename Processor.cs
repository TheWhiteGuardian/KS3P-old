using System;
using UnityEngine;
using KS3P.UnityPostProcessing;
using System.Collections.Generic;

namespace KS3P.Processor
{
    //This class holds nothing but a series of math functions.
    public class TWG
    {
        internal static void ParseModuleNotifcation(string module, bool isStart)
        {
            if (isStart)
            {
                Debug.Log("[KSP_PostProcessing]: Starting operation on module [" + module + "].");
            }
            else
            {
                Debug.Log("[KSP_PostProcessing]: Finished operation on module [" + module + "].");
            }
        }

        internal static void Load(string name)
        {
            Debug.Log("[KSP_PostProcessing]: Found information for effect [" + name + "]. Starting to process...");
        }
    }
    public class ProfileProcessor
    {

        public static void LoadProfile(out PostProcessingProfile ToEdit, List<ConfigNode> profileList, string scene, out bool foundScene)
        {
            //Will store all found nodes written for this specific scene
            List<ConfigNode> availableNodes = new List<ConfigNode>();

            try
            {
                //Add all nodes for this scene
                foreach (ConfigNode node in profileList)
                {
                    if (node.GetValue("Scene") == scene)
                    {
                        availableNodes.Add(node);
                    }
                }
                //Make into array
                ConfigNode[] nodes = availableNodes.ToArray();
                foundScene = true;
                ProcessProfile(nodes[0], out ToEdit, foundScene);
                return;
            }
            catch(IndexOutOfRangeException)
            {
                Debug.LogException(new IndexOutOfRangeException("[KSP_PostProcessing_Processor_LoadProfile]: Error loading profile: list is empty."));
                foundScene = false;
                ToEdit = ScriptableObject.CreateInstance<PostProcessingProfile>();
                return;
            }
        }
        public static void ProcessProfile(ConfigNode profiles, out PostProcessingProfile prof, bool foundScene)
        {
            
            //We make a new, empty profile for editing
            PostProcessingProfile profile = ScriptableObject.CreateInstance<PostProcessingProfile>();

            //Check if there is a scene at all
            if (!foundScene)
            {
                //If not, return empty and abort.
                prof = profile;
                return;
            }

            //This config node stores the anti-aliasing properties
            ConfigNode AntiAliasing = profiles.GetNode("Anti_Aliasing");

            //This config node stores the ambient occlusion properties
            ConfigNode AmbientOcclusion = profiles.GetNode("Ambient_Occlusion");

            //This config node stores the bloom properties
            ConfigNode Bloom = profiles.GetNode("Bloom");

            //This config node stores the chromatic abberation properties
            ConfigNode ChromaticAbberation = profiles.GetNode("Chromatic_Abberation");

            //This config node stores the color grading properties
            ConfigNode ColorGrading = profiles.GetNode("Color_Grading");

            //This config node stores the depth of field properties
            ConfigNode DepthOfField = profiles.GetNode("Depth_Of_Field");

            //This config node stores the dithering properties
            ConfigNode Dithering = profiles.GetNode("Dithering");

            //This config node stores the eye adaptation properties
            ConfigNode EyeAdaptation = profiles.GetNode("Eye_Adaptation");

            //This config node stores the advanced fog properties (not as spectacular as it sounds)
            ConfigNode Fog = profiles.GetNode("Fog");

            //This config node stores the grain properties
            ConfigNode Grain = profiles.GetNode("Grain");

            //This config node stores the motion blur properties
            ConfigNode MotionBlur = profiles.GetNode("Motion_Blur");

            //This config node stores the screen space reflection properties
            ConfigNode ScreenSpaceReflection = profiles.GetNode("Screen_Space_Reflection");

            //This confg node stores the user lut properties
            ConfigNode UserLut = profiles.GetNode("User_Lut");

            //This config node stores the vignette properties
            ConfigNode Vignette = profiles.GetNode("Vignette");

            //Initialize the Profile to default settings.
            EffectProcessor.InitializeProfile(profile);

            //If any of the above config nodes is null, we are not using its effect.


            if (AntiAliasing != null)
            {
                TWG.Load("AntiAliasing");
                //Store
                bool succeeded;
                //Process
                profile.antialiasing.settings = EffectProcessor.ProcessAntiAliasing(AntiAliasing, out succeeded);
                //If an exception occurred, disable AA for safety.
                profile.antialiasing.enabled = succeeded;
            }
            if (AmbientOcclusion != null)
            {
                TWG.Load("AmbientOcclusion");
                //Store
                bool succeeded;
                //Process
                profile.ambientOcclusion.settings = EffectProcessor.ProcessAmbientOcclusion(AmbientOcclusion, out succeeded);
                //Catch exception
                profile.ambientOcclusion.enabled = succeeded;
            }
            if (Bloom != null)
            {
                TWG.Load("Bloom");
                //Store
                bool succeeded;
                //Process
                profile.bloom.settings = EffectProcessor.ProcessBloom(Bloom, out succeeded);
                //Catch
                profile.bloom.enabled = succeeded;

            }
            if (ChromaticAbberation != null)
            {
                TWG.Load("ChromaticAbberation");
                //Store
                bool succeeded;
                //Process
                profile.chromaticAberration.settings = EffectProcessor.ProcessChromaticAbberation(ChromaticAbberation, out succeeded);
                //Catch
                profile.chromaticAberration.enabled = succeeded;
            }
            if (ColorGrading != null)
            {
                TWG.Load("ColorGrading");
                //Store
                bool succeeded;
                //Process
                profile.colorGrading.settings = EffectProcessor.ProcessColorGrading(ColorGrading, out succeeded);
                //Catch
                profile.chromaticAberration.enabled = succeeded;
            }
            if (DepthOfField != null)
            {
                TWG.Load("DepthOfField");
                //Store
                bool succeeded;
                //Process
                profile.depthOfField.settings = EffectProcessor.ProcessDepthOfField(DepthOfField, out succeeded);
                //Catch
                profile.depthOfField.enabled = succeeded;
            }
            //Dithering is not configurable
            profile.dithering.enabled = (Dithering != null);
            //Is dithering enabled = does the dithering node exist basically

            if (EyeAdaptation != null)
            {
                TWG.Load("EyeAdaptation");
                //Store
                bool succeeded;
                //Process
                profile.eyeAdaptation.settings = EffectProcessor.ProcessEyeAdaptation(EyeAdaptation, out succeeded);
                //Catch
                profile.eyeAdaptation.enabled = succeeded;
            }

            //Fog has only one value to be edited
            if (Fog != null)
            {
                TWG.Load("Fog");
                FogModel.Settings set;
                bool b;
                if (!bool.TryParse(Fog.GetValue("Exclude_Skybox"), out b))
                {
                    Debug.LogException(new InvalidCastException("[KSP_PostProcessing]: Error parsing [Exclude_Skybox] for module [Fog]! Disabling Fog!"));
                }
                else
                {
                    set.excludeSkybox = b;
                    profile.fog.settings = set;
                    profile.fog.enabled = true;
                }
            }

            if (Grain != null)
            {
                TWG.Load("Grain");
                //Store
                bool succeeded;
                //Process
                profile.grain.settings = EffectProcessor.ProcessGrain(Grain, out succeeded);
                //Catch
                profile.grain.enabled = succeeded;
            }

            if (MotionBlur != null)
            {
                TWG.Load("MotionBlur");
                //Store
                bool succeeded;
                //Process
                profile.motionBlur.settings = EffectProcessor.ProcessMotionBlur(MotionBlur, out succeeded);
                //Catch
                profile.motionBlur.enabled = succeeded;
            }

            if (ScreenSpaceReflection != null)
            {
                TWG.Load("ScreenSpaceReflection");
                //Store
                bool succeeded;
                //Process
                profile.screenSpaceReflection.settings = EffectProcessor.ProcessScreenSpaceReflection(ScreenSpaceReflection, out succeeded);
                //Catch
                profile.screenSpaceReflection.enabled = succeeded;
            }

            if (UserLut != null)
            {
                TWG.Load("UserLut");
                //Store
                bool succeeded;
                //Process
                profile.userLut.settings = EffectProcessor.ProcessUserLut(UserLut, out succeeded);
                //Catch
                profile.userLut.enabled = succeeded;
            }

            if (Vignette != null)
            {
                TWG.Load("Vignette");
                //Store
                bool succeeded;
                //Process
                profile.vignette.settings = EffectProcessor.ProcessVignette(Vignette, out succeeded);
                //Catch
                profile.vignette.enabled = succeeded;
            }

            Debug.Log("[KSP_PostProcessing]: Parsing operation complete for profile [" + nameof(profile) + "].");
            prof = profile;
            //Note that we only enable a Post-Processing effect if the config node is detected, AND if no errors occurred during the processing.

        }
    }

    public class EffectProcessor
    {
        #region Processors

        /// <summary>
        /// Initializes or resets a profile to default settings.
        /// </summary>
        /// <param name="p">The profile to be reset</param>
        internal static void InitializeProfile(PostProcessingProfile p)
        {
            p.antialiasing.enabled = false;
            p.ambientOcclusion.enabled = false;
            p.bloom.enabled = false;
            p.chromaticAberration.enabled = false;
            p.colorGrading.enabled = false;
            p.depthOfField.enabled = false;
            p.dithering.enabled = false;
            p.eyeAdaptation.enabled = false;
            p.fog.enabled = false;
            p.grain.enabled = false;
            p.motionBlur.enabled = false;
            p.screenSpaceReflection.enabled = false;
            p.userLut.enabled = false;
            p.vignette.enabled = false;
        }
        

        /// <summary>
        /// Reads and processes an Anti-Aliasing Config Node
        /// </summary>
        /// <param name="AANode">The Anti-Aliasing node.</param>
        /// <param name="succeeded">Returns false if an exception occurred.</param>
        /// <returns></returns>
        internal static AntialiasingModel.Settings ProcessAntiAliasing(ConfigNode AANode, out bool succeeded)
        {
            AntialiasingModel.Settings settings = new AntialiasingModel.Settings();

            //Because we can use either FXAA or TAA, we branch here
            string mode = AANode.GetValue("Mode");
            //Do we use FXAA?
            if (mode == "FXAA")
            {
                settings.method = AntialiasingModel.Method.Fxaa;
                int quality;
                //Succeeded becomes true, but this may change depending on whether or not an exception occurred.
                succeeded = true;

                //The FXAA preset
                if (!int.TryParse(AANode.GetValue("Quality"), out quality))
                {
                    //If we reach this code, an exception occured during the conversion
                    Debug.LogException(new InvalidCastException("[KSP_PostProcessing]: Error parsing [Quality] for module [Anti-Aliasing]! Disabling Anti-Aliasing!"));
                    succeeded = false;
                    return settings;
                }
                else
                {
                    settings.fxaaSettings.preset = (AntialiasingModel.FxaaPreset)Mathf.Clamp(quality, 0, 4);
                }

                //We're done here.
                return settings;
            }
            //Do we use TAA?
            else if (mode == "TAA")
            {
                Debug.LogWarning("[KSP_PostProcessing]: Warning! Temporal Anti-Aliasing may not work correctly with MSAA enabled!");
                settings.method = AntialiasingModel.Method.Taa;
                float JitterSpread, MotionBlending, StationaryBlending, Sharpen;
                
                //Succeeded becomes true, if an exception occurs it will become false.
                succeeded = true;
                
                //Jitter Spread
                if (!float.TryParse(AANode.GetValue("Jitter"), out JitterSpread))
                {
                    Debug.LogException(new InvalidCastException("[KSP_PostProcessing]: Error parsing [Jitter] for module [Anti-Aliasing]! Disabling Anti-Aliasing!"));
                    succeeded = false;
                    return settings;
                }
                else
                {
                    settings.taaSettings.jitterSpread = JitterSpread;
                }

                //Motion blending
                if (!float.TryParse(AANode.GetValue("Blend_Motion"), out MotionBlending))
                {
                    Debug.LogException(new InvalidCastException("[KSP_PostProcessing]: Error parsing [Blend_Motion] for module [Anti-Aliasing]! Disabling Anti-Aliasing!"));
                    succeeded = false;
                    return settings;
                }
                else
                {
                    settings.taaSettings.motionBlending = MotionBlending;
                }

                //Stationary blending
                if (!float.TryParse(AANode.GetValue("Blend_Stationary"), out StationaryBlending))
                {
                    Debug.LogException(new InvalidCastException("[KSP_PostProcessing]: Error parsing [Blend_Stationary] for module [Anti-Aliasing]! Disabling Anti-Aliasing!"));
                    succeeded = false;
                    return settings;
                }
                else
                {
                    settings.taaSettings.stationaryBlending = StationaryBlending;
                }

                //Sharpen
                if (!float.TryParse(AANode.GetValue("Sharpen"), out Sharpen))
                {
                    Debug.LogException(new InvalidCastException("[KSP_PostProcessing]: Error parsing [Sharpen] for module [Anti-Aliasing]! Disabling Anti-Aliasing!"));
                    succeeded = false;
                    return settings;
                }
                else
                {
                    settings.taaSettings.sharpen = Sharpen;
                }

                //We're done here.
                return settings;
            }
            //Error buffer
            else
            {
                Debug.LogException(new ArgumentNullException("[KSP_PostProcessing]: Could not load the Anti-Aliasing model! Use either 'FXAA' or 'TAA'!"));
                succeeded = false;
                return settings;
            }
        }

        /// <summary>
        /// Reads and processes an Ambient Occlusion Config Node
        /// </summary>
        /// <param name="AONode">The Ambient Occlusion node.</param>
        /// <param name="succeeded">Returns false if an exception occurred.</param>
        /// <returns></returns>
        internal static AmbientOcclusionModel.Settings ProcessAmbientOcclusion(ConfigNode AONode, out bool succeeded)
        {
            AmbientOcclusionModel.Settings settings = new AmbientOcclusionModel.Settings();
            
            bool ambientOnly, downsampling, forceForwardCompatibility, highPrecision;
            float intensity, radius;
            int sampleCount;
            succeeded = true;

            //Ambient Only
            if(!bool.TryParse(AONode.GetValue("Ambient_Only"), out ambientOnly))
            {
                Debug.LogException(new InvalidCastException("[KSP_PostProcessing]: Error parsing [Ambient_Only] for module [Ambient Occlusion]! Disabling Ambient Occlusion!"));
                succeeded = false;
                return settings;
            }
            else
            {
                settings.ambientOnly = ambientOnly;
            }

            //Downsampling
            if (!bool.TryParse(AONode.GetValue("Downsampling"), out downsampling))
            {
                Debug.LogException(new InvalidCastException("[KSP_PostProcessing]: Error parsing [Downsampling] for module [Ambient Occlusion]! Disabling Ambient Occlusion!"));
                succeeded = false;
                return settings;
            }
            else
            {
                settings.downsampling = downsampling;
            }

            //Force Forward Compatibility
            if (!bool.TryParse(AONode.GetValue("Force_Forward_Compatibility"), out forceForwardCompatibility))
            {
                Debug.LogException(new InvalidCastException("[KSP_PostProcessing]: Error parsing [Force_Forward_Compatibility] for module [Ambient Occlusion]! Disabling Ambient Occlusion!"));
                succeeded = false;
                return settings;
            }
            else
            {
                settings.forceForwardCompatibility = forceForwardCompatibility;
            }

            //highPrecision
            if (!bool.TryParse(AONode.GetValue("High_Precision"), out highPrecision))
            {
                Debug.LogException(new InvalidCastException("[KSP_PostProcessing]: Error parsing [High_Precision] for module [Ambient Occlusion]! Disabling Ambient Occlusion!"));
                succeeded = false;
                return settings;
            }
            else
            {
                settings.highPrecision = highPrecision;
            }

            //Intensity
            if (!float.TryParse(AONode.GetValue("Intensity"), out intensity))
            {
                Debug.LogException(new InvalidCastException("[KSP_PostProcessing]: Error parsing [Intensity] for module [Ambient Occlusion]! Disabling Ambient Occlusion!"));
                succeeded = false;
                return settings;
            }
            else
            {
                settings.intensity = intensity;
            }
            //Radius

            if (!float.TryParse(AONode.GetValue("Radius"), out radius))
            {
                Debug.LogException(new InvalidCastException("[KSP_PostProcessing]: Error parsing [Radius] for module [Ambient Occlusion]! Disabling Ambient Occlusion!"));
                succeeded = false;
                return settings;
            }
            else
            {
                settings.radius = radius;
            }

            //Sample Count
            if (!int.TryParse(AONode.GetValue("Sample_Count"), out sampleCount))
            {
                Debug.LogException(new InvalidCastException("[KSP_PostProcessing]: Error parsing [Sample_Count] for module [Ambient Occlusion]! Disabling Ambient Occlusion!"));
                succeeded = false;
                return settings;
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
            return settings;
        }

        /// <summary>
        /// Reads and processes a Bloom Config Node
        /// </summary>
        /// <param name="BNode">The Bloom node.</param>
        /// <param name="succeeded">Returns false if an exception occurred.</param>
        /// <returns></returns>
        internal static BloomModel.Settings ProcessBloom(ConfigNode BNode, out bool succeeded)
        {
            BloomModel.Settings settings = new BloomModel.Settings(); //The base effect settings, basically combines bloomSettings and dirtSettings
            BloomModel.BloomSettings bloomSettings = new BloomModel.BloomSettings(); //The bloom settings
            BloomModel.LensDirtSettings dirtSettings = new BloomModel.LensDirtSettings(); //The lens dirt settings

            succeeded = true;

            #region BloomSettings
            bool antiFlicker;
            float intensity, radius, softKnee, threshold;
            
            //Anti-flicker
            if (!bool.TryParse(BNode.GetValue("Anti_Flicker"), out antiFlicker))
            {
                Debug.LogException(new InvalidCastException("[KSP_PostProcessing]: Error parsing [Anti_Flicker] for module [Bloom]! Disabling Bloom!"));
                succeeded = false;
                return settings;
            }
            else
            {
                bloomSettings.antiFlicker = antiFlicker;
            }

            //Intensity
            if (!float.TryParse(BNode.GetValue("Intensity"), out intensity))
            {
                Debug.LogException(new InvalidCastException("[KSP_PostProcessing]: Error parsing [Intensity] for module [Bloom]! Disabling Bloom!"));
                succeeded = false;
                return settings;
            }
            else
            {
                bloomSettings.intensity = intensity;
            }

            //Radius
            if (!float.TryParse(BNode.GetValue("Radius"), out radius))
            {
                Debug.LogException(new InvalidCastException("[KSP_PostProcessing]: Error parsing [Radius] for module [Bloom]! Disabling Bloom!"));
                succeeded = false;
                return settings;
            }
            else
            {
                bloomSettings.radius = radius;
            }
            
            //Soft_Knee
            if (!float.TryParse(BNode.GetValue("Soft_Knee"), out softKnee))
            {
                Debug.LogException(new InvalidCastException("[KSP_PostProcessing]: Error parsing [Soft_Knee] for module [Bloom]! Disabling Bloom!"));
                succeeded = false;
                return settings;
            }
            else
            {
                bloomSettings.softKnee = softKnee;
            }

            //Threshold
            if (!float.TryParse(BNode.GetValue("Threshold"), out threshold))
            {
                Debug.LogException(new InvalidCastException("[KSP_PostProcessing]: Error parsing [Threshold] for module [Bloom]! Disabling Bloom!"));
                succeeded = false;
                return settings;
            }
            else
            {
                bloomSettings.threshold = threshold;
            }
            #endregion

            #region DirtSettings
            Texture2D dirtTex;
            float dirtIntensity;
            bool dirtEnabled;

            //Texture2D has no TryParse, we'll have to do this manually

            dirtEnabled = bool.Parse(BNode.GetValue("Dirt_Enabled"));

            if (dirtEnabled)
            {
                try
                {
                    //Try parsing
                    dirtTex = GameDatabase.Instance.GetTexture(BNode.GetValue("Dirt_Tex"), false);

                    //EDIT: allow for disabling the dirt effect
                    //Apply
                    dirtSettings.texture = dirtTex;


                    //No texture existence check because the texture can be null without Unity screaming for mercy
                }
                //If parse failed
                catch
                {
                    Debug.LogException(new InvalidCastException("[KSP_PostProcessing]: Error parsing [Dirt_Tex] for module [Bloom]! Disabling Bloom!"));
                    succeeded = false;
                    return settings;
                }

                //dirtIntensity
                if (!float.TryParse(BNode.GetValue("Dirt_Intensity"), out dirtIntensity))
                {
                    Debug.LogException(new InvalidCastException("[KSP_PostProcessing]: Error parsing [Dirt_Intensity] for module [Bloom]! Disabling Bloom!"));
                    succeeded = false;
                    return settings;
                }
                else
                {
                    dirtSettings.intensity = dirtIntensity;
                }
            }
            else
            {
                dirtSettings = BloomModel.LensDirtSettings.defaultSettings; //Else we set to default...
                dirtSettings.texture = GameDatabase.Instance.GetTexture("KS3P/Textures/Null", false); //Grab the null texture
                dirtSettings.intensity = 1f; //And disable the effect through intensity
            }

            #endregion

            //We're done here, let's wrap it up.
            settings.bloom = bloomSettings;
            settings.lensDirt = dirtSettings;
            return settings;
        }

        /// <summary>
        /// Reads and processes a Chromatic Abberation Node
        /// </summary>
        /// <param name="CANode">The Chromatic Abberation node.</param>
        /// <param name="succeeded">Returns false if an exception occurred.</param>
        /// <returns></returns>
        internal static ChromaticAberrationModel.Settings ProcessChromaticAbberation(ConfigNode CANode, out bool succeeded)
        {
            //Make a new settings instance to edit
            ChromaticAberrationModel.Settings settings = new ChromaticAberrationModel.Settings();

            Texture2D SpectralTex;
            float intensity;
            succeeded = true;
            //Texture2D has no TryParse, we'll have to do this manually
            try
            {
                //Try parsing
                SpectralTex = GameDatabase.Instance.GetTexture(CANode.GetValue("Spectral_Tex"), false);
                //Apply
                settings.spectralTexture = SpectralTex;

                //No texture existence check because the texture can be null without Unity screaming for mercy
            }
            //If parse failed
            catch
            {
                Debug.LogException(new InvalidCastException("[KSP_PostProcessing]: Error parsing [Spectral_Tex] for module [Chromatic Abberation]! Disabling Chromatic Abberation!"));
                succeeded = false;
                return settings;
            }

            if (!float.TryParse(CANode.GetValue("Intensity"), out intensity))
            {
                Debug.LogException(new InvalidCastException("[KSP_PostProcessing]: Error parsing [Intensity] for module [Chromatic Abberation]! Disabling Chromatic Abberation!"));
                succeeded = false;
                return settings;
            }
            else
            {
                settings.intensity = intensity;
            }

            //We're done here.
            return settings;
        }

        /// <summary>
        /// Reads and processes a Screen Space Reflection Node
        /// </summary>
        /// <param name="SSRNode">The Screen Space Reflection node.</param>
        /// <param name="succeeded">Returns false if a problem occurred during processing.</param>
        /// <returns></returns>
        internal static ScreenSpaceReflectionModel.Settings ProcessScreenSpaceReflection(ConfigNode SSRNode, out bool succeeded)
        {
            Debug.LogWarning("[KSP_PostProcessing]: Warning: [Screen Space Reflection] only works with the deferred rendering path!");
            ScreenSpaceReflectionModel.Settings settings = new ScreenSpaceReflectionModel.Settings();
            ScreenSpaceReflectionModel.IntensitySettings intensitySettings = new ScreenSpaceReflectionModel.IntensitySettings();
            ScreenSpaceReflectionModel.ReflectionSettings reflectionSettings = new ScreenSpaceReflectionModel.ReflectionSettings();
            ScreenSpaceReflectionModel.ScreenEdgeMask screenmaskSettings = new ScreenSpaceReflectionModel.ScreenEdgeMask();

            bool PhysicallyBased, HighQuality, reflectBack;
            float maxDistance, width, blur;
            float multiplier, fadeDistance, fresnelFade, fresnelPower, intensity;
            int iterationCount, stepSize;

            succeeded = true;

            if (!bool.TryParse(SSRNode.GetValue("Physically_Based"), out PhysicallyBased))
            {
                Debug.LogException(new InvalidCastException("[KSP_PostProcessing]: Error parsing [Physically_Based] for module [Screen Space Reflection]! Disabling Screen Space Reflection!"));
                succeeded = false;
                return settings;
            }
            else
            {
                if (PhysicallyBased) { reflectionSettings.blendType = ScreenSpaceReflectionModel.SSRReflectionBlendType.PhysicallyBased; }
                else { reflectionSettings.blendType = ScreenSpaceReflectionModel.SSRReflectionBlendType.Additive; }
            }

            if (!bool.TryParse(SSRNode.GetValue("High_Quality"), out HighQuality))
            {
                Debug.LogException(new InvalidCastException("[KSP_PostProcessing]: Error parsing [High_Quality] for module [Screen Space Reflection]! Disabling Screen Space Reflection!"));
                succeeded = false;
                return settings;
            }
            else
            {
                if (HighQuality) { reflectionSettings.reflectionQuality = ScreenSpaceReflectionModel.SSRResolution.High; }
                else { reflectionSettings.reflectionQuality = ScreenSpaceReflectionModel.SSRResolution.Low; }
            }

            if (!float.TryParse(SSRNode.GetValue("Max_Distance"), out maxDistance))
            {
                Debug.LogException(new InvalidCastException("[KSP_PostProcessing]: Error parsing [Max_Distance] for module [Screen Space Reflection]! Disabling Screen Space Reflection!"));
                succeeded = false;
                return settings;
            }
            else
            {
                reflectionSettings.maxDistance = maxDistance;
            }

            if (!int.TryParse(SSRNode.GetValue("Iteration_Count"), out iterationCount))
            {
                Debug.LogException(new InvalidCastException("[KSP_PostProcessing]: Error parsing [Iteration_Count] for module [Screen Space Reflection]! Disabling Screen Space Reflection!"));
                succeeded = false;
                return settings;
            }
            else
            {
                reflectionSettings.iterationCount = iterationCount;
            }

            if (!int.TryParse(SSRNode.GetValue("Step_Size"), out stepSize))
            {
                Debug.LogException(new InvalidCastException("[KSP_PostProcessing]: Error parsing [Step_Size] for module [Screen Space Reflection]! Disabling Screen Space Reflection!"));
                succeeded = false;
                return settings;
            }
            else
            {
                reflectionSettings.stepSize = stepSize;
            }

            if (!float.TryParse(SSRNode.GetValue("Width_Modifier"), out width))
            {
                Debug.LogException(new InvalidCastException("[KSP_PostProcessing]: Error parsing [Witdh_Modifier] for module [Screen Space Reflection]! Disabling Screen Space Reflection!"));
                succeeded = false;
                return settings;
            }
            else
            {
                reflectionSettings.widthModifier = width;
            }

            if (!float.TryParse(SSRNode.GetValue("Reflection_Blur"), out blur))
            {
                Debug.LogException(new InvalidCastException("[KSP_PostProcessing]: Error parsing [Reflection_Blur] for module [Screen Space Reflection]! Disabling Screen Space Reflection!"));
                succeeded = false;
                return settings;
            }
            else
            {
                reflectionSettings.reflectionBlur = blur;
            }

            if (!bool.TryParse(SSRNode.GetValue("Reflect_Backfaces"), out reflectBack))
            {
                Debug.LogException(new InvalidCastException("[KSP_PostProcessing]: Error parsing [Reflect_Backfaces] for module [Screen Space Reflection]! Disabling Screen Space Reflection!"));
                succeeded = false;
                return settings;
            }
            else
            {
                reflectionSettings.reflectBackfaces = reflectBack;
            }

            if (!float.TryParse(SSRNode.GetValue("Reflection_Multiplier"), out multiplier))
            {
                Debug.LogException(new InvalidCastException("[KSP_PostProcessing]: Error parsing [Reflection_Multiplier] for module [Screen Space Reflection]! Disabling Screen Space Reflection!"));
                succeeded = false;
                return settings;
            }
            else
            {
                intensitySettings.reflectionMultiplier = multiplier;
            }

            if (!float.TryParse(SSRNode.GetValue("Fade_Distance"), out fadeDistance))
            {
                Debug.LogException(new InvalidCastException("[KSP_PostProcessing]: Error parsing [Fade_Distance] for module [Screen Space Reflection]! Disabling Screen Space Reflection!"));
                succeeded = false;
                return settings;
            }
            else
            {
                intensitySettings.fadeDistance = fadeDistance;
            }

            if (!float.TryParse(SSRNode.GetValue("Fresnel_Fade"), out fresnelFade))
            {
                Debug.LogException(new InvalidCastException("[KSP_PostProcessing]: Error parsing [Fresnel_Fade] for module [Screen Space Reflection]! Disabling Screen Space Reflection!"));
                succeeded = false;
                return settings;
            }
            else
            {
                intensitySettings.fresnelFade = fresnelFade;
            }

            if (!float.TryParse(SSRNode.GetValue("Fresnel_Fade_Power"), out fresnelPower))
            {
                Debug.LogException(new InvalidCastException("[KSP_PostProcessing]: Error parsing [Fresnel_Fade_Power] for module [Screen Space Reflection]! Disabling Screen Space Reflection!"));
                succeeded = false;
                return settings;
            }
            else
            {
                intensitySettings.fresnelFadePower = fresnelPower;
            }

            if (!float.TryParse(SSRNode.GetValue("Intensity"), out intensity))
            {
                Debug.LogException(new InvalidCastException("[KSP_PostProcessing]: Error parsing [Intensity] for module [Screen Space Reflection]! Disabling Screen Space Reflection!"));
                succeeded = false;
                return settings;
            }
            else
            {
                screenmaskSettings.intensity = intensity;
            }

            //We're done here
            settings.intensity = intensitySettings;
            settings.reflection = reflectionSettings;
            settings.screenEdgeMask = screenmaskSettings;
            return settings;
        }

        /// <summary>
        /// Reads and processes a Depth Of Field Config Node
        /// </summary>
        /// <param name="DOFNode">The Depth Of Field node.</param>
        /// <param name="succeeded">Returns false if a problem occurred during processing.</param>
        /// <returns></returns>
        internal static DepthOfFieldModel.Settings ProcessDepthOfField(ConfigNode DOFNode, out bool succeeded)
        {
            DepthOfFieldModel.Settings settings = new DepthOfFieldModel.Settings();
            float FocusDistance, Aperture, FocalLength;
            bool UseCam;
            int size;

            succeeded = true;

            if (!float.TryParse(DOFNode.GetValue("Focus_Distance"), out FocusDistance))
            {
                Debug.LogException(new InvalidCastException("[KSP_PostProcessing]: Error parsing [Focus_Distance] for module [Depth Of Field]! Disabling Depth Of Field!"));
                succeeded = false;
                return settings;
            }
            else
            {
                settings.focusDistance = FocusDistance;
            }

            if (!float.TryParse(DOFNode.GetValue("Aperture"), out Aperture))
            {
                Debug.LogException(new InvalidCastException("[KSP_PostProcessing]: Error parsing [Aperture] for module [Depth Of Field]! Disabling Depth Of Field!"));
                succeeded = false;
                return settings;
            }
            else
            {
                settings.aperture = Mathf.Clamp(Aperture, 0.1f, 32);
            }

            if (!bool.TryParse(DOFNode.GetValue("Use_Camera_FOV"), out UseCam))
            {
                Debug.LogException(new InvalidCastException("[KSP_PostProcessing]: Error parsing [Use_Camera_FOV] for module [Depth Of Field]! Disabling Depth Of Field!"));
                succeeded = false;
                return settings;
            }
            else
            {
                settings.useCameraFov = UseCam;
            }

            if (!float.TryParse(DOFNode.GetValue("Focal_Length"), out FocalLength))
            {
                Debug.LogException(new InvalidCastException("[KSP_PostProcessing]: Error parsing [Focal_Length] for module [Depth Of Field]! Disabling Depth Of Field!"));
                succeeded = false;
                return settings;
            }
            else
            {
                settings.focalLength = Mathf.Clamp(FocalLength, 1f, 300f);
            }

            if (!int.TryParse(DOFNode.GetValue("Kernel_Size"), out size))
            {
                Debug.LogException(new InvalidCastException("[KSP_PostProcessing]: Error parsing [Kernel_Size] for module [Depth Of Field]! Disabling Depth Of Field!"));
                succeeded = false;
                return settings;
            }
            else
            {
                settings.kernelSize = (DepthOfFieldModel.KernelSize)Mathf.Clamp(size, 0, 3);
            }

            //We're done here.
            return settings;
        }

        /// <summary>
        /// Reads and processes a Motion Blur Config Node
        /// </summary>
        /// <param name="MBNode">The Motion Blur node.</param>
        /// <param name="succeeded">Returns false if a problem occurred during processing.</param>
        /// <returns></returns>
        internal static MotionBlurModel.Settings ProcessMotionBlur(ConfigNode MBNode, out bool succeeded)
        {
            MotionBlurModel.Settings settings = new MotionBlurModel.Settings();

            float ShutterAngle, FrameBlending;
            int SampleCount;

            succeeded = true;

            if (!float.TryParse(MBNode.GetValue("Shutter_Angle"), out ShutterAngle))
            {
                Debug.LogException(new InvalidCastException("[KSP_PostProcessing]: Error parsing [Shutter_Angle] for module [Motion Blur]! Disabling Motion Blur!"));
                succeeded = false;
                return settings;
            }
            else
            {
                settings.shutterAngle = Mathf.Clamp(ShutterAngle, 0f, 360f);
            }

            if (!int.TryParse(MBNode.GetValue("Sample_Count"), out SampleCount))
            {
                Debug.LogException(new InvalidCastException("[KSP_PostProcessing]: Error parsing [Sample_Count] for module [Motion Blur]! Disabling Motion Blur!"));
                succeeded = false;
                return settings;
            }
            else
            {
                settings.sampleCount = Mathf.Clamp(SampleCount, 4, 32);
            }

            if (!float.TryParse(MBNode.GetValue("Frame_Blending"), out FrameBlending))
            {
                Debug.LogException(new InvalidCastException("[KSP_PostProcessing]: Error parsing [Frame_Blending] for module [Motion Blur]! Disabling Motion Blur!"));
                succeeded = false;
                return settings;
            }
            else
            {
                settings.frameBlending = Mathf.Clamp01(FrameBlending);
            }

            //We're done here
            return settings;
        }

        /// <summary>
        /// Reads and processes an Eye Adaptation Config Node
        /// </summary>
        /// <param name="EANode">The Eye Adaptation node.</param>
        /// <param name="succeeded">Returns false if a problem occurred during processing.</param>
        /// <returns></returns>
        internal static EyeAdaptationModel.Settings ProcessEyeAdaptation(ConfigNode EANode, out bool succeeded)
        {
            EyeAdaptationModel.Settings settings = new EyeAdaptationModel.Settings();

            int MinLum, MaxLum, type;
            float Min, Max, speedUp, speedDown, keyValue;
            bool DynaKey;
            Vector2 range;

            succeeded = true;

            if (!int.TryParse(EANode.GetValue("Luminosity_Minimum"), out MinLum))
            {
                Debug.LogException(new InvalidCastException("[KSP_PostProcessing]: Error parsing [Luminosity_Minimum] for module [Eye Adaptation]! Disabling Eye Adaptation!"));
                succeeded = false;
                return settings;
            }
            else
            {
                settings.logMin = Mathf.Clamp(MinLum, -16, -1);
            }

            if (!int.TryParse(EANode.GetValue("Luminosity_Maximum"), out MaxLum))
            {
                Debug.LogException(new InvalidCastException("[KSP_PostProcessing]: Error parsing [Luminosity_Maximum] for module [Eye Adaptation]! Disabling Eye Adaptation!"));
                succeeded = false;
                return settings;
            }
            else
            {
                settings.logMax = Mathf.Clamp(MaxLum, 1, 16);
            }

            //Auto exposure stuff
            if (!float.TryParse(EANode.GetValue("Maximum_EV"), out Max))
            {
                Debug.LogException(new InvalidCastException("[KSP_PostProcessing]: Error parsing [Maximum_EV] for module [Eye Adaptation]! Disabling Eye Adaptation!"));
                succeeded = false;
                return settings;
            }
            else
            {
                settings.maxLuminance = Max;
            }

            if (!float.TryParse(EANode.GetValue("Minimum_EV"), out Min))
            {
                Debug.LogException(new InvalidCastException("[KSP_PostProcessing]: Error parsing [Minimum_EV] for module [Eye Adaptation]! Disabling Eye Adaptation!"));
                succeeded = false;
                return settings;
            }
            else
            {
                settings.minLuminance = Min;
            }

            if (!bool.TryParse(EANode.GetValue("Dynamic_Key_Value"), out DynaKey))
            {
                Debug.LogException(new InvalidCastException("[KSP_PostProcessing]: Error parsing [Dynamic_Key_Value] for module [Eye Adaptation]! Disabling Eye Adaptation!"));
                succeeded = false;
                return settings;
            }
            else
            {
                settings.dynamicKeyValue = DynaKey;
            }

            if (!int.TryParse(EANode.GetValue("Type"), out type))
            {
                Debug.LogException(new InvalidCastException("[KSP_PostProcessing]: Error parsing [Type] for module [Eye Adaptation]! Disabling Eye Adaptation!"));
                succeeded = false;
                return settings;
            }
            else
            {
                settings.adaptationType = (EyeAdaptationModel.EyeAdaptationType)Mathf.Clamp01(type);
            }

            if (!float.TryParse(EANode.GetValue("Speed_Up"), out speedUp))
            {
                Debug.LogException(new InvalidCastException("[KSP_PostProcessing]: Error parsing [Speed_Up] for module [Eye Adaptation]! Disabling Eye Adaptation!"));
                succeeded = false;
                return settings;
            }
            else
            {
                settings.speedUp = speedUp;
            }

            if (!float.TryParse(EANode.GetValue("Speed_Down"), out speedDown))
            {
                Debug.LogException(new InvalidCastException("[KSP_PostProcessing]: Error parsing [] for module [Eye Adaptation]! Disabling Eye Adaptation!"));
                succeeded = false;
                return settings;
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
                Debug.LogException(new InvalidCastException("[KSP_PostProcessing]: Error parsing [Range] for module [Eye Adaptation]! Disabling Eye Adaptation!"));
                succeeded = false;
                return settings;
            }

            if (!float.TryParse(EANode.GetValue("Key_Value"), out keyValue))
            {
                Debug.LogException(new InvalidCastException("[KSP_PostProcessing]: Error parsing [Key_Value] for module [Eye Adaptation]! Disabling Eye Adaptation!"));
                succeeded = false;
                return settings;
            }
            else
            {
                settings.keyValue = keyValue;
            }

            //We're done here

            return settings;
        }

        /// <summary>
        /// Reads and processes a Grain Config Node
        /// </summary>
        /// <param name="GNode">The Grain node.</param>
        /// <param name="succeeded">Returns false if a problem occurred during processing.</param>
        /// <returns></returns>
        internal static GrainModel.Settings ProcessGrain(ConfigNode GNode, out bool succeeded)
        {
            GrainModel.Settings settings = new GrainModel.Settings();

            bool colored;
            float intensity, luminanceContribution, size;

            succeeded = true;

            if (!bool.TryParse(GNode.GetValue("Colored"), out colored))
            {
                Debug.LogException(new InvalidCastException("[KSP_PostProcessing]: Error parsing [Colored] for module [Grain]! Disabling Grain!"));
                succeeded = false;
                return settings;
            }
            else
            {
                settings.colored = colored;
            }

            if (!float.TryParse(GNode.GetValue("Intensity"), out intensity))
            {
                Debug.LogException(new InvalidCastException("[KSP_PostProcessing]: Error parsing [Intensity] for module [Grain]! Disabling Grain!"));
                succeeded = false;
                return settings;
            }
            else
            {
                settings.intensity = intensity;
            }

            if (!float.TryParse(GNode.GetValue("Luminance_Contribution"), out luminanceContribution))
            {
                Debug.LogException(new InvalidCastException("[KSP_PostProcessing]: Error parsing [Luminance_Contribution] for module [Grain]! Disabling Grain!"));
                succeeded = false;
                return settings;
            }
            else
            {
                settings.luminanceContribution = luminanceContribution;
            }

            if (!float.TryParse(GNode.GetValue("Size"), out size))
            {
                Debug.LogException(new InvalidCastException("[KSP_PostProcessing]: Error parsing [Size] for module [Grain]! Disabling Grain!"));
                succeeded = false;
                return settings;
            }
            else
            {
                settings.size = size;
            }

            //We're done here.
            return settings;
        }

        /// <summary>
        /// Reads and processes a User Lut Config Node
        /// </summary>
        /// <param name="ULMode">The User Lut node.</param>
        /// <param name="succeeded">Returns false if a problem occurred during processing.</param>
        /// <returns></returns>
        internal static UserLutModel.Settings ProcessUserLut(ConfigNode ULNode, out bool succeeded)
        {
            UserLutModel.Settings settings = new UserLutModel.Settings();

            Texture2D lutTexture; float contribution;
            succeeded = true;

            try
            {
                lutTexture = GameDatabase.Instance.GetTexture(ULNode.GetValue("Lut_Texture"), false);
            }
            //Check if exception
            catch
            {
                Debug.LogException(new InvalidCastException("[KSP_PostProcessing]: Error parsing [Lut_Texture] for module [User Lut]! Disabling User Lut!"));
                succeeded = false;
                return settings;
            }
            //Check if null
            if (lutTexture == null)
            {
                Debug.LogException(new ArgumentNullException("[KSP_PostProcessing]: Error finding texture [Lut_Texture] for module [User Lut]! Disabling User Lut!"));
                succeeded = false;
                return settings;
            }
            else
            {
                settings.lut = lutTexture;
            }

            if (!float.TryParse(ULNode.GetValue("Contribution"), out contribution))
            {
                Debug.LogException(new InvalidCastException("[KSP_PostProcessing]: Error parsing [Contribution] for module [User Lut]! Disabling User Lut!"));
                succeeded = false;
                return settings;
            }
            else
            {
                settings.contribution = contribution;
            }

            //We're done here.
            return settings;
        }

        /// <summary>
        /// Reads and processes a Vignette Config Node
        /// </summary>
        /// <param name="VNode">The Vignette node.</param>
        /// <param name="succeeded">Returns false if an exception occurred.</param>
        /// <returns></returns>
        internal static VignetteModel.Settings ProcessVignette(ConfigNode VNode, out bool succeeded)
        {
            VignetteModel.Settings settings = new VignetteModel.Settings();
            Vector2 center;
            Color color;
            float intensity, opacity, roundness, smoothness;
            Texture mask;
            int mode;
            bool rounded;

            succeeded = true;

            try
            {
                center = ConfigNode.ParseVector2(VNode.GetValue("Center"));
            }
            catch
            {
                Debug.LogException(new InvalidCastException("[KSP_PostProcessing]: Error parsing [Center] for module [Vignette]! Disabling Vignette!"));
                succeeded = false;
                return settings;
            }
            //If we reach this code, we didn't return because exception
            settings.center = center;

            try
            {
                color = ConfigNode.ParseColor(VNode.GetValue("Color"));
            }
            catch
            {
                Debug.LogException(new InvalidCastException("[KSP_PostProcessing]: Error parsing [Color] for module [Vignette]! Disabling Vignette!"));
                succeeded = false;
                return settings;
            }
            settings.color = color;

            if (!float.TryParse(VNode.GetValue("Intensity"), out intensity))
            {
                Debug.LogException(new InvalidCastException("[KSP_PostProcessing]: Error parsing [Intensity] for module [Vignette]! Disabling Vignette!"));
                succeeded = false;
                return settings;
            }
            else
            {
                settings.intensity = intensity;
            }

            if (!float.TryParse(VNode.GetValue("Opacity"), out opacity))
            {
                Debug.LogException(new InvalidCastException("[KSP_PostProcessing]: Error parsing [Opacity] for module [Vignette]! Disabling Vignette!"));
                succeeded = false;
                return settings;
            }
            else
            {
                settings.opacity = opacity;
            }

            if (!float.TryParse(VNode.GetValue("Roundness"), out roundness))
            {
                Debug.LogException(new InvalidCastException("[KSP_PostProcessing]: Error parsing [Roundness] for module [Vignette]! Disabling Vignette!"));
                succeeded = false;
                return settings;
            }
            else
            {
                settings.roundness = roundness;
            }

            if (!float.TryParse(VNode.GetValue("Smoothness"), out smoothness))
            {
                Debug.LogException(new InvalidCastException("[KSP_PostProcessing]: Error parsing [Smoothness] for module [Vignette]! Disabling Vignette!"));
                succeeded = false;
                return settings;
            }
            else
            {
                settings.smoothness = smoothness;
            }

            if (!bool.TryParse(VNode.GetValue("Rounded"), out rounded))
            {
                Debug.LogException(new InvalidCastException("[KSP_PostProcessing]: Error parsing [Rounded] for module [Vignette]! Disabling Vignette!"));
                succeeded = false;
                return settings;
            }
            else
            {
                settings.rounded = rounded;
            }

            try
            {
                mask = GameDatabase.Instance.GetTexture(VNode.GetValue("Mask"), false);
            }
            catch
            {
                Debug.LogException(new InvalidCastException("[KSP_PostProcessing]: Error parsing [Mask] for module [Vignette]! Disabling Vignette!"));
                succeeded = false;
                return settings;
            }
            settings.mask = mask;

            if (!int.TryParse(VNode.GetValue("Mode"), out mode))
            {
                Debug.LogException(new InvalidCastException("[KSP_PostProcessing]: Error parsing [Mode] for module [Vignette]! Disabling Vignette!"));
                succeeded = false;
                return settings;
            }
            else
            {
                //Use integer index to grab enumerator value
                settings.mode = (VignetteModel.Mode)Mathf.Clamp01(mode);
            }

            //We're done here.
            return settings;
        }
        
        //The big one.
        internal static ColorGradingModel.Settings ProcessColorGrading(ConfigNode CGNode, out bool succeeded)
        {
            ColorGradingModel.Settings settings = new ColorGradingModel.Settings();
            succeeded = true;
            
            string preset = CGNode.GetValue("Preset");

            //Are we going for ACES filmic?
            if (preset == "ACES")
            {
                //ACES filmic tonemapper preset setup
                settings.tonemapping.tonemapper = ColorGradingModel.Tonemapper.ACES;
                settings.basic = ColorGradingModel.BasicSettings.defaultSettings;
                settings.channelMixer = ColorGradingModel.ChannelMixerSettings.defaultSettings;
                settings.colorWheels = ColorGradingModel.ColorWheelsSettings.defaultSettings;
                settings.curves = ColorGradingModel.CurvesSettings.defaultSettings;
                settings.tonemapping = ColorGradingModel.TonemappingSettings.defaultSettings;
                settings.tonemapping.tonemapper = ColorGradingModel.Tonemapper.ACES;
                return settings;
            }
            //No? Neutral then maybe?
            else if (preset == "Neutral")
            {
                settings.tonemapping.tonemapper = ColorGradingModel.Tonemapper.Neutral;
                settings.basic = ColorGradingModel.BasicSettings.defaultSettings;
                settings.channelMixer = ColorGradingModel.ChannelMixerSettings.defaultSettings;
                settings.colorWheels = ColorGradingModel.ColorWheelsSettings.defaultSettings;
                settings.curves = ColorGradingModel.CurvesSettings.defaultSettings;
                settings.tonemapping = ColorGradingModel.TonemappingSettings.defaultSettings;
                return settings;
            }
            //We either ignored preset or want no preset. Noted.
            else
            {
                settings.tonemapping.tonemapper = ColorGradingModel.Tonemapper.None;
                ColorGradingModel.BasicSettings baseSettings;
                ConfigNode BaseNode = CGNode.GetNode("Base");
                baseSettings = CG_ProcessBaseSettings(BaseNode, out succeeded);
                //If the previous operation failed, we're not continuing this.
                if (!succeeded)
                {
                    return settings;
                }
                ColorGradingModel.ChannelMixerSettings channelSettings = new ColorGradingModel.ChannelMixerSettings();
                ConfigNode mixerNode = CGNode.GetNode("ColorMixer");
                channelSettings = CG_ProcessMixerSettings(mixerNode, out succeeded);
                if (!succeeded)
                {
                    return settings;
                }
                ColorGradingModel.ColorWheelsSettings colorWheelSettings = new ColorGradingModel.ColorWheelsSettings();
                ConfigNode wheelNode = CGNode.GetNode("ColorWheels");
                colorWheelSettings = CG_ProcessColorWheels(wheelNode, out succeeded);
                if (!succeeded) //Has the last operation thrown an exception?
                {
                    return settings; //Then abort here.
                }
                ColorGradingModel.CurvesSettings curveSettings = new ColorGradingModel.CurvesSettings();
                ConfigNode curveNode = CGNode.GetNode("ColorCurves");
                curveSettings = CG_ProcessCurveSettings(curveNode, out succeeded);
                if (!succeeded)
                {
                    return settings; //Abort, abort!
                }
                ColorGradingModel.TonemappingSettings tonemapperSettings = new ColorGradingModel.TonemappingSettings();
                ConfigNode mapperNode = CGNode.GetNode("Tonemapper");
                tonemapperSettings = CG_ProcessTonemapperSettings(mapperNode, out succeeded);
                if (!succeeded)
                {
                    return settings;
                }

                settings.basic = baseSettings;
                settings.channelMixer = channelSettings;
                settings.colorWheels = colorWheelSettings;
                settings.curves = curveSettings;
                settings.tonemapping = tonemapperSettings;


                return settings;
            }
        }
        #endregion

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
                Debug.LogException(new InvalidCastException("[KSP_PostProcessing]: Error parsing [Contrast] for module [ColorGrading_Base]! Disabling Color Grading!"));
                succeeded = false;
                return settings;
            }
            else
            {
                settings.contrast = Mathf.Clamp(contrast, 0f, 2f);
            }

            if (!float.TryParse(baseNode.GetValue("Hue_Shift"), out hueShift))
            {
                Debug.LogException(new InvalidCastException("[KSP_PostProcessing]: Error parsing [Hue_Shift] for module [ColorGrading_Base]! Disabling Color Grading!"));
                succeeded = false;
                return settings;
            }
            else
            {
                settings.hueShift = Mathf.Clamp(hueShift, -180f, 180f);
            }

            if (!float.TryParse(baseNode.GetValue("Post_Exposure"), out postExposure))
            {
                Debug.LogException(new InvalidCastException("[KSP_PostProcessing]: Error parsing [Post_Exposure] for module [ColorGrading_Base]! Disabling Color Grading!"));
                succeeded = false;
                return settings;
            }
            else
            {
                settings.postExposure = postExposure;
            }

            if (!float.TryParse(baseNode.GetValue("Saturation"), out saturation))
            {
                Debug.LogException(new InvalidCastException("[KSP_PostProcessing]: Error parsing [Saturation] for module [ColorGrading_Base]! Disabling Color Grading!"));
                succeeded = false;
                return settings;
            }
            else
            {
                settings.saturation = Mathf.Clamp(saturation, 0f, 2f);
            }

            if (!float.TryParse(baseNode.GetValue("Tint"), out tint))
            {
                Debug.LogException(new InvalidCastException("[KSP_PostProcessing]: Error parsing [Tint] for module [ColorGrading_Base]! Disabling Color Grading!"));
                succeeded = false;
                return settings;
            }
            else
            {
                settings.tint = Mathf.Clamp(tint, -100f, 100f);
            }

            if (!float.TryParse(baseNode.GetValue("Temperature"), out temperature))
            {
                Debug.LogException(new InvalidCastException("[KSP_PostProcessing]: Error parsing [Temperature] for module [ColorGrading_Base]! Disabling Color Grading!"));
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
                Debug.LogException(new InvalidCastException("[KSP_PostProcessing]: Error parsing [Red] for module [ColorGrading_Mixer]! Disabling Color Grading!"));
                succeeded = false;
                return settings;
            }

            try
            {
                g = ConfigNode.ParseVector3(mixNode.GetValue("Green"));
                settings.green = g;
            }
            catch(InvalidCastException)
            {
                Debug.LogException(new InvalidCastException("[KSP_PostProcessing]: Error parsing [Green] for module [ColorGrading_Mixer]! Disabling Color Grading!"));
                succeeded = false;
                return settings;
            }

            try
            {
                b = ConfigNode.ParseVector3(mixNode.GetValue("Blue"));
                settings.blue = b;
            }
            catch(InvalidCastException)
            {
                Debug.LogException(new InvalidCastException("[KSP_PostProcessing]: Error parsing [Blue] for module [ColorGrading_Mixer]! Disabling Color Grading!"));
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
                    Debug.LogException(new InvalidCastException("[KSP_PostProcessing]: Error parsing [Gain_Linear] for module [ColorGrading_ColorWheels]! Disabling Color Grading!"));
                    succeeded = false;
                    return settings;
                }
                try
                {
                    wheelSettings.gamma = ConfigNode.ParseColor(wNode.GetValue("Gamma"));
                }
                catch (InvalidCastException)
                {
                    Debug.LogException(new InvalidCastException("[KSP_PostProcessing]: Error parsing [Gamma_Linear] for module [ColorGrading_ColorWheels]! Disabling Color Grading!"));
                    succeeded = false;
                    return settings;
                }
                try
                {
                    wheelSettings.lift = ConfigNode.ParseColor(wNode.GetValue("Lift"));
                }
                catch (InvalidCastException)
                {
                    Debug.LogException(new InvalidCastException("[KSP_PostProcessing]: Error parsing [] for module [ColorGrading_ColorWheels]! Disabling Color Grading!"));
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
                catch(InvalidCastException)
                {
                    Debug.LogException(new InvalidCastException("[KSP_PostProcessing]: Error parsing [Offset_Logarithmic] for module [ColorGrading_ColorWheels]! Disabling Color Grading!"));
                    succeeded = false;
                    return settings;
                }

                try
                {
                    wheelSettings.power = ConfigNode.ParseColor(wNode.GetValue("Power"));
                }
                catch(InvalidCastException)
                {
                    Debug.LogException(new InvalidCastException("[KSP_PostProcessing]: Error parsing [Power_Logarithmic] for module [ColorGrading_ColorWheels]! Disabling Color Grading!"));
                    succeeded = false;
                    return settings;
                }

                try
                {
                    wheelSettings.slope = ConfigNode.ParseColor(wNode.GetValue("Slope"));
                }
                catch(InvalidCastException)
                {
                    Debug.LogException(new InvalidCastException("[KSP_PostProcessing]: Error parsing [Slope_Logarithmic] for module [ColorGrading_ColorWheels]! Disabling Color Grading!"));
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
                Debug.LogException(new InvalidCastException("[KSP_PostProcessing]: Error parsing [] for module [ColorGrading_ColorWheels]! Disabling Color Grading!"));
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
                Debug.LogException(new InvalidCastException("[KSP_PostProcessing]: Error parsing curve [Blue] for module [ColorGrading_Curves]! Disabling Color Grading!"));
                succeeded = false;
                return settings;
            }
            else
            {
                settings.blue = blue;
            }

            if (!TryParseColorCurve(Green, out green))
            {
                Debug.LogException(new InvalidCastException("[KSP_PostProcessing]: Error parsing curve [Green] for module [ColorGrading_Curves]! Disabling Color Grading!"));
                succeeded = false;
                return settings;
            }
            else
            {
                settings.green = green;
            }

            if (!TryParseColorCurve(hVh, out hueVhue))
            {
                Debug.LogException(new InvalidCastException("[KSP_PostProcessing]: Error parsing curve [Hue_Versus_Hue] for module [ColorGrading_Curves]! Disabling Color Grading!"));
                succeeded = false;
                return settings;
            }
            else
            {
                settings.hueVShue = hueVhue;
            }

            if (!TryParseColorCurve(hVs, out hueVsat))
            {
                Debug.LogException(new InvalidCastException("[KSP_PostProcessing]: Error parsing curve [Hue_Versus_Saturation] for module [ColorGrading_Curves]! Disabling Color Grading!"));
                succeeded = false;
                return settings;
            }
            else
            {
                settings.hueVSsat = hueVsat;
            }

            if (!TryParseColorCurve(lVs, out lumVsat))
            {
                Debug.LogException(new InvalidCastException("[KSP_PostProcessing]: Error parsing curve [Luminosity_Versus_Saturation] for module [ColorGrading_Curves]! Disabling Color Grading!"));
                succeeded = false;
                return settings;
            }
            else
            {
                settings.lumVSsat = lumVsat;
            }

            if (!TryParseColorCurve(Master, out master))
            {
                Debug.LogException(new InvalidCastException("[KSP_PostProcessing]: Error parsing curve [Master] for module [ColorGrading_Curves]! Disabling Color Grading!"));
                succeeded = false;
                return settings;
            }
            else
            {
                settings.master = master;
            }

            if (!TryParseColorCurve(Red, out red))
            {
                Debug.LogException(new InvalidCastException("[KSP_PostProcessing]: Error parsing curve [Red] for module [ColorGrading_Curves]! Disabling Color Grading!"));
                succeeded = false;
                return settings;
            }
            else
            {
                settings.red = red;
            }

            if (!TryParseColorCurve(sVs, out satVsat))
            {
                Debug.LogException(new InvalidCastException("[KSP_PostProcessing]: Error parsing curve [Saturation_Versus_Saturation] for module [ColorGrading_Curves]! Disabling Color Grading!"));
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
                Debug.LogException(new InvalidCastException("[KSP_PostProcessing]: Error parsing [blackIn] for module [ColorGrading_Tonemapper]! Disabling Color Grading!"));
                succeeded = false;
                return settings;
            }
            else
            {
                settings.neutralBlackIn = Mathf.Clamp(blackIn, -0.1f, 0.1f);
            }

            if (!float.TryParse(TMNode.GetValue("Black_Out"), out blackOut))
            {
                Debug.LogException(new InvalidCastException("[KSP_PostProcessing]: Error parsing [Black_Out] for module [ColorGrading_Tonemapper]! Disabling Color Grading!"));
                succeeded = false;
                return settings;
            }
            else
            {
                settings.neutralBlackOut = Mathf.Clamp(blackOut, -0.09f, 0.1f);
            }

            if (!float.TryParse(TMNode.GetValue("White_In"), out whiteIn))
            {
                Debug.LogException(new InvalidCastException("[KSP_PostProcessing]: Error parsing [White_In] for module [ColorGrading_Tonemapper]! Disabling Color Grading!"));
                succeeded = false;
                return settings;
            }
            else
            {
                settings.neutralWhiteIn = Mathf.Clamp(whiteIn, 1f, 20f);
            }

            if (!float.TryParse(TMNode.GetValue("White_Out"), out whiteOut))
            {
                Debug.LogException(new InvalidCastException("[KSP_PostProcessing]: Error parsing [White_Out] for module [ColorGrading_Tonemapper]! Disabling Color Grading!"));
                succeeded = false;
                return settings;
            }
            else
            {
                settings.neutralWhiteOut = Mathf.Clamp(whiteOut, 1f, 19f);
            }

            if (!float.TryParse(TMNode.GetValue("White_Clip"), out whiteClip))
            {
                Debug.LogException(new InvalidCastException("[KSP_PostProcessing]: Error parsing [White_Clip] for module [ColorGrading_Tonemapper]! Disabling Color Grading!"));
                succeeded = false;
                return settings;
            }
            else
            {
                settings.neutralWhiteClip = Mathf.Clamp(whiteClip, 1f, 10f);
            }

            if (!float.TryParse(TMNode.GetValue("White_Level"), out whiteLevel))
            {
                Debug.LogException(new InvalidCastException("[KSP_PostProcessing]: Error parsing [White_Level] for module [ColorGrading_Tonemapper]! Disabling Color Grading!"));
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


        //This is gonna be interesting...
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
                Debug.LogException(new InvalidCastException("[KSP_PostProcessing_TryParseColorCurve]: Error parsing [Zero]!"));
                curve = null;
                return false;
            }

            //Set up the curve's Loop value
            bool isLoop;
            if (!bool.TryParse(target.GetValue("IsLooped"), out isLoop))
            {
                Debug.LogException(new InvalidCastException("[KSP_PostProcessing_TryParseColorCurve]: Error parsing [IsLooped]!"));
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
                Debug.LogException(new InvalidCastException("[KSP_PostProcessing_TryParseColorCurve]: Error parsing [Bounds]!"));
                curve = null;
                return false;
            }

            //Set up the curve's CurveKeys
            ConfigNode keyNode = target.GetNode("Curve");
            string[] Keys = keyNode.GetValues("Key"); //Get the curve keys
            //Make Vector2 list
            List<Vector2> KeyValues = new List<Vector2>();
            if (Keys.Length >= 0) //Check if curve keys are specified
            {
                //We have no array elements? Then we can't work.
                curve = null;
                return false;
            }
            else
            {
                //We (try to) process all keys
                for(int x = 0; x < Keys.Length; x++)
                {
                    try
                    {
                        KeyValues.Add(ParseFromString(Keys[x]));
                    }
                    catch
                    {
                        Debug.LogException(new InvalidCastException("[KSP_PostProcessing_TryParseColorCurve]: Error parsing curve key [" + Keys[x] + "]!"));
                        curve = null;
                        return false;
                    }
                }
            }

            //Set up the AnimationCurve
            foreach(Vector2 key in KeyValues)
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
                Debug.LogException(new InvalidCastException("[KSP_PostProcessing_TryParseColorCurve]: Error finalizing curve!"));
                curve = null;
                return false;
            }
        }


        static Vector2 Flip(Vector2 input)
        {
            return new Vector2(input.y, input.x);
        }

        static Vector2 ParseFromString(string input)
        {
            //Split input into 2
            List<Char> charList = new List<char>();
            charList.Add(',');
            Char[] separator = charList.ToArray();
            string[] parts = input.Split(separator, 2);
            try
            {
                return new Vector2(float.Parse(parts[0]), float.Parse(parts[1]));
            }
            catch
            {
                Debug.LogException(new InvalidOperationException("[KSP_PostProcessing_ParseFromString]: Error parsing string [" + input + "]! Returning empty Vector2!"));
                return Vector2.zero;
            }
        }
    }


    /*
     ToDo: instead of Start(), subscribe to OnSceneLoad Unity events to check if Post-Processing is necessary
     For example, we don't really need stuff like Grain in the astronaut complex
     
     */
}
