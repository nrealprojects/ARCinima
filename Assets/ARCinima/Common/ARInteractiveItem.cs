using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace NREAL.AR
{
    // This class should be added to any gameobject in the scene
    // that should react to input based on the user's gaze.
    // It contains events that can be subscribed to by classes that
    // need to know about input specifics to this gameobject.
    public class ARInteractiveItem : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
    {
        public event Action OnHover;             // Called when the gaze moves over this object
        public event Action OnOut;              // Called when the gaze leaves this object
        public event Action OnClick;            // Called when click input is detected whilst the gaze is over this object.
        public event Action OnDoubleClick;      // Called when double click input is detected whilst the gaze is over this object.
        public event Action OnUp;               // Called when Fire1 is released whilst the gaze is over this object.
        public event Action OnDown;             // Called when Fire1 is pressed whilst the gaze is over this object.
        public event Action OnLongPress;
        public event Action OnSelected;
        public event Action OnDeselected;

        protected bool m_IsOver;                // Is the gaze currently over this object?
        public bool IsBeingSelected { get; private set; }

        public bool IsOver
        {
            get
            {
                return m_IsOver;
            }
        }

        // The below functions are called by the VREyeRaycaster when the appropriate input is detected.
        // They in turn call the appropriate events should they have subscribers.
        public void Hover()
        {
            m_IsOver = true;

            if (OnHover != null)
            {
                OnHover();
            }
        }

        public void Out()
        {
            m_IsOver = false;

            if (OnOut != null)
            {
                OnOut();
            }
        }

        public void Click()
        {
            if (OnClick != null)
            {
                OnClick();
            }
        }

        public void DoubleClick()
        {
            if (OnDoubleClick != null)
            {
                OnDoubleClick();
            }
        }

        public void Up()
        {
            if (OnUp != null)
            {
                OnUp();
            }
        }

        public void Down()
        {
            if (OnDown != null)
            {
                OnDown();
            }
        }

        public void LongPress()
        {
            OnLongPress?.Invoke();
        }

        public void Selected()
        {
            if (!IsBeingSelected)
            {
                IsBeingSelected = true;
                if (OnSelected != null)
                {
                    OnSelected();
                }
            }
        }

        public void Deselected()
        {
            if (IsBeingSelected)
            {
                IsBeingSelected = false;
                if (OnDeselected != null)
                {
                    OnDeselected();
                }
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            Click();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            Hover();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            Out();
        }
    }
}