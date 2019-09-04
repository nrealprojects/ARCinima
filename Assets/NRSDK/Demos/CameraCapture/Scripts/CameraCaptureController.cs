using UnityEngine;
using UnityEngine.UI;

namespace NRKernal.NRExamples
{
    public class CameraCaptureController : MonoBehaviour
    {
        public RawImage CaptureImage;
        private NRCameraCapture CameraCapture { get; set; }

        public Text FrameCount;

        [Header("RGB Camera capture frame rate")]
        public int FrameRate;

        void Start()
        {
            CameraCapture = gameObject.AddComponent<NRCameraCapture>();
            CameraCapture.ImageFormat = CameraImageFormat.RGB_888;
            CameraCapture.UpdateFrameRate = FrameRate;
            CameraCapture.OnFirstFrameReady += OnFirstFrameReady;
            CameraCapture.OnError += OnError;

            CameraCapture.Play();
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.S))
            {
                CameraCapture.Play();
            }
            if (Input.GetKeyDown(KeyCode.T))
            {
                CameraCapture.Stop();
            }

            FrameCount.text = CameraCapture.FrameCount.ToString();
        }

        private void OnFirstFrameReady()
        {
            CaptureImage.texture = CameraCapture.Texture;
        }

        private void OnError(string msg)
        {
            Debug.Log(msg);
        }

        void OnDestroy()
        {
            CameraCapture.Release();
        }
    }
}
