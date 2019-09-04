using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using NREAL.AR;
using DG.Tweening;

namespace NREAL.AR.VideoPlayer
{
    public class VideoItem : MonoBehaviour
    {
        public delegate void VideoItemClick(VideoInfo video);
        public VideoItemClick itemClick = null;
        [SerializeField]
        RawImage m_Thumnail;
        [SerializeField]
        Text m_VideoName;
        [SerializeField]
        Text m_Descripe;
        [SerializeField]
        Text m_Extrainfo;
        [SerializeField]
        ButtonBase m_PlayBtn;
        private ARInteractiveItem hoverRegion;

        private VideoInfo _videoInfo;

        public void SetData(VideoInfo info)
        {
            this._videoInfo = info;
            m_VideoName.text = info.videoName;
            m_Descripe.text = info.describe;
            m_Extrainfo.text = info.extraInfo;

            if (_videoInfo != null && !string.IsNullOrEmpty(_videoInfo.thumbnail))
            {
                StartCoroutine(LoadThumnail(NRealTool.GetNrealResPath() + "/" + _videoInfo.thumbnail));
            }
        }

        private void Awake()
        {
            m_VideoName.gameObject.SetActive(false);
            m_Descripe.gameObject.SetActive(false);
            m_Extrainfo.gameObject.SetActive(false);
            m_PlayBtn.GetComponent<Image>().enabled = false;
            hoverRegion = m_Thumnail.gameObject.GetComponent<ARInteractiveItem>();
        }

        private void OnEnable()
        {
            m_PlayBtn.onEnter += OnItemEnter;
            m_PlayBtn.onExit += OnItemExit;
            m_PlayBtn.onClick += OnItemClick;

            hoverRegion.OnHover += OnHoverRegionEnter;
            hoverRegion.OnOut += OnHoverRegionExit;
        }

        private void OnHoverRegionEnter()
        {
            m_VideoName.gameObject.SetActive(true);
            m_Descripe.gameObject.SetActive(true);
            m_Extrainfo.gameObject.SetActive(true);
            m_PlayBtn.GetComponent<Image>().enabled = true;
            Invoke("SwitchCursorState", 0.05f);
        }

        private void SwitchCursorState()
        {
            //if (SceneSwitchManager.Instance != null && SceneSwitchManager.Instance.CursorObj != null)
            //{
            //    SceneSwitchManager.Instance.CursorObj.transform.Find("CursorOnHolograms").gameObject.SetActive(false);
            //    SceneSwitchManager.Instance.CursorObj.transform.Find("CursorOffHolograms").gameObject.SetActive(true);
            //}
        }

        private void OnHoverRegionExit()
        {
            //if (ARControllerManager.Instance.GetCurrentInteractible() != m_PlayBtn.GetComponent<ARInteractiveItem>())
            //{
            //    m_VideoName.gameObject.SetActive(false);
            //    m_Descripe.gameObject.SetActive(false);
            //    m_Extrainfo.gameObject.SetActive(false);
            //    m_PlayBtn.GetComponent<Image>().enabled = false;
            //}
            //else if (SceneSwitchManager.Instance != null && SceneSwitchManager.Instance.CursorObj != null)
            //{
            //    SceneSwitchManager.Instance.CursorObj.transform.Find("CursorOnHolograms").gameObject.SetActive(true);
            //    SceneSwitchManager.Instance.CursorObj.transform.Find("CursorOffHolograms").gameObject.SetActive(false);
            //}
        }

        private void OnDisable()
        {
            m_PlayBtn.onEnter -= OnItemEnter;
            m_PlayBtn.onExit -= OnItemExit;
            m_PlayBtn.onClick -= OnItemClick;
        }

        private void OnItemClick(ButtonBase item)
        {
            m_VideoName.gameObject.SetActive(false);
            m_Descripe.gameObject.SetActive(false);
            m_Extrainfo.gameObject.SetActive(false);
            m_PlayBtn.GetComponent<Image>().enabled = false;

            if (itemClick != null)
            {
                itemClick(_videoInfo);
            }
        }

        private void OnItemExit(ButtonBase item)
        {
            //if (ARControllerManager.Instance.GetCurrentInteractible() != hoverRegion)
            //{
            //    m_VideoName.gameObject.SetActive(false);
            //    m_Descripe.gameObject.SetActive(false);
            //    m_Extrainfo.gameObject.SetActive(false);
            //    m_PlayBtn.GetComponent<Image>().enabled = false;
            //}
            m_PlayBtn.transform.DOScale(1.12f, 0.6f);
            m_PlayBtn.transform.DOLocalMoveZ(-2, 0.6f);
        }

        private void OnItemEnter(ButtonBase item)
        {
            m_VideoName.gameObject.SetActive(true);
            m_Descripe.gameObject.SetActive(true);
            m_Extrainfo.gameObject.SetActive(true);
            m_PlayBtn.GetComponent<Image>().enabled = true;
            m_PlayBtn.transform.DOScale(1.15f, 0.6f);
            m_PlayBtn.transform.DOLocalMoveZ(-4, 0.6f);
        }

        private IEnumerator LoadThumnail(string path)
        {
            WWW www = new WWW(path);
            yield return www;
            if (www.isDone && string.IsNullOrEmpty(www.error))
            {
                m_Thumnail.texture = www.texture;
                www.Dispose();
                www = null;
            }
            else
            {
                Debug.LogError(www.error);
            }
        }
    }
}