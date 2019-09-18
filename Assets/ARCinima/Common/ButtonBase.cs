using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using DG.Tweening;

namespace NREAL.AR
{
    [RequireComponent(typeof(ARInteractiveItem))]
    public class ButtonBase : MonoBehaviour
    {
        public delegate void ButtonBaseCallBack(ButtonBase button);
        public ButtonBaseCallBack onEnter;
        public ButtonBaseCallBack onExit;
        public ButtonBaseCallBack onClick;
        public Image m_Image;
        public Image m_ImageActive;
        public Color m_NormalColor;
        public Color m_HoverColor;
        private ARInteractiveItem _interactiveItem;
        private Image currentImage;

        private Coroutine autoClick = null;

        private void Awake()
        {
            _interactiveItem = gameObject.GetComponent<ARInteractiveItem>();
            if (_interactiveItem == null)
            {
                _interactiveItem = gameObject.AddComponent<ARInteractiveItem>();
            }
            currentImage = m_Image;
        }

        private void OnEnable()
        {
            _interactiveItem.OnHover += OnFocusEnter;
            _interactiveItem.OnOut += OnFocusExit;
            _interactiveItem.OnClick += OnClick;
        }

        private void OnDisable()
        {
            _interactiveItem.OnHover -= OnFocusEnter;
            _interactiveItem.OnOut -= OnFocusExit;
            _interactiveItem.OnClick -= OnClick;
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
            //CursorHover.Instance.StopFill();
            /*
			if (Constant.buttonAutoClick)
            {
                if (autoClick != null)
                {
                    StopCoroutine(autoClick);
                    autoClick = null;
                }
                autoClick = StartCoroutine(AutoClick());
            }
            */

            if (currentImage != null)
                currentImage.color = m_HoverColor;
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
    }
}