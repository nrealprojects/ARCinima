using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System;
using DG.Tweening;
using RenderHeads.Media.AVProVideo;
using NRKernal;

namespace NREAL.AR.VideoPlayer
{
    public class VideoScreenPage : MonoBehaviour
    {
        public static string VideoPlayEvent = "VideoPlayEvent";

        public enum VideoPageState
        {
            Normal,
            MiniSize
        }

        public Action onReturnList = null;
        [SerializeField]
        MediaPlayer m_Player;
        [SerializeField]
        ScreenController m_ScreenController;
        [SerializeField]
        Slider m_Slider;
        [SerializeField]
        ButtonBase m_CloseBtn;
        [SerializeField]
        ButtonBase m_ReturnListBtn;
        [SerializeField]
        Text m_CurrentTime;
        [SerializeField]
        ARInteractiveItem m_ScreenRegion;
        [SerializeField]
        ARInteractiveItem m_UIControllerRegion;
        [SerializeField]
        ButtonBase m_PlayBtn;
        [SerializeField]
        ButtonBase m_PlayCtlBtn;
        [SerializeField]
        Text videoNameTxt;
        [SerializeField]
        GameObject m_QuitIcon;
        [SerializeField]
        GameObject m_SelectEffectUI;
        [SerializeField]
        ButtonBase m_StateChangeBtn;
        [SerializeField]
        ScreenDelayFollow m_ScreenFollow;
        [SerializeField]
        float m_ChangeStateMoveTime = 1.2f;


        private Vector3 originpos;

        private Transform miniSizeTarget;

        private VideoPageState curVideoPageState = VideoPageState.Normal;

        private Vector3 normalVideoPos;

        private Quaternion normalVideoRot;

        private Vector3 normalVideoScale;

        private bool screenLock = false;

        private bool isPlaneScreen = true;

        private bool isPlaying
        {
            get
            {
                return m_Player.Control.IsPlaying();
            }
        }

        void Awake()
        {
            m_Player.m_Loop = false;
        }

        void Start()
        {
            originpos = transform.position;
            m_Player.Events.AddListener(OnMediaPlayerEvent);
            if (m_ScreenFollow)
            {
                m_ScreenFollow.Init();
                m_ScreenFollow.enabled = false;
            }
            ARInteractiveItem item = GetComponent<ARInteractiveItem>();
            item.OnSelected += OnSelect;
            item.OnDeselected += OnDeselect;
            if (m_SelectEffectUI)
                m_SelectEffectUI.SetActive(false);
        }

        private void OnSelect()
        {
            if (m_SelectEffectUI)
                m_SelectEffectUI.SetActive(true);
        }

        private void OnDeselect()
        {
            if (m_SelectEffectUI)
                m_SelectEffectUI.SetActive(false);
        }

        public void OnMediaPlayerEvent(MediaPlayer mp, MediaPlayerEvent.EventType et, ErrorCode errorCode)
        {
            switch (et)
            {
                case MediaPlayerEvent.EventType.ReadyToPlay:
                    Debug.Log("*************OnReady!*********************");
                    m_Player.Play();
                    break;
                case MediaPlayerEvent.EventType.Started:
                    break;
                case MediaPlayerEvent.EventType.FirstFrameReady:
                    Debug.Log("*************OnVideoFirstFrameReady!*************");
                    m_ScreenController.SetTexture(m_Player.TextureProducer.GetTexture());
                    currentTime = 0;
                    break;
                case MediaPlayerEvent.EventType.MetaDataReady:
                    break;
                case MediaPlayerEvent.EventType.FinishedPlaying:
                    Debug.Log("*************OnVideoFinished!*************");
                    PlayerEventListener();
                    break;
            }
        }

        private float currentTime = 0;
        private void Update()
        {
            if (isPlaying)
            {
                currentTime += Time.deltaTime;
                m_Slider.value = currentTime * 1000 / (long)m_Player.Info.GetDurationMs();
                m_CurrentTime.text = NRealTool.GetTimeByMSeconds((long)(currentTime * 1000)) + " / " + NRealTool.GetTimeByMSeconds((long)m_Player.Info.GetDurationMs());
            }

            if (Input.GetKeyUp(KeyCode.Space))
            {
                OnPlayBtnClick(null);
            }

            if (NRInput.GetButtonDown(ControllerButton.HOME))
            {
                CancleLock();
            }
        }

