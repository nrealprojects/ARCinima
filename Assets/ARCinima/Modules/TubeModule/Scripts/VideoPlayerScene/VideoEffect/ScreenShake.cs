using UnityEngine;
using DG.Tweening;

public class ScreenShake : MonoBehaviour
{
    public Vector3 m_ShakeRegion;
    public int m_Strength;
    public float m_ShakeDuration;
    private Vector3 originPos;
    private GameObject shakeTran = null;

    public void Awake()
    {
        shakeTran = GameObject.Find("MovieScreen");
        if (shakeTran != null)
        {
            originPos = shakeTran.transform.position;
            shakeTran.transform.DOShakePosition(m_ShakeDuration, m_ShakeRegion, m_Strength);
        }
        else
        {
            Debug.LogError("shakeTran == null");
        }
    }

    private void OnDestroy()
    {
        if (shakeTran != null)
        {
            shakeTran.transform.position = originPos;
        }
    }
}