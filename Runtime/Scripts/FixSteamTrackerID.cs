using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using Valve.VR;
using VaroniaBackOffice;


#if VBO_Input
public class FixSteamTrackerID : MonoBehaviour
{
    public UnityEvent OnLeftTracker;
    public UnityEvent OnRightTracker;




    void OnEnable()
    {
        StartCoroutine(Start_());
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= SceneManager_sceneLoaded;
    }

    IEnumerator Start_()
    {
        SceneManager.sceneLoaded += SceneManager_sceneLoaded;

        yield return new WaitUntil(() => Config.VaroniaConfig != null);
        yield return new WaitForSeconds(1f);


        if (Config.VaroniaConfig.Controller == Controller.FOCUS3_VBS_VaroniaGun || Config.VaroniaConfig.Controller == Controller.FOCUS3_VBS_Striker || Config.VaroniaConfig.Controller == Controller.FOCUS3_VBS_HK416) // VIVE BUSINESS STREAMING (FOCUS WRIST)
        {
            StartCoroutine(ViveBuisnessStreaming());
        }
        else if (Config.VaroniaConfig.Controller == Controller.PICO_VSVR_VaroniaGun || Config.VaroniaConfig.Controller == Controller.PICO_VSVR_Striker  )  // VARONIA STREAMER VR (SWITF 2.0 PICO)
        {
            StartCoroutine(VaroniaStreamingVR());

        }
#if VBO_VORTEX
       else if (Config.VaroniaConfig.Controller == Controller.VORTEX_WEAPON_FOCUS) // FOR VORTEX Gun
        {
            StartCoroutine(VortexGun());
        }
#endif
        else   // Other
        {
            StartCoroutine(Other());
        }

    }



    IEnumerator ViveBuisnessStreaming()
    {
        while (SteamVR.instance != null)
        {
            for (int i = 0; i < SteamVR.connected.Length; ++i)
            {
                if (!SteamVR.connected[i])
                    continue;

                try
                {
                    VRControllerState_t state1 = new VRControllerState_t();
                    TrackedDevicePose_t pose1 = new TrackedDevicePose_t();
                    string RenderModelName = "";
                    if (OpenVR.System != null) RenderModelName = SteamVR.instance.GetStringProperty(Valve.VR.ETrackedDeviceProperty.Prop_RenderModelName_String, (uint)i);
                    var ControllerStateWithPose = false;
                    if (OpenVR.System != null)
                        ControllerStateWithPose = OpenVR.System.GetControllerStateWithPose(ETrackingUniverseOrigin.TrackingUniverseRawAndUncalibrated, (uint)i, ref state1, (uint)System.Runtime.InteropServices.Marshal.SizeOf(typeof(VRControllerState_t)), ref pose1);

                    if (RenderModelName.Contains("right_tracker") && ControllerStateWithPose)
                    {
                        OnRightTracker.Invoke();
                        GetComponent<SteamVR_TrackedObject>().index = (SteamVR_TrackedObject.EIndex)i;
                    }

                    if (RenderModelName.Contains("left_tracker") && ControllerStateWithPose)
                    {
                        OnLeftTracker.Invoke();
                        GetComponent<SteamVR_TrackedObject>().index = (SteamVR_TrackedObject.EIndex)i;
                    }
                }
                catch (System.Exception)
                {
                }
            }
            yield return new WaitForSeconds(1f);

        }
    }
    IEnumerator VaroniaStreamingVR()
    {
        while (SteamVR.instance != null)
        {


            for (int i = 0; i < SteamVR.connected.Length; ++i)
            {

                if (!SteamVR.connected[i])
                    continue;

                if (SteamVR.instance == null)
                    continue; // Sécurité supplémentaire

                try
                {
                    string C = "";
                    if (OpenVR.System != null) C = SteamVR.instance.GetStringProperty(Valve.VR.ETrackedDeviceProperty.Prop_ModelNumber_String, (uint)i);
                    var D = false;
                    VRControllerState_t state1 = new VRControllerState_t();
                    TrackedDevicePose_t pose1 = new TrackedDevicePose_t();

                    if (OpenVR.System != null)
                        D = OpenVR.System.GetControllerStateWithPose(ETrackingUniverseOrigin.TrackingUniverseRawAndUncalibrated, (uint)i, ref state1, (uint)System.Runtime.InteropServices.Marshal.SizeOf(typeof(VRControllerState_t)), ref pose1);

                    if (C.Contains("Vive Tracker") && D)
                    {
                        GetComponent<SteamVR_TrackedObject>().index = (SteamVR_TrackedObject.EIndex)i;
                    }
                }
                catch (System.Exception)
                {
                }
            }
            yield return new WaitForSeconds(1f);
        }
    }
    IEnumerator VortexGun()
    {
        while (SteamVR.instance != null)
        {


            for (int i = 0; i < SteamVR.connected.Length; ++i)
            {

                if (!SteamVR.connected[i])
                    continue;

                try
                {
                    var C = SteamVR.instance.GetStringProperty(Valve.VR.ETrackedDeviceProperty.Prop_RenderModelName_String, (uint)i);

                    if (C.Contains("left") && Config.VaroniaConfig.MainHand == MainHand.Left)
                    {
                        GetComponent<SteamVR_TrackedObject>().index = (SteamVR_TrackedObject.EIndex)i;
                    }

                    if (C.Contains("right") && Config.VaroniaConfig.MainHand == MainHand.Right)
                    {
                        GetComponent<SteamVR_TrackedObject>().index = (SteamVR_TrackedObject.EIndex)i;
                    }


                }
                catch (System.Exception)
                {
                }
            }
            yield return new WaitForSeconds(1f);
        }

    }
    IEnumerator Other()
    {
        while (SteamVR.instance != null)
        {

            if (Config.VaroniaConfig.MainHand == MainHand.Right)
                GetComponent<SteamVR_TrackedObject>().index = SteamVR_TrackedObject.EIndex.Device1;
            else
                GetComponent<SteamVR_TrackedObject>().index = SteamVR_TrackedObject.EIndex.Device2;

            yield return new WaitForSeconds(1f);

        }
    }



    //  /!\ SECURITY /!\
    private void SceneManager_sceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        StartCoroutine(wait());
    }

    IEnumerator wait()
    {
        var A = GetComponent<SteamVR_TrackedObject>().enabled = false;
        yield return new WaitForSeconds(0.1f);
        GetComponent<SteamVR_TrackedObject>().enabled = true;
    }





}
#endif