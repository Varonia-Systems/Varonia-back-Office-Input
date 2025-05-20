using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Valve.VR;

namespace VaroniaBackOffice
{

    public class WeaponVortex : MonoBehaviour
    {
#if VBO_VORTEX
        private bool primary_;
        private bool secondary_;
        private bool primary_Down_;
        private bool secondary_Down_;
        private bool primary_Up_;
        private bool secondary_Up_;

        public static WeaponVortex Vortex;


        public UnityEvent EventPrimaryDown;
        public UnityEvent EventPrimaryUp;


        public UnityEvent EventSecondaryDown;
        public UnityEvent EventSecondaryUp;

        private string Info;

        public bool DifferentInputLeftRight;

        [HideIf("DifferentInputLeftRight")]
        public SteamVR_Action_Boolean PrimarySteam;
        [HideIf("DifferentInputLeftRight")]
        public SteamVR_Action_Boolean SecondarySteam;



        [ShowIf("DifferentInputLeftRight")]
        public SteamVR_Action_Boolean PrimarySteam_Left;
        [ShowIf("DifferentInputLeftRight")]
        public SteamVR_Action_Boolean SecondarySteam_Left;

        [ShowIf("DifferentInputLeftRight")]
        public SteamVR_Action_Boolean PrimarySteam_Right;
        [ShowIf("DifferentInputLeftRight")]
        public SteamVR_Action_Boolean SecondarySteam_Right;



        [BoxGroup("Pivot")]
        public Transform Pivot;







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

        SteamVR_Input_Sources MainHand = SteamVR_Input_Sources.RightHand;

        public bool VortexIsReady;

        public IEnumerator Start()
        {
            Vortex = this;

            yield return new WaitUntil(() => Config.VaroniaConfig != null);

            if (Config.VaroniaConfig.Controller != Controller.VORTEX_WEAPON_FOCUS)
            {
                Destroy(gameObject);
                yield break;
            }

            if (Config.VaroniaConfig.UseVortexBackOffice)
            {

                yield return new WaitUntil(() => VortexIsReady);
                yield return new WaitForSeconds(0.8f);
            }



            if (Config.VaroniaConfig.MainHand == VaroniaBackOffice.MainHand.Right)
                MainHand = SteamVR_Input_Sources.RightHand;
            else
                MainHand = SteamVR_Input_Sources.LeftHand;



            VaroniaInput.Instance.Pivot = Pivot;



            EventPrimaryDown.AddListener(EventPrimaryDown_L);
            EventPrimaryUp.AddListener(EventPrimaryUp_L);

            EventSecondaryDown.AddListener(EventSecondaryDown_L);
            EventSecondaryUp.AddListener(EventSecondaryUp_L);

        }


        private void Update()
        {


            if (!DifferentInputLeftRight)
            {
                if (PrimarySteam.GetStateDown(MainHand)) EventPrimaryDown.Invoke();
                if (PrimarySteam.GetStateUp(MainHand)) EventPrimaryUp.Invoke();

                if (SecondarySteam.GetStateDown(MainHand)) EventSecondaryDown.Invoke();
                if (SecondarySteam.GetStateUp(MainHand)) EventSecondaryUp.Invoke();
            }



            if (DifferentInputLeftRight)
            {
                if (Config.VaroniaConfig.MainHand == VaroniaBackOffice.MainHand.Left)
                {
                    if (PrimarySteam_Left.stateDown) EventPrimaryDown.Invoke();
                    if (PrimarySteam_Left.stateUp) EventPrimaryUp.Invoke();


                    if (SecondarySteam_Left.stateDown) EventSecondaryDown.Invoke();
                    if (SecondarySteam_Left.stateUp) EventSecondaryUp.Invoke();
                }

                if (Config.VaroniaConfig.MainHand == VaroniaBackOffice.MainHand.Right)
                {
                    if (PrimarySteam_Right.stateDown) EventPrimaryDown.Invoke();
                    if (PrimarySteam_Right.stateUp) EventPrimaryUp.Invoke();


                    if (SecondarySteam_Right.stateDown) EventSecondaryDown.Invoke();
                    if (SecondarySteam_Right.stateUp) EventSecondaryUp.Invoke();
                }
            }

        }

#else

        void Awake()
        {

            Destroy(gameObject);
        }

#endif
    }

}

