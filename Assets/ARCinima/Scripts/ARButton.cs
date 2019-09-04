using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class ARButton : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public string Key;
    public Action<string> OnClick;
    public Action<string> OnEnter;
    public Action<string> OnExit;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (OnClick != null)
        {
            OnClick(Key);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (OnEnter != null)
        {
            OnEnter(Key);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (OnExit != null)
        {
            OnExit(Key);
        }
    }
}
