namespace NREAL.AR.VideoPlayer
{
    public class MidiaPlayer : VideoPlayerBase
    {
        #region interface
        public override void Init()
        {

        }

        public override void Load(string uri)
        {

        }

        /// <summary>
        /// Set the player to play.
        /// </summary>
        public override void Play()
        {

        }

        /// <summary>
        /// Set the player to pause.
        /// </summary>
        public override void Pause()
        {

        }

        /// <summary>
        /// 停止播放
        /// </summary>
        public override void Stop() { }

        /// <summary>
        /// 获取播放器状态
        /// Born, Initialized, Preparing, Prepared, Started, Paused, Stopped, Completed, Dead
        /// </summary>
        /// <returns>播放器状态</returns>
        public override PlayerState GetState()
        {
            return PlayerState.Started;
        }

        /// <summary>
        /// 按照给定的毫秒来定位视频
        /// </summary>
        /// <param name="positionMs">毫秒为单位的视频位置</param>
        public override void SeekTo(long positionMs)
        {

        }

        /// <summary>
        /// 按照百分比来定位视频
        /// </summary>
        /// <param name="positionPercentage">要定位到位置的百分比</param>
        public override void SeekTo(float positionPercentage)
        {

        }

        /// <summary>
        /// 设置播放器的音量
        /// </summary>
        /// <param name="volume">要设置的音量大小</param>
        public override void SetVolume(float volume)
        {

        }

        /// <summary>
        /// 获取当前播放视频的长度，结果单位为毫秒
        /// </summary>
        /// <returns>视频长度，单位为毫秒</returns>
        public override long GetDuration()
        {
            return -1;
        }

        /// <summary>
        /// 获取当前视频播放的位置，结果单位为毫秒
        /// </summary>
        /// <returns>视频当前播放到的位置，单位为毫秒</returns>
        public override long GetCurrentPosition()
        {
            return -1;
        }

        /// <summary>
        /// 获取当前视频播放进度的百分比
        /// </summary>
        /// <returns>视频播放进度的百分比</returns>
        public override float GetCurrentPercentage()
        {
            return -1;
        }

        /// <summary>
        /// 获取视频缓冲的进度，结果单位为毫秒
        /// </summary>
        /// <returns>视频缓冲的进度，单位为毫秒</returns>
        public override long GetBufferedPosition()
        {
            return -1;
        }

        /// <summary>
        /// 获取视频缓冲进度的百分比
        /// </summary>
        /// <returns>视频缓冲进度的百分比</returns>
        public override float GetBufferedPercentage()
        {
            return -1;
        }

        /// <summary>
        /// 获取视频源的宽度
        /// </summary>
        /// <returns>视频源的宽度</returns>
        public override int GetVideoWidth()
        {
            return -1;
        }

        /// <summary>
        /// 获取视频源的高度
        /// </summary>
        /// <returns>视频源的高度</returns>
        public override int GetVideoHeight()
        {
            return -1;
        }

        public override bool IsPlaying()
        {
            return false;
        }

        public override void Release()
        {

        }
        #endregion
    }
}
