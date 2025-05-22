using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using uPLibrary.Networking.M2Mqtt.Messages;

#if VBO_Input
namespace VaroniaBackOffice
{

    public class Varonia_Weapon_MQTT : MonoBehaviour
    {

        [BoxGroup("Parameter")] public float WaitTimeLostTracking = 1;
        // 3.15 = 0 %  4.2 % 100
        [BoxGroup("Info")] public float BatteryLevel;
        // 0 NoSignal 1 Perfect
        [BoxGroup("Info")] public float RSSI;
        [BoxGroup("Info")] public long BOOT_Time;
        [BoxGroup("Info")] public bool IsConnected;

        private bool primary_;
        private bool secondary_;
        private bool primary_Down_;
        private bool secondary_Down_;
        private bool primary_Up_;
        private bool secondary_Up_;

        public static Varonia_Weapon_MQTT instance;


        public UnityEvent EventPrimaryDown;
        public UnityEvent EventPrimaryUp;


        public UnityEvent EventSecondaryDown;
        public UnityEvent EventSecondaryUp;


        [BoxGroup("Render")] public GameObject Render;
        [BoxGroup("Render")] public GameObject NotConnect;
        [BoxGroup("Render")] public GameObject Connect;
        [BoxGroup("Render")] public Transform Trigger, SecondaryButton;
        [BoxGroup("Render")] public Image BatteryLevel_UI;
        [BoxGroup("Render")] public GameObject Primary_UI, Secondary_UI;
        [BoxGroup("Render")] public GameObject Wrist_Left, Wrist_Right;

        [BoxGroup("Pivot")]
        public Transform Pivot;



        [BoxGroup("Controller Type")]
        public Controller Controller;



        Coroutine coroutine_;





        public IEnumerator Start()
        {

            yield return new WaitUntil(() => Config.VaroniaConfig != null);

            if (Config.VaroniaConfig.Controller != Controller)
            {
                Destroy(gameObject);
                yield break;
            }

            yield return new WaitUntil(() => MQTTVaronia.instance != null);
           instance = this;


            

            MQTTVaronia.instance.client.Subscribe(new string[] { "DeviceToUnity/" + Config.VaroniaConfig.WeaponMAC + "/#" }, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });

            Debug.Log(Controller + " Subscribe to MQTT whith MAC adress : " + Config.VaroniaConfig.WeaponMAC);

            MQTTVaronia.instance.ReceiveMsg.AddListener(event_);

            VaroniaInput.Instance.Render = Render;
            VaroniaInput.Instance.WaitTimeLostWeaponTracking = WaitTimeLostTracking;
            VaroniaInput.Instance.Pivot = Pivot;

            EventPrimaryDown.AddListener(EventPrimaryDown_L);
            EventPrimaryUp.AddListener(EventPrimaryUp_L);

            EventSecondaryDown.AddListener(EventSecondaryDown_L);
            EventSecondaryUp.AddListener(EventSecondaryUp_L);





        }



        // Check connection time out 
        public IEnumerator CheckCo()
        {

            IsConnected = true;
            if (IsConnected) { Connect.SetActive(true); NotConnect.SetActive(false); }
            yield return new WaitForSeconds(5f);
            IsConnected = false;
            if (!IsConnected) { Connect.SetActive(false); NotConnect.SetActive(true); }
        }


        public void event_(string title, byte[] value)
        {
            string stringvalue = System.Text.Encoding.UTF8.GetString(value);


            if (title.StartsWith("DeviceToUnity"))
                title = title.Split('/').LastOrDefault();
            else
                return;

            if (coroutine_ != null)
                StopCoroutine(coroutine_); //Receives a message from the weapon and resets the timeout coroutine.

            //Relunch Coroutine
            coroutine_ = StartCoroutine(CheckCo());

            try
            {

                if (title == "BAT") // Receive battery info
                {
                    float MAX = 4.2f;
                    float MIN = 3.15f;
                    float Value = (float)Math.Round(float.Parse(stringvalue), 1);

                    BatteryLevel = (float)Math.Round(((Value - MIN) / (MAX - MIN) * 100), 0); // Set battery level (0-1)  0 => Empty    1=> Full
                    BatteryLevel_UI.fillAmount = BatteryLevel / 100f; // Update battery screen in render
                }

                if (title == "BOOT_TIME") // The time the weapon is active, in seconds.
                    BOOT_Time = long.Parse(stringvalue);

                if (title == "RSSI") //Received Signal Strength Indicator (exemple : -30 => Very Good => -90 Very Bad)
                    RSSI = float.Parse(stringvalue);

                // Trigger
                if (title == "1")
                {
                    if (stringvalue == "1") // Trigger down
                    {
                        if (!primary_)
                            EventPrimaryDown.Invoke();
                    }

                    if (stringvalue == "0") // Trigger Up
                    {
                        if (primary_)
                            EventPrimaryUp.Invoke();
                    }
                }
                //Front side buttons
                if (title == "2")
                {
                    if (stringvalue == "1") // button down
                    {
                        if (!secondary_)
                        {

                            EventSecondaryDown.Invoke();
                        }

                    }

                    if (stringvalue == "0") // button up
                    {
                        if (secondary_)
                        {

                            EventSecondaryUp.Invoke();
                        }

                    }
                }
            }
            catch (Exception)
            {

            }

        }




