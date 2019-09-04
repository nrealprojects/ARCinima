using System.Collections.Generic;
using System.Xml.Serialization;

namespace NREAL.AR.VideoPlayer
{
    [XmlRoot("VideoList")]
    public class VideoConfig
    {
        [XmlElement("VideoInfo")]
        public List<VideoInfo> videos { get; set; }
    }

    [XmlType("VideoInfo")]
    public class VideoInfo
    {
        public int videoID { get; set; }
        [XmlAttribute("videoName")]
        public string videoName { get; set; }
        [XmlAttribute("videoType")]
        public string videoType { get; set; }
        [XmlAttribute("videoPath")]
        public string videoPath { get; set; }
        [XmlAttribute("thumbnail")]
        public string thumbnail { get; set; }
        [XmlAttribute("describe")]
        public string describe { get; set; }
        [XmlAttribute("extraInfo")]
        public string extraInfo { get; set; }

        public VideoInfo Copy()
        {
            VideoInfo info = new VideoInfo();
            info.videoID = this.videoID;
            info.videoName = this.videoName;
            info.videoType = this.videoType;
            info.videoPath = this.videoPath;
            info.thumbnail = this.thumbnail;
            info.describe = this.describe;
            info.extraInfo = this.extraInfo;
            return info;
        }
    }
}
