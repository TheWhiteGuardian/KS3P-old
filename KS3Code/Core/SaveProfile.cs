using System.IO;
using KS3P.Shaders;
using System.Collections.Generic;
using UnityEngine;

namespace KS3P.Core
{
    //Node: color curves have been altered from Unity default to include accessibility to fields.
    public class SaveProfile : MonoBehaviour
    {
        public static void Save(PostProcessingProfile profile, string fileName, string saveLocation)
        {
            List<string> lines = new List<string>();
            
            lines.Add("SETUP");
            lines.Add("{");
            lines.Append("Scene = [SceneName has to be assigned manually]", 1);

            //Check if something is enabled
            //If disabled, we don't add the node, as the non-existance of a node means that it is marked as disabled

            if(profile.antialiasing.enabled)
            {
                WriteAA(lines, profile.antialiasing.settings);
            }
            if(profile.ambientOcclusion.enabled)
            {
                WriteAO(lines, profile.ambientOcclusion.settings);
            }
            if(profile.bloom.enabled)
            {
                WriteB(lines, profile.bloom.settings);
            }
            if(profile.chromaticAberration.enabled)
            {
                WriteCA(lines, profile.chromaticAberration.settings);
            }
            if(profile.colorGrading.enabled)
            {
                WriteCG(lines, profile.colorGrading.settings);
            }
            if(profile.depthOfField.enabled)
            {
                WriteDOF(lines, profile.depthOfField.settings);
            }
            if(profile.dithering.enabled)
            {
                lines.Append("Dithering", 1);
                lines.Append("{", 1);
                lines.Append("}", 1);
            }
            if(profile.eyeAdaptation.enabled)
            {
                WriteEA(lines, profile.eyeAdaptation.settings);
            }
            if(profile.grain.enabled)
            {
                WriteG(lines, profile.grain.settings);
            }
            if(profile.motionBlur.enabled)
            {
                WriteMB(lines, profile.motionBlur.settings);
            }
            if(profile.userLut.enabled)
            {
                WriteUL(lines, profile.userLut.settings);
            }
            if(profile.vignette.enabled)
            {
                WriteV(lines, profile.vignette.settings);
            }

            File.WriteAllLines(saveLocation + "/" + fileName + ".txt", lines.ToArray());
            Debug.Log("[KS3P]: Saved profile to file at [" + saveLocation + "], file name [" + fileName + "].");
        }



