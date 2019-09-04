using UnityEngine;
using DG.Tweening;

public class FlareEffect : MonoBehaviour
{
    [SerializeField]
    LensFlare flare;
    [SerializeField]
    Color color;
    [SerializeField]
    float birghtBegin;
    [SerializeField]
    float birghtEnd;
    [SerializeField]
    float duration;

    void Start()
    {
        flare.brightness = birghtBegin;
        flare.color = color;
        DOTween.To(() => flare.brightness, x => flare.brightness = x, birghtEnd, duration);
    }
}