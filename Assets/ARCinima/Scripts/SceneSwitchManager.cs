using NREAL.AR;
using NRKernal;
using UnityEngine;

public class SceneSwitchManager : Singleton<SceneSwitchManager>
{
    public GameObject IconRoot;
    [SerializeField] ARButton[] m_Buttons;
    [SerializeField] SceneBase[] m_Scenes;

    private bool isInSubTheme = true;

    void Start()
    {
        InitIcons();
        GoToMainScene();
    }

    #region button event
    void InitIcons()
    {
        // Initialize each icon and bind the events.
        for (int i = 0; i < m_Buttons.Length; i++)
        {
            m_Buttons[i].OnClick += OnIconClick;
        }
    }

    void OnIconClick(string key)
    {
        SwitchScene(key);
    }
    #endregion

    public void GoToMainScene()
    {
        if (!isInSubTheme)
        {
            return;
        }
        for (int i = 0; i < m_Scenes.Length; i++)
        {
            m_Scenes[i].Disappear();
        }

        ShowIcons(true);

        isInSubTheme = false;
    }

    private void SwitchScene(string key)
    {
        for (int i = 0; i < m_Scenes.Length; i++)
        {
            if (m_Scenes[i].Key.Equals(key))
            {
                m_Scenes[i].Show();
                ShowIcons(false);

                isInSubTheme = true;
            }
            else
            {
                m_Scenes[i].Disappear();
            }
        }
    }

    private void ShowIcons(bool isvisible)
    {
        IconRoot.SetActive(isvisible);
    }
}
