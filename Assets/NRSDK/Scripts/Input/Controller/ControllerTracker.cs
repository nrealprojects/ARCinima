/****************************************************************************
* Copyright 2019 Nreal Techonology Limited. All rights reserved.
*                                                                                                                                                          
* This file is part of NRSDK.                                                                                                          
*                                                                                                                                                           
* https://www.nreal.ai/        
* 
*****************************************************************************/

namespace NRKernal
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    /// @cond EXCLUDE_FROM_DOXYGEN
    public class ControllerTracker : MonoBehaviour {
        public ControllerHandEnum defaultHandEnum;
        public NRPointerRaycaster raycaster;
        public Transform modelAnchor;

        private float m_VerifyYAngle = 0f;
        private bool m_IsEnabled = false;
        private bool m_Is6dof;
        private Vector3 m_DefaultLocalOffset;
        private Vector3 m_TargetPos = Vector3.zero;
        private bool m_IsMovingToTarget = false;
        private Transform m_CameraCenter
        {
            get
            {
                return NRInput.CameraCenter;
            }
        }

        private const float TrackTargetSpeed = 6f;
        private const float MaxDistanceFromTarget = 0.12f;

        private void Awake()
        {
            m_DefaultLocalOffset = transform.localPosition;
        }

        private void OnEnable()
        {
            NRInput.OnControllerRecentered += OnRecentered;
        }

        private void OnDisable()
        {
            NRInput.OnControllerRecentered -= OnRecentered;
        }

        private void Update()
        {
            if (m_CameraCenter == null)
                return;
            m_IsEnabled = NRInput.CheckControllerAvailable(defaultHandEnum);
            UpdateTracker();
        }

        private void UpdateTracker()
        {
            raycaster.gameObject.SetActive(m_IsEnabled && NRInput.RaycastMode == RaycastModeEnum.Laser);
            modelAnchor.gameObject.SetActive(m_IsEnabled);
            if (m_IsEnabled)
                TrackPose();
        }

        private void TrackPose()
        {
            m_Is6dof = NRInput.GetControllerAvailableFeature(ControllerAvailableFeature.CONTROLLER_AVAILABLE_FEATURE_POSITION)
                && NRInput.GetControllerAvailableFeature(ControllerAvailableFeature.CONTROLLER_AVAILABLE_FEATURE_ROTATION);
            if (m_Is6dof)
                UpdatePosition();
            else
                SmoothTrackTargetPosition();
            UpdateRotation();
        }

        private void UpdatePosition()
        {
            transform.localPosition = m_DefaultLocalOffset + NRInput.GetPosition(defaultHandEnum);
        }

        private void UpdateRotation()
        {
            transform.localRotation = NRInput.GetRotation(defaultHandEnum);
            transform.Rotate(Vector3.up * m_VerifyYAngle, Space.World);
        }

        private void SmoothTrackTargetPosition()
        {
            int sign = defaultHandEnum == ControllerHandEnum.Right ? 1 : -1;
            m_TargetPos = m_CameraCenter.position + Vector3.up * m_DefaultLocalOffset.y
                + new Vector3(m_CameraCenter.right.x, 0f, m_CameraCenter.right.z).normalized * Mathf.Abs(m_DefaultLocalOffset.x) * sign
                + new Vector3(m_CameraCenter.forward.x, 0f, m_CameraCenter.forward.z).normalized * m_DefaultLocalOffset.z;
            if (!m_IsMovingToTarget)
            {
                if (Vector3.Distance(transform.position, m_TargetPos) > MaxDistanceFromTarget)
                    m_IsMovingToTarget = true;
            }
            else
            {
                transform.position = Vector3.Lerp(transform.position, m_TargetPos, TrackTargetSpeed * Time.deltaTime);
                if (Vector3.Distance(m_TargetPos, transform.position) < 0.02f)
                    m_IsMovingToTarget = false;
            }
        }

        public void OnRecentered(ControllerHandEnum handEnum)
        {
            if (handEnum != defaultHandEnum)
                return;
            Vector3 horizontalFoward = Vector3.ProjectOnPlane(m_CameraCenter.forward, Vector3.up);
            m_VerifyYAngle = Mathf.Sign(Vector3.Cross(Vector3.forward, horizontalFoward).y) * Vector3.Angle(horizontalFoward, Vector3.forward);
        }
    }
    /// @endcond
}