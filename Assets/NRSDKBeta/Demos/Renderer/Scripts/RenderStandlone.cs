﻿using UnityEngine;

namespace NRKernal.NRExamples
{
    public class RenderStandlone : MonoBehaviour
    {
        public Camera left;
        public Camera right;

        private NRRenderer m_NRRenderer;

        void Start()
        {
            m_NRRenderer = gameObject.AddComponent<NRRenderer>();
            m_NRRenderer.Initialize(left, right, GetHeadPose);
        }

        private void GetHeadPose(ref Pose pose)
        {
            pose = new Pose(left.transform.position, left.transform.rotation);
        }
    }
}
