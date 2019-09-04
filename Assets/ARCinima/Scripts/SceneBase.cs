using UnityEngine;

public class SceneBase : MonoBehaviour
{
    public string Key;
    private Vector3 m_OriginPos;

    void Awake()
    {
        m_OriginPos = transform.position;
        Debug.Log(m_OriginPos);
    }

    public virtual void Show()
    {
        transform.position = m_OriginPos;
    }

    public virtual void Disappear()
    {
        transform.position = Vector3.one * 10000;
    }
}
