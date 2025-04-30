using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Valve.VR;

#if VBO_Input
namespace VaroniaBackOffice
{

    public class Weapon_HK416 : MonoBehaviour
    {
        
        public bool IsConnected;

        private bool primary_;
        private bool secondary_;
        private bool primary_Down_;
        private bool secondary_Down_;
        private bool primary_Up_;
        private bool secondary_Up_;



        public SteamVR_Action_Boolean PrimarySteam;

        public static Weapon_HK416 HK;


        public UnityEvent EventPrimaryDown;
        public UnityEvent EventPrimaryUp;


        public UnityEvent EventSecondaryDown;
        public UnityEvent EventSecondaryUp;

        private string Info;

        

        [BoxGroup("Render")] public GameObject Render;
        [BoxGroup("Render")] public GameObject NotConnect;
        [BoxGroup("Render")] public GameObject Connect;
        [BoxGroup("Render")] public GameObject Wrist_Left, Wrist_Right;

        [BoxGroup("Pivot")]
        public Transform Pivot;


        FixSteamTrackerID fixWrist;

  

        public IEnumerator CheckCo()
        {

            IsConnected = true;
            if (IsConnected) { Connect.SetActive(true); NotConnect.SetActive(false); }
            yield return new WaitForSeconds(5f);
            IsConnected = false;
            if (!IsConnected) { Connect.SetActive(false); NotConnect.SetActive(true); }
        }



        void EventPrimaryDown_L()
        {
            VaroniaInput.Instance.EventPrimaryDown.Invoke();
            primary_ = true;
            primary_Down_ = true;
        }
        void EventPrimaryUp_L()
        {
            VaroniaInput.Instance.EventPrimaryUp.Invoke();         
            primary_ = false;
            primary_Up_ = true;
        }
        void EventSecondaryDown_L()
        {
            VaroniaInput.Instance.EventSecondaryDown.Invoke();
            secondary_ = true;
            secondary_Down_ = true;
        }
        void EventSecondaryUp_L()
        {
            VaroniaInput.Instance.EventSecondaryUp.Invoke();
            secondary_ = false;
            secondary_Up_ = true;
        }


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


        void Awake()
        {


        }

        private void Update()
        {



            if (PrimarySteam.stateDown)
                EventPrimaryDown.Invoke();
            if (PrimarySteam.stateUp)
                EventPrimaryUp.Invoke();

    


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



        public IEnumerator Start()
        {
            yield return new WaitUntil(() => Config.VaroniaConfig != null);

            if (Config.VaroniaConfig.Controller != Controller.FOCUS3_VBS_HK416)
            {
                Destroy(gameObject);
                yield break;
            }

            Render.SetActive(true);


            HK = this;

            VaroniaInput.Instance.Pivot = Pivot;

            EventPrimaryDown.AddListener(EventPrimaryDown_L);
            EventPrimaryUp.AddListener(EventPrimaryUp_L);

            EventSecondaryDown.AddListener(EventSecondaryDown_L);
            EventSecondaryUp.AddListener(EventSecondaryUp_L);

            fixWrist = GetComponentInChildren<FixSteamTrackerID>();

            fixWrist.OnLeftTracker.AddListener(EventWrist_L);
            fixWrist.OnRightTracker.AddListener(EventWrist_R);

            SceneManager.activeSceneChanged += SceneManager_activeSceneChanged;
        }

        private void SceneManager_activeSceneChanged(Scene arg0, Scene arg1)
        {
            Render.SetActive(false);
        }

        public void DebugInfo()
        {
            string IsCo = "";

            if (IsConnected)
                IsCo = "<color=green>true</color>";
            else
                IsCo = "<color=red>false</color>";

            Info = "<color=white>----- Weapon HK416 -----</color>";
            Info += "\nIs Connected : " + IsCo+"\n";
         

            DebugVaronia.Instance.TextDebugInfo.text += Info;

        }


    }

}
#endif