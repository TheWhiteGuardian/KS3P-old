using System.IO;
using System.Collections.Generic;
using UnityEngine;

namespace KSP_PostProcessing
{
    public static class ConfigWriter
    {
        static void Write(string tab, string name, object value, object defaultValue, ref List<string> data)
        {
            if(value != defaultValue)
            {
                data.Add(tab + name + " = " + value.ToString());
            }
        }
        static void Write(string ttab, string qtab, string name, ColorGradingCurve value, ColorGradingCurve defaultValue, ref List<string> data)
        {
            if(value.Equals(defaultValue))
            {
                data.Add(ttab + name);
                data.Add(ttab + "{");

                Write(qtab, "zero", value.ZeroValue, 0f, ref data);
                Write(qtab, "loop", value.IsLooped, false, ref data);
                Write(qtab, "bounds", value.Range, Vector2.zero, ref data);

                foreach(Keyframe key in value.curve.keys)
                {
                    data.Add(qtab + "key = " + key.time + ", " + key.value + ", " + key.inTangent + ", " + key.outTangent);
                }

                data.Add(ttab + "}");
            }
        }
        
        /*
        static void ToNode(AntialiasingModel aaModel, ref List<string> data, string tab, string doubletab)
        {
            if(aaModel.enabled)
            {
                AntialiasingModel.TaaSettings settings = aaModel.settings.taaSettings;
                AntialiasingModel.TaaSettings defaultsettings = AntialiasingModel.TaaSettings.defaultSettings;
                data.Add(tab + "Anti_Aliasing");
                data.Add(tab + "{");

                // method
                AntialiasingModel.Method chosenmethod = aaModel.settings.method;
                if(chosenmethod != AntialiasingModel.Settings.defaultSettings.method)
                {
                    data.Add(doubletab + "method = " + chosenmethod.ToString("g"));
                }

                // fxaapreset
                AntialiasingModel.FxaaPreset chosenpreset = aaModel.settings.fxaaSettings.preset;
                if(chosenpreset != AntialiasingModel.FxaaSettings.defaultSettings.preset)
                {
                    data.Add(doubletab + "fxaapreset = " + chosenpreset.ToString("g"));
                }

                // jitterspread
                Write(doubletab, "jitterspread", settings.jitterSpread, defaultsettings.jitterSpread, ref data);

                // sharpen
                Write(doubletab, "sharpen", settings.sharpen, defaultsettings.sharpen, ref data);
                
                // motionblending
                Write(doubletab, "motionblending", settings.motionBlending, defaultsettings.motionBlending, ref data);

                // stationaryblending
                Write(doubletab, "stationaryblending", settings.stationaryBlending, defaultsettings.stationaryBlending, ref data);

                data.Add(tab + "}");
            }
        }
        static void ToNode(AntialiasingModel aaModel, ref ConfigNode node)
        {
            if(aaModel.enabled)
            {
                ConfigNode aaNode = new ConfigNode("Anti_Aliasing");

                // write values to node if they differ from default values
                boo
                
                node.AddNode(aaNode);
            }
        }
        static void ToNode(AmbientOcclusionModel aoModel, ref List<string> data, string tab, string doubletab)
        {
            if(aoModel.enabled)
            {
                AmbientOcclusionModel.Settings defaultSettings = AmbientOcclusionModel.Settings.defaultSettings;
                var settings = aoModel.settings;
                data.Add(tab + "Anti_Aliasing");
                data.Add(tab + "{");
                Write(doubletab, "ambientonly", settings.ambientOnly, defaultSettings.ambientOnly, ref data);
                Write(doubletab, "downsampling", settings.downsampling, defaultSettings.downsampling, ref data);
                Write(doubletab, "forceforwardcompatibility", settings.forceForwardCompatibility, defaultSettings.forceForwardCompatibility, ref data);
                Write(doubletab, "highprecision", settings.highPrecision, defaultSettings.highPrecision, ref data);
                Write(doubletab, "intensity", settings.intensity, defaultSettings.intensity, ref data);
                Write(doubletab, "radius", settings.radius, defaultSettings.radius, ref data);
                Write(doubletab, "samplecount", settings.sampleCount, defaultSettings.sampleCount, ref data);
                data.Add(tab + "}");
            }
        }
        static void ToNode(DepthOfFieldModel dofModel, ref List<string> data, string tab, string doubletab)
        {
            if(dofModel.enabled)
            {
                DepthOfFieldModel.Settings defaultSettings = DepthOfFieldModel.Settings.defaultSettings;
                var settings = dofModel.settings;
                data.Add(tab + "Depth_Of_Field");
                data.Add(tab + "{");
                Write(doubletab, "aperture", settings.aperture, defaultSettings.aperture, ref data);
                Write(doubletab, "focallength", settings.focalLength, defaultSettings.focalLength, ref data);
                Write(doubletab, "focusdistance", settings.focusDistance, defaultSettings.focusDistance, ref data);
                Write(doubletab, "kernelsize", settings.kernelSize, defaultSettings.kernelSize, ref data);
                Write(doubletab, "usecamerafov", settings.useCameraFov, defaultSettings.useCameraFov, ref data);
                data.Add(tab + "}");
            }
        }
        static void ToNode(MotionBlurModel mbModel, ref List<string> data, string tab, string doubletab)
        {
            if(mbModel.enabled)
            {
                MotionBlurModel.Settings defaultSettings = MotionBlurModel.Settings.defaultSettings;
                var settings = mbModel.settings;
                data.Add(tab + "Motion_Blur");
                data.Add(tab + "{");
                Write(doubletab, "frameblending", settings.frameBlending, defaultSettings.frameBlending, ref data);
                Write(doubletab, "samplecount", settings.sampleCount, defaultSettings.sampleCount, ref data);
                Write(doubletab, "shutterangle", settings.shutterAngle, defaultSettings.shutterAngle, ref data);
                data.Add(tab + "}");
            }
        }
        static void ToNode(EyeAdaptationModel eaModel, ref List<string> data, string tab, string doubletab)
        {
            if(eaModel.enabled)
            {
                var settings = eaModel.settings;
                var defaultSettings = EyeAdaptationModel.Settings.defaultSettings;
                data.Add(tab + "Eye_Adaptation");
                data.Add(tab + "{");
                if(settings.adaptationType != defaultSettings.adaptationType)
                {
                    data.Add(doubletab + "adaptationtype = " + settings.adaptationType.ToString("g"));
                }
                Write(doubletab, "dynamickeyvalue", settings.dynamicKeyValue, defaultSettings.dynamicKeyValue, ref data);
                Write(doubletab, "highpercent", settings.highPercent, defaultSettings.highPercent, ref data);
                Write(doubletab, "keyvalue", settings.keyValue, defaultSettings.keyValue, ref data);
                Write(doubletab, "logmax", settings.logMax, defaultSettings.logMax, ref data);
                Write(doubletab, "logmin", settings.logMin, defaultSettings.logMin, ref data);
                Write(doubletab, "lowpercent", settings.lowPercent, defaultSettings.lowPercent, ref data);
                Write(doubletab, "maxluminance", settings.maxLuminance, defaultSettings.maxLuminance, ref data);
                Write(doubletab, "minluminance", settings.minLuminance, defaultSettings.minLuminance, ref data);
                Write(doubletab, "speeddown", settings.speedDown, defaultSettings.speedDown, ref data);
                Write(doubletab, "speedup", settings.speedUp, defaultSettings.speedUp, ref data);
                data.Add(tab + "}");
            }
        }
        static void ToNode(BloomModel bModel, ref List<string> data, string tab, string doubletab, string path)
        {
            if(bModel.enabled)
            {
                data.Add(tab + "Bloom");
                data.Add(tab + "{");
                var settings = bModel.settings;
                var defaultSettings = BloomModel.Settings.defaultSettings;
                if(path != null)
                {
                    Write(doubletab, "dirtintensity", settings.lensDirt.intensity, defaultSettings.lensDirt.intensity, ref data);
                    data.Add(doubletab + "dirttexture = " + path);
                }
                Write(doubletab, "bloomantiflicker", settings.bloom.antiFlicker, defaultSettings.bloom.antiFlicker, ref data);
                Write(doubletab, "bloomintensity", settings.bloom.intensity, defaultSettings.bloom.intensity, ref data);
                Write(doubletab, "bloomradius", settings.bloom.radius, defaultSettings.bloom.radius, ref data);
                Write(doubletab, "bloomsoftknee", settings.bloom.softKnee, defaultSettings.bloom.softKnee, ref data);
                Write(doubletab, "bloomthreshold", settings.bloom.threshold, defaultSettings.bloom.threshold, ref data);
                data.Add(tab + "}");
            }
        }
        static void ToNode(ColorGradingModel cgModel, ref List<string> data, string tab, string doubletab)
        {
            if(cgModel.enabled)
            {
                var basicSettings = cgModel.settings.basic;
                var basicDefault = ColorGradingModel.BasicSettings.defaultSettings;

                var mixerSettings = cgModel.settings.channelMixer;
                var mixerDefault = ColorGradingModel.ChannelMixerSettings.defaultSettings;

                var wheelSettings = cgModel.settings.colorWheels;
                var wheelDefault = ColorGradingModel.ColorWheelsSettings.defaultSettings;

                var curveSettings = cgModel.settings.curves;
                var curveDefault = ColorGradingModel.CurvesSettings.defaultSettings;

                var mapSettings = cgModel.settings.tonemapping;
                var mapDefault = ColorGradingModel.TonemappingSettings.defaultSettings;

                var tripletab = tab + tab + tab;

                data.Add(tab + "Color_Grading");
                data.Add(tab + "{");

                if(basicSettings.Equals(basicDefault))
                {
                    data.Add(doubletab + "Basic");
                    data.Add(doubletab + "{");

                    Write(tripletab, "contrast", basicSettings.contrast, basicDefault.contrast, ref data);
                    Write(tripletab, "hueshift", basicSettings.hueShift, basicDefault.hueShift, ref data);
                    Write(tripletab, "postexposure", basicSettings.postExposure, basicDefault.postExposure, ref data);
                    Write(tripletab, "saturation", basicSettings.saturation, basicDefault.saturation, ref data);
                    Write(tripletab, "temperature", basicSettings.temperature, basicDefault.temperature, ref data);
                    Write(tripletab, "tint", basicSettings.tint, basicSettings.tint, ref data);

                    data.Add(doubletab + "}");
                }
                if(mixerSettings.Equals(mixerDefault))
                {
                    data.Add(doubletab + "Mixer");
                    data.Add(doubletab + "{");

                    Write(tripletab, "red", mixerSettings.red, mixerDefault.red, ref data);
                    Write(tripletab, "green", mixerSettings.green, mixerDefault.green, ref data);
                    Write(tripletab, "blue", mixerSettings.blue, mixerDefault.blue, ref data);

                    data.Add(doubletab + "}");
                }
                if(wheelSettings.Equals(wheelDefault))
                {
                    data.Add(doubletab + "Wheels");
                    data.Add(doubletab + "{");
                    
                    if(wheelSettings.mode != wheelDefault.mode)
                    {
                        data.Add(tripletab + "mode = " + wheelSettings.mode);
                    }
                    Write(tripletab, "gain", wheelSettings.linear.gain, wheelDefault.linear.gain, ref data);
                    Write(tripletab, "gamma", wheelSettings.linear.gamma, wheelDefault.linear.gamma, ref data);
                    Write(tripletab, "lift", wheelSettings.linear.lift, wheelDefault.linear.lift, ref data);
                    Write(tripletab, "offset", wheelSettings.log.offset, wheelDefault.log.offset, ref data);
                    Write(tripletab, "power", wheelSettings.log.power, wheelDefault.log.power, ref data);
                    Write(tripletab, "slope", wheelSettings.log.slope, wheelDefault.log.slope, ref data);

                    data.Add(doubletab + "}");
                }
                if(curveSettings.Equals(curveDefault))
                {
                    string quattab = tripletab + tab;
                    data.Add(doubletab + "Curves");
                    data.Add(doubletab + "{");
                    Write(tripletab, quattab, "master", curveSettings.master, curveDefault.master, ref data);
                    Write(tripletab, quattab, "red", curveSettings.red, curveDefault.red, ref data);
                    Write(tripletab, quattab, "green", curveSettings.green, curveDefault.green, ref data);
                    Write(tripletab, quattab, "blue", curveSettings.blue, curveDefault.blue, ref data);
                    Write(tripletab, quattab, "hueversushue", curveSettings.hueVShue, curveDefault.hueVShue, ref data);
                    Write(tripletab, quattab, "hueversussaturation", curveSettings.hueVSsat, curveDefault.hueVSsat, ref data);
                    Write(tripletab, quattab, "luminosityversussaturation", curveSettings.lumVSsat, curveDefault.lumVSsat, ref data);
                    Write(tripletab, quattab, "saturationversussaturation", curveSettings.satVSsat, curveDefault.satVSsat, ref data);
                    data.Add(doubletab + "}");
                }
                if(mapSettings.Equals(mapDefault))
                {
                    data.Add(doubletab + "Tonemapper");
                    data.Add(doubletab + "{");

                    if(mapSettings.tonemapper != mapDefault.tonemapper)
                    {
                        data.Add(tripletab + "tonemapper = " + mapSettings.tonemapper.ToString("g"));
                    }
                    Write(tripletab, "blackin", mapSettings.neutralBlackIn, mapDefault.neutralBlackIn, ref data);
                    Write(tripletab, "blackout", mapSettings.neutralBlackOut, mapDefault.neutralBlackOut, ref data);
                    Write(tripletab, "whiteclip", mapSettings.neutralWhiteClip, mapDefault.neutralWhiteClip, ref data);
                    Write(tripletab, "whitein", mapSettings.neutralWhiteIn, mapDefault.neutralWhiteIn, ref data);
                    Write(tripletab, "whitelevel", mapSettings.neutralWhiteLevel, mapDefault.neutralWhiteLevel, ref data);
                    Write(tripletab, "whiteout", mapSettings.neutralWhiteOut, mapDefault.neutralWhiteOut, ref data);

                    data.Add(doubletab + "}");
                }

                data.Add(tab + "}");
            }
        }
        static void ToNode(UserLutModel ulModel, ref List<string> data, string tab, string doubletab, string path)
        {
            if(ulModel.enabled)
            {
                var settings = ulModel.settings;
                var defaultSettings = UserLutModel.Settings.defaultSettings;
                data.Add(tab + "User_Lut");
                data.Add(tab + "{");

                Write(doubletab, "contribution", settings.contribution, defaultSettings.contribution, ref data);
                data.Add(doubletab + "lut = " + path);

                data.Add(tab + "}");
            }
        }
        static void ToNode(ChromaticAberrationModel caModel, ref List<string> data, string tab, string doubletab, string path)
        {
            if(caModel.enabled)
            {
                var settings = caModel.settings;
                var defaultSettings = ChromaticAberrationModel.Settings.defaultSettings;
                data.Add(tab + "Chromatic_Abberation");
                data.Add(tab + "{");
                Write(doubletab, "intensity", settings.intensity, defaultSettings.intensity, ref data);
                data.Add(doubletab + "texture = " + path);
                data.Add(tab + "}");
            }
        }
        static void ToNode(GrainModel gModel, ref List<string> data, string tab, string doubletab)
        {
            if (gModel.enabled)
            {
                var settings = gModel.settings;
                var defaultSettings = GrainModel.Settings.defaultSettings;

                data.Add(tab + "Grain");
                data.Add(tab + "{");

                Write(doubletab, "colored", settings.colored, defaultSettings.colored, ref data);
                Write(doubletab, "intensity", settings.intensity, defaultSettings.intensity, ref data);
                Write(doubletab, "luminancecontribution", settings.luminanceContribution, defaultSettings.luminanceContribution, ref data);
                Write(doubletab, "size", settings.size, defaultSettings.size, ref data);

                data.Add(tab + "}");
            }
        }
        static void ToNode(VignetteModel vModel, ref List<string> data, string tab, string doubletab, string path)
        {
            if (vModel.enabled)
            {
                var settings = vModel.settings;
                var defaultSettings = VignetteModel.Settings.defaultSettings;

                data.Add(tab + "Vignette");
                data.Add(tab + "{");

                Write(doubletab, "center", settings.center, defaultSettings.center, ref data);
                Write(doubletab, "color", settings.color, defaultSettings.color, ref data);
                Write(doubletab, "intensity", settings.intensity, defaultSettings.intensity, ref data);
                data.Add(doubletab + "mask = " + path);
                if(settings.mode != defaultSettings.mode)
                {
                    data.Add(doubletab + "mode = " + settings.mode.ToString("g"));
                }
                Write(doubletab, "opacity", settings.opacity, defaultSettings.opacity, ref data);
                Write(doubletab, "rounded", settings.rounded, defaultSettings.rounded, ref data);
                Write(doubletab, "roundndess", settings.roundness, defaultSettings.roundness, ref data);
                Write(doubletab, "smoothness", settings.smoothness, defaultSettings.smoothness, ref data);

                data.Add(tab + "}");
            }
        }
        static void ToNode(ScreenSpaceReflectionModel ssrModel, ref List<string> data, string tab, string doubletab)
        {
            if(ssrModel.enabled)
            {
                var settings = ssrModel.settings;
                var defaultSettings = ScreenSpaceReflectionModel.Settings.defaultSettings;
                var s_r = settings.reflection;
                var d_r = defaultSettings.reflection;

                data.Add(tab + "Screen_Space_Reflection");
                data.Add(tab + "{");

                if(s_r.blendType != d_r.blendType)
                {
                    data.Add(doubletab + "blendtype = " + s_r.blendType.ToString("g"));
                }
                if(s_r.reflectionQuality != d_r.reflectionQuality)
                {
                    data.Add(doubletab + "reflectionquality = " + s_r.reflectionQuality.ToString("g"));
                }

                Write(doubletab, "maxdistance", s_r.maxDistance, d_r.maxDistance, ref data);
                Write(doubletab, "iterationcount", s_r.iterationCount, d_r.iterationCount, ref data);
                Write(doubletab, "stepsize", s_r.stepSize, d_r.stepSize, ref data);
                Write(doubletab, "widthmodifier", s_r.widthModifier, d_r.widthModifier, ref data);
                Write(doubletab, "reflectionblur", s_r.reflectionBlur, d_r.reflectionBlur, ref data);
                Write(doubletab, "reflectbackfaces", s_r.reflectBackfaces, d_r.reflectBackfaces, ref data);

                var s_i = settings.intensity;
                var d_i = defaultSettings.intensity;

                Write(doubletab, "reflectionmultiplier", s_i.reflectionMultiplier, d_i.reflectionMultiplier, ref data);
                Write(doubletab, "fadedistance", s_i.fadeDistance, d_i.fadeDistance, ref data);
                Write(doubletab, "fresnelfade", s_i.fresnelFade, d_i.fresnelFade, ref data);
                Write(doubletab, "fresnelfadepower", s_i.fresnelFadePower, d_i.fresnelFadePower, ref data);
                Write(doubletab, "intensity", settings.screenEdgeMask.intensity, defaultSettings.screenEdgeMask.intensity, ref data);

                data.Add(tab + "}");
            }
        }
        */

