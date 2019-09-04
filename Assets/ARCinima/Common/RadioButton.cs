using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace NREAL.AR
{
    public class RadioButton : MonoBehaviour
    {
        public delegate void RadioButtonClick(ButtonState state);
        public RadioButtonClick OnClick = null;
        [SerializeField]
        ARInteractiveItem m_ButtonDefault;
        [SerializeField]
        Image m_ImageDefault;
        [SerializeField]
        ARInteractiveItem m_ButtonOther;
        [SerializeField]
        Image m_ImageOther;
        [SerializeField]
        Color m_Normal;
        [SerializeField]
        Color m_Hover;
        [SerializeField]
        Color m_Selected;

        public enum ButtonState
        {
            Default,
            Other,
        }
        private ButtonState currentState = ButtonState.Default;

        private void OnEnable()
        {
            m_ButtonDefault.OnClick += OnDefaultBtnClick;
            m_ButtonDefault.OnHover += OnDefaultBtnHover;
            m_ButtonDefault.OnOut += OnDefaultBtnOut;

            m_ButtonOther.OnClick += OnOtherBtnClick;
            m_ButtonOther.OnHover += OnOtherBtnHover;
            m_ButtonOther.OnOut += OnOtherBtnOut;
        }

        private void OnDisable()
        {
            m_ButtonDefault.OnClick -= OnDefaultBtnClick;
            m_ButtonDefault.OnHover -= OnDefaultBtnHover;
            m_ButtonDefault.OnOut -= OnDefaultBtnOut;

            m_ButtonOther.OnClick -= OnOtherBtnClick;
            m_ButtonOther.OnHover -= OnOtherBtnHover;
            m_ButtonOther.OnOut -= OnOtherBtnOut;
        }

        void UpdateButtonState()
        {
            switch (currentState)
            {
                case ButtonState.Default:
                    m_ImageDefault.color = m_Selected;
                    m_ImageOther.color = m_Normal;
                    break;
                case ButtonState.Other:
                    m_ImageDefault.color = m_Normal;
                    m_ImageOther.color = m_Selected;
                    break;
                default:
                    break;
            }
        }

        private void OnDefaultBtnClick()
        {
            if (currentState == ButtonState.Other)
            {
                if (OnClick != null)
                {
                    OnClick(ButtonState.Default);
                }
                currentState = ButtonState.Default;
                this.UpdateButtonState();
            }
        }

        private void OnDefaultBtnHover()
        {
            if (currentState == ButtonState.Other)
            {
                m_ImageDefault.color = m_Hover;
            }
        }

        private void OnDefaultBtnOut()
        {
            if (currentState == ButtonState.Other)
            {
                m_ImageDefault.color = m_Normal;
            }
        }

        private void OnOtherBtnClick()
        {
            if (currentState == ButtonState.Default)
            {
                if (OnClick != null)
                {
                    OnClick(ButtonState.Other);
                }
                currentState = ButtonState.Other;
                this.UpdateButtonState();
            }
        }

        private void OnOtherBtnHover()
        {
            if (currentState == ButtonState.Default)
            {
                m_ImageOther.color = m_Hover;
            }
        }

        private void OnOtherBtnOut()
        {
            if (currentState == ButtonState.Default)
            {
                m_ImageOther.color = m_Normal;
            }
        }
    }
}