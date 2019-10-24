using System.Collections.Generic;
using UnityEngine;

namespace NRKernal.NRExamples
{
    public class PlaneDetectTester : MonoBehaviour
    {
        /// <summary>
        /// A prefab for tracking and visualizing detected planes.
        /// </summary>
        public GameObject DetectedPlanePrefab;

        /// <summary>
        /// A list to hold new planes NRSDK began tracking in the current frame. This object is used across
        /// the application to avoid per-frame allocations.
        /// </summary>
        private List<NRTrackablePlane> m_NewPlanes = new List<NRTrackablePlane>();

        public void Update()
        {
            NRFrame.GetTrackables<NRTrackablePlane>(m_NewPlanes, NRTrackableQueryFilter.New);
            for (int i = 0; i < m_NewPlanes.Count; i++)
            {
                // Instantiate a plane visualization prefab and set it to track the new plane. The transform is set to
                // the origin with an identity rotation since the mesh for our prefab is updated in Unity World
                // coordinates.
                NRAnchor anchor = m_NewPlanes[i].CreateAnchor();
                anchor.transform.parent = transform;
                GameObject planeObject = Instantiate(DetectedPlanePrefab, Vector3.zero, Quaternion.identity, anchor.transform);
                planeObject.GetComponent<TrackablePlaneBehaviour>().Initialize(m_NewPlanes[i]);
            }
        }
    }
}