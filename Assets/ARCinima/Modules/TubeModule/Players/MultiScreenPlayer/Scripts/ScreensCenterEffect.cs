using Game;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreensCenterEffect : MonoBehaviour {
    public PlexusEffect extraEffect;

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        SetExtraEffectEnabled(true);
    }

    private void OnDisable()
    {
        SetExtraEffectEnabled(false);
    }

    private void SetExtraEffectEnabled(bool isEnabled)
    {
        if (extraEffect)
            extraEffect.gameObject.SetActive(isEnabled);
    }
}
