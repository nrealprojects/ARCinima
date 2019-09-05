using NRKernal;
using UnityEngine;

public class SceneBase : MonoBehaviour
{
    public string Key;
    private Vector3 m_OriginPos;
    public bool isActive = false;

    void Awake()
    {
        m_OriginPos = transform.position;
    }

    private void Update()
    {
        if (NRInput.GetButtonDown(ControllerButton.HOME) && !Key.Equals("cinima") && isActive)
        {
            SceneSwitchManager.Instance.GoToMainScene();
        }
    }

    public virtual void Show()
    {
        isActive = true;
        transform.position = m_OriginPos;
    }

    public virtual void Disappear()
    {
        isActive = false;
        transform.position = Vector3.one * 10000;
    }
}
