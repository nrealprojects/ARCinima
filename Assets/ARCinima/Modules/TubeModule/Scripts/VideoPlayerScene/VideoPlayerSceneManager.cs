using UnityEngine.Video;
using UnityEngine;
using DG.Tweening;
using System.Collections;
using NREAL.AR;

namespace NREAL.AR.VideoPlayer
{
    public class VideoPlayerSceneManager : MonoBehaviour
    {
        public string VideoName = "DemoVideo.mov";
        [SerializeField] RenderHeads.Media.AVProVideo.MediaPlayer mPlayer;
        [SerializeField] Renderer mScreen;
        [SerializeField] GameObject mPlayBtn;
        [SerializeField] Material mScreenRender;
        [SerializeField] Material mScreenDefault;

        private bool isPlaying
        {
            get
            {
                return mPlayer.Control.IsPlaying();
            }
        }

        public string URL
        {
            get
            {
                return NRealTool.GetNrealResPath() + "/" + VideoName;
            }
        }

        void Awake()
        {
            //mPlayer.m_AutoStart = false;
            //mPlayer.m_VideoLocation = MediaPlayer.FileLocation.AbsolutePathOrURL;
            //mPlayer.m_VideoPath = URL;
            //mPlayer.m_Loop = true;
            mPlayer.m_VideoPath = URL;
            mPlayer.m_Loop = true;

            this.Reset();
        }

        void Update()
        {
            if (Input.GetKeyUp(KeyCode.RightArrow))
            {
                if (!isPlaying)
                {
                    this.Play();
                }
                else
                {
                    this.Pause();
                }
            }
            else if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                var currentZ = gameObject.transform.position.z;
                gameObject.transform.DOMoveZ(currentZ + 0.5f, 0.5f);
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                var currentZ = gameObject.transform.position.z;
                gameObject.transform.DOMoveZ(currentZ - 0.5f, 0.5f);
            }
        }

        #region play controle
        public void Play()
        {
            mPlayer.Play();
            SwitchToPlayScreen();
            this.UpdateIconStatus();
        }

        public void Pause()
        {
            mPlayer.Pause();
            this.UpdateIconStatus();
        }

        public void Reset()
        {
            SwitchToDefaultScreen();
            if (_autoHideIcon != null)
            {
                StopCoroutine(_autoHideIcon);
            }
            mPlayBtn.transform.Find("play").gameObject.SetActive(false);
            mPlayBtn.transform.Find("stop").gameObject.SetActive(false);
        }

        public void SwitchToDefaultScreen()
        {
            mScreen.material = mScreenDefault; ;
        }

        public void SwitchToPlayScreen()
        {
            mScreen.material = mScreenRender;
        }
        #endregion

        #region interactive event
        private void UpdateIconStatus()
        {
            if (_autoHideIcon != null)
            {
                StopCoroutine(_autoHideIcon);
                _autoHideIcon = null;
            }
            _autoHideIcon = StartCoroutine(AutoHideIcon());
        }

        private Coroutine _autoHideIcon = null;
        private IEnumerator AutoHideIcon()
        {
            mPlayBtn.transform.Find("play").gameObject.SetActive(!isPlaying);
            mPlayBtn.transform.Find("stop").gameObject.SetActive(isPlaying);
            yield return new WaitForSeconds(2f);
            mPlayBtn.transform.Find("play").gameObject.SetActive(false);
            mPlayBtn.transform.Find("stop").gameObject.SetActive(false);
        }
        #endregion

        void OnDisable()
        {
            mPlayer.Pause();
        }
    }
}