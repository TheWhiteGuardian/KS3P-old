using System;
using UnityEngine;
using KS3P.UnityPostProcessing;

namespace KS3P.Configuration
{
    //These classes run when a scene is loaded.
    //Depending on the scene, they load different profiles
    //If data for the profile of a scene is missing, PP is disabled for that scene.
    [KSPAddon(KSPAddon.Startup.Credits, false)]
    public class Applier_Credits : MonoBehaviour
    {
        static bool Enabled;
        //In Start, we check the status of the sister script in ProfileParser
        void Start()
        {
            Debug.Log("[KSP_PostProcessing_Patcher]: Checking Profile Parser status...");
            //If patched has been made true, we don't enter the while loop at all.
            while(!ProfileParser.EffectApplier.patched)
            {
                Debug.Log("[KSP_PostProcessing]: Waiting for Profile Parser to finish parsing config.");
                if (!ProfileParser.EffectApplier.processing && !ProfileParser.EffectApplier.patched)
                {
                    Debug.LogWarning("[KSP_PostProcessing]: Warning! Profile Parser seems to be inactive. Forcing patch...");
                    ProfileParser.EffectApplier.Load();
                }
            }
            Debug.Log("[KSP_PostProcessing]: Profile Parser greenlight received.");
            Enabled = ProfileParser.EffectApplier.creditsExists;
            //Effectively the while loop is there to stall until the ProfileParser is done.

            //Therefore we get right down to business.
            if (Enabled)
            {
                if (Camera.main.gameObject.GetComponent<PostProcessingBehaviour>() == null) //If the camera has no post processing component
                {
                    Camera.main.gameObject.AddComponent<PostProcessingBehaviour>();
                }
                Camera.main.gameObject.GetComponent<PostProcessingBehaviour>().profile = ProfileParser.EffectApplier.Credits;
            }
            if (Camera.main.gameObject.GetComponent<PostProcessingBehaviour>() != null)
            {
                Camera.main.gameObject.GetComponent<PostProcessingBehaviour>().enabled = Enabled;
            }
        }
        
    }

    [KSPAddon(KSPAddon.Startup.EditorSPH, false)]
    public class Applier_SPH : MonoBehaviour
    {
        static bool Enabled;
        //In Start, we check the status of the sister script in ProfileParser
        void Start()
        {
            Debug.Log("[KSP_PostProcessing_Patcher]: Checking Profile Parser status...");
            //If patched has been made true, we don't enter the while loop at all.
            while (!ProfileParser.EffectApplier.patched)
            {
                Debug.Log("[KSP_PostProcessing]: Waiting for Profile Parser to finish parsing config.");
                if (!ProfileParser.EffectApplier.processing && !ProfileParser.EffectApplier.patched)
                {
                    Debug.LogWarning("[KSP_PostProcessing]: Warning! Profile Parser seems to be inactive. Forcing patch...");
                    ProfileParser.EffectApplier.Load();
                }
            }
            Debug.Log("[KSP_PostProcessing]: Profile Parser greenlight received.");
            Enabled = ProfileParser.EffectApplier.SPHExists;
            //Effectively the while loop is there to stall until the ProfileParser is done.

            //Therefore we get right down to business.
            if (Enabled)
            {
                if (Camera.main.gameObject.GetComponent<PostProcessingBehaviour>() == null) //If the camera has no post processing component
                {
                    Camera.main.gameObject.AddComponent<PostProcessingBehaviour>();
                }
                Camera.main.gameObject.GetComponent<PostProcessingBehaviour>().profile = ProfileParser.EffectApplier.SPH;
            }
            if (Camera.main.gameObject.GetComponent<PostProcessingBehaviour>() != null)
            {
                Camera.main.gameObject.GetComponent<PostProcessingBehaviour>().enabled = Enabled;
            }
        }

    }

    [KSPAddon(KSPAddon.Startup.EditorVAB, false)]
    public class Applier_VAB : MonoBehaviour
    {
        static bool Enabled;
        //In Start, we check the status of the sister script in ProfileParser
        void Start()
        {
            Debug.Log("[KSP_PostProcessing_Patcher]: Checking Profile Parser status...");
            //If patched has been made true, we don't enter the while loop at all.
            while (!ProfileParser.EffectApplier.patched)
            {
                Debug.Log("[KSP_PostProcessing]: Waiting for Profile Parser to finish parsing config.");
                if (!ProfileParser.EffectApplier.processing && !ProfileParser.EffectApplier.patched)
                {
                    Debug.LogWarning("[KSP_PostProcessing]: Warning! Profile Parser seems to be inactive. Forcing patch...");
                    ProfileParser.EffectApplier.Load();
                }
            }
            Debug.Log("[KSP_PostProcessing]: Profile Parser greenlight received.");
            Enabled = ProfileParser.EffectApplier.VABExists;
            //Effectively the while loop is there to stall until the ProfileParser is done.

            //Therefore we get right down to business.
            if (Enabled)
            {
                if (Camera.main.gameObject.GetComponent<PostProcessingBehaviour>() == null) //If the camera has no post processing component
                {
                    Camera.main.gameObject.AddComponent<PostProcessingBehaviour>();
                }
                Camera.main.gameObject.GetComponent<PostProcessingBehaviour>().profile = ProfileParser.EffectApplier.VAB;
            }
            if (Camera.main.gameObject.GetComponent<PostProcessingBehaviour>() != null)
            {
                Camera.main.gameObject.GetComponent<PostProcessingBehaviour>().enabled = Enabled;
            }
        }

    }

