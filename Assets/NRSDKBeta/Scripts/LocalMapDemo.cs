namespace NRKernal
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    public class LocalMapDemo : MonoBehaviour
    {
        List<NRWorldAnchor> anchorList = new List<NRWorldAnchor>();
        Dictionary<UInt64, GameObject> anchorDict = new Dictionary<ulong, GameObject>();
        public GameObject AnchorPrefab;

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.O))
            {
                NRWorldMap.Instance.Create();
            }
            if (Input.GetKeyDown(KeyCode.S))
            {
                NRWorldMap.Instance.WriteWorldMap();
            }
            if (Input.GetKeyDown(KeyCode.L))
            {
                NRWorldMap.Instance.LoadWorldMap();
            }

            if (Input.GetKeyDown(KeyCode.A))
            {
                NRWorldMap.Instance.AddAnchor(new Pose(transform.position, transform.rotation));
            }

            NRWorldMap.Instance.GetAnchors(anchorList);

            UpdateAnchorData();
        }

        private void UpdateAnchorData()
        {
            foreach (var item in anchorList)
            {
                GameObject go;
                if (!anchorDict.TryGetValue(item.AnchorNativeHandle, out go))
                {
                    go = Instantiate(AnchorPrefab);
                    anchorDict.Add(item.AnchorNativeHandle, go);
                }
                var pose = item.GetCenterPose();
                go.transform.position = pose.position;
                go.transform.rotation = pose.rotation;
            }
        }
    }
}
