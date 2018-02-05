using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using KS3P.Processor;
using KS3P.Shaders;

/*
 Todo
 Integrate lockdown
     */


namespace KS3P.Core
{
    [KSPAddon(KSPAddon.Startup.Instantly, false)]
    public sealed class ProcessingController : MonoBehaviour
    {
        /// <summary>
        /// The bundle currently used in rendering
        /// </summary>
        public static ProfileBundle targetBundle;
        /// <summary>
        /// The time in s for KS3P to wait with updating on scene load
        /// </summary>
        static float bufferTime;
        /// <summary>
        /// Stores all valid names, accessible through an integer
        /// </summary>
        Dictionary<int, string> nameCollection = new Dictionary<int, string>();
        /*
        /// <summary>
        /// Inverted indexation of nameCollection, used in the starting function
        /// </summary>
        Dictionary<string, int> nameCollectionInv = new Dictionary<string, int>(); //This is so that when we parse the starting profile from the settings.cfg, we can easily grab the corresponding index number
                                                                                   //Maybe use a for-loop for this?
                                                                                   */
        /// <summary>
        /// If false, KS3P is not enabled.
        /// </summary>
        public static bool KS3PEnabled = true;
        /// <summary>
        /// Returns the main camera game object
        /// </summary>
        GameObject mainCam
        {
            get { if (Camera.main != null) return Camera.main.gameObject; else return null; }
        }
        bool mainCamExists
        {
            get
            {
                if (mainCam == null) { return false; } else return true;
            }
        }
        /// <summary>
        /// Calling this void adds a post-processing behaviour component to the main camera.
        /// </summary>
        void AddBehaviour()
        {
            mainCam.gameObject.AddComponent<PostProcessingBehaviour>();
        }
        /// <summary>
        /// Returns the post-processing behaviour component attached to the camera.
        /// </summary>
        PostProcessingBehaviour GetBehaviour
        {
            get { return mainCam.GetComponent<PostProcessingBehaviour>(); }
        }
        /// <summary>
        /// Calling this function automatically updates the targetBundle
        /// </summary>
        void UpdateBundle()
        {
            targetBundle = ProfileParser.profileBundles[nameCollection[activeProfile]];
        }
        /// <summary>
        /// Returns true if the main camera has a post-processing behaviour component
        /// </summary>
        bool cameraHasBehaviour
        {
            get
            {
                if (GetBehaviour != null) return true;
                else return false;
            }
        }
        bool nameLibBuilt = false;
        /// <summary>
        /// The active profile
        /// </summary>
        int activeProfile;
        /// <summary>
        /// The active profile last frame (used to check if an update is needed)
        /// </summary>
        int activeProfileLastFrame;
        bool activeProfileSelected = false;
        /// <summary>
        /// If all shaders have been loaded, this bool becomes true
        /// </summary>
        bool loadedShaders = false;
        /// <summary>
        /// If true, the loading buffer has been initialized
        /// </summary>
        bool loadedBuffer = false;
        bool nameExists(string targetname)
        {
            for (int x = 0; x < nameCollection.Count; x++)
            {
                if (nameCollection[x] == targetname)
                {
                    return true;
                }
            }
            return false;
        }
        int GetIndexOf(string name)
        {
            for(int x = 0; x < nameCollection.Count; x++)
            {
                if (nameCollection[x] == name) { return x; }
            }
            Debug.LogError("[KS3P]: Error! Could not find index corresponding to name [" + name + "].");
            return 0;
        }
        int GetStartupIndex()
        {
            string startup = ConfigNode.Load(KSPUtil.ApplicationRootPath + "GameData/KS3P/Settings.cfg").GetValue("Startup");
            if (!nameExists(startup))
            {
                Debug.LogError("[KS3P]: Startup target profile not found! Setting to backup!");
                return 0;
            }
            else
            {
                return GetIndexOf(startup);
            }
        }
        private void Awake()
        {
            Debug.Log("[KS3P]: I am now awake. Anyone made bacon yet?");
            DontDestroyOnLoad(this); //Keep loaded
            if (!loadedShaders)
            {
                ShaderLoader.LoadAssetBundle("KS3P/Shaders", "postprocessingshaders");
                loadedShaders = true;
            }
            if (!ProfileParser.isLoaded)
            {
                ProfileParser.Load();
                ProfileParser.isLoaded = true;
            }
            if (!loadedBuffer)
            {
                bufferTime = float.Parse(ConfigNode.Load(KSPUtil.ApplicationRootPath + "GameData/KS3P/Settings.cfg").GetValue("BufferTimeOnSceneLoad"));
            }
            if (!nameLibBuilt)
            {
                for (int x = 0; x < ProfileParser.validNames.Count; x++)
                {
                    nameCollection.Add(x, ProfileParser.validNames[x]);
                    //nameCollectionInv.Add(ProfileParser.validNames[x], x);
                }
                nameLibBuilt = true;
            }
            if (!activeProfileSelected)
            {
                activeProfile = GetStartupIndex();
                activeProfileSelected = true;
            }
            if(!ProfileParser.lockdown)
            {
                //If locked down, we will not subscribe and instead shut down KS3P
                UpdateBundle();
                GameEvents.onGameSceneLoadRequested.Add(new EventData<GameScenes>.OnEvent(OnSceneChange)); //Subscribe to scene change
            }
        }
        //This void is called every scene change and also tells what scene we're heading into.
        void OnSceneChange(GameScenes scene)
        {
            Debug.Log("[KS3P]: Scene change detected. Initializing KS3P update in 10 seconds...");
            StartCoroutine(KS3PUpdate(scene, true));
        }
        IEnumerator KS3PUpdate(GameScenes scene, bool buffer)
        {
            if(buffer)
            {
                yield return new WaitForSecondsRealtime(bufferTime);
            }
            Debug.Log("[KS3P]: KS3P update activate! Target scene[" + scene + "].");
            if (scene == GameScenes.FLIGHT)
            {
                //If we are heading into flight, start the flight update routine
                StartCoroutine(FlightRoutine());
            }
            else
            {
                //If not, stop the routine to avoid nullrefs.
                StopCoroutine(FlightRoutine());
                //Are we in the main menu?
                if (scene == GameScenes.MAINMENU)
                {
                    CameraOperation(ProfileType.MainMenu);
                }
                //We are in an editor scene
                else if (scene == GameScenes.EDITOR)
                {
                    //Are we in the SPH?
                    if (EditorDriver.editorFacility == EditorFacility.SPH)
                    {
                        CameraOperation(ProfileType.SPH);
                    }
                    //Are we in the VAB?
                    else if (EditorDriver.editorFacility == EditorFacility.VAB) { CameraOperation(ProfileType.VAB); }
                }
                else if (scene == GameScenes.SPACECENTER)
                {
                    CameraOperation(ProfileType.KSC);
                }
                else if (scene == GameScenes.TRACKSTATION)
                {
                    ScaledCameraOperation(ProfileType.TS);
                }
            }
        }
        private void Update()
        {
            if (Input.GetKey(KeyCode.LeftAlt) && Input.GetKeyDown(KeyCode.Alpha3))
            {
                if (KS3PEnabled)
                {
                    DisableKS3P();
                    KS3PEnabled = false;
                }
                else
                {
                    EnableKS3P();
                    KS3PEnabled = true;
                }
            }
            if (Input.GetKey(KeyCode.LeftAlt))
            {
                if (Input.GetKeyDown(KeyCode.LeftBracket)) Increase();
                if (Input.GetKeyDown(KeyCode.RightBracket)) Decrease();
            }
        }
        void CameraOperation(ProfileType t)
        {
            if(mainCamExists)
            {
                if (!cameraHasBehaviour)
                {
                    AddBehaviour();
                }
                GetBehaviour.profile = targetBundle.GetProfile(t);
                GetBehaviour.enabled = !(ProfileParser.lockdown || !KS3PEnabled); //Lockdown comp.
            }
        }
        void ScaledCameraOperation(ProfileType t)
        {
            GameObject scaledCamera = GameObject.Find("Camera ScaledSpace");
            if (scaledCamera != null)
            {
                if (scaledCamera.GetComponent<PostProcessingBehaviour>() == null)
                {
                    scaledCamera.AddComponent<PostProcessingBehaviour>();
                }
                scaledCamera.GetComponent<PostProcessingBehaviour>().profile = targetBundle.GetProfile(t);
                scaledCamera.GetComponent<PostProcessingBehaviour>().enabled = !(ProfileParser.lockdown || !KS3PEnabled); //Lockdown comp.
            }
        }
        void Increase()
        {
            //Clamp between zero and max index number to avoid an IndexOutOfRangeException
            Mathf.Clamp(activeProfile + 1, 0, nameCollection.Count - 1);
            Debug.Log("[KS3P]: Selecting profile [" + nameCollection[activeProfile] + "].");
            //This is to avoid an unnecessary update.
            //Note that, if we are already at the minimum or maximum clamp count, activeProfile is not altered.
            //Thus a new bundle is not selected, and an update is not needed.
            if (activeProfileLastFrame != activeProfile)
            {
                UpdateBundle();
                activeProfileLastFrame = activeProfile;
            }
        }
        void Decrease()
        {
            //Clamp between zero and max index number to avoid an IndexOutOfRangeException
            Mathf.Clamp(activeProfile - 1, 0, nameCollection.Count - 1);
            Debug.Log("[KS3P]: Selecting profile [" + nameCollection[activeProfile] + "].");
            //This is to avoid an unnecessary update.
            //Note that, if we are already at the minimum or maximum clamp count, activeProfile is not altered.
            //Thus a new bundle is not selected, and an update is not needed.
            if (activeProfileLastFrame != activeProfile)
            {
                UpdateBundle();
                activeProfileLastFrame = activeProfile;
            }
        }
        //This coroutine is started if the scene changes to flight.
        enum KS3PFlightState
        {
            IVA,
            EVA,
            Flight,
            Map,
            Initial
        }
        /// <summary>
        /// This state is updated by the FlightRoutine coroutine to avoid unnecessary void calling
        /// </summary>
        KS3PFlightState lastFrame = KS3PFlightState.Initial;
        IEnumerator FlightRoutine()
        {
            lastFrame = KS3PFlightState.Initial; //Reset to initial before we start the operation
            while(true)
            {
                //Ensure FG ready state
                if (FlightGlobals.ready)
                {
                    if (MapView.MapIsEnabled)
                    {
                        if (lastFrame != KS3PFlightState.Map) { ScaledCameraOperation(ProfileType.Map); lastFrame = KS3PFlightState.Map; }
                    }
                    else if(InternalCamera.Instance != null && InternalCamera.Instance.isActive)
                    {
                        //IVA
                        if (lastFrame != KS3PFlightState.IVA) { CameraOperation(ProfileType.IVA); lastFrame = KS3PFlightState.IVA; }
                    }
                    else if (FlightGlobals.ActiveVessel.isEVA)
                    {
                        //EVA
                        if (lastFrame != KS3PFlightState.EVA) { CameraOperation(ProfileType.EVA); lastFrame = KS3PFlightState.EVA; }
                    }
                    else
                    {
                        //Normal flight
                        if (lastFrame != KS3PFlightState.Flight) { CameraOperation(ProfileType.Flight); lastFrame = KS3PFlightState.Flight; }
                    }
                }
                yield return null;
            }
        }
        /// <summary>
        /// If this void is called, KS3P is forced awake
        /// </summary>
        public void EnableKS3P()
        {
            Debug.Log("[KS3P]: Starting up... [EnableKS3P()]");
            if(!ProfileParser.lockdown)
            {
                StartCoroutine(KS3PUpdate(HighLogic.LoadedScene, false)); //Call OnSceneChange to re-initialize camera procedure, using HighLogic to get the current scene
            }
            else
            {
                Debug.LogWarning("[KS3P]: Could not restart - KS3P is in lockdown!");
            }
        }
        /// <summary>
        /// If this void is called, KS3P is forced to sleep
        /// </summary>
        public void DisableKS3P()
        {
            Debug.Log("[KS3P]: Shutting down... [DisableKS3P()]");
            if (mainCamExists)
            {
                if (mainCam != null)
                {
                    GetBehaviour.enabled = false;
                }
            }
        }
    }
    /*
    //The KS3P manager framework
    public abstract class KS3P : MonoBehaviour
    {
        public static ProfileBundle targetBundle;
        bool listenersAdded = false;
        public static void ConfirmHandshake(string name)
        {
            Debug.Log("[KS3P_BaseManagerClass]: Hanshake received, setting new target bundle named [" + name + "].");
        }
        private void Start()
        {
            if (!listenersAdded)
            {
                ProcessingController.OnEnableKS3P.AddListener(new UnityAction(EnableKS3P));
                ProcessingController.OnDisableKS3P.AddListener(new UnityAction(DisableKS3P));
            }
        }
        public void Apply(ProfileType t)
        {
            if (Camera.main != null ) //&& targetBundle.SceneExists(t))
            {
                if (Camera.main.GetComponent<PostProcessingBehaviour>() != null)
                {
                    Camera.main.gameObject.AddComponent<PostProcessingBehaviour>();
                }
                Camera.main.gameObject.GetComponent<PostProcessingBehaviour>().profile = targetBundle.GetProfile(t);
                Camera.main.gameObject.GetComponent<PostProcessingBehaviour>().enabled = ProcessingController.KS3PEnabled;
            }
        }
        public void FlightCheck()
        {
            if (InternalCamera.Instance.isActive) Apply(ProfileType.IVA);
            else if (FlightGlobals.ready && FlightGlobals.ActiveVessel.isEVA) Apply(ProfileType.EVA);
            else Apply(ProfileType.Flight);
        }
        /// <summary>
        /// Called when the disable combo is executed
        /// </summary>
        public void DisableKS3P()
        {
            if (Camera.main != null)
            {
                if (Camera.main.GetComponent<PostProcessingBehaviour>() != null) { Camera.main.GetComponent<PostProcessingBehaviour>().enabled = false; }
            }
        }
        /// <summary>
        /// Calls when the enable combo is executed
        /// </summary>
        public void EnableKS3P()
        {
            if (Camera.main != null)
            {
                if (Camera.main.GetComponent<PostProcessingBehaviour>() != null) { Camera.main.GetComponent<PostProcessingBehaviour>().enabled = true; }
            }
        }
    }

    [KSPAddon(KSPAddon.Startup.EditorSPH, false)]
    public class SPH_Manager : KS3P
    {
        private void Start()
        {
            Apply(ProfileType.SPH);
        }
    }
    [KSPAddon(KSPAddon.Startup.EditorVAB, false)]
    public class VAB_Manager : KS3P
    {
        private void Start()
        {
            Apply(ProfileType.VAB);
        }
    }
    [KSPAddon(KSPAddon.Startup.Flight, false)]
    public class Flight_Manager : KS3P
    {
        private void Update()
        {
            FlightCheck();
        }
    }
    [KSPAddon(KSPAddon.Startup.MainMenu, false)]
    public class MM_Manager : KS3P
    {
        private void Start()
        {
            Apply(ProfileType.MainMenu);
        }
    }
    [KSPAddon(KSPAddon.Startup.SpaceCentre, false)]
    public class KSC_Manager : KS3P
    {
        private void Start()
        {
            Apply(ProfileType.KSC);
        }
    }
    /*
    public enum Target
    {
        FlightProfiles,
        VAB,
        SPH,
        MM,
        KSC,
        None
    }
    
    public enum KS3PFlightState
    {
        Main,
        EVA,
        IVA,
        Initial
    }
    [KSPAddon(KSPAddon.Startup.EveryScene, false)]
    public class KS3P : MonoBehaviour
    {
        ProfileBundle bundle;
        Dictionary<int, string> nameCollection = new Dictionary<int, string>();
        Dictionary<string, int> nameCollectionInv = new Dictionary<string, int>();
        bool isActive = true;
        bool nameLibBuilt = false;
        int activeProfile;
        bool activeProfileSelected = false;
        int GetStartupIndex()
        {
            string startup = ConfigNode.Load(KSPUtil.ApplicationRootPath + "GameData/KS3P/Settings.cfg").GetValue("Startup");
            if (nameCollectionInv.ContainsKey(startup))
            {
                Debug.LogError("[KS3P]: Startup target profile not found! Setting to backup!");
                return 0;
            }
            else
            {
                return nameCollectionInv[startup];
            }
        }
        Target getProfile()
        {
            if (HighLogic.LoadedSceneIsFlight) return Target.FlightProfiles;
            else if (HighLogic.LoadedSceneIsEditor)
            {
                if (EditorDriver.editorFacility == EditorFacility.SPH) return Target.SPH;
                else return Target.VAB;
            }
            else if (HighLogic.LoadedScene == GameScenes.MAINMENU) return Target.MM;
            else if (HighLogic.LoadedScene == GameScenes.SPACECENTER) return Target.KSC;
            else return Target.None;
        }
        Target current;
        void Start()
        {
            //stop all coroutines for safety, restart later
            StopCoroutine(PatchFlightCamera(bundle));
            StopCoroutine(KillSwitch());
            StopCoroutine(Swap());
            //Restart coroutines
            StartCoroutine(KillSwitch());
            StartCoroutine(Swap());
            current = getProfile();
            if (isActive) Activate();
            else Deactivate();
        }
        bool SceneExist(Target profile)
        {
            switch(profile)
            {
                case Target.KSC: return bundle.SceneExists(ProfileType.KSC);
                case Target.MM: return bundle.SceneExists(ProfileType.MainMenu);
                case Target.SPH: return bundle.SceneExists(ProfileType.SPH);
                case Target.VAB: return bundle.SceneExists(ProfileType.VAB);
                case Target.None: return false;
                case Target.FlightProfiles: return bundle.SceneExists(ProfileType.Flight) || bundle.SceneExists(ProfileType.EVA) || bundle.SceneExists(ProfileType.IVA);
                default: return false;
            }
        }
        /// <summary>
        /// Calls the correct PatchCamera function depending on the target profile input parameter
        /// </summary>
        /// <param name="targetProfile"></param>
        void Execute(Target targetProfile)
        {
            if (targetProfile == Target.None || !SceneExist(targetProfile))
            {
                Deactivate();
            }
            else
            {
                bundle = ProfileParser.profileBundles[nameCollection[activeProfile]];
                if (targetProfile == Target.KSC && bundle.SceneExists(ProfileType.KSC))
                {
                    PatchCamera(bundle.GetProfile(ProfileType.KSC));
                }
                else if (targetProfile == Target.MM && bundle.SceneExists(ProfileType.MainMenu))
                {
                    PatchCamera(bundle.GetProfile(ProfileType.MainMenu));
                }
                else if (targetProfile == Target.SPH && bundle.SceneExists(ProfileType.SPH))
                {
                    PatchCamera(bundle.GetProfile(ProfileType.SPH));
                }
                else if (targetProfile == Target.VAB && bundle.SceneExists(ProfileType.VAB))
                {
                    PatchCamera(bundle.GetProfile(ProfileType.VAB));
                }
                else
                {
                    StartCoroutine(PatchFlightCamera(bundle));
                }
            }
        }
        /*
        private IEnumerator PatchFlightCamera(ProfileBundle bundle)
        {
            KS3PFlightState lastframe = KS3PFlightState.Initial; //Set to initial to force update on first cycle
            while(true)
            {
                //Check IVA status
                if (InternalCamera.Instance.isActive)
                {
                    if (lastframe != KS3PFlightState.IVA)
                    {
                        PatchCamera(bundle.GetProfile(ProfileType.IVA));
                    }
                }
                //Check EVA status
                //Flightglobals ready, active vessel is an EVA kerbal, and the last state was not EVA (no update needed)
                else if (FlightGlobals.ready && FlightGlobals.ActiveVessel.isEVA)
                {
                    if (lastframe != KS3PFlightState.EVA)
                    {
                        PatchCamera(bundle.GetProfile(ProfileType.EVA));
                    }
                }
                else
                {
                    if (lastframe != KS3PFlightState.Main)
                    {
                        PatchCamera(bundle.GetProfile(ProfileType.Flight));
                    }
                }
            }
        }

        void PatchCamera(PostProcessingProfile profile)
        {
            if (Camera.main != null)
            {
                if (Camera.main.gameObject.GetComponent<PostProcessingBehaviour>() != null)
                {
                    Camera.main.gameObject.AddComponent<PostProcessingBehaviour>();
                }
                Camera.main.gameObject.GetComponent<PostProcessingBehaviour>().profile = profile;
                Camera.main.gameObject.GetComponent<PostProcessingBehaviour>().enabled = true;
            }
        }
        
        public void Deactivate()
        {
            StopCoroutine(PatchFlightCamera(bundle));
            Debug.Log("[KS3P]: Shutting down...");
            if (Camera.main != null && Camera.main.gameObject.GetComponent<PostProcessingBehaviour>() != null)
            {
                Camera.main.gameObject.GetComponent<PostProcessingBehaviour>().enabled = false;
            }
        }
        public void Activate()
        {
            Debug.Log("[KS3P]: Booting up...");
            if (!ProfileParser.isLoaded)
            {
                ProfileParser.Load();
                ProfileParser.isLoaded = true;
            }
            if (!nameLibBuilt)
            {
                for(int x = 0; x < ProfileParser.validNames.Count; x++)
                {
                    nameCollection.Add(x, ProfileParser.validNames[x]);
                    nameCollectionInv.Add(ProfileParser.validNames[x], x);
                }
                nameLibBuilt = true;
            }
            if (!activeProfileSelected)
            {
                activeProfile = GetStartupIndex();
                activeProfileSelected = true;
            }
            if (!ProfileParser.lockdown)
            {
                Execute(current);
                //Safety-restart of the coroutines
                StopCoroutine(KillSwitch());
                StopCoroutine(Swap());
                StartCoroutine(KillSwitch());
                StartCoroutine(Swap());
            }
            else
            {
                Deactivate();
            }
        }
        
        IEnumerator KillSwitch()
        {
            //Wander forever...
            while(true)
            {
                if ((Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt)) && Input.GetKeyDown(KeyCode.Alpha3))
                {
                    if (isActive)
                    {
                        Deactivate();
                        isActive = false;
                    }
                    else
                    {
                        Activate();
                        isActive = true;
                    }
                }
                yield return null;
            }
        }
        IEnumerator Swap()
        {
            while(true)
            {
                if((Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt)))
                {
                    if (Input.GetKeyDown(KeyCode.LeftBracket)) Increase();
                    if (Input.GetKeyDown(KeyCode.RightBracket)) Decrease();
                }
            }
        }
        void Increase()
        {
            Mathf.Clamp(activeProfile + 1, 0, nameCollection.Count - 1);
            Debug.Log("[KS3P]: Selecting profile [" + nameCollection[activeProfile] + "].");
            Activate();
        }
        void Decrease()
        {
            Mathf.Clamp(activeProfile - 1, 0, nameCollection.Count - 1);
            Debug.Log("[KS3P]: Selecting profile [" + nameCollection[activeProfile] + "].");
            Activate();
        }
    }
    */
}