        private static void WriteAA(List<string> lines, AntialiasingModel.Settings settings)
        {
            lines.Append("Anti_Aliasing", 1);
            lines.Append("{", 1);
            switch(settings.method)
            {
                case AntialiasingModel.Method.Fxaa:
                    lines.Append("Mode = FXAA", 2);
                    lines.Append("Quality = " + (int)settings.fxaaSettings.preset, 2);
                    break;
                case AntialiasingModel.Method.Taa:
                    lines.Append("Mode = TAA", 2);
                    lines.Append("Jitter = " + settings.taaSettings.jitterSpread, 2);
                    lines.Append("Blend_Motion = " + settings.taaSettings.motionBlending, 2);
                    lines.Append("Blend_Stationary = " + settings.taaSettings.stationaryBlending, 2);
                    lines.Append("Sharpen" + settings.taaSettings.sharpen, 2);
                    break;
            }
            lines.Append("}", 1);
        }
        private static void WriteAO(List<string> lines, AmbientOcclusionModel.Settings settings)
        {
            lines.Append("Ambient_Occlusion", 1);
            lines.Append("{", 1);

            lines.Append("Ambient_Only = " + settings.ambientOnly.Convert(), 2);
            lines.Append("Downsampling = " + settings.downsampling.Convert(), 2);
            lines.Append("Force_Forward_Compatibility = " + settings.forceForwardCompatibility.Convert(), 2);
            lines.Append("High_Precision = " + settings.highPrecision.Convert(), 2);
            lines.Append("Intensity = " + settings.intensity, 2);
            lines.Append("Radius = " + settings.radius, 2);
            lines.Append("Sample_Count = " + (int)settings.sampleCount, 2);

            lines.Append("}", 1);
        }
        private static void WriteB(List<string> lines, BloomModel.Settings settings)
        {
            lines.Append("Bloom", 1);
            lines.Append("{", 1);

            lines.Append("Anti_Flicker = " + settings.bloom.antiFlicker.Convert(), 2);
            lines.Append("Intensity = " + settings.bloom.intensity, 2);
            lines.Append("Radius = " + settings.bloom.radius, 2);
            lines.Append("Soft_Knee = " + settings.bloom.softKnee, 2);
            lines.Append("Threshold = " + settings.bloom.threshold, 2);
            Texture2D nullTex = GameDatabase.Instance.GetTexture("KS3P/Textures/Null", false);
            if(nullTex == settings.lensDirt.texture)
            {
                lines.Append("Dirt_Enabled = false", 2);
            }
            else
            {
                lines.Append("Dirt_Enabled = true", 2);
                lines.Append("Dirt_Tex = " + settings.lensDirt.texture.name, 2);
                lines.Append("Dirt_Intensity = " + settings.lensDirt.intensity, 2);
            }
            lines.Append("}", 1);
        }
        private static void WriteCA(List<string> lines, ChromaticAberrationModel.Settings settings)
        {
            lines.Append("Chromatic_Abberation", 1);
            lines.Append("{", 1);
            lines.Append("Spectral_Tex = " + settings.spectralTexture.name, 2);
            lines.Append("Intensity = " + settings.intensity, 2);
            lines.Append("}", 1);
        }
        private static void WriteCG(List<string> lines, ColorGradingModel.Settings settings)
        {
            lines.Append("Color_Grading", 1);
            lines.Append("{", 1);

            lines.Append("Base", 2);
            lines.Append("{", 2);
            lines.Append("Contrast = " + settings.basic.contrast, 3);
            lines.Append("Hue_Shift = " + settings.basic.hueShift, 3);
            lines.Append("Post_Exposure = " + settings.basic.postExposure, 3); ;
            lines.Append("Saturation = " + settings.basic.saturation, 3);
            lines.Append("Tint = " + settings.basic.tint, 3);
            lines.Append("Temperature = " + settings.basic.temperature, 3);
            lines.Append("}", 2);

            lines.Append("ColorMixer", 2);
            lines.Append("{", 2);
            lines.Append("Red = " + settings.channelMixer.red.Convert(), 3);
            lines.Append("Green = " + settings.channelMixer.green.Convert(), 3);
            lines.Append("Blue = " + settings.channelMixer.blue.Convert(), 3);
            lines.Append("}", 2);

            lines.Append("ColorWheels", 2);
            lines.Append("{", 2);
            switch(settings.colorWheels.mode)
            {
                case ColorGradingModel.ColorWheelMode.Linear:
                    lines.Append("WheelMode = Linear", 3);
                    lines.Append("Gain = " + settings.colorWheels.linear.gain.Convert(), 3);
                    lines.Append("Gamma = " + settings.colorWheels.linear.gamma.Convert(), 3);
                    lines.Append("Lift = " + settings.colorWheels.linear.lift.Convert(), 3);
                    break;
                case ColorGradingModel.ColorWheelMode.Log:
                    lines.Append("WheelMode = Logarithmic", 3);
                    lines.Append("Offset = " + settings.colorWheels.log.offset.Convert(), 3);
                    lines.Append("Power = " + settings.colorWheels.log.power.Convert(), 3);
                    lines.Append("Slope = " + settings.colorWheels.log.slope.Convert(), 3);
                    break;
            }
            lines.Append("}", 2);

            lines.Append("ColorCurves", 2);
            lines.Append("{", 2);
            WriteColorCurve(lines, "Master", settings.curves.master, 3);
            WriteColorCurve(lines, "Red", settings.curves.red, 3);
            WriteColorCurve(lines, "Green", settings.curves.green, 3);
            WriteColorCurve(lines, "Blue", settings.curves.blue, 3);
            WriteColorCurve(lines, "HueVersusHue", settings.curves.hueVShue, 3);
            WriteColorCurve(lines, "HueVersusSaturation", settings.curves.hueVSsat, 3);
            WriteColorCurve(lines, "LuminosityVersusSaturation", settings.curves.lumVSsat, 3);
            WriteColorCurve(lines, "SaturationVersusSaturation", settings.curves.satVSsat, 3);
            lines.Append("}", 2);

            lines.Append("Tonemapper", 2);
            lines.Append("{", 2);
            switch(settings.tonemapping.tonemapper)
            {
                case ColorGradingModel.Tonemapper.ACES:
                    lines.Append("Tonemapper = ACES", 3);
                    break;
                case ColorGradingModel.Tonemapper.Neutral:
                    lines.Append("Tonemapper = Neutral", 3);
                    break;
                case ColorGradingModel.Tonemapper.None:
                    lines.Append("Tonemapper = None", 3);
                    break;
            }
            lines.Append("Black_In = " + settings.tonemapping.neutralBlackIn, 3);
            lines.Append("Black_Out = " + settings.tonemapping.neutralBlackOut, 3);
            lines.Append("White_In = " + settings.tonemapping.neutralWhiteIn, 3);
            lines.Append("White_Out = " + settings.tonemapping.neutralWhiteOut, 3);
            lines.Append("White_Clip = " + settings.tonemapping.neutralWhiteClip, 3);
            lines.Append("White_Level = " + settings.tonemapping.neutralWhiteLevel, 3);
            lines.Append("}", 2);

            lines.Append("}", 1);
        }
        private static void WriteDOF(List<string> lines, DepthOfFieldModel.Settings settings)
        {
            lines.Append("Depth_Of_Field", 1);
            lines.Append("{", 1);
            lines.Append("Focus_Distance = " + settings.focusDistance, 2);
            lines.Append("Aperture = " + settings.aperture, 2);
            lines.Append("Use_Camera_FOV = " + settings.useCameraFov.Convert(), 2);
            lines.Append("Focal_Length = " + settings.focalLength, 2);
            lines.Append("Kernel_Size = " + (int)settings.kernelSize, 2);
            lines.Append("}", 1);
        }
        private static void WriteEA(List<string> lines, EyeAdaptationModel.Settings settings)
        {
            lines.Append("Eye_Adaptation", 1);
            lines.Append("{", 1);

            lines.Append("Luminosity_Minimum = " + settings.logMin, 2);
            lines.Append("Luminosity_Maximum = " + settings.logMax, 2);
            lines.Append("Maximum_EV = " + settings.maxLuminance, 2);
            lines.Append("Minimum_EV = " + settings.minLuminance, 2);
            lines.Append("Dynamic_Key_Value = " + settings.dynamicKeyValue.Convert(), 2);
            lines.Append("Type = " + (int)settings.adaptationType, 2);
            lines.Append("Speed_Up = " + settings.speedUp, 2);
            lines.Append("Speed_Down = " + settings.speedDown, 2);
            lines.Append("Range = " + settings.lowPercent + ", " + settings.highPercent, 2);
            lines.Append("Key_Value = " + settings.keyValue, 2);

            lines.Append("}", 1);
        }
        private static void WriteG(List<string> lines, GrainModel.Settings settings)
        {
            lines.Append("Grain", 1);
            lines.Append("{", 1);
            lines.Append("Colored = " + settings.colored.Convert(), 2);
            lines.Append("Intensity = " + settings.intensity, 2);
            lines.Append("Luminance_Contribution = " + settings.luminanceContribution, 2);
            lines.Append("Size = " + settings.size, 2);
            lines.Append("}", 1);
        }
        private static void WriteMB(List<string> lines, MotionBlurModel.Settings settings)
        {
            lines.Append("Motion_Blur", 1);
            lines.Append("{", 1);
            lines.Append("Shutter_Angle = " + settings.shutterAngle, 2);
            lines.Append("Sample_Count = " + settings.sampleCount, 2);
            lines.Append("Frame_Blending = " + settings.frameBlending, 2);
            lines.Append("}", 1);
        }
        private static void WriteUL(List<string> lines, UserLutModel.Settings settings)
        {
            lines.Append("User_Lut", 1);
            lines.Append("{", 1);
            lines.Append("Lut_Texture = " + settings.lut.name, 2);
            lines.Append("Contribution = " + settings.contribution, 2);
            lines.Append("}", 1);
        }
        private static void WriteV(List<string> lines, VignetteModel.Settings settings)
        {
            lines.Append("Vignette", 1);
            lines.Append("{", 1);
            lines.Append("Center = " + settings.center.Convert(), 2);
            lines.Append("Color = " + settings.color.Convert(), 2);
            lines.Append("Intensity = " + settings.intensity, 2);
            lines.Append("Opacity = " + settings.opacity, 2);
            lines.Append("Roundness = " + settings.roundness, 2);
            lines.Append("Smoothness = " + settings.smoothness, 2);
            lines.Append("Rounded = " + settings.rounded.Convert(), 2);
            lines.Append("Mask = " + settings.mask.name, 2);
            lines.Append("Mode = " + (int)settings.mode, 2);
        }

