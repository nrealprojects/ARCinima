using UnityEngine;
using System.Text;
using NRKernal;
using System.IO;
using System.Collections;

public class PoseWriter : MonoBehaviour
{
    StringBuilder st = new StringBuilder();


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            StartCoroutine("StartReadPose");
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            StopCoroutine("StartReadPose");
#if UNITY_EDITOR
            string path = Path.Combine(Application.persistentDataPath, "pose.txt");
#else
            string path = "sdcard/pose.txt";
#endif
            Debug.Log(path);
            File.WriteAllText(path, st.ToString());
        }
    }

    private IEnumerator StartReadPose()
    {
        while (true)
        {
            yield return new WaitForEndOfFrame();
            if (NRSessionManager.Instance != null && NRSessionManager.Instance.NRHMDPoseTracker != null)
            {
                var leftcamera = NRSessionManager.Instance.NRHMDPoseTracker.leftCamera.transform;
                st.AppendLine(string.Format("{0} {1}", leftcamera.position.ToString(), leftcamera.rotation.ToString()));
            }
        }
    }
}
