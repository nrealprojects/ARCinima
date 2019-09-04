using UnityEngine;
using DG.Tweening;

public class MultiScreenMenu : MonoBehaviour
{
    public ARButton[] listBtnArr;
    private Vector3[] originScaleArr;
    private float scaleTime = 0.25f;

    void Awake()
    {
        originScaleArr = new Vector3[listBtnArr.Length];
        for (int i = 0; i < listBtnArr.Length; i++)
        {
            int k = i;
            originScaleArr[k] = listBtnArr[k].transform.localScale;
            listBtnArr[i].OnClick += (string key) =>
            {
                ShowList(k);
            };
        }
    }

    void OnEnable()
    {
        for (int i = 0; i < listBtnArr.Length; i++)
        {
            listBtnArr[i].transform.localScale = Vector3.zero;
            listBtnArr[i].transform.DOKill();
            listBtnArr[i].transform.DOScale(originScaleArr[i], scaleTime).SetEase(Ease.OutBack);
        }
    }

    private void ShowList(int index)
    {
        MultiScreenPlayerManager.Instance.ShowScreenList(index);
        gameObject.SetActive(false);
    }
}
