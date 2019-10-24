namespace NRKernal
{
    using System;
    using UnityEngine;

    /**
    * @brief Create a rgb camera texture.
    */
    public class NRRGBCamTexture
    {
        public Action<RGBTextureFrame> OnUpdate;

        public int Height
        {
            get
            {
                return NRRgbCamera.Resolution.width;
            }
        }

        public int Width
        {
            get
            {
                return NRRgbCamera.Resolution.height;
            }
        }

        private bool m_IsPlaying = false;
        public bool IsPlaying
        {
            get
            {
                return m_IsPlaying;
            }
        }

        public bool DidUpdateThisFrame
        {
            get
            {
                return NRRgbCamera.HasFrame();
            }
        }

        public int FrameCount = 0;

        private Texture2D m_texture;

        public RGBTextureFrame CurrentFrame;

        public void Play()
        {
            if (m_IsPlaying)
            {
                return;
            }
            NRRgbCamera.OnImageUpdate += OnFrameUpdated;
            InternalUpdater.Instance.OnUpdate += UpdateTexture;
            NRRgbCamera.Play();
            m_IsPlaying = true;
        }

        public void Pause()
        {
            Stop();
        }

        public Texture2D GetTexture()
        {
            if (m_texture == null)
            {
                m_texture = new Texture2D(NRRgbCamera.Resolution.width, NRRgbCamera.Resolution.height, TextureFormat.RGB24, false);
            }
            return m_texture;
        }

        private void OnFrameUpdated()
        {
            FrameCount++;
        }

        private void UpdateTexture()
        {
            if (!NRRgbCamera.HasFrame())
            {
                return;
            }
            RGBRawDataFrame rgbRawDataFrame = NRRgbCamera.GetRGBFrame();

            if (m_texture == null || m_texture.width != NRRgbCamera.Resolution.width ||
                 m_texture.height != NRRgbCamera.Resolution.height)
            {
                m_texture = new Texture2D(NRRgbCamera.Resolution.width, NRRgbCamera.Resolution.height, TextureFormat.RGB24, false);
            }
            m_texture.LoadRawTextureData(rgbRawDataFrame.data);
            m_texture.Apply();

            CurrentFrame.timeStamp = rgbRawDataFrame.timeStamp;
            CurrentFrame.texture = m_texture;

            if (OnUpdate != null)
            {
                OnUpdate(CurrentFrame);
            }
        }

        public void Stop()
        {
            if (!m_IsPlaying)
            {
                return;
            }
            NRRgbCamera.OnImageUpdate -= OnFrameUpdated;
            InternalUpdater.Instance.OnUpdate -= UpdateTexture;
            NRRgbCamera.Stop();
            m_IsPlaying = false;
        }
    }

    public struct RGBTextureFrame
    {
        public UInt64 timeStamp;
        public Texture2D texture;
    }

    public class InternalUpdater : MonoBehaviour
    {
        private static InternalUpdater m_Instance;

        public static InternalUpdater Instance
        {
            get
            {
                if (m_Instance == null)
                {
                    GameObject updateObj = new GameObject("updateObj");
                    GameObject.DontDestroyOnLoad(updateObj);
                    m_Instance = updateObj.AddComponent<InternalUpdater>();
                }
                return m_Instance;
            }
        }

        public Action OnUpdate;

        private void Update()
        {
            if (OnUpdate != null)
            {
                OnUpdate();
            }
        }
    }
}
