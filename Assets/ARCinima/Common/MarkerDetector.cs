using System.Collections.Generic;
using UnityEngine;
using NRKernal;

public class MarkerDetector : MonoBehaviour
{
    public delegate void OnARMarkerUpdate(Vector3 position, Quaternion rotation);
    public static OnARMarkerUpdate onARMarkerUpdate;

    private List<NRTrackableImage> m_TrackableImages = new List<NRTrackableImage>();

    private int framCount = 0;

//    void Update()
//    {
//#if UNITY_EDITOR
//        if (Input.GetKeyDown(KeyCode.T))
//        {
//            onARMarkerUpdate(new Vector3(0,-1.5f,2),Quaternion.identity);
//        }
//#endif
//        if (framCount > 3)
//        {
//            return;
//        }
//        NRSession.StartGainMarkerData();
//        NRSession.GetTrackables<NRTrackableImage>(m_TrackableImages, TrackableQueryFilter.All);

//        for (int i = 0; i < m_TrackableImages.Count; i++)
//        {
//            if (onARMarkerUpdate != null)
//            {
//                framCount++;
//                var center = m_TrackableImages[i].GetCenterPose();
//                onARMarkerUpdate(center.position, center.rotation);
//            }
//        }
//        if (framCount == 4)
//            NRSession.StopGainMarkerData();
//    }
}
