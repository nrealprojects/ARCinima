using System.Collections;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Rendering;
using UnityEngine.UI;
using NRKernal;

namespace NREAL.AR.VideoPlayer
{
    public class VideoInfoPageCES : MonoBehaviour
    {
        public VideoItem.VideoItemClick itemClick = null;
        [SerializeField]
        VideoItemCES[] m_VideoItemList;
        [SerializeField]
        ButtonBase m_HomeBtn;
        private static VideoConfig videoConfig = null;

        void Start()
        {
            foreach (var item in m_VideoItemList)
            {
                item.gameObject.SetActive(false);
            }
            if (videoConfig == null)
            {
                StartCoroutine(LoadVideoList());
            }
            else
            {
                CreateVideoItems();
            }
        }

        #region item event
        private void OnHomeItemExit(ButtonBase button)
        {
            m_HomeBtn.transform.DOScale(0.15f, 0.6f);
            m_HomeBtn.transform.DOLocalMoveZ(-0.01f, 0.6f);
        }

        private void OnHomeItemClick(ButtonBase button)
        {
            if (SceneSwitchManager.Instance != null)
                SceneSwitchManager.Instance.GoToMainScene();
        }

        private void OnHomeItemEnter(ButtonBase button)
        {
            m_HomeBtn.transform.DOScale(0.17f, 0.6f);
            m_HomeBtn.transform.DOLocalMoveZ(-0.02f, 0.6f);
        }

        private void OnVideoItemClick(VideoInfo video)
        {
            if (itemClick != null)
            {
                itemClick(video);
            }
        }
        #endregion

        private IEnumerator LoadVideoList()
        {
            string path = NRealTool.GetNrealResPath() + "/MultiScreenVideoList/MultiScreenVideoConfig.xml";
            WWW www = new WWW(path);
            yield return www;
            if (www.isDone && string.IsNullOrEmpty(www.error))
            {
                videoConfig = XmlSerializeHelper.DeSerialize<VideoConfig>(www.text);
                CreateVideoItems();
                www.Dispose();
                www = null;
            }
            else
            {
                NRDebug.LogError(www.error);
            }
        }

        public void CreateVideoItems()
        {
            for (int i = 0; i < videoConfig.videos.Count && i < m_VideoItemList.Length; i++)
            {
                VideoItemCES item = m_VideoItemList[i];
                item.gameObject.SetActive(true);
                item.SetData(videoConfig.videos[i]);
                item.itemClick += OnVideoItemClick;
            }
            
        }

        public CompareFunction CompareOP = CompareFunction.Always;
        public int Stencil = 255;
        public StencilOp StencilOp = StencilOp.Keep;

        private void SetupStencil()
        {
            var m = m_VideoItemList[0].GetComponentInChildren<Graphic>().material;

            if ((int)m.GetFloat("_StencilComp") != (int)CompareOP )
            {
                m.SetFloat("_StencilComp", (float)CompareOP);
            }

            if ((int)m.GetFloat("_Stencil") != Stencil)
            {
                m.SetFloat("_Stencil", (float)Stencil);
            }

            if ((int)m.GetFloat("_StencilOp") != (int)StencilOp)
            {
                m.SetFloat("_StencilOp", (float)StencilOp);
            }

            
        }
    }
}