    [KSPAddon(KSPAddon.Startup.Flight, false)]
    public class Applier_Flight : MonoBehaviour
    {
        static bool Enabled;
        //In Start, we check the status of the sister script in ProfileParser
        void Start()
        {
            Debug.Log("[KSP_PostProcessing_Patcher]: Checking Profile Parser status...");
            //If patched has been made true, we don't enter the while loop at all.
            while (!ProfileParser.EffectApplier.patched)
            {
                Debug.Log("[KSP_PostProcessing]: Waiting for Profile Parser to finish parsing config.");
                if (!ProfileParser.EffectApplier.processing && !ProfileParser.EffectApplier.patched)
                {
                    Debug.LogWarning("[KSP_PostProcessing]: Warning! Profile Parser seems to be inactive. Forcing patch...");
                    ProfileParser.EffectApplier.Load();
                }
            }
            Debug.Log("[KSP_PostProcessing]: Profile Parser greenlight received.");
            Enabled = ProfileParser.EffectApplier.FlightExists;
            //Effectively the while loop is there to stall until the ProfileParser is done.

            //Therefore we get right down to business.
            if (Enabled)
            {
                if (Camera.main.gameObject.GetComponent<PostProcessingBehaviour>() == null) //If the camera has no post processing component
                {
                    Camera.main.gameObject.AddComponent<PostProcessingBehaviour>();
                }
                Camera.main.gameObject.GetComponent<PostProcessingBehaviour>().profile = ProfileParser.EffectApplier.Flight;
            }
            if (Camera.main.gameObject.GetComponent<PostProcessingBehaviour>() != null)
            {
                Camera.main.gameObject.GetComponent<PostProcessingBehaviour>().enabled = Enabled;
            }
        }

    }

    [KSPAddon(KSPAddon.Startup.MainMenu, false)]
    public class Applier_MainMenu : MonoBehaviour
    {
        static bool Enabled;
        //In Start, we check the status of the sister script in ProfileParser
        void Start()
        {
            Debug.Log("[KSP_PostProcessing_Patcher]: Checking Profile Parser status...");
            //If patched has been made true, we don't enter the while loop at all.
            while (!ProfileParser.EffectApplier.patched)
            {
                Debug.Log("[KSP_PostProcessing]: Waiting for Profile Parser to finish parsing config.");
                if (!ProfileParser.EffectApplier.processing && !ProfileParser.EffectApplier.patched)
                {
                    Debug.LogWarning("[KSP_PostProcessing]: Warning! Profile Parser seems to be inactive. Forcing patch...");
                    ProfileParser.EffectApplier.Load();
                }
            }
            Debug.Log("[KSP_PostProcessing]: Profile Parser greenlight received.");
            Enabled = ProfileParser.EffectApplier.MMExists;
            //Effectively the while loop is there to stall until the ProfileParser is done.

            //Therefore we get right down to business.
            if (Enabled)
            {
                if (Camera.main.gameObject.GetComponent<PostProcessingBehaviour>() == null) //If the camera has no post processing component
                {
                    Camera.main.gameObject.AddComponent<PostProcessingBehaviour>();
                }
                Camera.main.gameObject.GetComponent<PostProcessingBehaviour>().profile = ProfileParser.EffectApplier.MainMenu;
            }
            if (Camera.main.gameObject.GetComponent<PostProcessingBehaviour>() != null)
            {
                Camera.main.gameObject.GetComponent<PostProcessingBehaviour>().enabled = Enabled;
            }
        }

    }

