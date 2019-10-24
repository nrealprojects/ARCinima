using UnityEngine;
using Unity.Collections;
using System.IO;
using System.Collections.Generic;
using System;

#if UNITY_2018_2_OR_NEWER
using UnityEngine.Rendering;
#else
using UnityEngine.Experimental.Rendering;
#endif

public class AsyncCapture : MonoBehaviour
{
    Queue<AsyncGPUReadbackRequest> _requests = new Queue<AsyncGPUReadbackRequest>();

    void Start()
    {
        Camera.main.targetTexture = new RenderTexture(480, 320, 24);
    }

    void Update()
    {
        while (_requests.Count > 0)
        {
            var req = _requests.Peek();

            if (req.hasError)
            {
                Debug.Log("GPU readback error detected.");
                _requests.Dequeue();
            }
            else if (req.done)
            {
                var buffer = req.GetData<Color32>();

                var camera = GetComponent<Camera>();
                SaveBitmap(buffer, camera.pixelWidth, camera.pixelHeight);
                _requests.Dequeue();
            }
            else
            {
                break;
            }
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("begin1 :" + GetTimeStamp());
            //Camera.main.Render();
            //Debug.Log("begin2 :" + GetTimeStamp());
            AsyncGPUReadback.Request(Camera.main.targetTexture, 0, (request) =>
            {
                //if (request.done)
                //{
                //    var buffer = request.GetData<Color32>();
                //    SaveBitmap(buffer, Camera.main.targetTexture.width, Camera.main.targetTexture.height);
                //}
                //Debug.Log("call back:" + request.done);
                Debug.Log("end:" + request.done + " time:" + GetTimeStamp().ToString());
            });
        }
    }

    //void OnRenderImage(RenderTexture source, RenderTexture destination)
    //{
    //    //if (_requests.Count < 8)
    //    //    _requests.Enqueue(AsyncGPUReadback.Request(source));
    //    //else
    //    //    Debug.Log("Too many requests.");

    //    //Graphics.Blit(source, destination);

    //    AsyncGPUReadback.Request(source, 0, (request) =>
    //    {
    //        Debug.Log("call back:" + request.done + " time:" + GetTimeStamp().ToString());
    //    });

    //    Graphics.Blit(source, destination);
    //}

    void SaveBitmap(NativeArray<Color32> buffer, int width, int height)
    {
        Debug.Log("save image.");
        var tex = new Texture2D(width, height, TextureFormat.RGBA32, false);
        tex.SetPixels32(buffer.ToArray());
        tex.Apply();
        File.WriteAllBytes("test.png", ImageConversion.EncodeToPNG(tex));
        Destroy(tex);
    }

    public static long GetTimeStamp()
    {
        TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
        return Convert.ToInt64(ts.TotalMilliseconds);
    }
}