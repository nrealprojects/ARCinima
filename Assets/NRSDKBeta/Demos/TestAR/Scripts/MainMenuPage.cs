namespace NRKernal.NRExamples
{
    using System;
    using NRKernal;
    using UnityEngine;
    using UnityEngine.UI;

    public class MainMenuPage : MonoBehaviour
    {
        public Transform Buttons;
        public Transform TestSceneRoot;
        public NRButton slamBtn;
        public NRButton planeBtn;
        public NRButton markerBtn;
        public NRButton m2pBtn;
        public NRButton singleMarkerBtn;
        public GameObject slamTestPrefab;
        public GameObject planeDetectPrefab;
        public GameObject markerDetectPrefab;
        public GameObject m2pPrefab;
        public GameObject singleMarkerPrefab;

        private GameObject currentTest;

        public enum TestType
        {
            Slam,
            Plane,
            Marker,
            M2p,
            SingleMarker,
        }

        private bool m_IsInTest = false;

        void Start()
        {
            slamBtn.OnClick += SlamBtn_OnClick;
            planeBtn.OnClick += PlaneBtn_OnClick;
            markerBtn.OnClick += MarkerBtn_OnClick;
            m2pBtn.OnClick += M2pBtn_OnClick;
            singleMarkerBtn.OnClick += SingleMarkerBtn_OnClick;
        }

        private void SingleMarkerBtn_OnClick()
        {
            JumpTo(TestType.SingleMarker);
        }

        private void M2pBtn_OnClick()
        {
            JumpTo(TestType.M2p);
        }

        private void MarkerBtn_OnClick()
        {
            JumpTo(TestType.Marker);
        }

        private void PlaneBtn_OnClick()
        {
            JumpTo(TestType.Plane);
        }

        private void SlamBtn_OnClick()
        {
            JumpTo(TestType.Slam);
        }

        private void JumpTo(TestType testtype)
        {
            if (m_IsInTest)
            {
                return;
            }

            switch (testtype)
            {
                case TestType.Slam:
                    currentTest = Instantiate(slamTestPrefab, TestSceneRoot);
                    break;
                case TestType.Plane:
                    currentTest = Instantiate(planeDetectPrefab, TestSceneRoot);
                    break;
                case TestType.Marker:
                    currentTest = Instantiate(markerDetectPrefab, TestSceneRoot);
                    break;
                case TestType.M2p:
                    currentTest = Instantiate(m2pPrefab, TestSceneRoot);
                    break;
                case TestType.SingleMarker:
                    currentTest = Instantiate(singleMarkerPrefab, TestSceneRoot);
                    break;
                default:
                    break;
            }
            Buttons.gameObject.SetActive(false);
            m_IsInTest = true;
        }

        private void Update()
        {
            if ((NRInput.GetButtonDown(ControllerButton.APP) || Input.GetKeyDown(KeyCode.Q)) && m_IsInTest)
            {
                BackToMainPage();
            }

            if (Input.GetKeyDown(KeyCode.S))
            {
                JumpTo(TestType.Slam);
            }
            if (Input.GetKeyDown(KeyCode.P))
            {
                JumpTo(TestType.Plane);
            }
            if (Input.GetKeyDown(KeyCode.M))
            {
                JumpTo(TestType.Marker);
            }
            if (Input.GetKeyDown(KeyCode.O))
            {
                JumpTo(TestType.M2p);
            }
            if (Input.GetKeyDown(KeyCode.N))
            {
                JumpTo(TestType.SingleMarker);
            }
        }

        private void BackToMainPage()
        {
            if (currentTest != null)
            {
                DestroyImmediate(currentTest);
                currentTest = null;
            }
            Buttons.gameObject.SetActive(true);
            m_IsInTest = false;
        }
    }
}