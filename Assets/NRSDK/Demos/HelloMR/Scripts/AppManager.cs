using UnityEngine;

namespace NRKernal.NRExamples
{
    [DisallowMultipleComponent]
    public class AppManager : MonoBehaviour
    {
        public float quitLongPressTime = 3f;
        private float quitTimer = 0f;
#if !UNITY_EDITOR
        private bool hasNRInput;
#endif

        private void Awake()
        {
#if !UNITY_EDITOR
            hasNRInput = FindObjectOfType<NRInput>() != null;
#endif
        }

        private void Update()
        {
            if (NRInput.GetButtonDown(ControllerButton.HOME) || Input.GetKeyDown(KeyCode.R))
            {
                NRSessionManager.Instance.Recenter();
            }

            CheckQuit();
        }

        private void CheckQuit()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                QuitApplication();
                return;
            }

#if !UNITY_EDITOR
            if (!hasNRInput)
            {
                if (Input.GetMouseButtonDown(1))
                    QuitApplication();
                return;
            }
#endif

            if (quitLongPressTime == 0f && NRInput.GetButtonDown(ControllerButton.HOME))
            {
                QuitApplication();
                return;
            }

            if (NRInput.GetButton(ControllerButton.HOME))
            {
                quitTimer += Time.deltaTime;
                if (quitTimer > quitLongPressTime)
                {
                    quitTimer = 0f;
                    QuitApplication();
                }
            }
            else
            {
                quitTimer = 0f;
            }
        }

        private void QuitApplication()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}