        //Resets down and up to false at the end of the frame
        private void LateUpdate()
        {
            if (primary_Down_)
                primary_Down_ = false;

            if (primary_Up_)
                primary_Up_ = false;

            if (secondary_Down_)
                secondary_Down_ = false;

            if (secondary_Up_)
                secondary_Up_ = false;



            if (Config.VaroniaConfig == null)
                return;

            DebugInfo();


        }
        //Function that returns the status of the buttons
        #region Buttons status
        public bool Primary_Shoot_Down()
        {
            return primary_Down_;
        }

        public bool Primary_Shoot_Up()
        {
            return primary_Up_;
        }
        public bool Primary_Shoot()
        {
            return primary_;
        }

        public bool Secondary_Shoot_Down()
        {
            return secondary_Down_;
        }
        public bool Secondary_Shoot_Up()
        {
            return secondary_Up_;
        }
        public bool Secondary_Shoot()
        {
            return secondary_;
        }
        #endregion
        #region Debug Info

        private string Info;
        public void DebugInfo()
        {
            string BattLevel = "";
            string IsCo = "";
            string RSSI_ = "";

            if (BatteryLevel > 0.2f)
                BattLevel = "<color=green>" + BatteryLevel + " %</color>";
            else
                BattLevel = "<color=red>" + BatteryLevel + " %</color>";

            if (RSSI > 0.5f)
                RSSI_ = "<color=green>" + RSSI + "</color>";
            else
                RSSI_ = "<color=red>" + RSSI + "</color>";



            if (IsConnected)
                IsCo = "<color=green>true</color>";
            else
                IsCo = "<color=red>false</color>";

            Info = "<color=white>----- Weapon MQTT -----</color>";
            Info += "\nIs Connected : " + IsCo;
            Info += "\nRSSI: " + RSSI_;
            Info += "\n Battery Level : " + BattLevel;
            Info += "\n Boot Time : " + (BOOT_Time / 1000) + " Sec";
            Info += "\nMAC : " + Config.VaroniaConfig.WeaponMAC + "\n";


            DebugVaronia.Instance.TextDebugInfo.text += Info;

        }
        #endregion
        #region Animations
        IEnumerator TriggerDown()
        {
            Trigger.localPosition = new Vector3(0, 0, 0);
            for (int i = 0; i < 7; i++)
            {
                yield return new WaitForSeconds(0.01f);
                Trigger.localPosition += new Vector3(-0.001f, 0, 0);
            }
        }

        IEnumerator TriggerUp()
        {
            Trigger.localPosition = new Vector3(-0.007f, 0, 0);
            for (int i = 0; i < 7; i++)
            {
                yield return new WaitForSeconds(0.01f);
                Trigger.localPosition += new Vector3(0.001f, 0, 0);
            }
        }

        void SecondaryDown()
        {
            SecondaryButton.transform.localScale = new Vector3(1, 1, 0.95f);
        }

        void SecondaryUp()
        {
            SecondaryButton.transform.localScale = new Vector3(1, 1, 1.05f);
        }
        #endregion
        #region Events
        void EventPrimaryDown_L()
        {
            VaroniaInput.Instance.EventPrimaryDown.Invoke();
            Primary_UI.SetActive(true);
            StartCoroutine(TriggerDown());
            primary_ = true;
            primary_Down_ = true;
        }
        void EventPrimaryUp_L()
        {
            Primary_UI.SetActive(false);
            VaroniaInput.Instance.EventPrimaryUp.Invoke();
            StartCoroutine(TriggerUp());
            primary_ = false;
            primary_Up_ = true;
        }
        void EventSecondaryDown_L()
        {
            Secondary_UI.SetActive(true);
            VaroniaInput.Instance.EventSecondaryDown.Invoke();
            SecondaryDown();
            secondary_ = true;
            secondary_Down_ = true;
        }
        void EventSecondaryUp_L()
        {
            Secondary_UI.SetActive(false);
            VaroniaInput.Instance.EventSecondaryUp.Invoke();
            SecondaryUp();
            secondary_ = false;
            secondary_Up_ = true;
        }
        #endregion

  


    }

}
#endif