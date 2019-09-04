using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using NREAL.AR;

public class CursorHover : Singleton<CursorHover>
{
    [SerializeField] Image m_HoveringSprite;

    public void Fill()
    {
        if (m_HoveringSprite != null && Constant.buttonAutoClick)
        {
            m_HoveringSprite.gameObject.SetActive(true);
            m_HoveringSprite.fillAmount = 0;
            m_HoveringSprite.DOFillAmount(1.0f, Constant.limitTime).SetEase(Ease.Linear);
        }
    }

    public void StopFill()
    {
        if (m_HoveringSprite != null && Constant.buttonAutoClick)
        {
            m_HoveringSprite.gameObject.SetActive(true);
            m_HoveringSprite.fillAmount = 0;
            m_HoveringSprite.DOKill();
        }
    }
}
