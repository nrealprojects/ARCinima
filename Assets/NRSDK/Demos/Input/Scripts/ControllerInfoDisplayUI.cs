using System;
using UnityEngine;
using UnityEngine.UI;

namespace NRKernal.NRExamples
{
    public class ControllerInfoDisplayUI : MonoBehaviour
    {
        public Text mainInfoText;
        public Text extraInfoText;

        private string m_ExtraInfoStr;
        private int m_MaxLine = 20;

        private void Update()
        {
            if (NRInput.GetButtonDown(ControllerButton.TRIGGER))
            {
                NRInput.TriggerHapticVibration(0.5f);
                AddExtraInfo("trigger_btn_down");
            }

            if (NRInput.GetButtonDown(ControllerButton.HOME))
                AddExtraInfo("home_btn_down");

            if (NRInput.GetButtonDown(ControllerButton.APP))
                AddExtraInfo("app_btn_down");

            if (NRInput.GetButtonUp(ControllerButton.TRIGGER))
                AddExtraInfo("trigger_btn_up");

            if (NRInput.GetButtonUp(ControllerButton.HOME))
                AddExtraInfo("home_btn_up");

            if (NRInput.GetButtonUp(ControllerButton.APP))
                AddExtraInfo("app_btn_up");

            FollowMainCam();
            RefreshInfoTexts();
        }

        private void FollowMainCam()
        {
            transform.position = Camera.main.transform.position;
            transform.rotation = Camera.main.transform.rotation;
        }

        private void RefreshInfoTexts()
        {
            mainInfoText.text =
                "controller count: " + NRInput.GetAvailableControllersCount().ToString() + "\n"
                + "type: " + NRInput.GetControllerType().ToString() + "\n"
                + "domain hand: " + NRInput.DomainHand.ToString() + "\n"
                + "position available: " + NRInput.GetControllerAvailableFeature(ControllerAvailableFeature.CONTROLLER_AVAILABLE_FEATURE_POSITION).ToString() + "\n"
                + "rotation available: " + NRInput.GetControllerAvailableFeature(ControllerAvailableFeature.CONTROLLER_AVAILABLE_FEATURE_ROTATION).ToString() + "\n"
                + "gyro available: " + NRInput.GetControllerAvailableFeature(ControllerAvailableFeature.CONTROLLER_AVAILABLE_FEATURE_GYRO).ToString() + "\n"
                + "accel available: " + NRInput.GetControllerAvailableFeature(ControllerAvailableFeature.CONTROLLER_AVAILABLE_FEATURE_ACCEL).ToString() + "\n"
                + "mag available: " + NRInput.GetControllerAvailableFeature(ControllerAvailableFeature.CONTROLLER_AVAILABLE_FEATURE_MAG).ToString() + "\n"
                + "battery available: " + NRInput.GetControllerAvailableFeature(ControllerAvailableFeature.CONTROLLER_AVAILABLE_FEATURE_BATTERY).ToString() + "\n"
                + "vibration available: " + NRInput.GetControllerAvailableFeature(ControllerAvailableFeature.CONTROLLER_AVAILABLE_FEATURE_HAPTIC_VIBRATE).ToString() + "\n"
                + "rotation: " + NRInput.GetRotation().ToString("F3") + "\n"
                + "position: " + NRInput.GetPosition().ToString("F3") + "\n"
                + "touch: " + NRInput.GetTouch().ToString("F3") + "\n"
                + "trigger, home, app: " + NRInput.GetButton(ControllerButton.TRIGGER).ToString() + NRInput.GetButton(ControllerButton.HOME).ToString() + NRInput.GetButton(ControllerButton.APP).ToString() + "\n"
                + "gyro: " + NRInput.GetGyro().ToString("F3") + "\n"
                + "accel: " + NRInput.GetAccel().ToString("F3") + "\n"
                + "mag: " + NRInput.GetMag().ToString("F3") + "\n"
                + "battery: " + NRInput.GetControllerBattery();
            extraInfoText.text = m_ExtraInfoStr;
        }

        private void AddExtraInfo(string infoStr)
        {
            if (string.IsNullOrEmpty(infoStr))
                return;
            if (string.IsNullOrEmpty(m_ExtraInfoStr))
                m_ExtraInfoStr = infoStr;
            else
                m_ExtraInfoStr = m_ExtraInfoStr + Environment.NewLine + infoStr;
            int count = m_ExtraInfoStr.Split(new string[] { Environment.NewLine }, StringSplitOptions.None).Length;
            if (count > m_MaxLine)
                m_ExtraInfoStr = m_ExtraInfoStr.Substring(m_ExtraInfoStr.IndexOf(Environment.NewLine) + Environment.NewLine.Length);
        }
    }
}
