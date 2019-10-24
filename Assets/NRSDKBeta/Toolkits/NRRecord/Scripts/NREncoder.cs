namespace NRToolkit.Record
{
    using UnityEngine;
    using System;
    using System.IO;
    using NRKernal;

    public class NREncoder
    {
        public NativeEncoder NativeEncoder { get; set; }
        private bool m_IsStarted = false;

        public NativeEncodeConfig EncodeConfig;

        public NREncoder(int width, int height, int bitRate, int fps, CodecType codectype, string path)
        {
            EncodeConfig = new NativeEncodeConfig(width, height, bitRate, fps, codectype, path);
            NativeEncoder = new NativeEncoder();
            NativeEncoder.Create();
        }

        public NREncoder(NativeEncodeConfig config)
        {
            EncodeConfig = new NativeEncodeConfig(config);
            NativeEncoder = new NativeEncoder();
            NativeEncoder.Create();
        }

        public void Start()
        {
            if (m_IsStarted)
            {
                return;
            }

            if (EncodeConfig.codecType == (int)CodecType.Local)
            {
                EncodeConfig.outPutPath = Path.Combine(EncodeConfig.outPutPath, NRTools.GetTimeStamp() + ".mp4");
            }
            NativeEncoder.SetConfigration(EncodeConfig);
            NativeEncoder.Start();

            m_IsStarted = true;
        }

        public void Commit(RenderTexture rt, UInt64 timestamp)
        {
            if (!m_IsStarted)
            {
                return;
            }
            NativeEncoder.UpdateSurface(rt.GetNativeTexturePtr(), timestamp);
        }

        public void Stop()
        {
            if (!m_IsStarted)
            {
                return;
            }
            NativeEncoder.Stop();
            m_IsStarted = false;
        }
    }
}
