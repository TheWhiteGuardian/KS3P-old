using System;
using UnityEngine;
using KS3P.Shaders;
using System.Collections.Generic;


namespace KS3P.Processor
{
    
    public enum ProfileType
    {
        MainMenu,
        VAB,
        SPH,
        KSC,
        Flight,
        IVA,
        EVA,
        TS,
        Map
    }
    /// <summary>
    /// Stores all profiles of a specific config
    /// </summary>
    public struct ProfileBundle
    {
        /// <summary>
        /// Contains all profiles, one per scene type
        /// </summary>
        Dictionary<ProfileType, PostProcessingProfile> profiles;
        /// <summary>
        /// Get a profile from the bundle, dependent on the target scene
        /// </summary>
        /// <param name="type">The scene that the profile should be grabbed for</param>
        /// <returns></returns>
        public PostProcessingProfile GetProfile(ProfileType type)
        {
            return profiles[type];
        }
        /// <summary>
        /// Returns true if the target scene already exists within the bundle.
        /// </summary>
        /// <param name="type">The scene to check data for.</param>
        /// <returns></returns>
        public bool SceneExists(ProfileType type)
        {
            return profiles.ContainsKey(type);
        }
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="profiles"></param>
        public ProfileBundle(Dictionary<ProfileType, PostProcessingProfile> profiles)
        {
            this.profiles = profiles;
        }
        /// <summary>
        /// Returns an empty ProfileBundle
        /// </summary>
        public ProfileBundle Empty
        {
            get { return new ProfileBundle(new Dictionary<ProfileType, PostProcessingProfile>()); }
        }
        /// <summary>
        /// Adds a profile to the index
        /// </summary>
        /// <param name="type">The target scene</param>
        /// <param name="profile">The profile corresponding to the target scene</param>
        public void Append(ProfileType type, PostProcessingProfile profile)
        {
            profiles.Add(type, profile);
        }
        /// <summary>
        /// Append function, but checks if target scene already exists. Returns true if the indexing was successful.
        /// </summary>
        /// <param name="type">The target scene</param>
        /// <param name="profile">The profile corresponding to the target scene</param>
        /// <returns>True if the profile was added successfully.</returns>
        public bool TryAppend(ProfileType type, PostProcessingProfile profile)
        {
            if (SceneExists(type))
            {
                return false;
            }
            else
            {
                Append(type, profile);
                return true;
            }
        }
    }
    public class ProfileParser
    {
        /// <summary>
        /// Key: author of this profile bundle. Value: the profile bundle itself
        /// </summary>
        public static Dictionary<string, ProfileBundle> profileBundles = new Dictionary<string, ProfileBundle>();
        /// <summary>
        /// True if the indexing function has been run at least once.
        /// </summary>
        public static bool isLoaded = false;
        /// <summary>
        /// Returns true if the bundle list is empty.
        /// </summary>
        public static bool empty
        {
            get
            {
                if (profileBundles.Count > 0)
                {
                    return false;
                }
                else return true;
            }
        }
        /// <summary>
        /// If an error occurred, lockdown is initiated.
        /// </summary>
        public static bool lockdown = false;
        public static void NoProfileError()
        {
            Debug.LogError("[KS3P.Processor]: Error! Could not find any profiles!");
        }
        public static List<string> validNames = new List<string>();
        static ConfigNode[] nodes;