        static ConfigNode ToNode(Profile profile)
        {
            return null;

            /*
            ConfigNode node = new ConfigNode("Profile");
            ToNode(profile.profile.antialiasing, ref node);
            // write all effects

            ConfigNode wrapper = new ConfigNode();
            wrapper.AddNode(node);

            /*
             public void ExportConfig()
            {
                ConfigNode config = PlanetConfigExporter.CreateConfig(this);
                ConfigNode kopernicus = new ConfigNode("@Kopernicus:NEEDS[!Kopernicus]");
                kopernicus.AddNode(config);
                ConfigNode wrapper = new ConfigNode();
                wrapper.AddNode(kopernicus);
                
                // Save the node
                String dir = "GameData/KittopiaTech/PluginData/" + celestialBody.transform.name + "/" +
                             DateTime.Now.ToString("yyyy-MM-dd_hh-mm-ss");
                Directory.CreateDirectory(KSPUtil.ApplicationRootPath + dir);
                wrapper.Save(dir + "/" + name + ".cfg",
                    "KittopiaTech - a Kopernicus Visual Editor");
}
             */
        }

        public static void ToFile(Profile profile)
        {
            return;
            /*
            char[] space = { ' ' };
            var parts = profile.identifier.Split(space, 2);
            string path = KS3PUtil.GetRoot();
            path = Path.Combine(path, "GameData");
            path = Path.Combine(path, "KS3P");
            path = Path.Combine(path, "Export");
            path = Path.Combine(path, parts[0] + ".txt");
            KS3P.Log("Attempting to export to location: [" + path + "]");
            File.WriteAllLines(path, ToLines(profile).ToArray());
            */
        }

