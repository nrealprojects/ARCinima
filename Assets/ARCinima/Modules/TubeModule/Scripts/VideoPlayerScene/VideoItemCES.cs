using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using NREAL.AR;
using DG.Tweening;


namespace NREAL.AR.VideoPlayer
{
    public class VideoItemCES : MonoBehaviour
    {
        public delegate void VideoItemClick(VideoInfo video);
        public VideoItemClick itemClick = null;

        public SpriteRenderer m_Thumnail;
        public TextMesh m_VideoName;
        public TextMesh m_Descripe;

        public ButtonBaseCES m_PlayBtn;
        private ARInteractiveItem hoverRegion;

        private VideoInfo _videoInfo;

        public void SetData(VideoInfo info)
        {
            this._videoInfo = info;
            m_VideoName.text = info.videoName;
            m_Descripe.text = info.describe;

            if (_videoInfo != null && !string.IsNullOrEmpty(_videoInfo.thumbnail))
            {
                StartCoroutine(LoadThumnail(NRealTool.GetNrealResPath() + "/" + _videoInfo.thumbnail));
            }
        }

        private void Awake()
        {
            m_VideoName.gameObject.SetActive(false);
            m_Descripe.gameObject.SetActive(false);
            m_PlayBtn.GetComponent<SpriteRenderer>().enabled = false;
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
            m_PlayBtn.GetComponent<SpriteRenderer>().enabled = true;
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
            var raycaster = GameObject.FindObjectOfType<NRKernal.NRPointerRaycaster>();

            var result = raycaster.FirstRaycastResult();

            if (!result.isValid || result.gameObject != m_PlayBtn)
            {
                m_VideoName.gameObject.SetActive(false);
                m_Descripe.gameObject.SetActive(false);
                m_PlayBtn.GetComponent<SpriteRenderer>().enabled = false;
            }
        }

        private void OnDisable()
        {
            m_PlayBtn.onEnter -= OnItemEnter;
            m_PlayBtn.onExit -= OnItemExit;
            m_PlayBtn.onClick -= OnItemClick;
        }

        private void OnItemClick(ButtonBaseCES item)
        {
            m_VideoName.gameObject.SetActive(false);
            m_Descripe.gameObject.SetActive(false);
            m_PlayBtn.GetComponent<SpriteRenderer>().enabled = false;

            if (itemClick != null)
            {
                itemClick(_videoInfo);
            }
        }

        private void OnItemExit(ButtonBaseCES item)
        {
            var raycaster = GameObject.FindObjectOfType<NRKernal.NRPointerRaycaster>();

            var result = raycaster.FirstRaycastResult();

            if (!result.isValid || result.gameObject != hoverRegion)
            {
                m_VideoName.gameObject.SetActive(false);
                m_Descripe.gameObject.SetActive(false);
                m_PlayBtn.GetComponent<SpriteRenderer>().enabled = false;
            }
            //m_PlayBtn.transform.DOScale(1.12f, 0.6f);
            //m_PlayBtn.transform.DOLocalMoveZ(-2, 0.6f);
        }

        private void OnItemEnter(ButtonBaseCES item)
        {
            m_VideoName.gameObject.SetActive(true);
            m_Descripe.gameObject.SetActive(true);
            m_PlayBtn.GetComponent<SpriteRenderer>().enabled = true;
            //m_PlayBtn.transform.DOScale(1.15f, 0.6f);
            //m_PlayBtn.transform.DOLocalMoveZ(-4, 0.6f);
        }

        private IEnumerator LoadThumnail(string path)
        {
            WWW www = new WWW(path);
            yield return www;
            if (www.isDone && string.IsNullOrEmpty(www.error))
            {
                int width = www.texture.width;
                int height = www.texture.height;
                Sprite sprite = Sprite.Create(www.texture, new Rect(0f, 0f, width, height), new Vector2(0.5f, 0.5f), 100f);
                m_Thumnail.drawMode = SpriteDrawMode.Sliced;
                m_Thumnail.transform.localScale = Vector3.one;

                m_Thumnail.sprite = sprite;
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