        private static void WriteColorCurve(List<string> lines, string name, ColorGradingCurve curve, int indentation)
        {
            lines.Append(name, indentation);
            lines.Append("{", indentation);
            lines.Append("Zero = " + curve.ZeroValue, indentation + 1);
            lines.Append("IsLooped = " + curve.IsLooped, indentation + 1);
            lines.Append("Bounds = " + curve.Range, indentation + 1);
            lines.Append("Curve", indentation + 1);
            lines.Append("{", indentation + 1);
            Keyframe[] keys = curve.curve.keys;
            for(int i = 0; i < keys.Length; i++)
            {
                lines.Append("Key = " + keys[i].Convert(), indentation + 2);
            }
            lines.Append("}", indentation + 1);
            lines.Append("}", indentation);
        }
    }
    static class Extensions
    {
        public static string Convert(this Vector2 v)
        {
            return v.x + ", " + v.y;
        }
        public static string Convert(this Vector3 v)
        {
            return v.x + ", " + v.y + ", " + v.z;
        }
        public static string Convert(this bool b)
        {
            return (b) ? "true" : "false";
        }
        public static string Convert(this Color c)
        {
            return c.r + ", " + c.g + ", " + c.b + ", " + c.a;
        }
        public static string Convert(this Keyframe f)
        {
            return f.time + ", " + f.value + ", " + f.inTangent + ", " + f.outTangent;
        }
        public static float ToFloat(this string s)
        {
            return float.Parse(s);
        }
        public static Vector4 ToVector4(this string[] vs)
        {
            return new Vector4(vs[0].ToFloat(), vs[1].ToFloat(), vs[2].ToFloat(), vs[3].ToFloat());
        }
        public static void Append(this List<string> list, string ToAdd, int indentation)
        {
            string s = null;
            for(int i = 0; i < indentation; i++)
            {
                s += "    ";
            }
            list.Add(s + ToAdd);
        }
    }
}