using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace NRKernal.NRExamples
{
    public class NRButton : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
    {
        public Action OnClick;
        public Action OnEnter;
        public Action OnExit;

        public void OnPointerClick(PointerEventData eventData)
        {
            if (OnClick != null)
            {
                OnClick();
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (OnEnter != null)
            {
                OnEnter();
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (OnExit != null)
            {
                OnExit();
            }
        }
    }
}
