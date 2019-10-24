namespace NRToolkit.Record
{
    using UnityEngine;
    using UnityEngine.Serialization;

    [CreateAssetMenu(fileName = "NRRecordConfig", menuName = "NRInternal/RecordConfig", order = 1)]
    public class NRRecordConfig : ScriptableObject
    {
        [FormerlySerializedAs("Record Width")]
        public int Width;
        [FormerlySerializedAs("Record Height")]
        public int height;
        [FormerlySerializedAs("Record BitRate")]
        public int bitRate;
        [FormerlySerializedAs("Record FPS")]
        public int fps;
        // 0 local; 1 rtmp ; 2...
        [FormerlySerializedAs("Record CodecType")]
        public CodecType codecType;
        [FormerlySerializedAs("Record OutPutPath")]
        public string outPutPath;
        [FormerlySerializedAs("Record BlendMode")]
        public BlendMode BlendMode = BlendMode.Blend;


        public NativeEncodeConfig ToNativeConfig()
        {
            return new NativeEncodeConfig(Width, height, bitRate, fps, codecType, outPutPath);
        }
    }
}