        private void OnEnable()
        {
            m_CurrentTime.transform.parent.gameObject.SetActive(false);
            m_PlayBtn.gameObject.SetActive(false);
            m_QuitIcon.SetActive(false);

            m_ScreenRegion.OnHover += OnScreenRegionEnter;
            m_ScreenRegion.OnOut += OnScreenRegionExit;
            m_UIControllerRegion.OnHover += OnUIControllerRegionEnter;
            m_UIControllerRegion.OnOut += OnUIControllerRegionOut;
            m_PlayBtn.onClick += OnPlayBtnClick;
            m_PlayCtlBtn.onClick += OnPlayBtnClick;
            m_ReturnListBtn.onClick += OnReturnClick;
            if (m_CloseBtn)
                m_CloseBtn.onClick += OnCloseButtonClick;
            m_StateChangeBtn.onClick += OnStateChangeButtonClick;
        }

        private void OnDisable()
        {
            Time.timeScale = 1;
            m_ReturnListBtn.onClick -= OnReturnClick;
            if (m_CloseBtn)
                m_CloseBtn.onClick -= OnCloseButtonClick;
            m_ScreenRegion.OnHover -= OnScreenRegionEnter;
            m_ScreenRegion.OnOut -= OnScreenRegionExit;
            m_PlayBtn.onClick -= OnPlayBtnClick;
            m_PlayCtlBtn.onClick -= OnPlayBtnClick;
            m_StateChangeBtn.onClick -= OnStateChangeButtonClick;
        }

        private void PlayerEventListener()
        {
            m_Player.Stop();
            if (onReturnList != null)
            {
                onReturnList();
            }
        }

        private void OnLockButtonClick(ButtonBase button)
        {
            ToggleScreenLock();
        }

        public void ToggleScreenLock()
        {
            if (m_ScreenFollow)
            {
                m_ScreenFollow.enabled = (!m_ScreenFollow.enabled);
            }
        }

        public void DisableFollow()
        {
            if (m_ScreenFollow)
                m_ScreenFollow.enabled = false;
        }

        public void PlayNormalVideo(VideoInfo video)
        {
            m_ScreenController.SetScreenType(m_ScreenController.GetScreenType(video.videoType));
            m_Player.OpenVideoFromFile(MediaPlayer.FileLocation.AbsolutePathOrURL, NRealTool.GetNrealResPath() + "/" + video.videoPath);
            videoNameTxt.text = video.videoName;
            //m_ScreenChangeBtn.gameObject.SetActive(false);
            Messenger.Broadcast(VideoPlayEvent);
        }

        public void RefreshPlayBtn(bool isplaying)
        {
            if (!isplaying)
            {
                m_PlayBtn.gameObject.SetActive(true);
                m_PlayCtlBtn.SwitchImage(true);
            }
            else
            {
                m_PlayBtn.gameObject.SetActive(false);
                m_PlayCtlBtn.SwitchImage(false);
            }
        }

        private void CancleLock()
        {
            if (screenLock)
            {
                LockScreen(false);
            }
        }

        private void LockScreen(bool islock)
        {
            //m_ScreenRegion.transform.parent.GetComponent<MoveWithCamera>().enabled = islock;
            screenLock = islock;
            if (islock)
            {
                ShowUI(false);
                StartCoroutine(ShowQuitIcon());
            }
        }

        private IEnumerator ShowQuitIcon()
        {
            yield return new WaitForSeconds(36f);
            if (m_QuitIcon != null)
            {
                m_QuitIcon.SetActive(true);
            }
        }

        private void ShowUI(bool show)
        {
            m_CurrentTime.transform.parent.gameObject.SetActive(show);
            if (!show)
            {
                m_PlayBtn.gameObject.SetActive(false);
            }
        }

        private void PlayScreenAnimation()
        {
            Vector3 target = transform.position + transform.forward * 3;
            transform.DOMove(target, 0.3f).OnComplete(() =>
            {
                transform.DOMove(originpos, 0.2f);
            });
        }

        #region Button Event
        private void OnPlayBtnClick(ButtonBase button)
        {
            if (isPlaying)
            {
                m_Player.Pause();
                Time.timeScale = 0;

                RefreshPlayBtn(false);
            }
            else
            {
                m_Player.Play();
                m_CurrentTime.transform.parent.gameObject.SetActive(false);
                Time.timeScale = 1;

                RefreshPlayBtn(true);
            }
        }

        private void OnScreenRegionEnter()
        {
            //NRInput.ReticleVisualActive = false;
            if (isPlaying)
            {
                m_CurrentTime.transform.parent.gameObject.SetActive(false);
            }
        }

        private void OnScreenRegionExit()
        {
            //NRInput.ReticleVisualActive = true;
            // m_CurrentTime.transform.parent.gameObject.SetActive(true);
        }

        private void OnUIControllerRegionEnter()
        {
            m_CurrentTime.transform.parent.gameObject.SetActive(true);
        }

