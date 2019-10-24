namespace NRToolkit.Record
{
    public struct NativeEncodeConfig
    {
        public int Width;
        public int height;
        public int bitRate;
        public int fps;
        public int codecType;    // 0 local; 1 rtmp ; 2...
        public string outPutPath;

        public NativeEncodeConfig(int w, int h, int bitrate, int f, CodecType codectype, string path)
        {
            this.Width = w;
            this.height = h;
            this.bitRate = bitrate;
            this.fps = f;
            this.codecType = (int)codectype;
            this.outPutPath = path;
        }

        public NativeEncodeConfig(NativeEncodeConfig config)
        {
            this.Width = config.Width;
            this.height = config.height;
            this.bitRate = config.bitRate;
            this.fps = config.fps;
            this.codecType = config.codecType;
            this.outPutPath = config.outPutPath;
        }

        public override string ToString()
        {
            return LitJson.JsonMapper.ToJson(this);
        }
    }

    public enum CodecType
    {
        Local = 0,
        Rtmp = 1
    }

    public enum BlendMode
    {
        RGBOnly,
        VirtualOnly,
        Blend,
        WidescreenBlend
    }
}
