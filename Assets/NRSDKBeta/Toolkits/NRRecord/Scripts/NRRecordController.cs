namespace NRToolkit.Record
{
    using UnityEngine;
    using NRKernal;
    using System;
    using System.IO;

    public class NRRecordController : MonoBehaviour
    {
        public NRRecordConfig RecordConfig;
        public Transform RGBCameraRig;
        public Camera CaptureCamera;
        public NRRecordPreviewer PreviewScreen;
        public NRRGBCamTexture RGBTexture { get; set; }
        public CameraInput CameraInput { get; set; }
        public NREncoder Encoder { get; set; }
        public Texture PreviewTexture
        {
            get
            {
                return CameraInput.BlendTexture;
            }
        }
        private BlendMode BlendMode { get; set; }
        private bool m_IsStarted = false;
        private bool m_IsInit = false;

        public static NRRecordController Create()
        {
            NRRecordController recordcontroller = GameObject.FindObjectOfType<NRRecordController>();
            if (recordcontroller == null)
            {
                recordcontroller = Instantiate(Resources.Load<NRRecordController>("Prefabs/NRRecordController"));
            }
            return recordcontroller;
        }

        private void Start()
        {
            Invoke("Init", 2f);
        }

        private void Init()
        {
            if (m_IsInit)
            {
                return;
            }
            if (RecordConfig == null)
            {
                NRDebugger.LogError("RecordConfig is null!!!!!");
            }
            BlendMode = RecordConfig.BlendMode;
            var config = RecordConfig.ToNativeConfig();
            if (!Directory.Exists(config.outPutPath))
            {
                Directory.CreateDirectory(config.outPutPath);
            }

            NRDebugger.LogError(config.ToString());
            Encoder = new NREncoder(config);
            CameraInput = new CameraInput(Encoder, CaptureCamera, BlendMode, false);

            RGBTexture = new NRRGBCamTexture();
            PreviewScreen.SetData(RGBTexture.GetTexture(), false);
            PreviewScreen.gameObject.SetActive(false);
            RGBTexture.OnUpdate += OnFrame;
            m_IsInit = true;
        }

        private void OnFrame(RGBTextureFrame frame)
        {
            if (!m_IsStarted)
            {
                return;
            }
            // update camera pose
            UpdateHeadPoseByTimestamp(frame.timeStamp);

            CameraInput.OnFrame(frame);

            PreviewScreen.SetData(CameraInput.BlendTexture, true);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space) || NRInput.GetButtonDown(ControllerButton.APP))
            {
                PreviewScreen.gameObject.SetActive(!PreviewScreen.gameObject.activeInHierarchy);

                NRInput.LaserVisualActive = !PreviewScreen.gameObject.activeInHierarchy;
                NRInput.ReticleVisualActive = !PreviewScreen.gameObject.activeInHierarchy;
            }
            this.BindPreviewTOController();
        }

        public void StartRecord()
        {
            if (m_IsStarted || !m_IsInit)
            {
                return;
            }
            RGBTexture.Play();
            Encoder.Start();
            m_IsStarted = true;
        }

        public void StopRecord()
        {
            if (!m_IsStarted)
            {
                return;
            }
            RGBTexture.Stop();
            Encoder.Stop();
            PreviewScreen.SetData(CameraInput.BlendTexture, false);
            m_IsStarted = false;
        }

        public void SwitchMode(BlendMode mode)
        {
            if (m_IsStarted)
            {
                return;
            }

            BlendMode = mode;
            if (CameraInput != null)
            {
                CameraInput.Dispose();
            }
            CameraInput = new CameraInput(Encoder, CaptureCamera, BlendMode);
        }

        private void BindPreviewTOController()
        {
            var inputAnchor = NRInput.AnchorsHelper.GetAnchor(ControllerAnchorEnum.RightModelAnchor);
            PreviewScreen.transform.position = inputAnchor.TransformPoint(Vector3.forward * 0.2f);
            PreviewScreen.transform.up = inputAnchor.forward;
        }

        private void UpdateHeadPoseByTimestamp(UInt64 timestamp, UInt64 predict = 0)
        {
            Pose head_pose = Pose.identity;
            var result = NRFrame.GetHeadPoseByTime(ref head_pose, timestamp, predict);
            if (result)
            {
                RGBCameraRig.transform.position = head_pose.position;
                RGBCameraRig.transform.rotation = head_pose.rotation;
            }
        }
    }
}
