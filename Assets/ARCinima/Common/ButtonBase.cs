using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using DG.Tweening;
using UnityEngine.EventSystems;

namespace NREAL.AR
{
    public class ButtonBase : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
    {
        public delegate void ButtonBaseCallBack(ButtonBase button);
        public ButtonBaseCallBack onEnter;
        public ButtonBaseCallBack onExit;
        public ButtonBaseCallBack onClick;
        public Image m_Image;
        public Image m_ImageActive;
        public Color m_NormalColor;
        public Color m_HoverColor;
        public float hoverScaleFactor = 1f;
        public float hoverScaleTime = 0.4f;

        private Image currentImage;
        private Coroutine autoClick = null;
        private Vector3 m_OriginScale;

        private void Awake()
        {
            currentImage = m_Image;
            m_OriginScale = transform.localScale;
        }

        public void SwitchImage(bool active)
        {
            if (active)
            {
                currentImage = m_ImageActive;
                currentImage.enabled = true;
                m_Image.enabled = false;
            }
            else
            {
                currentImage = m_Image;
                currentImage.enabled = true;
                m_ImageActive.enabled = false;
            }
        }

        private void OnFocusEnter()
        {
            if (onEnter != null)
            {
                onEnter(this);
            }

            if (currentImage != null)
                currentImage.color = m_HoverColor;
            if (hoverScaleFactor != 1f)
                transform.DOScale(m_OriginScale * hoverScaleFactor, hoverScaleTime);
        }

        private void OnFocusExit()
        {
            if (onExit != null)
            {
                onExit(this);
            }

            if (autoClick != null)
            {
                StopCoroutine(autoClick);
                autoClick = null;
            }
            //CursorHover.Instance.StopFill();

            if (currentImage != null)
                currentImage.color = m_NormalColor;
            if (hoverScaleFactor != 1f)
                transform.DOScale(m_OriginScale, hoverScaleTime);
        }

        private void OnClick()
        {
            if (onClick != null)
            {
                onClick(this);
            }
        }

        private IEnumerator AutoClick()
        {
            CursorHover.Instance.Fill();
            yield return new WaitForSeconds(Constant.limitTime);
            if (onClick != null)
            {
                onClick(this);
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            OnClick();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            OnFocusEnter();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            OnFocusExit();
        }
    }
}