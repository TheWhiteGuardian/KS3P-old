using System;
using UnityEngine;
using KS3P.UnityPostProcessing;
using KS3P.Processor;
using System.Collections.Generic;


namespace KS3P.ProfileParser
{
    

    //This class combines all of the code, essentially this class forms the bridge between the Unity Asset Bundle and Kerbal Space Program

    //This code loads the profile. It loads only once, the first time the game reaches the main menu
    [KSPAddon(KSPAddon.Startup.MainMenu, true)]
    public class EffectApplier : MonoBehaviour
    {
        public static ConfigNode configFile;

        //This will store all profiles loaded from the config file
        static ConfigNode[] profileArray;

        //Credits
        //SpacePlaneHangar
        //VAB
        //Flight
        //MainMenu
        //Settings
        //KSC
        //Trackingstation
        public static List<ConfigNode> ProfileList = new List<ConfigNode>(); //Store all profiles in a dictionary
        public static PostProcessingProfile Credits;
        public static PostProcessingProfile SPH;
        public static PostProcessingProfile VAB;
        public static PostProcessingProfile Flight;
        public static PostProcessingProfile MainMenu;
        public static PostProcessingProfile Settings;
        public static PostProcessingProfile KSC;
        public static PostProcessingProfile TS;
        public static bool patched = false; //False on startup, will permanently become true once the profiles have been patched.
        public static bool processing = false; //This bool shows whether or not the Profile Parser is busy.
        public static bool creditsExists, SPHExists, VABExists, FlightExists, MMExists, SettingsExists, KSCExists, TSExists; //This is for disabling PP for scenes for which there is no data.
        public static bool loadedCFG = false, loadedShaders = false;
        //In the Start function, we intialize
        void Start()
        {
            processing = true; //To avoid other initalizers from calling the void while this class is already processing it.
            if (!patched) { Load(); } //To avoid double-loading if another script took over
            else
            {
                processing = false; //Another script did the job for us? Then skip.
            }
        }
        
