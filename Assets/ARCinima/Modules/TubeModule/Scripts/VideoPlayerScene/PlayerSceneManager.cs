using NRKernal;
using System.Collections;
using UnityEngine;

namespace NREAL.AR.VideoPlayer
{
    public class PlayerSceneManager : MonoBehaviour
    {
        [SerializeField]
        VideoInfoPageCES m_VideoInfoPage;
        [SerializeField]
        VideoScreenPage m_VideoScreenPage;

        private Vector3 m_originScreenPos;
        private Quaternion m_originScreenRot;
        private Vector3 m_originScreenScale;

        public enum PageType
        {
            videoList,
            videoScreen
        }
        public static PageType currentPage = PageType.videoList;

        public void Start()
        {
            m_originScreenPos = m_VideoScreenPage.transform.position;
            m_originScreenRot = m_VideoScreenPage.transform.rotation;
            m_originScreenScale = m_VideoScreenPage.transform.localScale;
        }

        public void OnEnable()
        {
            m_VideoInfoPage.gameObject.SetActive(true);
            m_VideoScreenPage.gameObject.SetActive(false);
            m_VideoInfoPage.itemClick += OnVideoItemClick;
            m_VideoScreenPage.onReturnList += OnReturnVideoList;
            //if (SceneSwitchManager.Instance != null)
            //    SceneSwitchManager.Instance.CursorObj.SetActive(true);
            //NRInput.Instance.SetLaserVisible(true);
        }

        public void OnDisable()
        {
            m_VideoInfoPage.itemClick -= OnVideoItemClick;
            m_VideoScreenPage.onReturnList -= OnReturnVideoList;
        }

        private void OnReturnVideoList()
        {
            Time.timeScale = 1f;
            MaskFader.Instance.FadeOut(() =>
            {
                Invoke("ReturnVideoList", 0.1f);
            });
        }

        private void OnVideoItemClick(VideoInfo video)
        {
            MaskFader.Instance.FadeOut(() =>
            {
                StartCoroutine(PlayVideo(video));
            });
        }

        private IEnumerator PlayVideo(VideoInfo video)
        {
            m_VideoInfoPage.gameObject.SetActive(false);
            m_VideoScreenPage.gameObject.SetActive(true);
            m_VideoScreenPage.PlayVideo(video);
            currentPage = PageType.videoScreen;
            yield return new WaitForSeconds(0.2f);
            MaskFader.Instance.FadeIn();
        }

        private void ResetScreenPageTransform()
        {
            m_VideoScreenPage.transform.position = m_originScreenPos;
            m_VideoScreenPage.transform.rotation = m_originScreenRot;
            m_VideoScreenPage.transform.localScale = m_originScreenScale;
        }

        private void ReturnVideoList()
        {
            //if (ARControllerManager.Instance.CurrentSelectedTransform != null)
            //    ARControllerManager.Instance.Controller.OnCancel();
            ResetScreenPageTransform();
            m_VideoInfoPage.gameObject.SetActive(true);
            m_VideoScreenPage.gameObject.SetActive(false);
            currentPage = PageType.videoList;
            MaskFader.Instance.FadeIn();
        }
    }
}