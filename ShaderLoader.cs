using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace KS3P.ShaderLoader
{
    //Mostly borrowed from https://github.com/Kopernicus/Kopernicus/blob/master/src/Kopernicus.Components/ShaderLoader.cs
    //Credit goes to Thomas P.
    public class ShaderLoader
    {
        /// <summary>
        /// The collection of all shaders
        /// </summary>
        static Dictionary<String, Shader> shaderDictionary = new Dictionary<string, Shader>();


        public static Shader GetShader(string shaderName)
        {
            Debug.Log("[KSP_PostProcessing_ShaderLoader]: Searching for shader [" + shaderName + "].");

            if (shaderDictionary.ContainsKey(shaderName))
            {
                return shaderDictionary[shaderName];
            }

            //If we reach this part, we have found no shader

            Debug.LogError("[KSP_PostProcessing_ShaderLoader]: Error! No shader found with name [" + shaderName + "].");
            return null;
        }

        public static void LoadAssetBundle(String path, String bundleName)
        {
            path = Path.Combine(KSPUtil.ApplicationRootPath + "GameData/", path);

            if (Application.platform == RuntimePlatform.WindowsPlayer && SystemInfo.graphicsDeviceVersion.StartsWith("OpenGL"))
            {
                path = Path.Combine(path, bundleName + "-linux.unity3d"); //For OpenGL users on Windows we load the Linux shaders to fix OpenGL issues
            }
            else if (Application.platform == RuntimePlatform.WindowsPlayer)
            {
                path = Path.Combine(path, bundleName + "-windows.unity3d");
            }
            else if (Application.platform == RuntimePlatform.LinuxPlayer)
            {
                path = Path.Combine(path, bundleName + "-linux.unity3d");
            }
            else
            {
                path = Path.Combine(path, bundleName + "-macosx.unity3d");
            }

            Debug.Log("[KSP_PostProcessing_ShaderLoader]: Loading asset bundle at path " + path);

            using (WWW www = new WWW("file://" + path))
            {
                AssetBundle bundle = www.assetBundle;
                Shader[] shaders = bundle.LoadAllAssets<Shader>();
                foreach(Shader shader in shaders)
                {
                    Debug.Log("[KSP_PostProcessing_ShaderLoader]: Adding shader [" + shader.name + "].");
                    shaderDictionary.Add(shader.name, shader);
                }
                bundle.Unload(false);
            }
        }
    }
}