        static List<string> ToLines(Profile profile)
        {
            return null;
            /*
            List<string> lines = new List<string>()
            {
                "Profile",
                "{"
            };

            char[] separator = { ' ' };
            string[] parts = profile.identifier.Split(separator);

            lines.Add("    name = " + parts[0]);
            lines.Add("    author = " + parts[2]);

            string targetscenes = "    scene = ";

            string[] scenenames =
            {
                "mainmenu",
                "spacecenter",
                "vab",
                "sph",
                "trackingstation",
                "flight",
                "eva",
                "iva",
                "mapview"
            };

            for(int i = 0; i < 9; i++)
            {
                if(profile.scenes[i])
                {
                    targetscenes += scenenames[i] + ", ";
                }
            }
            targetscenes = targetscenes.RemoveEnd(2);

            string tab = "    ";
            string doubletab = "        ";

            ToNode(profile.profile.antialiasing, ref lines, tab, doubletab);
            ToNode(profile.profile.ambientOcclusion, ref lines, tab, doubletab);
            ToNode(profile.profile.bloom, ref lines, tab, doubletab, profile.dirtTex);
            ToNode(profile.profile.chromaticAberration, ref lines, tab, doubletab, profile.chromaticTex);
            ToNode(profile.profile.colorGrading, ref lines, tab, doubletab);
            ToNode(profile.profile.depthOfField, ref lines, tab, doubletab);
            if(profile.profile.dithering.enabled)
            {
                lines.Add(doubletab + "Dithering");
                lines.Add(doubletab + "{");
                lines.Add(doubletab + "}");
            }
            ToNode(profile.profile.eyeAdaptation, ref lines, tab, doubletab);
            ToNode(profile.profile.grain, ref lines, tab, doubletab);
            ToNode(profile.profile.motionBlur, ref lines, tab, doubletab);
            ToNode(profile.profile.userLut, ref lines, tab, doubletab, profile.lutTex);
            ToNode(profile.profile.vignette, ref lines, tab, doubletab, profile.vignetteMask);
            ToNode(profile.profile.screenSpaceReflection, ref lines, tab, doubletab);

            lines.Add("}");
            return lines;
            */
        }
    }
}