        public static void Load()
        {
            Debug.Log("[KSP_PostProcessing_ProfileParser]: Beginning parsing operation.");
            //Load the config file
            if (!loadedCFG) //If the CFG has been loaded already, skip
            {
                Debug.Log("[KSP_PostProcessing_ProfileParser]: Loading config file...");
                configFile = ConfigNode.Load(KSPUtil.ApplicationRootPath + "/GameData/KS3P/Config.cfg");
                Debug.Log("[KSP_PostProcessing_ProfileParser]: Config file loaded.");
                loadedCFG = true;
            }

            //Load the shaders
            if (!loadedShaders) //If the shaders have been loaded already, skip
            {
                Debug.Log("[KSP_PostProcessing_ProfileParser]: Loading shaders...");
                ShaderLoader.ShaderLoader.LoadAssetBundle("KS3P/Shaders", "postprocessingshaders");
                Debug.Log("[KSP_PostProcessing_ProfileParser]: Shaders loaded.");
            }

            //Replacing Shader.Find in the Unity MaterialFactory with the external shader loading stuff should fix things up nicely. :)

            //Store all profiles

            Debug.Log("[KSP_PostProcessing_ProfileParser]: Initializing profile...");

            Debug.Log("[KSP_PostProcessing_ProfileParser]: Loading profiles...");
            profileArray = configFile.GetNodes("Post_Processing");
            Debug.Log("[KSP_PostProcessing_ProfileParser]: Found [" + profileArray.Length + "] profiles.");


            //Add profiles
            Debug.Log("[KSP_PostProcessing_ProfileParser]: Indexing profiles...");
            foreach (ConfigNode profile in profileArray)
            {
                ProfileList.Add(profile);
                Debug.Log("[KSP_PostProcessing_ProfileParser]: Adding profile for scene: [" + profile.GetValue("Scene") + "].");
            }

            Debug.Log("[KSP_PostProcessing_ProfileParser]: Initializing Post-Processing profiles...");

            Debug.Log("[KSP_PostProcesing_ProfileParser]: Generating profile for scene [Credits]...");
            ProfileProcessor.LoadProfile(out Credits, ProfileList, "Credits", out creditsExists);
            if (!creditsExists) { Debug.LogWarning("[KSP_PostProcessing_ProfileParser]: Warning! No data found for scene [Credits], disabling Post-Processing for this scene!"); }

            Debug.Log("[KSP_PostProcesing_ProfileParser]: Generating profile for scene [SpacePlaneHangar]...");
            ProfileProcessor.LoadProfile(out SPH, ProfileList, "SpacePlaneHangar", out SPHExists);
            if (!SPHExists) { Debug.LogWarning("[KSP_PostProcessing_ProfileParser]: Warning! No data found for scene [SpacePlaneHangar], disabling Post-Processing for this scene!"); }

            Debug.Log("[KSP_PostProcesing_ProfileParser]: Generating profile for scene [VehicleAssemblyBuilding]...");
            ProfileProcessor.LoadProfile(out VAB, ProfileList, "VehicleAssemblyBuilding", out VABExists);
            if (!VABExists) { Debug.LogWarning("[KSP_PostProcessing_ProfileParser]: Warning! No data found for scene [VehicleAssemblyBuilding], disabling Post-Processing for this scene!"); }

            Debug.Log("[KSP_PostProcesing_ProfileParser]: Generating profile for scene [Flight]...");
            ProfileProcessor.LoadProfile(out Flight, ProfileList, "Flight", out FlightExists);
            if (!FlightExists) { Debug.LogWarning("[KSP_PostProcessing_ProfileParser]: Warning! No data found for scene [Flight], disabling Post-Processing for this scene!"); }

            Debug.Log("[KSP_PostProcesing_ProfileParser]: Generating profile for scene [MainMenu]...");
            ProfileProcessor.LoadProfile(out MainMenu, ProfileList, "MainMenu", out MMExists);
            if (!MMExists) { Debug.LogWarning("[KSP_PostProcessing_ProfileParser]: Warning! No data found for scene [MainMenu], disabling Post-Processing for this scene!"); }

            Debug.Log("[KSP_PostProcesing_ProfileParser]: Generating profile for scene [Settings]...");
            ProfileProcessor.LoadProfile(out Settings, ProfileList, "Settings", out SettingsExists);
            if (!SettingsExists) { Debug.LogWarning("[KSP_PostProcessing_ProfileParser]: Warning! No data found for scene [Settings], disabling Post-Processing for this scene!"); }

            Debug.Log("[KSP_PostProcesing_ProfileParser]: Generating profile for scene [KerbalSpaceCenter]...");
            ProfileProcessor.LoadProfile(out KSC, ProfileList, "KerbalSpaceCenter", out KSCExists);
            if (!KSCExists) { Debug.LogWarning("[KSP_PostProcessing_ProfileParser]: Warning! No data found for scene [KerbalSpaceCenter], disabling Post-Processing for this scene!"); }

            Debug.Log("[KSP_PostProcesing_ProfileParser]: Generating profile for scene [TrackingStation]...");
            ProfileProcessor.LoadProfile(out TS, ProfileList, "TrackingStation", out TSExists);
            if (!TSExists) { Debug.LogWarning("[KSP_PostProcessing_ProfileParser]: Warning! No data found for scene [TrackingStation], disabling Post-Processing for this scene!"); }

            processing = false;
            Debug.Log("[KSP_PostProcessing_ProfileParser]: Finished initializing profile.");
            patched = true; //We finished patching.
            //GameEvents.onLevelWasLoaded.Add(new EventData<GameScenes>.OnEvent(OnSceneChange));
        }




        /*
       /// <summary>
       /// Executed when KSP changes a scene
       /// </summary>
       /// <param name="scene">The loaded scene</param>
       static void OnSceneChange(GameScenes scene)
       {
           //Pick depending on the scene
           switch (scene)
           {
               case GameScenes.LOADING:
                   PatchCamera(loading);
                   break;
               case GameScenes.LOADINGBUFFER:
                   PatchCamera(loadingBuffer);
                   break;
               case GameScenes.MAINMENU:
                   PatchCamera(mainMenu);
                   break;
               case GameScenes.SETTINGS:
                   PatchCamera(settings);
                   break;
               case GameScenes.CREDITS:
                   PatchCamera(credits);
                   break;
               case GameScenes.SPACECENTER:
                   PatchCamera(ksc);
                   break;
               case GameScenes.EDITOR:
                   PatchCamera(editor);
                   break;
               case GameScenes.FLIGHT:
                   PatchCamera(flight);
                   break;
               case GameScenes.TRACKSTATION:
                   PatchCamera(trackstation);
                   break;
               case GameScenes.PSYSTEM:
                   PatchCamera(psystem);
                   break;
           }
       }

       #region EnabledScenes
       static bool loading = false;
       static bool loadingBuffer = false;
       static bool mainMenu = false;
       static bool settings = false;
       static bool credits = false;
       static bool ksc = false;
       static bool editor = false;
       static bool flight = false;
       static bool trackstation = false;
       static bool psystem = false;
       #endregion


       static void PatchCamera(bool add)
       {
           if (Camera.main.gameObject.GetComponent<PostProcessingBehaviour>() == null)
           {
               Camera.main.gameObject.AddComponent<PostProcessingBehaviour>();
           }

           Camera.main.gameObject.GetComponent<PostProcessingBehaviour>().profile = profile;
           Camera.main.gameObject.GetComponent<PostProcessingBehaviour>().enabled = add;
       }
       */
    }
}