        public static void Load()
        {
            nodes = GetKS3PNodes();
            if (nodes.Length > 0) Process(nodes);
            else
            {
                lockdown = true;
                Debug.LogException(new ArgumentNullException(nameof(nodes), "[KS3P_Processor]: No configs found!"));
            }
        }
        private static ConfigNode[] GetKS3PNodes()
        {
            List<ConfigNode> nodes = new List<ConfigNode>();
            UrlDir.UrlConfig[] uCfg = GameDatabase.Instance.GetConfigs("Post_Processing");
            foreach(UrlDir.UrlConfig config in uCfg)
            {
                nodes.Add(config.config);
            }
            return nodes.ToArray();
        }
        private static void Process(ConfigNode[] nodes)
        {
            //Process all nodes
            foreach(ConfigNode node in nodes)
            {
                try
                {
                    Dictionary<ProfileType, PostProcessingProfile> bundleContents = new Dictionary<ProfileType, PostProcessingProfile>();
                    string name = node.GetValue("Name");
                    Debug.Log("[KS3P_Processor]: Processing post-processing setup bundle named [" + name + "].");
                    ConfigNode[] setups = node.GetNodes("SETUP");
                    try
                    {
                        foreach (ConfigNode setup in setups)
                        {
                            //Get scene as ProfileType enumerator.
                            //Also ignore case! Neat!
                            ProfileType targetScene = (ProfileType)Enum.Parse(typeof(ProfileType), setup.GetValue("Scene"), true);
                            Debug.Log("[KS3P_Processor]: Processing post-processing data for scene [" + nameof(targetScene) + "].");
                            bundleContents.Add(targetScene, ProfileProcessor.ProcessProfile(setup));
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.LogException(new Exception("[KS3P_Processor]: Error! Disabling KS3P! Exception occurred in second foreach loop at [" + e.StackTrace + "].", e.InnerException));
                    }
                    validNames.Add(name);
                    profileBundles.Add(name, new ProfileBundle(bundleContents));
                    isLoaded = true;
                    Save(validNames.ToArray());
                }
                //If an exception occurred, abort
                catch(Exception e)
                {
                    Debug.LogException(new Exception("[KS3P_Processor]: Error! Disabling KS3P! Exception occurred at [" + e.StackTrace + "].", e.InnerException));
                }
            }
        }
        /// <summary>
        /// Saves the list of valid names in the settings.cfg file
        /// </summary>
        /// <param name="names"></param>
        private static void Save(string[] names)
        {
            if (ConfigNode.Load(KSPUtil.ApplicationRootPath + "GameData/KS3P/Settings.cfg").GetValue("NameList") == null)
            {
                ConfigNode.Load(KSPUtil.ApplicationRootPath + "GameData/KS3P/Settings.cfg").AddValue("NameList", Compile(names));
            }
            else
            {
                ConfigNode.Load(KSPUtil.ApplicationRootPath + "GameData/KS3P/Settings.cfg").SetValue("NameList", Compile(names), true);
            }
        }
        private static string Compile(string[] names)
        {
            string constructor = "";
            //Do not process final name
            for(int x = 0; x < (names.Length - 1); x++)
            {
                constructor += (names[x] + ", ");
            }
            //NOW we process final name
            constructor += (names[names.Length - 1] + ".");
            return constructor;
        }
    }
    /*
    //This class combines all of the code, essentially this class forms the bridge between the Unity Asset Bundle and Kerbal Space Program

    //This code loads the profile. It loads only once, the first time the game reaches the main menu

    public struct Entry
    {
        public bool enabled;
        public PostProcessingProfile profile;
        public Entry(bool b, PostProcessingProfile p)
        {
            enabled = b;
            profile = p;
        }
    }

    [KSPAddon(KSPAddon.Startup.MainMenu, true)]
    public class EffectApplier : MonoBehaviour
    {
        public static Dictionary<string, string> selectedPresets = new Dictionary<string, string>();
        public static ConfigNode[] configFile;
        public static ConfigNode KS3PSettings;
        public static List<string> foundNames = new List<string>();




        //These dictionaries link a PPP with the preset name
        public static Dictionary<string, Entry> Credits = new Dictionary<string, Entry>();
        public static Dictionary<int, string> CreditsProfiles = new Dictionary<int, string>();
        public static Dictionary<string, Entry> SPH = new Dictionary<string, Entry>();
        public static Dictionary<int, string> SPHProfiles = new Dictionary<int, string>();
        public static Dictionary<string, Entry> VAB = new Dictionary<string, Entry>();
        public static Dictionary<int, string> VABProfiles = new Dictionary<int, string>();
        public static Dictionary<string, Entry> Flight = new Dictionary<string, Entry>();
        public static Dictionary<int, string> FlightProfiles = new Dictionary<int, string>();
        public static Dictionary<string, Entry> MainMenu = new Dictionary<string, Entry>();
        public static Dictionary<int, string> MMProfiles = new Dictionary<int, string>();
        public static Dictionary<string, Entry> Settings = new Dictionary<string, Entry>();
        public static Dictionary<int, string> SettingsProfiles = new Dictionary<int, string>();
        public static Dictionary<string, Entry> KSC = new Dictionary<string, Entry>();
        public static Dictionary<int, string> KSCProfiles = new Dictionary<int, string>();

        //For flight subscenes
        public static Dictionary<string, Entry> EVA = new Dictionary<string, Entry>();
        public static Dictionary<int, string> EVAProfiles = new Dictionary<int, string>();
        public static Dictionary<string, Entry> IVA = new Dictionary<string, Entry>();
        public static Dictionary<int, string> IVAProfiles = new Dictionary<int, string>();

        public static bool patched = false; //False on startup, will permanently become true once the profiles have been patched.
        public static bool processing = false; //This bool shows whether or not the Profile Parser is busy.

        public static bool loadedCFG = false, loadedShaders = false;
        //In the Start function, we intialize
        void Start()
        {
            //Fil in preset list
            KS3PSettings = ConfigNode.Load(KSPUtil.ApplicationRootPath + "/GameData/KS3P/Settings.cfg").GetNode("KS3P_Settings"); //Store settings in Config Node to allow ModuleManager patching
            selectedPresets.Add("Credits", KS3PSettings.GetValue("SelectedProfile_Credits"));
            selectedPresets.Add("SPH", KS3PSettings.GetValue("SelectedProfile_SPH"));
            selectedPresets.Add("VAB", KS3PSettings.GetValue("SelectedProfile_VAB"));
            selectedPresets.Add("Flight", KS3PSettings.GetValue("SelectedProfile_Flight"));
            selectedPresets.Add("MainMenu", KS3PSettings.GetValue("SelectedProfile_MainMenu"));
            selectedPresets.Add("Settings", KS3PSettings.GetValue("SelectedProfile_Settings"));
            selectedPresets.Add("KSC", KS3PSettings.GetValue("SelectedProfile_SpaceCenter"));
            selectedPresets.Add("EVA", KS3PSettings.GetValue("SelectedProfile_EVA"));
            selectedPresets.Add("IVA", KS3PSettings.GetValue("SelectedProfile_IVA"));

            //Load configs
            configFile = serializeConfigs();

            processing = true; //To avoid other initalizers from calling the void while this class is already processing it.

            if (!patched) { Load(); } //To avoid double-loading if another script took over
            else
            {
                processing = false; //Another script did the job for us? Then skip.
            }
        }

        static List<ConfigNode> serializedArray(ConfigNode[] arr)
        {
            List<ConfigNode> found = new List<ConfigNode>();
            for (int x = 0; x < arr.Length; x++)
            {
                found.Add(arr[x]);
            }
            return found;
        }

        public static ConfigNode[] serializeConfigs()
        {
            UrlDir.UrlConfig[] foundConfigs = GameDatabase.Instance.GetConfigs("Post_Processing"); //Grab all urlDir configs
            ConfigNode[] nodes = new ConfigNode[foundConfigs.Length]; //Init new array
            for (int x = 0; x < foundConfigs.Length; x++) //Cycle through all found configs
            {
                nodes[x] = foundConfigs[x].config;
            }
            return nodes;
        }
        public static void LoadProfile(ConfigNode profile, int index)
        {
            string name = profile.GetValue("Name");
            Debug.Log("[KSP_PostProcessing_ProfileParser]: Processing config file [" + name + ".");
            foundNames.Add(name);
            //This will store all scene-profiles loaded from the config file
            List<ConfigNode> ProfileList = serializedArray(profile.GetNodes("SETUP")); //Store all profiles in a list



            Debug.Log("[KSP_PostProcesing_ProfileParser]: Generating profile for scene [Credits]...");
            Credits.Add(name, ProfileProcessor.LoadProfile(ProfileList, "Credits"));
            if (!Credits[name].enabled) { Debug.LogWarning("[KSP_PostProcessing_ProfileParser]: Warning! No data found for scene [Credits], disabling Post-Processing for this scene!"); }


            Debug.Log("[KSP_PostProcesing_ProfileParser]: Generating profile for scene [SpacePlaneHangar]...");
            SPH.Add(name, ProfileProcessor.LoadProfile(ProfileList, "SpacePlaneHangar"));
            if (!SPH[name].enabled) { Debug.LogWarning("[KSP_PostProcessing_ProfileParser]: Warning! No data found for scene [SpacePlaneHangar], disabling Post-Processing for this scene!"); }


            Debug.Log("[KSP_PostProcesing_ProfileParser]: Generating profile for scene [VehicleAssemblyBuilding]...");
            VAB.Add(name, ProfileProcessor.LoadProfile(ProfileList, "VehicleAssemblyBuilding"));
            if (!VAB[name].enabled) { Debug.LogWarning("[KSP_PostProcessing_ProfileParser]: Warning! No data found for scene [VehicleAssemblyBuilding], disabling Post-Processing for this scene!"); }


            Debug.Log("[KSP_PostProcesing_ProfileParser]: Generating profile for scene [Flight]...");
            Flight.Add(name, ProfileProcessor.LoadProfile(ProfileList, "Flight"));
            if (!Flight[name].enabled) { Debug.LogWarning("[KSP_PostProcessing_ProfileParser]: Warning! No data found for scene [Flight], disabling Post-Processing for this scene!"); }


            Debug.Log("[KSP_PostProcesing_ProfileParser]: Generating profile for scene [MainMenu]...");
            MainMenu.Add(name, ProfileProcessor.LoadProfile(ProfileList, "MainMenu"));
            if (!MainMenu[name].enabled) { Debug.LogWarning("[KSP_PostProcessing_ProfileParser]: Warning! No data found for scene [MainMenu], disabling Post-Processing for this scene!"); }


            Debug.Log("[KSP_PostProcesing_ProfileParser]: Generating profile for scene [Settings]...");
            Settings.Add(name, ProfileProcessor.LoadProfile(ProfileList, "Settings"));
            if (!SettingsExists) { Debug.LogWarning("[KSP_PostProcessing_ProfileParser]: Warning! No data found for scene [Settings], disabling Post-Processing for this scene!"); }


            Debug.Log("[KSP_PostProcesing_ProfileParser]: Generating profile for scene [KerbalSpaceCenter]...");
            KSC.Add(name, ProfileProcessor.LoadProfile(ProfileList, "KerbalSpaceCenter"));
            if (!KSCExists) { Debug.LogWarning("[KSP_PostProcessing_ProfileParser]: Warning! No data found for scene [KerbalSpaceCenter], disabling Post-Processing for this scene!"); }

            //Debug.Log("[KSP_PostProcesing_ProfileParser]: Generating profile for scene [TrackingStation]...");
            //ProfileProcessor.LoadProfile(out TS, ProfileList, "TrackingStation", out TSExists);
            //if (!TSExists) { Debug.LogWarning("[KSP_PostProcessing_ProfileParser]: Warning! No data found for scene [TrackingStation], disabling Post-Processing for this scene!"); }

            //Debug.Log("[KSP_PostProcesing_ProfileParser]: Generating profile for scene [Galaxy]...");
            //ProfileProcessor.LoadProfile(out Galaxy, ProfileList, "Galaxy", out GalaxyExists);
            //if (!GalaxyExists) { Debug.LogWarning("[KSP_PostProcessing_ProfileParser]: Warning! No data found for scene [Galaxy], disabling Post-Processing for this scene!"); }

            //For the subscenes
            Debug.Log("[KSP_PostProcesing_ProfileParser]: Generating profile for situation [EVA]...");
            EVA.Add(name, ProfileProcessor.LoadProfile(ProfileList, "EVA"));
            if (!EVAExists) { Debug.LogWarning("[KSP_PostProcessing_ProfileParser]: Warning! No data found for situation [EVA], disabling dynamic Post-Processing for this situation!"); }

            Debug.Log("[KSP_PostProcesing_ProfileParser]: Generating profile for situation [IVA]...");
            IVA.Add(name, ProfileProcessor.LoadProfile(ProfileList, "IVA"));
            if (!IVAExists) { Debug.LogWarning("[KSP_PostProcessing_ProfileParser]: Warning! No data found for situation [IVA], disabling dynamic Post-Processing for this situation!"); }
        }
        public static void Load()
        {
            Debug.Log("[KSP_PostProcessing_ProfileParser]: Beginning parsing operation.");
            //Load the config file
            if (!loadedCFG) //If the CFG has been loaded already, skip
            {
                Debug.Log("[KSP_PostProcessing_ProfileParser]: Loading config file...");
                configFile = serializeConfigs();
                Debug.Log("[KSP_PostProcessing_ProfileParser]: Config file loaded.");
                loadedCFG = true;
            }

            //Load the shaders
            if (!loadedShaders) //If the shaders have been loaded already, skip
            {
                Debug.Log("[KSP_PostProcessing_ProfileParser]: Loading shaders...");
                ShaderLoader.LoadAssetBundle("KS3P/Shaders", "postprocessingshaders");
                Debug.Log("[KSP_PostProcessing_ProfileParser]: Shaders loaded.");
            }

            //Replacing Shader.Find in the Unity MaterialFactory with the external shader loading stuff should fix things up nicely. :)

            //Store all profiles

            Debug.Log("[KSP_PostProcessing_ProfileParser]: Initializing profile...");

            Debug.Log("[KSP_PostProcessing_ProfileParser]: Loading profiles...");

            Debug.Log("[KSP_PostProcessing_ProfileParser]: Found [" + configFile.Length + "] KS3P config files.");
            for (int x = 0; x < configFile.Length; x++)
            {
                LoadProfile(configFile[x], x);
            }

            //Add profiles
            Debug.Log("[KSP_PostProcessing_ProfileParser]: Indexing profiles...");

            Debug.Log("[KSP_PostProcessing_ProfileParser]: Initializing Post-Processing profiles...");




            //Debug.Log("[KSP_PostProcesing_ProfileParser]: Generating profile for situation [MapView]...");
            //ProfileProcessor.LoadProfile(out MapView, ProfileList, "MapView", out MapViewExists);
            //if (!MapViewExists) { Debug.LogWarning("[KSP_PostProcessing_ProfileParser]: Warning! No data found for situation [MapView], disabling dynamic Post-Processing for this situation!"); }




            processing = false;
            Debug.Log("[KSP_PostProcessing_ProfileParser]: Finished initializing profile.");
            patched = true; //We finished patching.
            //GameEvents.onLevelWasLoaded.Add(new EventData<GameScenes>.OnEvent(OnSceneChange));
        }
    }
    */
}