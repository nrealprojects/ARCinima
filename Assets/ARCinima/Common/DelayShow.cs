using UnityEngine;

public class DelayShow : MonoBehaviour
{
    public GameObject androidObj;
    public GameObject macObj;
    public GameObject normalObj;
    public float delay = 36f;

    // Use this for initialization
    void Awake()
    {
        if (androidObj != null)
        {
            androidObj.SetActive(false);
        }
        if (macObj != null)
        {
            macObj.SetActive(false);
        }
        if (normalObj != null)
        {
            normalObj.SetActive(false);
        }
        Invoke("Show", delay);
    }

    private void Show()
    {
        if (normalObj != null)
        {
            normalObj.SetActive(true);
        }
#if UNTIY_ANDROID
         if (androidObj !=null)
        {
            androidObj.SetActive(true);
        }
#else
        if (macObj != null)
        {
            macObj.SetActive(true);
        }
#endif
    }

    private void OnDestroy()
    {
        CancelInvoke();
    }
}