using NRKernal;
using System.Collections.Generic;
using UnityEngine;

public class MarkerTester : MonoBehaviour
{
    /// <summary>
    /// A prefab for tracking and visualizing detected planes.
    /// </summary>
    public GameObject DetectedMarkerPrefab;

    public Vector3 OffSet;

    /// <summary>
    /// A list to hold new planes NRSDK began tracking in the current frame. This object is used across
    /// the application to avoid per-frame allocations.
    /// </summary>
    private List<NRTrackableImage> m_NewMarkers = new List<NRTrackableImage>();

    public void Update()
    {
        NRFrame.GetTrackables<NRTrackableImage>(m_NewMarkers, NRTrackableQueryFilter.All);
        for (int i = 0; i < m_NewMarkers.Count; i++)
        {
            var detectedMarker = m_NewMarkers[i];
            if (detectedMarker != null && detectedMarker.GetTrackingState() == TrackingState.Tracking)
            {
                Vector2 size = detectedMarker.Size;
                DetectedMarkerPrefab.transform.localScale = new Vector3(size.x, size.y, size.x);
                var pose = detectedMarker.GetCenterPose();
                var up_offset = pose.up * size.x * 0.5f;
                DetectedMarkerPrefab.transform.position = pose.position + up_offset + OffSet;
                DetectedMarkerPrefab.transform.rotation = pose.rotation;
            }
        }
    }
}
