namespace NRToolkit.Record
{
    using NRKernal;
    using System;
    using UnityEngine;

    public class CameraInput : IDisposable
    {
        public Camera TargetCamera;
        public NREncoder Encoder { get; set; }

        public int FrameCount { get; set; }
        public Material BlendMaterial { get; set; }

        public RenderTexture BlendTexture { get; set; }

        public BlendMode BlendMode { get; set; }

        public CameraInput(NREncoder encoder, Camera camera, BlendMode mode, bool islinear = true)
        {
            TargetCamera = camera;
            Encoder = encoder;

            var width = Encoder.EncodeConfig.Width;
            var height = Encoder.EncodeConfig.height;
            BlendMode = mode;

            Shader blendshader;
            if (islinear)
            {
                blendshader = Resources.Load<Shader>("Shaders/AlphaBlendLinear");
            }
            else
            {
                blendshader = Resources.Load<Shader>("Shaders/AlphaBlendGamma");
            }

            switch (BlendMode)
            {
                case BlendMode.RGBOnly:
                    BlendTexture = new RenderTexture(width, height, 24);
                    break;
                case BlendMode.VirtualOnly:
                    BlendTexture = new RenderTexture(width, height, 24);
                    break;
                case BlendMode.Blend:
                    BlendMaterial = new Material(blendshader);
                    BlendTexture = new RenderTexture(width, height, 24);
                    break;
                case BlendMode.WidescreenBlend:
                    // TODO
                    //BlendMaterial = new Material(Resources.Load<Shader>("Shaders/AlphaBlend"));
                    //BlendTexture = new RenderTexture(2 * width, height, 24);
                    break;
                default:
                    break;
            }

            TargetCamera.targetTexture = new RenderTexture(width, height, 24, RenderTextureFormat.ARGB32);
        }

        public void OnFrame(RGBTextureFrame frame)
        {
            // Render every camera
            TargetCamera.Render();

            switch (BlendMode)
            {
                case BlendMode.RGBOnly:
                    Graphics.Blit(frame.texture, BlendTexture);
                    break;
                case BlendMode.VirtualOnly:
                    Graphics.Blit(TargetCamera.targetTexture, BlendTexture);
                    break;
                case BlendMode.Blend:
                    BlendMaterial.SetTexture("_MainTex", TargetCamera.targetTexture);
                    BlendMaterial.SetTexture("_BcakGroundTex", frame.texture);
                    Graphics.Blit(TargetCamera.targetTexture, BlendTexture, BlendMaterial);
                    break;
                case BlendMode.WidescreenBlend:
                    // TODO

                    break;
                default:
                    break;
            }

            // Commit frame                
            Encoder.Commit(BlendTexture, frame.timeStamp);
        }

        public void Dispose()
        {
            BlendTexture.Release();
            BlendTexture = null;
        }
    }
}
