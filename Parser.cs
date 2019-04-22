using System;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using System.Collections;
//using UnityEngine;
//using System.Reflection;

namespace KSP_PostProcessing.Parsers
{
    /*
    public static class Parser
    {
        public static bool TryParse(string input, Type targetType, out object output)
        {
            if(targetType == typeof(string))
            {
                output = input;
                return true;
            }
            else if(targetType == typeof(char))
            {
                output = input[0];
                return true;
            }
            else if (targetType == typeof(float) || targetType == typeof(double))
            {
                double parsed;
                if(double.TryParse(input, out parsed))
                {
                    output = Convert.ChangeType(parsed, targetType);
                    return true;
                }
                else
                {
                    output = null;
                    return false;
                }
            }
            else if (targetType == typeof(int) || targetType == typeof(long) || targetType == typeof(short) || targetType == typeof(sbyte))
            {
                long parsed;
                if(long.TryParse(input, out parsed))
                {
                    output = Convert.ChangeType(parsed, targetType);
                    return true;
                }
                else
                {
                    output = null;
                    return false;
                }
            }
            else if (targetType == typeof(uint) || targetType == typeof(ulong) || targetType == typeof(ushort) || targetType == typeof(byte))
            {
                ulong parsed;
                if(ulong.TryParse(input, out parsed))
                {
                    output = Convert.ChangeType(parsed, targetType);
                    return true;
                }
                else
                {
                    output = null;
                    return false;
                }
            }
            else if (targetType.IsEnum)
            {
                try
                {
                    output = Enum.Parse(targetType, input, true);
                    return true;
                }
                catch
                {
                    output = null;
                    return false;
                }
            }
            else if (targetType == typeof(Vector2))
            {
                try
                {
                    string[] parts = input.Split(separator);
                    output = (new Vector2(float.Parse(parts[0]), float.Parse(parts[1])));
                    return true;
                }
                catch
                {
                    output = null;
                    return false;
                }
            }
            else if (targetType == typeof(Vector3))
            {
                try
                {
                    string[] parts = input.Split(separator);
                    output = (new Vector3(float.Parse(parts[0]), float.Parse(parts[1]), float.Parse(parts[2])));
                    return true;
                }
                catch
                {
                    output = null;
                    return false;
                }
            }
            else if (targetType == typeof(Vector4))
            {
                try
                {
                    string[] parts = input.Split(separator);
                    output = (new Vector4(float.Parse(parts[0]), float.Parse(parts[1]), float.Parse(parts[2]), float.Parse(parts[3])));
                    return true;
                }
                catch
                {
                    output = null;
                    return false;
                }
            }
            else if (targetType == typeof(Color))
            {
                if (input.StartsWith("#"))
                {
                    // we're dealing with a hex string
                    Color finalColor;
                    if (ColorUtility.TryParseHtmlString(input, out finalColor))
                    {
                        output = finalColor;
                        return true;
                    }
                    else
                    {
                        output = null;
                        return false;
                    }
                }
                else if (input.StartsWith("RGBA"))
                {
                    try
                    {
                        // we're dealing with an RGBA string
                        string[] parts = input.Split(separator);
                        if (parts.Length == 4)
                        {
                            output = (new Color((float.Parse(parts[0]) / 255f), (float.Parse(parts[1]) / 255f), (float.Parse(parts[2]) / 255f), (float.Parse(parts[3]) / 255f)));
                            return true;
                        }
                        else
                        {
                            output = (new Color((float.Parse(parts[0]) / 255f), (float.Parse(parts[1]) / 255f), (float.Parse(parts[2]) / 255f), 1f));
                            return true;
                        }
                    }
                    catch
                    {
                        output = null;
                        return false;
                    }
                }
                else
                {
                    try
                    {
                        string[] parts = input.Split(separator);
                        if (parts.Length == 4)
                        {
                            output = (new Color(float.Parse(parts[0]), float.Parse(parts[1]), float.Parse(parts[2]), float.Parse(parts[3])));
                            return true;
                        }
                        else
                        {
                            output = (new Color(float.Parse(parts[0]), float.Parse(parts[1]), float.Parse(parts[2]), 1f));
                            return true;
                        }
                    }
                    catch
                    {
                        output = null;
                        return false;
                    }
                }
            }
            else if (targetType == typeof(Texture) || targetType == typeof(Texture2D))
            {
                try
                {
                    output = GameDatabase.Instance.GetTexture(input, false);
                    return true;
                }
                catch
                {
                    output = null;
                    return false;
                }
            }
            else
            {
                output = null;
                return false;
            }
        }

        static char[] separator = { ',' };

        public static bool TryGetFiltered(string target, ConfigNode tosearch, out string result)
        {
            var values = tosearch.values;
            for (int i = 0; i < values.Count; i++)
            {
                if (KS3PUtil.Prepare(values[i].name) == target)
                {
                    result = values[i].value;
                    return true;
                }
            }
            result = string.Empty;
            return false;
        }
        
        public static object ParseObject(ConfigNode data, Type type)
        {
            var item = Activator.CreateInstance(type);
            PropertyInfo[] values = item.GetType().GetProperties();
            string filtered;
            object parsed;
            foreach (PropertyInfo info in values)
            {
                if ((info.PropertyType.IsClass && !(info.PropertyType == typeof(Texture) || info.PropertyType == typeof(Texture2D))) || (info.PropertyType.IsValueType && !info.PropertyType.IsEnum))
                {
                    info.SetValue(item, ParseObject(data, info.PropertyType), null);
                }
                if (TryGetFiltered(KS3PUtil.Prepare(info.Name), data, out filtered))
                {
                    if (TryParse(filtered, info.PropertyType, out parsed))
                    {
                        info.SetValue(item, parsed, null);
                    }
                }
            }
            return item;
        }

        public static T ParseObject<T>(ConfigNode data)
        {
            return (T)ParseObject(data, typeof(T));
        }
    }
    */