    [KSPAddon(KSPAddon.Startup.Settings, false)]
    public class Applier_Settings : MonoBehaviour
    {
        static bool Enabled;
        //In Start, we check the status of the sister script in ProfileParser
        void Start()
        {
            Debug.Log("[KSP_PostProcessing_Patcher]: Checking Profile Parser status...");
            //If patched has been made true, we don't enter the while loop at all.
            while (!ProfileParser.EffectApplier.patched)
            {
                Debug.Log("[KSP_PostProcessing]: Waiting for Profile Parser to finish parsing config.");
                if (!ProfileParser.EffectApplier.processing && !ProfileParser.EffectApplier.patched)
                {
                    Debug.LogWarning("[KSP_PostProcessing]: Warning! Profile Parser seems to be inactive. Forcing patch...");
                    ProfileParser.EffectApplier.Load();
                }
            }
            Debug.Log("[KSP_PostProcessing]: Profile Parser greenlight received.");
            Enabled = ProfileParser.EffectApplier.SettingsExists;
            //Effectively the while loop is there to stall until the ProfileParser is done.

            //Therefore we get right down to business.
            if (Enabled)
            {
                if (Camera.main.gameObject.GetComponent<PostProcessingBehaviour>() == null) //If the camera has no post processing component
                {
                    Camera.main.gameObject.AddComponent<PostProcessingBehaviour>();
                }
                Camera.main.gameObject.GetComponent<PostProcessingBehaviour>().profile = ProfileParser.EffectApplier.Settings;
            }
            if (Camera.main.gameObject.GetComponent<PostProcessingBehaviour>() != null)
            {
                Camera.main.gameObject.GetComponent<PostProcessingBehaviour>().enabled = Enabled;
            }
        }

    }

    [KSPAddon(KSPAddon.Startup.SpaceCentre, false)]
    public class Applier_KSC : MonoBehaviour
    {
        static bool Enabled;
        //In Start, we check the status of the sister script in ProfileParser
        void Start()
        {
            Debug.Log("[KSP_PostProcessing_Patcher]: Checking Profile Parser status...");
            //If patched has been made true, we don't enter the while loop at all.
            while (!ProfileParser.EffectApplier.patched)
            {
                Debug.Log("[KSP_PostProcessing]: Waiting for Profile Parser to finish parsing config.");
                if (!ProfileParser.EffectApplier.processing && !ProfileParser.EffectApplier.patched)
                {
                    Debug.LogWarning("[KSP_PostProcessing]: Warning! Profile Parser seems to be inactive. Forcing patch...");
                    ProfileParser.EffectApplier.Load();
                }
            }
            Debug.Log("[KSP_PostProcessing]: Profile Parser greenlight received.");
            Enabled = ProfileParser.EffectApplier.KSCExists;
            //Effectively the while loop is there to stall until the ProfileParser is done.

            //Therefore we get right down to business.
            if (Enabled)
            {
                if (Camera.main.gameObject.GetComponent<PostProcessingBehaviour>() == null) //If the camera has no post processing component
                {
                    Camera.main.gameObject.AddComponent<PostProcessingBehaviour>();
                }
                Camera.main.gameObject.GetComponent<PostProcessingBehaviour>().profile = ProfileParser.EffectApplier.KSC;
            }
            if (Camera.main.gameObject.GetComponent<PostProcessingBehaviour>() != null)
            {
                Camera.main.gameObject.GetComponent<PostProcessingBehaviour>().enabled = Enabled;
            }
        }

    }

    [KSPAddon(KSPAddon.Startup.TrackingStation, false)]
    public class Applier_TR : MonoBehaviour
    {
        static bool Enabled;
        //In Start, we check the status of the sister script in ProfileParser
        void Start()
        {
            Debug.Log("[KSP_PostProcessing_Patcher]: Checking Profile Parser status...");
            //If patched has been made true, we don't enter the while loop at all.
            while (!ProfileParser.EffectApplier.patched)
            {
                Debug.Log("[KSP_PostProcessing]: Waiting for Profile Parser to finish parsing config.");
                if (!ProfileParser.EffectApplier.processing && !ProfileParser.EffectApplier.patched)
                {
                    Debug.LogWarning("[KSP_PostProcessing]: Warning! Profile Parser seems to be inactive. Forcing patch...");
                    ProfileParser.EffectApplier.Load();
                }
            }
            Debug.Log("[KSP_PostProcessing]: Profile Parser greenlight received.");
            Enabled = ProfileParser.EffectApplier.TSExists;
            //Effectively the while loop is there to stall until the ProfileParser is done.

            //Therefore we get right down to business.
            if (Enabled)
            {
                if (Camera.main.gameObject.GetComponent<PostProcessingBehaviour>() == null) //If the camera has no post processing component
                {
                    Camera.main.gameObject.AddComponent<PostProcessingBehaviour>();
                }
                Camera.main.gameObject.GetComponent<PostProcessingBehaviour>().profile = ProfileParser.EffectApplier.TS;
            }
            if (Camera.main.gameObject.GetComponent<PostProcessingBehaviour>() != null)
            {
                Camera.main.gameObject.GetComponent<PostProcessingBehaviour>().enabled = Enabled;
            }
        }

    }
}
