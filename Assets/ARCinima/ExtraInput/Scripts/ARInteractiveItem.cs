using System;
using UnityEngine;
using UnityEngine.EventSystems;
using NRKernal;

public class ARInteractiveItem : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    public static bool forceDeselectOnce = false;

    public event Action OnHover;
    public event Action OnOut;
    public event Action OnClick;
    public event Action OnUp;
    public event Action OnDown;
    public event Action OnLongPress;
    public event Action OnSelected;
    public event Action OnDeselected;

    public bool IsBeingSelected { get { return SelectingRaycaster != null; } }
    public bool IsMoveable { get { return m_MoveableItem != null; } }
    public bool IsHovering { get; private set; }
    public NRPointerRaycaster SelectingRaycaster { get; private set; }

    protected MoveablePhysicsObject m_MoveableItem;

    private void Awake()
    {
        m_MoveableItem = GetComponent<MoveablePhysicsObject>();
    }

    private void OnDisable()
    {
        Deselected();
    }

    public void Hover()
    {
        IsHovering = true;

        if (OnHover != null)
        {
            OnHover();
        }
    }

    public void Out()
    {
        IsHovering = false;

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

    public void Selected(NRPointerRaycaster raycaster)
    {
        SelectingRaycaster = raycaster;
        if (OnSelected != null)
            OnSelected();
        NRInputModule.Instance.enabled = false;
        NRInput.ReticleVisualActive = false;
        forceDeselectOnce = false;
    }

    public void Deselected()
    {
        if (IsBeingSelected)
        {
            if (OnDeselected != null)
                OnDeselected();
            m_DeselectFrame = Time.frameCount;
            SelectingRaycaster = null;
            NRInputModule.Instance.enabled = true;
            NRInput.ReticleVisualActive = true;
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

    public void OnPointerDown(PointerEventData eventData)
    {
        Down();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        Up();
        JudgePickup(eventData);
    }

    private void Update()
    {
        JudgeDrop();
    }

    private float m_TriggerDownTime;
    private int m_DeselectFrame;
    private Vector2 m_TouchDownPos;
    private bool m_TouchMoved = false;
    private float m_ClickInterval;

    private const float JUDGE_TOUCH_MOVED_DISTANCE = 0.1f;

    private void JudgeDrop()
    {
        if (!IsMoveable)
            return;
        if (!IsBeingSelected)
            return;
        if (NRInput.GetButtonDown(ControllerButton.TRIGGER))
        {
            m_TriggerDownTime = Time.time;
            m_TouchDownPos = NRInput.GetTouch();
        }
        if (NRInput.GetButton(ControllerButton.TRIGGER) && !m_TouchMoved)
            m_TouchMoved = Vector2.Distance(NRInput.GetTouch(), m_TouchDownPos) > JUDGE_TOUCH_MOVED_DISTANCE;
        if (NRInput.GetButtonUp(ControllerButton.TRIGGER))
        {
            if ((Time.time - m_TriggerDownTime) < SelectingRaycaster.clickInterval)
                forceDeselectOnce = true;
            if (NRInput.GetControllerType() == ControllerType.CONTROLLER_TYPE_PHONE && m_TouchMoved)
                forceDeselectOnce = false;
            m_TouchMoved = false;
        }
        else if (NRInput.GetButtonUp(ControllerButton.HOME))
        {
            forceDeselectOnce = true;
        }
        if (forceDeselectOnce)
        {
            forceDeselectOnce = false;
            Deselected();
        }
    }

    private void JudgePickup(PointerEventData eventData)
    {
        if (!IsMoveable)
            return;
        if (IsBeingSelected)
            return;
        if (Time.frameCount == m_DeselectFrame)
            return;
        if (NRInput.RaycastMode == RaycastModeEnum.Gaze)
            return;
        if (eventData is NRPointerEventData)
            Selected((eventData as NRPointerEventData).raycaster);
    }
}