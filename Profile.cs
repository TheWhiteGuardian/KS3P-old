using UnityEngine;
using System.IO;
using KSP_PostProcessing.Parsers;
using System.Collections.Generic;
using System.Collections;

namespace KSP_PostProcessing
{
    /// <summary>
    /// A KS3P post-processing profile.
    /// </summary>
    [System.Serializable]
    public sealed class Profile
    {
        public PostProcessingProfile profile;
        public string identifier;
        public BitArray scenes = new BitArray(9, false);
        public string dirtTex;
        public string lutTex;
        public string vignetteMask;
        public string chromaticTex;
        public Profile(ConfigNode node)
        {
            string ProfileName = "Undefined";
            string AuthorName = "Unknown";
            string filtered;
            foreach (ConfigNode.Value v in node.values)
            {
                filtered = KS3PUtil.Prepare(v.name);
                if (filtered == "name")
                {
                    ProfileName = v.value;
                }
                else if (filtered == "author")
                {
                    AuthorName = v.value;
                }
                else if (filtered == "scene")
                {
                    scenes = MiscParser.ParseSceneList(v.value);
                }
            }

            Dictionary<string, ConfigNode> nodes = new Dictionary<string, ConfigNode>();
            foreach (ConfigNode subnode in node.nodes)
            {
                nodes.Add(KS3PUtil.Prepare(subnode.name), subnode);
            }
            profile = ScriptableObject.CreateInstance<PostProcessingProfile>();
            profile.antialiasing = new AntiAliasingParser().Parse(nodes.Grab("antialiasing"));
            profile.ambientOcclusion = new AmbientOcclusionParser().Parse(nodes.Grab("ambientocclusion"));
            profile.depthOfField = new DepthOfFieldParser().Parse(nodes.Grab("depthoffield"));
            profile.motionBlur = new MotionBlurParser().Parse(nodes.Grab("motionblur"));
            profile.eyeAdaptation = new EyeAdaptationParser().Parse(nodes.Grab("eyeadaptation"));
            profile.bloom = new BloomParser().Parse(nodes.Grab("bloom"), out dirtTex);
            profile.colorGrading = new ColorGradingParser().Parse(nodes.Grab("colorgrading"));
            profile.userLut = new UserLutParser().Parse(nodes.Grab("userlut"), out lutTex);
            profile.chromaticAberration = new ChromaticAbberationParser().Parse(nodes.Grab("chromaticabberation"), out chromaticTex);
            profile.grain = new GrainParser().Parse(nodes.Grab("grain"));
            profile.vignette = new VignetteParser().Parse(nodes.Grab("vignette"), out vignetteMask);
            profile.dithering = new DitheringModel()
            {
                enabled = (nodes.Grab("dithering") != null),
                settings = DitheringModel.Settings.defaultSettings
            };
            profile.screenSpaceReflection = new ScreenSpaceReflectionParser().Parse(nodes.Grab("screenspacereflection"));
            identifier = ProfileName + " by " + AuthorName;
        }
        public static implicit operator Profile(ConfigNode node) { return new Profile(node); }
    }
}