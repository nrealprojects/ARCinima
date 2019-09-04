using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using NREAL.AR;

public class ScreenAnimation : MonoBehaviour
{
    public GameObject frame1;
    public GameObject frame2;
    public GameObject frame3;
    private MeshRenderer[] screens;

    private void Start()
    {
        screens = gameObject.GetComponentsInChildren<MeshRenderer>();
        //MediaPlayerCtrl mediaplayer = GameObject.FindObjectOfType<MediaPlayerCtrl>();
        //if (mediaplayer != null)
        //{
        //    for (int i = 0; i < screens.Length; i++)
        //    {
        //        screens[i].material.mainTexture = mediaplayer.GetVideoTexture();
        //    }
        //}
        StartCoroutine(ScreenAnimator());
    }

    public IEnumerator ScreenAnimator()
    {
        frame1.SetActive(true);
        frame2.SetActive(false);
        frame3.SetActive(false);
        yield return new WaitForSeconds(0.6f);
        frame1.SetActive(false);
        frame2.SetActive(true);
        yield return new WaitForSeconds(0.6f);
        frame1.SetActive(false);
        frame2.SetActive(false);
        frame3.SetActive(true);
        yield return new WaitForSeconds(1f);
        float origin_width, origin_height;
        RectGrid grid = frame3.GetComponent<RectGrid>();
        origin_width = grid.Width;
        origin_height = grid.Height;
        float target_width = 1.2f * origin_width;
        float target_height = 1.2f * origin_height;
        float current_width = origin_width;
        float current_height = origin_height;
        float ratio = 0;
        float delta = 0.005f;
        while ((Mathf.Abs(current_width - target_width) > float.Epsilon))
        {
            current_width = Mathf.Lerp(origin_width, target_width, ratio);
            current_height = Mathf.Lerp(origin_height, target_height, ratio);
            ratio += delta;
            grid.Width = current_width;
            grid.Height = current_height;
            grid.Rank();
            yield return new WaitForEndOfFrame();
        }
        yield return new WaitForSeconds(1f);
        ratio = 0;
        delta = 0.01f;
        while ((Mathf.Abs(current_width - origin_width) > float.Epsilon))
        {
            current_width = Mathf.Lerp(target_width, origin_width, ratio);
            current_height = Mathf.Lerp(target_height, origin_height, ratio);
            ratio += delta;
            grid.Width = current_width;
            grid.Height = current_height;
            grid.Rank();
            yield return new WaitForEndOfFrame();
        }
    }

    //private IEnumerator ScreenAnimator()
    //{
    //    yield return new WaitForFixedUpdate();
    //    RandomHelper helper = new RandomHelper();
    //    helper.GetRandomArray<Transform>(Screens);
    //    for (int i = 0; i < Screens.Length; i++)
    //    {
    //        Screens[i].transform.DOScale(origin, 0.6f);
    //        yield return new WaitForSeconds(0.02f);
    //    }
    //}
}