        private void OnUIControllerRegionOut()
        {
            //if ((ARControllerManager.Instance.GetCurrentInteractible())
            //    && (ARControllerManager.Instance.GetCurrentInteractible().gameObject.tag.Equals("VideoController")) == false)
            //{
            //    m_CurrentTime.transform.parent.gameObject.SetActive(false);
            //}
        }

        private void OnReturnClick(ButtonBase button)
        {
            if (MultiScreenPlayerManager.Instance)
            {
                MultiScreenPlayerManager.Instance.OnCancel();
            }
            else
            {
                Stop();
                m_Player.Stop();
                if (onReturnList != null)
                {
                    onReturnList();
                }
            }
        }

        public void Stop()
        {
            m_Player.Stop();
        }

        #endregion

        public void PlayVideo(VideoInfo video)
        {
            m_ScreenController.SetScreenType(m_ScreenController.GetScreenType(video.videoType));
            //string path = System.IO.Path.Combine(Application.streamingAssetsPath, video.videoPath);
            //Debug.LogError(path);
            Debug.Log("start load video:" + video.videoPath);
            m_Player.m_VideoPath = video.videoPath;
            m_Player.OpenVideoFromFile(MediaPlayer.FileLocation.AbsolutePathOrURL, NRealTool.GetNrealResPath() + "/" + video.videoPath, true);
            Messenger.Broadcast(VideoPlayEvent);

            //特殊逻辑，FOV这个视频需要锁屏
            if (video.videoPath.Contains("FOV"))
            {
                LockScreen(true);
                //GameObject.Find("ARCameraManager").GetComponent<StereoCameraParam>().SwitchCameraParam(true);
            }
            else
            {
                LockScreen(false);
            }
        }

        private void OnCloseButtonClick(ButtonBase button)
        {
            if (MultiScreenPlayerManager.Instance)
                MultiScreenPlayerManager.Instance.Close();
        }

        private void OnCloseButtonEnter(ButtonBase button)
        {
            m_CloseBtn.transform.DOScale(0.08f, 0.6f);
            //m_ReturnListBtn.transform.DOLocalMoveZ(-0.02f, 0.6f);
        }

        private void OnCloseButtonExit(ButtonBase button)
        {
            m_CloseBtn.transform.DOScale(0.06f, 0.6f);
            //m_ReturnListBtn.transform.DOLocalMoveZ(-0.01f, 0.6f);
        }

        private void OnStateChangeButtonClick(ButtonBase button)
        {
            VideoPageState newState = curVideoPageState == VideoPageState.Normal ? VideoPageState.MiniSize : VideoPageState.Normal;
            ChangeVideoPageState(newState);
        }

        public void SetMiniSizeTarget(Transform target)
        {
            miniSizeTarget = target;
        }

        public void ChangeVideoPageState(VideoPageState newState)
        {
            if (curVideoPageState == newState || miniSizeTarget == null)
                return;
            switch (newState)
            {
                case VideoPageState.Normal:
                    OnVideoNormalState();
                    break;
                case VideoPageState.MiniSize:
                    OnVideoMiniSizeState();
                    break;
                default:
                    break;
            }
            curVideoPageState = newState;
        }

        private void OnVideoNormalState()
        {
            m_StateChangeBtn.SwitchImage(false);
            SetMoveable(false);
            transform.DOKill();
            transform.DOKill();
            transform.DOScale(normalVideoScale, m_ChangeStateMoveTime);
            transform.DORotate(normalVideoRot.eulerAngles, m_ChangeStateMoveTime);
            transform.DOMove(normalVideoPos, m_ChangeStateMoveTime).OnComplete(delegate
            {
                SetMoveable(true);
            });
        }

        private void OnVideoMiniSizeState()
        {
            m_StateChangeBtn.SwitchImage(true);
            RecordNormalTransformInfo();
            SetMoveable(false);
            transform.DOKill();
            transform.DOScale(miniSizeTarget.localScale, m_ChangeStateMoveTime);
            transform.DORotate(miniSizeTarget.eulerAngles, m_ChangeStateMoveTime);
            transform.DOMove(miniSizeTarget.position, m_ChangeStateMoveTime).OnComplete(delegate
            {
                SetMoveable(true);
            });
        }

        public void ResetState()
        {
            curVideoPageState = VideoPageState.Normal;
            m_StateChangeBtn.SwitchImage(false);
            transform.DOKill();
            SetMoveable(true);
        }

        private void SetMoveable(bool isEnabled)
        {
            if (GetComponent<MoveablePhysicsObject>())
                GetComponent<MoveablePhysicsObject>().enabled = isEnabled;
        }

        private void RecordNormalTransformInfo()
        {
            normalVideoPos = transform.position;
            normalVideoRot = transform.rotation;
            normalVideoScale = transform.localScale;
        }
    }
}