    /// <summary>
    /// The base class for all PostProcessingModel parsers
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class Parser<T>
    {
        protected void Warning(string message)
        {
            KS3P.Error("[" + nameof(T) + "]: message");
        }
        protected void Exception(string message, Exception exception)
        {
            KS3P.Exception("[" + nameof(T) + "]: Exception caught while parsing!", exception);
        }
        protected virtual T Default()
        {
            return default(T);
        }
        protected virtual T Parse(ConfigNode.ValueList values) { return Default(); }
        public virtual T Parse(ConfigNode node)
        {
            if(node == null)
            {
                return Default();
            }
            else
            {
                return Parse(node.values);
            }
        }
        public bool TryParseEnum<E>(string enumString, out E parsed) where E : struct
        {
            if(string.IsNullOrEmpty(enumString))
            {
                Warning("Error parsing enumerator [" + nameof(E) + "]: given string that is empty or null!");
                parsed = default(E);
                return false;
            }
            try
            {
                parsed = (E)Enum.Parse(typeof(E), enumString);
                return true;
            }
            catch(Exception e)
            {
                Exception("Exception caught parsing enumerator [" + nameof(E) + "]", e);
                parsed = default(E);
                return false;
            }
        }
        
        protected void ProcessStream(ConfigNode.ValueList valueStream, ref string[] values)
        {
            // for processing
            string[] data = new string[values.Length];
            string formatted;
            int i;

            for(i = 0; i < data.Length; i++)
            {
                data[i] = null;
            }

            // process all values
            foreach(ConfigNode.Value value in valueStream)
            {
                // format this value's name
                formatted = KS3PUtil.Prepare(value.name);
                
                // does our list of requested values contain this name?
                for(i = 0; i < values.Length; i++)
                {
                    // check
                    if(values[i] == formatted)
                    {
                        // we got one. Add it to the dictionary and terminate the loop.
                        data[i] = value.value;
                        break;
                    }
                }
            }
            values = data;
        }

        protected Vector2 ParseVector2(string target)
        {
            float[] data = { 0f, 0f };
            char[] separator = { ',' };

            string[] snippets = target.Split(separator, 2);
            float parsed = 0f;
            for (int i = 0; i < snippets.Length; i++)
            {
                if (float.TryParse(snippets[i], out parsed))
                {
                    data[i] = parsed;
                }
            }
            return new Vector2(data[0], data[1]);
        }
        protected Vector3 ParseVector3(string target)
        {
            float[] data = { 0f, 0f, 0f };
            char[] separator = { ',' };

            string[] snippets = target.Split(separator, 3);
            float parsed = 0f;
            for(int i = 0; i < snippets.Length; i++)
            {
                if(float.TryParse(snippets[i], out parsed))
                {
                    data[i] = parsed;
                }
            }
            return new Vector3(data[0], data[1], data[2]);
        }
        protected Color ParseColor(string target)
        {
            if(target.StartsWith("#"))
            {
                Color parsedHTML;
                if(!ColorUtility.TryParseHtmlString(target, out parsedHTML))
                {
                    parsedHTML = new Color(0f, 0f, 0f, 1f);
                }
                return parsedHTML;
            }
            else
            {
                float[] data = { 0f, 0f, 0f, 1f };
                char[] separator = { ',' };

                string[] snippets = target.Split(separator, 3);
                float parsed = 0f;

                if (target.StartsWith("RGBA"))
                {
                    for (int i = 0; i < snippets.Length; i++)
                    {
                        if (float.TryParse(snippets[i], out parsed))
                        {
                            data[i] = parsed / 255f;
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < snippets.Length; i++)
                    {
                        if (float.TryParse(snippets[i], out parsed))
                        {
                            data[i] = parsed;
                        }
                    }
                }
                return new Color(data[0], data[1], data[2], data[3]);
            }
        }
        
        protected bool TryParseCurve(ConfigNode node, out ColorGradingCurve curve)
        {
            if(node == null)
            {
                curve = null;
                return false;
            }
            else
            {
                curve = ParseCurve(node);
                return true;
            }
        }

        protected ColorGradingCurve ParseCurve(ConfigNode node)
        {
            float zero = 0f;
            bool loop = false;
            Vector2 bounds = Vector2.zero;
            AnimationCurve curve = new AnimationCurve();
            foreach(ConfigNode.Value value in node.values)
            {
                if(KS3PUtil.Prepare(value.name) == "key")
                {
                    curve.TryAdd(value.value);
                }
                else if(KS3PUtil.Prepare(value.name) == "zero")
                {
                    if(!float.TryParse(value.value, out zero))
                    {
                        zero = 0f;
                    }
                }
                else if(KS3PUtil.Prepare(value.name) == "loop")
                {
                    if(!bool.TryParse(value.value, out loop))
                    {
                        loop = false;
                    }
                }
                else if(KS3PUtil.Prepare(value.name) == "bounds")
                {
                    bounds = ParseVector2(value.value);
                }
            }
            return new ColorGradingCurve(curve, zero, loop, bounds);
        }
    }

    /// <summary>
    /// A slight addition to the base Parser class that contains an out parameter for returning the strings assigned to texture paths.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class TextureParser<T> : Parser<T>
    {
        public virtual T Parse(ConfigNode node, out string path)
        {
            if (node == null)
            {
                path = null;
                return Default();
            }
            else
            {
                return Parse(node.values, out path);
            }
        }
        protected virtual T Parse(ConfigNode.ValueList values, out string path)
        {
            path = null;
            return Default();
        }
    }

    public static class MiscParser
    {
        public static bool TryParseEnum<E>(string enumString, out E parsed) where E : struct
        {
            if (string.IsNullOrEmpty(enumString))
            {
                KS3P.Warning("Error parsing enumerator [" + nameof(E) + "]: given string that is empty or null!");
                parsed = default(E);
                return false;
            }
            try
            {
                parsed = (E)Enum.Parse(typeof(E), enumString);
                return true;
            }
            catch (Exception e)
            {
                KS3P.Exception("Exception caught parsing enumerator [" + nameof(E) + "]", e);
                parsed = default(E);
                return false;
            }
        }

        public static BitArray ParseSceneList(string input)
        {
            char[] separator = { ',' };
            string[] parts = input.Split(separator);

            BitArray toreturn = new BitArray(9, false);

            string[] keywords =
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

            if(parts.Length == 0)
            {
                return toreturn;
            }
            else
            {
                try
                {
                    for(int i = 0; i < parts.Length; i++)
                    {
                        parts[i] = KS3PUtil.Prepare(parts[i]);
                        if(keywords.Contains(parts[i]))
                        {
                            toreturn[keywords.IndexOf(parts[i])] = true;
                        }
                    }
                    return toreturn;
                }
                catch(Exception e)
                {
                    KS3P.Exception("[MiscParser]: Exception caught while parsing target scenes! Setting to Disable!", e);
                    return toreturn;
                }
            }
        }
    }
    
    public sealed class AntiAliasingParser : Parser<AntialiasingModel>
    {
        protected override AntialiasingModel Default()
        {
            return new AntialiasingModel()
            {
                enabled = false,
                settings = AntialiasingModel.Settings.defaultSettings
            };
        }

        protected override AntialiasingModel Parse(ConfigNode.ValueList values)
        {
            AntialiasingModel.Settings settings = AntialiasingModel.Settings.defaultSettings;
            AntialiasingModel.TaaSettings taaSettings = settings.taaSettings;

            string[] data = {
              "method",
              "fxaapreset",
              "jitterspread",
              "sharpen",
              "motionblending",
              "stationaryblending",
              "enabled" };
            ProcessStream(values, ref data);

            // parse target AA method
            if(data[0] != null)
            {
                AntialiasingModel.Method method;
                if (TryParseEnum(data[0], out method))
                {
                    settings.method = method;
                }
            }

            // parse Fast Approximate Anti Aliasing
            if(data[1] != null)
            {
                AntialiasingModel.FxaaPreset fxaaPreset;
                if (TryParseEnum(data[1], out fxaaPreset))
                {
                    settings.fxaaSettings = new AntialiasingModel.FxaaSettings()
                    {
                        preset = fxaaPreset
                    };
                }
            }

            // parse Temporal Anti Aliasing
            float calc; // for handling floats
            if(data[2] != null && float.TryParse(data[2], out calc))
            {
                taaSettings.jitterSpread = calc;
            }

            if(data[4] != null && float.TryParse(data[4], out calc))
            {
                taaSettings.motionBlending = calc;
            }

            if(data[3] != null && float.TryParse(data[4], out calc))
            {
                taaSettings.sharpen = calc;
            }

            if(data[5] != null && float.TryParse(data[5], out calc))
            {
                taaSettings.stationaryBlending = calc;
            }

            settings.taaSettings = taaSettings;
            bool aaEnabled;
            if(data[6] == null || !bool.TryParse(data[6], out aaEnabled))
            {
                aaEnabled = true;
            }
            return new AntialiasingModel()
            {
                enabled = aaEnabled,
                settings = settings
            };
        }
    }

    public sealed class AmbientOcclusionParser : Parser<AmbientOcclusionModel>
    {
        protected override AmbientOcclusionModel Default()
        {
            return new AmbientOcclusionModel()
            {
                enabled = false,
                settings = AmbientOcclusionModel.Settings.defaultSettings
            };
        }

        protected override AmbientOcclusionModel Parse(ConfigNode.ValueList values)
        {
            AmbientOcclusionModel.Settings settings = AmbientOcclusionModel.Settings.defaultSettings;
            bool calcBool;
            float calcFloat;

            string[] data = {
                "ambientonly",
                "downsampling",
                "forceforwardcompatibility",
                "highprecision",
                "intensity",
                "radius",
                "enabled",
                "samplecount"
            };

            ProcessStream(values, ref data);

            if(data[0] != null && bool.TryParse(data[0], out calcBool))
            {
                settings.ambientOnly = calcBool;
            }
            if(data[1] != null && bool.TryParse(data[1], out calcBool))
            {
                settings.downsampling = calcBool;
            }
            if(data[2] != null && bool.TryParse(data[2], out calcBool))
            {
                settings.forceForwardCompatibility = calcBool;
            }
            if(data[3] != null && bool.TryParse(data[3], out calcBool))
            {
                settings.highPrecision = calcBool;
            }
            if(data[4] != null && float.TryParse(data[4], out calcFloat))
            {
                settings.intensity = calcFloat;
            }
            if(data[5] != null && float.TryParse(data[5], out calcFloat))
            {
                settings.radius = calcFloat;
            }

            if(data[6] == null || !bool.TryParse(data[6], out calcBool))
            {
                calcBool = true;
            }

            AmbientOcclusionModel.SampleCount sampleCount;
            if(data[7] != null && TryParseEnum(data[7], out sampleCount))
            {
                settings.sampleCount = sampleCount;
            }

            return new AmbientOcclusionModel()
            {
                enabled = calcBool,
                settings = settings
            };
        }
    }

    public sealed class DepthOfFieldParser : Parser<DepthOfFieldModel>
    {
        protected override DepthOfFieldModel Default()
        {
            return new DepthOfFieldModel()
            {
                enabled = false,
                settings = DepthOfFieldModel.Settings.defaultSettings
            };
        }

        protected override DepthOfFieldModel Parse(ConfigNode.ValueList values)
        {
            DepthOfFieldModel.Settings settings = DepthOfFieldModel.Settings.defaultSettings;

            bool calcBool;
            float calcFloat;

            string[] data = {
                "aperture",
                "focallength",
                "focusdistance",
                "kernelsize",
                "usecamerafov",
                "enabled"
            };

            ProcessStream(values, ref data);

            if(data[0] != null && float.TryParse(data[0], out calcFloat))
            {
                settings.aperture = calcFloat;
            }
            if(data[1] != null && float.TryParse(data[1], out calcFloat))
            {
                settings.focalLength = calcFloat;
            }
            if(data[2] != null && float.TryParse(data[2], out calcFloat))
            {
                settings.focusDistance = calcFloat;
            }
            if(data[3] != null)
            {
                DepthOfFieldModel.KernelSize size;
                if(TryParseEnum(data[3], out size))
                {
                    settings.kernelSize = size;
                }
            }
            if(data[4] != null && bool.TryParse(data[4], out calcBool))
            {
                settings.useCameraFov = calcBool;
            }
            if(data[5] == null || !bool.TryParse(data[5], out calcBool))
            {
                calcBool = true;
            }

            return new DepthOfFieldModel()
            {
                enabled = calcBool,
                settings = settings
            };
        }
    }

    public sealed class MotionBlurParser : Parser<MotionBlurModel>
    {
        protected override MotionBlurModel Default()
        {
            return new MotionBlurModel()
            {
                enabled = false,
                settings = MotionBlurModel.Settings.defaultSettings
            };
        }

        protected override MotionBlurModel Parse(ConfigNode.ValueList values)
        {
            MotionBlurModel.Settings settings = MotionBlurModel.Settings.defaultSettings;

            string[] data =
            {
                "frameblending",
                "samplecount",
                "shutterangle",
                "enabled"
            };

            ProcessStream(values, ref data);

            float calcFloat;
            bool enabled;

            if(data[0] != null && float.TryParse(data[0], out calcFloat))
            {
                settings.frameBlending = calcFloat;
            }
            if(data[1] != null && float.TryParse(data[1], out calcFloat))
            {
                settings.sampleCount = Convert.ToInt32(calcFloat);
            }
            if(data[2] != null && float.TryParse(data[2], out calcFloat))
            {
                settings.shutterAngle = calcFloat;
            }
            if(data[3] == null || !bool.TryParse(data[3], out enabled))
            {
                enabled = true;
            }

            return new MotionBlurModel()
            {
                enabled = enabled,
                settings = settings
            };
        }
    }

    public sealed class EyeAdaptationParser : Parser<EyeAdaptationModel>
    {
        protected override EyeAdaptationModel Default()
        {
            return new EyeAdaptationModel()
            {
                enabled = false,
                settings = EyeAdaptationModel.Settings.defaultSettings
            };
        }

        protected override EyeAdaptationModel Parse(ConfigNode.ValueList values)
        {
            EyeAdaptationModel.Settings settings = EyeAdaptationModel.Settings.defaultSettings;
            string[] data =
            {
                "adaptationtype",  // 0
                "dynamickeyvalue", // 1
                "highpercent",     // 2
                "keyvalue",        // 3
                "logmax",          // 4
                "logmin",          // 5
                "lowpercent",      // 6
                "maxluminance",    // 7
                "minluminance",    // 8
                "speeddown",       // 9
                "speedup",         // 10
                "enabled"          // 11
            };
            ProcessStream(values, ref data);
            bool calcBool;
            float calcFloat;
            int calcInt;
            if(data[0] != null)
            {
                EyeAdaptationModel.EyeAdaptationType type;
                if(TryParseEnum(data[0], out type))
                {
                    settings.adaptationType = type;
                }
            }
            if(data[1] != null && bool.TryParse(data[1], out calcBool))
            {
                settings.dynamicKeyValue = calcBool;
            }
            if(data[2] != null && float.TryParse(data[2], out calcFloat))
            {
                settings.highPercent = calcFloat;
            }
            if(data[3] != null && float.TryParse(data[3], out calcFloat))
            {
                settings.keyValue = calcFloat;
            }
            if(data[4] != null && int.TryParse(data[4], out calcInt))
            {
                settings.logMax = calcInt;
            }
            if(data[5] != null && int.TryParse(data[5], out calcInt))
            {
                settings.logMin = calcInt;
            }
            if(data[6] != null && float.TryParse(data[6], out calcFloat))
            {
                settings.lowPercent = calcFloat;
            }
            if(data[7] != null && float.TryParse(data[7], out calcFloat))
            {
                settings.maxLuminance = calcFloat;
            }
            if(data[8] != null && float.TryParse(data[8], out calcFloat))
            {
                settings.minLuminance = calcFloat;
            }
            if(data[9] != null && float.TryParse(data[9], out calcFloat))
            {
                settings.speedDown = calcFloat;
            }
            if(data[10] != null && float.TryParse(data[10], out calcFloat))
            {
                settings.speedUp = calcFloat;
            }
            if(data[11] == null || !bool.TryParse(data[11], out calcBool))
            {
                calcBool = true;
            }

            return new EyeAdaptationModel()
            {
                enabled = calcBool,
                settings = settings
            };
        }
    }

    public sealed class BloomParser : TextureParser<BloomModel>
    {
        protected override BloomModel Default()
        {
            return new BloomModel()
            {
                enabled = false,
                settings = BloomModel.Settings.defaultSettings
            };
        }
        protected override BloomModel Parse(ConfigNode.ValueList values, out string path)
        {
            BloomModel.LensDirtSettings dirtSettings = BloomModel.LensDirtSettings.defaultSettings;
            BloomModel.BloomSettings bloomSettings = BloomModel.BloomSettings.defaultSettings;
            string[] data =
            {
                "dirtintensity",    // 0
                "dirttexture",      // 1
                "dirtenabled",      // 2
                "bloomantiflicker", // 3
                "bloomintensity",   // 4
                "bloomradius",      // 5
                "bloomsoftknee",    // 6
                "bloomthreshold",   // 7
                "enabled"           // 8
            };
            ProcessStream(values, ref data);
            bool calcBool;
            float calcFloat;

            if(data[2] == null)
            {
                if(bool.TryParse(data[2], out calcBool))
                {
                    if(calcBool)
                    {
                        // explicit enable
                        if(data[0] != null && float.TryParse(data[0], out calcFloat))
                        {
                            dirtSettings.intensity = calcFloat;
                        }
                        else
                        {
                            dirtSettings.intensity = 0f;
                        }
                        if(data[1] != null)
                        {
                            GameDatabase database = GameDatabase.Instance;
                            Texture2D tex = database.GetTexture(data[1], false);
                            if(tex)
                            {
                                path = data[1];
                                dirtSettings.texture = tex;
                                KS3P.Register(data[1], tex, KS3P.TexType.LensDirt);
                            }
                            else
                            {
                                Warning("Failed to load dirt texture path [" + data[1] + "], loading blank fallback texture.");
                                dirtSettings.texture = database.GetTexture("KS3P/Textures/Fallback.png", false);
                                path = "KS3P/Textures/Fallback.png";
                            }
                        }
                        else
                        {
                            dirtSettings.texture = GameDatabase.Instance.GetTexture("KS3P/Textures/Fallback.png", false);
                            path = "KS3P/Textures/Fallback.png";
                        }
                    }
                    else
                    {
                        dirtSettings.intensity = 0f; // disable
                        dirtSettings.texture = GameDatabase.Instance.GetTexture("KS3P/Textures/Fallback.png", false);
                        path = "KS3P/Textures/Fallback.png";
                    }
                }
                else
                {
                    // failed to parse, enable
                    if (data[0] != null && float.TryParse(data[0], out calcFloat))
                    {
                        dirtSettings.intensity = calcFloat;
                    }
                    else
                    {
                        dirtSettings.intensity = 0f;
                    }
                    if (data[1] != null)
                    {
                        GameDatabase database = GameDatabase.Instance;
                        Texture2D tex = database.GetTexture(data[1], false);
                        if (tex)
                        {
                            dirtSettings.texture = tex;
                            path = data[1];
                            KS3P.Register(data[1], tex, KS3P.TexType.LensDirt);
                        }
                        else
                        {
                            Warning("Failed to load dirt texture path [" + data[1] + "], loading blank fallback texture.");
                            dirtSettings.texture = database.GetTexture("KS3P/Textures/Fallback.png", false);
                            path = "KS3P/Textures/Fallback.png";
                        }
                    }
                    else
                    {
                        dirtSettings.texture = GameDatabase.Instance.GetTexture("KS3P/Textures/Fallback.png", false);
                        path = "KS3P/Textures/Fallback.png";
                    }
                }
            }
            else
            {
                // no specific given, enable
                if (data[0] != null && float.TryParse(data[0], out calcFloat))
                {
                    dirtSettings.intensity = calcFloat;
                }
                else
                {
                    dirtSettings.intensity = 0f;
                }
                if (data[1] != null)
                {
                    GameDatabase database = GameDatabase.Instance;
                    Texture2D tex = database.GetTexture(data[1], false);
                    if (tex)
                    {
                        dirtSettings.texture = tex;
                        path = data[1];
                        KS3P.Register(data[1], tex, KS3P.TexType.LensDirt);
                    }
                    else
                    {
                        Warning("Failed to load dirt texture path [" + data[1] + "], loading blank fallback texture.");
                        dirtSettings.texture = database.GetTexture("KS3P/Textures/Fallback.png", false);
                        path = "KS3P/Textures/Fallback.png";
                    }
                }
                else
                {
                    dirtSettings.texture = GameDatabase.Instance.GetTexture("KS3P/Textures/Fallback.png", false);
                    path = "KS3P/Textures/Fallback.png";
                }
            }

            if(data[3] != null && bool.TryParse(data[3], out calcBool))
            {
                bloomSettings.antiFlicker = calcBool;
            }
            if(data[4] != null && float.TryParse(data[4], out calcFloat))
            {
                bloomSettings.intensity = calcFloat;
            }
            if(data[5] != null && float.TryParse(data[5], out calcFloat))
            {
                bloomSettings.radius = calcFloat;
            }
            if(data[6] != null && float.TryParse(data[6], out calcFloat))
            {
                bloomSettings.softKnee = calcFloat;
            }
            if(data[7] != null && float.TryParse(data[7], out calcFloat))
            {
                bloomSettings.threshold = calcFloat;
            }
            
            if(data[8] == null || !bool.TryParse(data[8], out calcBool))
            {
                calcBool = true;
            }

            return new BloomModel()
            {
                enabled = calcBool,
                settings = new BloomModel.Settings()
                {
                    bloom = bloomSettings,
                    lensDirt = dirtSettings
                }
            };
        }
    }

    public sealed class ColorGradingParser : Parser<ColorGradingModel>
    {
        protected override ColorGradingModel Default()
        {
            return new ColorGradingModel()
            {
                enabled = false,
                settings = ColorGradingModel.Settings.defaultSettings
            };
        }

        public override ColorGradingModel Parse(ConfigNode node)
        {
            if (node == null)
            {
                return Default();
            }
            else
            {
                string[] data = { "enabled" };
                ProcessStream(node.values, ref data);

                bool calcBool;

                if(data[0] == null || !bool.TryParse(data[0], out calcBool))
                {
                    calcBool = true;
                }

                string[] names =
                {
                    "basic",
                    "mixer",
                    "wheels",
                    "curves",
                    "tonemapper"
                };
                ConfigNode[] nodes =
                {
                    null, // 0, basic
                    null, // 1, mixer
                    null, // 2, wheels
                    null, // 3, curves
                    null  // 4, tonemapper
                };
                int index = 0;
                foreach(ConfigNode subnode in node.nodes)
                {
                    if(names.Contains(KS3PUtil.Prepare(subnode.name), out index))
                    {
                        nodes[index] = subnode;
                    }
                }

                return new ColorGradingModel()
                {
                    enabled = calcBool,

                    settings = new ColorGradingModel.Settings()
                    {
                        basic = ParseBasic(nodes[0]),
                        channelMixer = ParseMixer(nodes[1]),
                        colorWheels = ParseWheels(nodes[2]),
                        curves = ParseCurves(nodes[3]),
                        tonemapping = ParseTonemapper(nodes[4])
                    }
                };
            }
        }

        ColorGradingModel.BasicSettings ParseBasic(ConfigNode node)
        {
            ColorGradingModel.BasicSettings settings = ColorGradingModel.BasicSettings.defaultSettings;
            if(node == null)
            {
                return settings;
            }
            else
            {
                string[] data =
                {
                    "contrast",     // 0
                    "hueshift",     // 1
                    "postexposure", // 2
                    "saturation",   // 3
                    "temperature",  // 4
                    "tint"          // 5
                };
                ProcessStream(node.values, ref data);
                float calcFloat;

                if (data[0] != null && float.TryParse(data[0], out calcFloat))
                {
                    settings.contrast = calcFloat;
                }
                if (data[1] != null && float.TryParse(data[1], out calcFloat))
                {
                    settings.hueShift = calcFloat;
                }
                if (data[2] != null && float.TryParse(data[2], out calcFloat))
                {
                    settings.postExposure = calcFloat;
                }
                if (data[3] != null && float.TryParse(data[3], out calcFloat))
                {
                    settings.saturation = calcFloat;
                }
                if (data[4] != null && float.TryParse(data[4], out calcFloat))
                {
                    settings.temperature = calcFloat;
                }
                if (data[5] != null && float.TryParse(data[5], out calcFloat))
                {
                    settings.tint = calcFloat;
                }

                return settings;
            }
        }
        ColorGradingModel.ChannelMixerSettings ParseMixer(ConfigNode node)
        {
            ColorGradingModel.ChannelMixerSettings settings = ColorGradingModel.ChannelMixerSettings.defaultSettings;
            if(node == null)
            {
                return settings;
            }
            else
            {
                string[] data =
                {
                    "red",      // 0
                    "green",    // 1
                    "blue"     // 2
                };
                ProcessStream(node.values, ref data);
                if(data[0] != null)
                {
                    settings.red = ParseVector3(data[0]);
                }
                if (data[1] != null)
                {
                    settings.green = ParseVector3(data[1]);
                }
                if (data[2] != null)
                {
                    settings.blue = ParseVector3(data[2]);
                }
                return settings;
            }
        }
        ColorGradingModel.ColorWheelsSettings ParseWheels(ConfigNode node)
        {
            ColorGradingModel.ColorWheelsSettings settings = ColorGradingModel.ColorWheelsSettings.defaultSettings;
            if(node == null)
            {
                return settings;
            }
            else
            {
                string[] data = {
                    "mode",     // 0
                    "gain",     // 1
                    "gamma",    // 2
                    "lift",     // 3
                    "offset",   // 4
                    "power",    // 5
                    "slope"     // 6
                };
                ProcessStream(node.values, ref data);

                ColorGradingModel.LinearWheelsSettings linear = ColorGradingModel.LinearWheelsSettings.defaultSettings;
                ColorGradingModel.LogWheelsSettings log = ColorGradingModel.LogWheelsSettings.defaultSettings;

                if (data[0] != null)
                {
                    ColorGradingModel.ColorWheelMode method;
                    if (TryParseEnum(data[0], out method))
                    {
                        settings.mode = method;
                    }
                }
                if(data[1] != null)
                {
                    linear.gain = ParseColor(data[1]);
                }
                if(data[2] != null)
                {
                    linear.gamma = ParseColor(data[2]);
                }
                if(data[3] != null)
                {
                    linear.lift = ParseColor(data[3]);
                }
                if(data[4] != null)
                {
                    log.offset = ParseColor(data[4]);
                }
                if(data[5] != null)
                {
                    log.power = ParseColor(data[5]);
                }
                if(data[6] != null)
                {
                    log.slope = ParseColor(data[6]);
                }
                settings.linear = linear;
                settings.log = log;
                return settings;
            }
        }
        ColorGradingModel.CurvesSettings ParseCurves(ConfigNode node)
        {
            ColorGradingModel.CurvesSettings settings = ColorGradingModel.CurvesSettings.defaultSettings;

            if(node == null)
            {
                return settings;
            }
            else
            {
                ColorGradingCurve parsedCurve;

                string[] data =
                {
                    "master",                       // 0
                    "red",                          // 1
                    "green",                        // 2
                    "blue",                         // 3
                    "hueversushue",                 // 4
                    "hueversussaturation",          // 5
                    "luminosityversussaturation",   // 6
                    "saturationversussaturation"    // 7
                };
                ConfigNode[] nodes =
                {
                    null, // 0
                    null, // 1
                    null, // 2
                    null, // 3
                    null, // 4
                    null, // 5
                    null, // 6
                    null // 7
                };

                int pos;
                foreach (ConfigNode subnode in node.nodes)
                {
                    if (data.Contains(KS3PUtil.Prepare(subnode.name), out pos))
                    {
                        nodes[pos] = subnode;
                    }
                }

                if (TryParseCurve(nodes[0], out parsedCurve))
                {
                    settings.master = parsedCurve;
                }
                if (TryParseCurve(nodes[1], out parsedCurve))
                {
                    settings.red = parsedCurve;
                }
                if (TryParseCurve(nodes[2], out parsedCurve))
                {
                    settings.green = parsedCurve;
                }
                if (TryParseCurve(nodes[3], out parsedCurve))
                {
                    settings.blue = parsedCurve;
                }
                if (TryParseCurve(nodes[4], out parsedCurve))
                {
                    settings.hueVShue = parsedCurve;
                }
                if (TryParseCurve(nodes[5], out parsedCurve))
                {
                    settings.hueVSsat = parsedCurve;
                }
                if (TryParseCurve(nodes[6], out parsedCurve))
                {
                    settings.lumVSsat = parsedCurve;
                }
                if (TryParseCurve(nodes[7], out parsedCurve))
                {
                    settings.satVSsat = parsedCurve;
                }

                return settings;
            }
        }
        ColorGradingModel.TonemappingSettings ParseTonemapper(ConfigNode node)
        {
            ColorGradingModel.TonemappingSettings settings = ColorGradingModel.TonemappingSettings.defaultSettings;

            if(node == null)
            {
                return settings;
            }
            else
            {
                string[] data = {
                  "tonemapper", // 0
                  "blackin",    // 1
                  "blackout",   // 2
                  "whiteclip",  // 3
                  "whitein",    // 4
                  "whitelevel", // 5
                  "whiteout"    // 6
                };
                ProcessStream(node.values, ref data);

                float calcFloat;

                if (data[0] != null)
                {
                    ColorGradingModel.Tonemapper mapper;
                    if (TryParseEnum(data[0], out mapper))
                    {
                        settings.tonemapper = mapper;
                    }
                }
                if (data[1] != null && float.TryParse(data[1], out calcFloat))
                {
                    settings.neutralBlackIn = calcFloat;
                }
                if (data[2] != null && float.TryParse(data[2], out calcFloat))
                {
                    settings.neutralBlackOut = calcFloat;
                }
                if (data[3] != null && float.TryParse(data[3], out calcFloat))
                {
                    settings.neutralWhiteClip = calcFloat;
                }
                if (data[4] != null && float.TryParse(data[4], out calcFloat))
                {
                    settings.neutralWhiteIn = calcFloat;
                }
                if (data[5] != null && float.TryParse(data[5], out calcFloat))
                {
                    settings.neutralWhiteLevel = calcFloat;
                }
                if (data[6] != null && float.TryParse(data[6], out calcFloat))
                {
                    settings.neutralWhiteOut = calcFloat;
                }

                return settings;
            }
        }
    }

    public sealed class UserLutParser : TextureParser<UserLutModel>
    {
        protected override UserLutModel Default()
        {
            return new UserLutModel()
            {
                enabled = false,
                settings = UserLutModel.Settings.defaultSettings
            };
        }

        protected override UserLutModel Parse(ConfigNode.ValueList values, out string path)
        {
            UserLutModel.Settings settings = UserLutModel.Settings.defaultSettings;

            string[] data = { "contribution", "lut", "enabled" };
            ProcessStream(values, ref data);

            float calcFloat;
            bool enabled;

            if(data[0] != null && float.TryParse(data[0], out calcFloat))
            {
                settings.contribution = calcFloat;
            }

            GameDatabase database = GameDatabase.Instance;
            if (data[1] != null)
            {
                Texture2D tex = database.GetTexture(data[1], false);
                if(tex)
                {
                    settings.lut = tex;
                    path = data[1];
                    KS3P.Register(data[1], tex, KS3P.TexType.Lut);
                }
                else
                {
                    Warning("Failed to load dirt texture path [" + data[1] + "], loading blank fallback texture.");
                    settings.lut = database.GetTexture("KS3P/Textures/Fallback.png", false);
                    path = "KS3P/Textures/Fallback.png";
                }
            }
            else
            {
                settings.lut = database.GetTexture("KS3P/Textures/Fallback.png", false);
                path = "KS3P/Textures/Fallback.png";
            }
            
            if(data[2] == null || !bool.TryParse(data[2], out enabled))
            {
                enabled = true;
            }

            return new UserLutModel()
            {
                enabled = enabled,
                settings = settings
            };
        }
    }

    public sealed class ChromaticAbberationParser : TextureParser<ChromaticAberrationModel>
    {
        protected override ChromaticAberrationModel Default()
        {
            return new ChromaticAberrationModel()
            {
                enabled = false,
                settings = ChromaticAberrationModel.Settings.defaultSettings
            };
        }
        protected override ChromaticAberrationModel Parse(ConfigNode.ValueList values, out string path)
        {
            ChromaticAberrationModel.Settings settings = ChromaticAberrationModel.Settings.defaultSettings;

            string[] data = { "intensity", "texture", "enabled" };
            ProcessStream(values, ref data);

            float calcFloat;
            bool enabled;

            if (data[0] != null && float.TryParse(data[0], out calcFloat))
            {
                settings.intensity = calcFloat;
            }

            GameDatabase database = GameDatabase.Instance;
            if (data[1] != null)
            {
                Texture2D tex = database.GetTexture(data[1], false);
                if (tex)
                {
                    settings.spectralTexture = tex;
                    path = data[1];
                    KS3P.Register(data[1], tex, KS3P.TexType.ChromaticTex);
                }
                else
                {
                    Warning("Failed to load spectral texture path [" + data[1] + "], loading blank fallback texture.");
                    settings.spectralTexture = database.GetTexture("KS3P/Textures/Fallback.png", false);
                    path = "KS3P/Textures/Fallback.png";
                }
            }
            else
            {
                settings.spectralTexture = database.GetTexture("KS3P/Textures/Fallback.png", false);
                path = "KS3P/Textures/Fallback.png";
            }

            if (data[2] == null || !bool.TryParse(data[2], out enabled))
            {
                enabled = true;
            }

            return new ChromaticAberrationModel()
            {
                enabled = enabled,
                settings = settings
            };
        }
    }

    public sealed class GrainParser : Parser<GrainModel>
    {
        protected override GrainModel Default()
        {
            return new GrainModel()
            {
                enabled = false,
                settings = GrainModel.Settings.defaultSettings
            };
        }
        protected override GrainModel Parse(ConfigNode.ValueList values)
        {
            GrainModel.Settings settings = GrainModel.Settings.defaultSettings;

            string[] data =
            {
                "colored",
                "intensity",
                "luminancecontribution",
                "size",
                "enabled"
            };
            ProcessStream(values, ref data);

            bool calcBool;
            float calcFloat;

            if(data[0] != null && bool.TryParse(data[0], out calcBool))
            {
                settings.colored = calcBool;
            }
            if(data[1] != null && float.TryParse(data[1], out calcFloat))
            {
                settings.intensity = calcFloat;
            }
            if(data[2] != null && float.TryParse(data[2], out calcFloat))
            {
                settings.luminanceContribution = calcFloat;
            }
            if(data[3] != null && float.TryParse(data[3], out calcFloat))
            {
                settings.size = calcFloat;
            }
            if(data[4] == null || !bool.TryParse(data[4], out calcBool))
            {
                calcBool = true;
            }

            return new GrainModel()
            {
                enabled = calcBool,
                settings = settings
            };
        }
    }

    public sealed class VignetteParser : TextureParser<VignetteModel>
    {
        protected override VignetteModel Default()
        {
            return new VignetteModel()
            {
                enabled = false,
                settings = VignetteModel.Settings.defaultSettings
            };
        }
        protected override VignetteModel Parse(ConfigNode.ValueList values, out string path)
        {
            VignetteModel.Settings settings = VignetteModel.Settings.defaultSettings;

            string[] data =
            {
                "center",       // 0
                "color",        // 1
                "intensity",    // 2
                "mask",         // 3
                "mode",         // 4
                "opacity",      // 5
                "rounded",      // 6
                "roundness",    // 7
                "smoothness",   // 8
                "enabled"       // 9
            };
            ProcessStream(values, ref data);

            float calcFloat;
            bool calcBool;

            if(data[0] != null)
            {
                settings.center = ParseVector2(data[0]);
            }
            if(data[1] != null)
            {
                settings.color = ParseColor(data[1]);
            }
            if(data[2] != null && float.TryParse(data[2], out calcFloat))
            {
                settings.intensity = calcFloat;
            }
            GameDatabase database = GameDatabase.Instance;
            if (data[3] != null)
            {
                Texture2D tex = database.GetTexture(data[3], false);
                if (tex)
                {
                    settings.mask = tex;
                    path = data[3];
                    KS3P.Register(data[3], tex, KS3P.TexType.VignetteMask);
                }
                else
                {
                    Warning("Failed to load mask texture path [" + data[3] + "], loading blank fallback texture.");
                    settings.mask = database.GetTexture("KS3P/Textures/Fallback.png", false);
                    path = "KS3P/Textures/Fallback.png";
                }
            }
            else
            {
                settings.mask = database.GetTexture("KS3P/Textures/Fallback.png", false);
                path = "KS3P/Textures/Fallback.png";
            }
            if(data[4] != null)
            {
                VignetteModel.Mode mode;
                if(TryParseEnum(data[4], out mode))
                {
                    settings.mode = mode;
                }
            }
            if(data[5] != null && float.TryParse(data[5], out calcFloat))
            {
                settings.opacity = calcFloat;
            }
            if(data[6] != null && bool.TryParse(data[6], out calcBool))
            {
                settings.rounded = calcBool;
            }
            if(data[7] != null && float.TryParse(data[7], out calcFloat))
            {
                settings.roundness = calcFloat;
            }
            if(data[8] != null && float.TryParse(data[8], out calcFloat))
            {
                settings.smoothness = calcFloat;
            }
            if(data[9] == null || !bool.TryParse(data[9], out calcBool))
            {
                calcBool = true;
            }

            return new VignetteModel()
            {
                enabled = calcBool,
                settings = settings
            };
        }
    }

    public sealed class ScreenSpaceReflectionParser : Parser<ScreenSpaceReflectionModel>
    {
        protected override ScreenSpaceReflectionModel Default()
        {
            return new ScreenSpaceReflectionModel()
            {
                enabled = false,
                settings = ScreenSpaceReflectionModel.Settings.defaultSettings
            };
        }
        protected override ScreenSpaceReflectionModel Parse(ConfigNode.ValueList values)
        {
            ScreenSpaceReflectionModel.Settings settings = ScreenSpaceReflectionModel.Settings.defaultSettings;
            var i_s = settings.intensity;
            var r_s = settings.reflection;
            var m_s = settings.screenEdgeMask;
            float calcFloat;
            int calcInt;
            bool calcBool;
            string[] data =
            {
                "blendtype",
                "reflectionquality",
                "maxdistance",
                "iterationcount",
                "stepsize",
                "widthmodifier",
                "reflectionblur",
                "reflectbackfaces",
                "reflectionmultiplier",
                "fadedistance",
                "fresnelfade",
                "fresnelfadepower",
                "intensity",
                "enabled"
            };
            
            if(data[0] != null)
            {
                ScreenSpaceReflectionModel.SSRReflectionBlendType btype;
                if(MiscParser.TryParseEnum(data[0], out btype))
                {
                    r_s.blendType = btype;
                }
            }
            if(data[1] != null)
            {
                ScreenSpaceReflectionModel.SSRResolution res;
                if(MiscParser.TryParseEnum(data[1], out res))
                {
                    r_s.reflectionQuality = res;
                }
            }
            if(data[2] != null && float.TryParse(data[2], out calcFloat))
            {
                r_s.maxDistance = calcFloat;
            }
            if(data[3] != null && int.TryParse(data[3], out calcInt))
            {
                r_s.iterationCount = calcInt;
            }
            if(data[4] != null && int.TryParse(data[4], out calcInt))
            {
                r_s.stepSize = calcInt;
            }
            if(data[5] != null && float.TryParse(data[5], out calcFloat))
            {
                r_s.widthModifier = calcFloat;
            }
            if(data[6] != null && float.TryParse(data[6], out calcFloat))
            {
                r_s.reflectionBlur = calcFloat;
            }
            if(data[7] != null && bool.TryParse(data[7], out calcBool))
            {
                r_s.reflectBackfaces = calcBool;
            }
            if(data[8] != null && float.TryParse(data[8], out calcFloat))
            {
                i_s.reflectionMultiplier = calcFloat;
            }
            if(data[9] != null && float.TryParse(data[9], out calcFloat))
            {
                i_s.fadeDistance = calcFloat;
            }
            if(data[10] != null && float.TryParse(data[10], out calcFloat))
            {
                i_s.fresnelFade = calcFloat;
            }
            if(data[11] != null && float.TryParse(data[11], out calcFloat))
            {
                i_s.fresnelFadePower = calcFloat;
            }
            if(data[12] != null && float.TryParse(data[12], out calcFloat))
            {
                m_s.intensity = calcFloat;
            }

            if(data[13] == null || !bool.TryParse(data[13], out calcBool))
            {
                calcBool = true;
            }

            settings.intensity = i_s;
            settings.reflection = r_s;
            settings.screenEdgeMask = m_s;
            return new ScreenSpaceReflectionModel()
            {
                settings = settings,
                enabled = calcBool
            };
        }
    }
}