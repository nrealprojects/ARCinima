using UnityEngine;
using UnityEngine.UI;

namespace NRKernal.NRExamples
{
    public class CameraCaptureController : MonoBehaviour
    {
        public RawImage CaptureImage;
        private NRRGBCamTexture RGBCamTexture { get; set; }

        public Text FrameCount;

        void Start()
        {
            RGBCamTexture = new NRRGBCamTexture();
            CaptureImage.texture = RGBCamTexture.GetTexture();
            RGBCamTexture.Play();
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.S))
            {
                RGBCamTexture.Play();
            }
            if (Input.GetKeyDown(KeyCode.T))
            {
                RGBCamTexture.Stop();
            }

            FrameCount.text = RGBCamTexture.FrameCount.ToString();
        }

        void OnDestroy()
        {
            RGBCamTexture.Stop();
        }
    }
}
