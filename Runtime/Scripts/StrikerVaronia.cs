

#if Striker
using StrikerLink.Unity.Runtime.Core;
using StrikerLink.Unity.Runtime.HapticEngine;
#endif

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using System.Linq;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public enum Color_L
{
    Green = 1,
    Orange = 2,
    Red = 3,
    Yellow = 4,
    Dead = 5,
}

namespace VaroniaBackOffice
{

    public class StrikerVaronia : MonoBehaviour
    {
#if Striker

        public StrikerDevice strikerDevice;

        // public List<Weapon_Haptic> weapons;

        public HapticEffectAsset Init;

        public HapticEffectAsset Respawn_H;

        public bool Debug;


        [BoxGroup("Pivot")]
        public Transform Pivot;


        FixSteamTrackerID fixWrist;





        public static StrikerVaronia Instance;


        //[ShowIf("Debug")]
        //public WEAPON_TYPE WeapTest;
        [ShowIf("Debug")]
        public bool FULLAUTO;
        [ReadOnly]
        [ShowIf("Debug")]
        public bool IsTrigger;
        [ReadOnly]
        [ShowIf("Debug")]
        public bool IsReload;
        [ReadOnly]
        [ShowIf("Debug")]
        public bool IsFront;
        [ReadOnly]
        [ShowIf("Debug")]
        public Vector3 Trackpad_R;
        [ShowIf("Debug")]
        public Vector3 Trackpad_L;
        [ShowIf("Debug")]
        public bool Function_1;
        [ShowIf("Debug")]
        public bool Function_2;

        [HideInInspector]
        public Color_L _color;



        private string Info;


        [BoxGroup("Render")] public GameObject Render, BadLed, GoodLed;
        [BoxGroup("Render")] public Transform Trigger, SecondaryButton_L, SecondaryButton_R;
        [BoxGroup("Render")] public Image BatteryLevel_UI;
        [BoxGroup("Render")] public GameObject Wrist_Left, Wrist_Right;


        [BoxGroup("Parameter")] public float WaitTimeLostTracking = 1;

        public IEnumerator Start()
        {
            Instance = this;

        
            yield return new WaitUntil(() => Config.VaroniaConfig != null);

            if (Config.VaroniaConfig.Controller != Controller.FOCUS3_VBS_Striker)
            {
                Destroy(gameObject);
                yield break;
            }

            VaroniaInput.Instance.Render = Render;
            VaroniaInput.Instance.WaitTimeLostWeaponTracking = WaitTimeLostTracking;
            VaroniaInput.Instance.Pivot = Pivot;



            fixWrist = GetComponentInChildren<FixSteamTrackerID>();


            fixWrist.OnLeftTracker.AddListener(EventWrist_L);
            fixWrist.OnRightTracker.AddListener(EventWrist_R);

           // Render.SetActive(true);
            VaroniaInput.Instance.Pivot = Pivot;
            SceneManager.activeSceneChanged += SceneManager_activeSceneChanged;


            yield return new WaitUntil(() => strikerDevice.isConnected);
            yield return new WaitForSeconds(1);

            BadLed.SetActive(false);
            GoodLed.SetActive(true);

            for (int i = 0; i < 3; i++)
            {
                strikerDevice.PlaySolidLedEffect(Color.green);
                strikerDevice.PlaySolidLedEffect(Color.green, 0, StrikerLink.Shared.Devices.Types.DeviceMavrik.LedGroup.FrontRings);
                yield return new WaitForSeconds(0.3f);

                    strikerDevice.FireHaptic(Init);
                strikerDevice.PlaySolidLedEffect(Color.blue);
                strikerDevice.PlaySolidLedEffect(Color.blue, 0, StrikerLink.Shared.Devices.Types.DeviceMavrik.LedGroup.FrontRings);
                yield return new WaitForSeconds(0.1f);

            }
            strikerDevice.PlaySolidLedEffect(Color.green, 0, StrikerLink.Shared.Devices.Types.DeviceMavrik.LedGroup.FrontRings);
            strikerDevice.PlaySolidLedEffect(Color.green);



            while (true)
            {

                BadLed.SetActive(!strikerDevice.isConnected);
                GoodLed.SetActive(strikerDevice.isConnected);
                yield return new WaitForSeconds(0.1f);
            }
        }


        public void LedTop(Color C, Color_L C_)
        {
            if (_color == C_)
                return;

            strikerDevice.PlaySolidLedEffect(C, 0, StrikerLink.Shared.Devices.Types.DeviceMavrik.LedGroup.TopLine);
            _color = C_;
        }


        public void LedFront(Color C)
        {
            strikerDevice.PlaySolidLedEffect(C, 0, StrikerLink.Shared.Devices.Types.DeviceMavrik.LedGroup.FrontRings);
        }




        public void Respawn()
        {

         
            strikerDevice.FireHaptic(Respawn_H);
        }


        bool LibraryOk;
        private void Update()
        {


            if (SteamFocus3Varonia.Instance == null)
                return;


            DebugInfo();
            InputWrapper();


#if VBO_VBS
            if (SteamFocus3Varonia.Instance.HMD_Ready && !LibraryOk)
            {
                GetComponent<StrikerController>().UpdateHapticLibrary();
                LibraryOk = true;
            }

            if (!SteamFocus3Varonia.Instance.HMD_Ready && LibraryOk)
                LibraryOk = false;
#endif
        }

        public void LateUpdate()
        {
            if (DebugVaronia.Instance == null)
                return;

            DebugVaronia.Instance.TextDebugInfo.text += Info;


        }



