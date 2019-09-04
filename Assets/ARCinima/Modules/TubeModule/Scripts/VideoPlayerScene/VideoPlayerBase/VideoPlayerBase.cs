using System;
using UnityEngine;

namespace NREAL.AR.VideoPlayer
{
    public class VideoPlayerBase : MonoBehaviour
    {
        public bool isInitialized { get; protected set; }

        //对各类事件的处理
        public delegate void OnInitializeFinishHandler();
        public delegate void OnStateChangedHandler(string state);
        public delegate void OnPlayCompletedHandler();
        public delegate void OnSetSourceHandler();
        public delegate void OnPrepareFinishHandler();
        public delegate void OnVideoFirstFrameReadyHandler();
        public delegate void OnSeekCompleteHandler();
        public delegate void OnBufferStateChangedHandler(string isbuffer);
        public delegate void OnErrorHandler(string errorCode);


        /// <summary>
        /// 当播放器初始化完成时触发
        /// </summary>
        public OnInitializeFinishHandler OnInitializeFinish;

        /// <summary>
        /// 当播放器状态改变时触发
        /// </summary>
        public OnStateChangedHandler OnStateChanged;

        /// <summary>
        /// 当播放完成时触发
        /// </summary>
        public OnPlayCompletedHandler OnPlayCompleted;

        public OnSetSourceHandler OnSetSource;

        /// <summary>
        /// 当视频准备完成时触发
        /// </summary>
        public OnPrepareFinishHandler OnPrepareFinish;

        /// <summary>
        /// 当视频第一帧画面准备好时触发
        /// </summary>
        public OnVideoFirstFrameReadyHandler OnFirstFrameReady;

        /// <summary>
        /// 当拖动进度条时触发
        /// </summary>
        public OnSeekCompleteHandler OnSeekComplete;

        /// <summary>
        /// 当缓冲状态改变时候触发
        /// </summary>
        public OnBufferStateChangedHandler OnBufferStateChanged;

        /// <summary>
        /// 当播放器错误时候触发
        /// </summary>
        public OnErrorHandler OnError;


        #region interface
        public virtual void Init()
        {

        }
        public virtual void Load(string uri)
        {

        }

        /// <summary>
        /// Set the player to play.
        /// </summary>
        public virtual void Play()
        {

        }

        /// <summary>
        /// Set the player to pause.
        /// </summary>
        public virtual void Pause()
        {

        }

        /// <summary>
        /// 停止播放
        /// </summary>
        public virtual void Stop() { }

        /// <summary>
        /// 获取播放器状态
        /// Born, Initialized, Preparing, Prepared, Started, Paused, Stopped, Completed, Dead
        /// </summary>
        /// <returns>播放器状态</returns>
        public virtual PlayerState GetState()
        {
            return PlayerState.Started;
        }

        /// <summary>
        /// 按照给定的毫秒来定位视频
        /// </summary>
        /// <param name="positionMs">毫秒为单位的视频位置</param>
        public virtual void SeekTo(long positionMs)
        {

        }

        /// <summary>
        /// 按照百分比来定位视频
        /// </summary>
        /// <param name="positionPercentage">要定位到位置的百分比</param>
        public virtual void SeekTo(float positionPercentage)
        {

        }

        /// <summary>
        /// 设置播放器的音量
        /// </summary>
        /// <param name="volume">要设置的音量大小</param>
        public virtual void SetVolume(float volume)
        {

        }

        /// <summary>
        /// 获取当前播放视频的长度，结果单位为毫秒
        /// </summary>
        /// <returns>视频长度，单位为毫秒</returns>
        public virtual long GetDuration()
        {
            return -1;
        }

        /// <summary>
        /// 获取当前视频播放的位置，结果单位为毫秒
        /// </summary>
        /// <returns>视频当前播放到的位置，单位为毫秒</returns>
        public virtual long GetCurrentPosition()
        {
            return -1;
        }

        /// <summary>
        /// 获取当前视频播放进度的百分比
        /// </summary>
        /// <returns>视频播放进度的百分比</returns>
        public virtual float GetCurrentPercentage()
        {
            return -1;
        }

        /// <summary>
        /// 获取视频缓冲的进度，结果单位为毫秒
        /// </summary>
        /// <returns>视频缓冲的进度，单位为毫秒</returns>
        public virtual long GetBufferedPosition()
        {
            return -1;
        }

        /// <summary>
        /// 获取视频缓冲进度的百分比
        /// </summary>
        /// <returns>视频缓冲进度的百分比</returns>
        public virtual float GetBufferedPercentage()
        {
            return -1;
        }

        /// <summary>
        /// 获取视频源的宽度
        /// </summary>
        /// <returns>视频源的宽度</returns>
        public virtual int GetVideoWidth()
        {
            return -1;
        }

        /// <summary>
        /// 获取视频源的高度
        /// </summary>
        /// <returns>视频源的高度</returns>
        public virtual int GetVideoHeight()
        {
            return -1;
        }

        public virtual bool IsPlaying()
        {
            return false;
        }

        public virtual void Release()
        {

        }
        #endregion

        void OnDestroy()
        {
            Release();
        }
    }

    public enum PlayerState
    {
        Initialized, Preparing, Prepared, Started, Paused, Stopped, Completed, Dead,
    }

}