﻿using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace VaroniaBackOffice
{
    public class VaroniaInput : MonoBehaviour
    {
        public DateTime LastInput;

        public static VaroniaInput Instance;

        public UnityEvent EventPrimaryDown;
        public UnityEvent EventPrimaryUp;


        public UnityEvent EventSecondaryDown;
        public UnityEvent EventSecondaryUp;


        public UnityEvent EventReloadDown;
        public UnityEvent EventReloadUp;



        private bool primary_;
        private bool primary_Down_;
        private bool primary_Up_;

        private bool secondary_;
        private bool secondary_Down_;
        private bool secondary_Up_;


        private bool reload_;
        private bool reload_Down_;
        private bool reload_Up_;


        public GameObject Render;



        public Image _1, _2,_3;


        [BoxGroup("Pivot")]
        public Transform Pivot;


        [BoxGroup("Other")]
        public Transform OffsetCompensation;

        [BoxGroup("Other")]
        public float WaitTimeLostWeaponTracking = 1;


        [BoxGroup("Parameter")] public bool showDebugRenderInit, hideDebugRenderAfterChangeScene;


        private void Awake()
        {
            Instance = this;
        }



        private void SceneManager_activeSceneChanged(Scene arg0, Scene arg1)
        {
            Render.SetActive(false);
        }




        private IEnumerator Start()
        {
            DontDestroyOnLoad(gameObject);




            EventPrimaryDown.AddListener(EventPrimaryDown_L);
            EventPrimaryUp.AddListener(EventPrimaryUp_L);

            EventSecondaryDown.AddListener(EventSecondaryDown_L);
            EventSecondaryUp.AddListener(EventSecondaryUp_L);

            EventReloadDown.AddListener(EventReloadDown_L);
            EventReloadUp.AddListener(EventReloadUp_L);


            yield return new WaitUntil(() => Render != null);

            // Debug Render
            Render.SetActive(showDebugRenderInit);
            if (hideDebugRenderAfterChangeScene)
                SceneManager.activeSceneChanged += SceneManager_activeSceneChanged;

        }


        void EventPrimaryDown_L()
        {
            LastInput = DateTime.Now;

            primary_ = true;
            primary_Down_ = true;

            _1.color = Color.red;

        }
        void EventPrimaryUp_L()
        {
            LastInput = DateTime.Now;

            primary_ = false;
            primary_Up_ = true;

            _1.color = Color.white;

        }
        void EventSecondaryDown_L()
        {
            LastInput = DateTime.Now;

            secondary_ = true;
            secondary_Down_ = true;


            _2.color = Color.red;

        }
        void EventSecondaryUp_L()
        {
            LastInput = DateTime.Now;

            secondary_ = false;
            secondary_Up_ = true;

            _2.color = Color.white;


        }
        void EventReloadDown_L()
        {
            LastInput = DateTime.Now;

            reload_ = true;
            reload_Down_ = true;

            _3.color = Color.red;

        }
        void EventReloadUp_L()
        {
            LastInput = DateTime.Now;

            reload_ = false;
            reload_Up_ = true;

            _3.color = Color.white;
        }




        private void Update()
        {

            if (DebugVaronia.Instance.AdvDebugMove)
            {
                MouseHook.Update();

                if (MouseHook.GetMouseButtonDown(0))
                    VaroniaInput.Instance.EventPrimaryDown.Invoke();

                if (MouseHook.GetMouseButtonUp(0))
                    VaroniaInput.Instance.EventPrimaryUp.Invoke();

                if (MouseHook.GetMouseButtonDown(1))
                    VaroniaInput.Instance.EventSecondaryDown.Invoke();

                if (MouseHook.GetMouseButtonUp(1))
                    VaroniaInput.Instance.EventSecondaryUp.Invoke();

                if (MouseHook.GetMouseButtonDown(2))
                    VaroniaInput.Instance.EventReloadDown.Invoke();

                if (MouseHook.GetMouseButtonUp(2))
                    VaroniaInput.Instance.EventReloadUp.Invoke();

                MouseHook.LateUpdate();
            }
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


            if (reload_Down_)
                reload_Down_ = false;

            if (reload_Up_)
                reload_Up_ = false;



            if (VaroniaGlobal.VG == null || VaroniaGlobal.VG.Rig == null)
                return;

         
             OffsetCompensation.transform.position = VaroniaGlobal.VG.Rig.position; OffsetCompensation.transform.rotation = VaroniaGlobal.VG.Rig.rotation; 



        }

        public bool Primary_Down()
        {
            return primary_Down_;
        }

        public bool Primary_Up()
        {
            return primary_Up_;
        }
        public bool Primary_()
        {
            return primary_;
        }

        public bool Secondary_Down()
        {
            return secondary_Down_;
        }
        public bool Secondary_Up()
        {
            return secondary_Up_;
        }
        public bool Secondary_()
        {
            return secondary_;
        }



        public bool Reload_Down()
        {
            return reload_Down_;
        }
        public bool Reload_Up()
        {
            return reload_Up_;
        }
        public bool Reload_()
        {
            return reload_;
        }


        public void HideRender()
        {
            if(Render != null)
            Render.SetActive(false);
        }



    }
}