        public void DebugInfo()
        {
            string BattLevel = "";
            string IsCo = "";
            string IsRe = "";

            if (strikerDevice.batteryLevel > 0.2f)
                BattLevel = "<color=green>" + (strikerDevice.batteryLevel * 100) + " %</color>";
            else
                BattLevel = "<color=red>" + (strikerDevice.batteryLevel * 100) + " %</color>";

            if (strikerDevice.isConnected)
                IsCo = "<color=green>true</color>";
            else
                IsCo = "<color=red>false</color>";


            if (strikerDevice.isReady)
                IsRe = "<color=green>true</color>";
            else
                IsRe = "<color=red>false</color>";



            Info = "<color=white>----- Striker VR -----</color>";
            Info += "\nIs Connected : " + IsCo;
            Info += "\nIs Ready : " + IsRe;
            Info += "\n Battery Level : " + BattLevel;
            Info += "\nUID : " + strikerDevice.GUID + "\n";
        }





        public virtual void InputWrapper()
        {

            // Trigger Down
            if (strikerDevice.GetTriggerDown())
            {
                VaroniaInput.Instance.EventPrimaryDown.Invoke();
                StartCoroutine(TriggerDown());
                if (Debug) IsTrigger = true;

            }
            // Trigger Up
            if (strikerDevice.GetTriggerUp())
            {
                StartCoroutine(TriggerUp());
                VaroniaInput.Instance.EventPrimaryUp.Invoke();
                if (Debug) IsTrigger = false;
            }


            // Reload Input Down
            if (strikerDevice.GetSensorDown(StrikerLink.Shared.Devices.DeviceFeatures.DeviceSensor.ReloadTouched))
            {

                if (Debug) IsReload = true;
                VaroniaInput.Instance.EventReloadDown.Invoke();
            }




            // Reload Input Up
            if (strikerDevice.GetSensorUp(StrikerLink.Shared.Devices.DeviceFeatures.DeviceSensor.ReloadTouched))
            {
                if (Debug) IsReload = false;
                VaroniaInput.Instance.EventReloadUp.Invoke();
            }



            // Lateral Back Trigger
            if (strikerDevice.GetButtonDown(StrikerLink.Shared.Devices.DeviceFeatures.DeviceButton.SideLeft) || strikerDevice.GetButtonDown(StrikerLink.Shared.Devices.DeviceFeatures.DeviceButton.SideRight))
            {

            }

            // Lateral Front Trigger Down
            if (strikerDevice.GetButtonDown(StrikerLink.Shared.Devices.DeviceFeatures.DeviceButton.TouchpadLeft) || strikerDevice.GetButtonDown(StrikerLink.Shared.Devices.DeviceFeatures.DeviceButton.TouchpadRight))
            {

                if (Debug) IsFront = true;
                VaroniaInput.Instance.EventSecondaryDown.Invoke();
                SecondaryDown();
            }

            // Lateral Front Trigger Up
            if (strikerDevice.GetButtonUp(StrikerLink.Shared.Devices.DeviceFeatures.DeviceButton.TouchpadLeft) || strikerDevice.GetButtonUp(StrikerLink.Shared.Devices.DeviceFeatures.DeviceButton.TouchpadRight))
            {
                if (Debug) IsFront = false;
                SecondaryUp();
                VaroniaInput.Instance.EventSecondaryUp.Invoke();
            }

            Trackpad_R = strikerDevice.GetTouchpad(StrikerLink.Shared.Devices.DeviceFeatures.DeviceTouchpad.TouchpadRight, true);
            Trackpad_L = strikerDevice.GetTouchpad(StrikerLink.Shared.Devices.DeviceFeatures.DeviceTouchpad.TouchpadLeft, true);


            Function_1 = strikerDevice.GetButton(StrikerLink.Shared.Devices.DeviceFeatures.DeviceButton.MenuTop);
            Function_2 = strikerDevice.GetButton(StrikerLink.Shared.Devices.DeviceFeatures.DeviceButton.MenuBottom);
        }






        void EventWrist_L()
        {
            Wrist_Left.SetActive(true);
            Wrist_Right.SetActive(false);
        }


        void EventWrist_R()
        {
            Wrist_Left.SetActive(false);
            Wrist_Right.SetActive(true);
        }

        private void SceneManager_activeSceneChanged(Scene arg0, Scene arg1)
        {
            Render.SetActive(false);
        }




        // Animations

        IEnumerator TriggerDown()
        {
            Trigger.localPosition = new Vector3(0, 0.03f, 0.011f);
            for (int i = 0; i < 7; i++)
            {
                yield return new WaitForSeconds(0.01f);
                Trigger.localPosition -= new Vector3(0, 0.002f, 0);
            }
        }

        IEnumerator TriggerUp()
        {
            Trigger.localPosition = new Vector3(0, 0.016f, 0.011f);
            for (int i = 0; i < 7; i++)
            {
                yield return new WaitForSeconds(0.01f);
                Trigger.localPosition += new Vector3(0, 0.002f, 0);
            }
        }

        void SecondaryDown()
        {
            SecondaryButton_L.transform.localPosition = new Vector3(0.015f, 0.209f, 0.06f);
            SecondaryButton_R.transform.localPosition = new Vector3(-0.015f, 0.209f, 0.06f);
        }

        void SecondaryUp()
        {
            SecondaryButton_L.transform.localPosition = new Vector3(0.018f, 0.209f, 0.06f);
            SecondaryButton_R.transform.localPosition = new Vector3(-0.018f, 0.209f, 0.06f);
        }

        
#endif
    }
}
