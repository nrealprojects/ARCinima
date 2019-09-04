using UnityEngine;
using DG.Tweening;

public class DelayFollow : MonoBehaviour
{
    private Transform cameraRoot;
    private Vector3 screenCenter = Vector3.zero;
    private float distance;
    private bool isMoving = false;
    private float distanceThreshold = 1f;              //距离阈值

    void Start()
    {
        cameraRoot = GameObject.Find("ARCameraManager").transform;
        distance = Vector3.Distance(transform.position, cameraRoot.position);
    }

    void Update()
    {
        screenCenter = new Vector3(Screen.width / 4.0f, Screen.height / 2.0f, distance);
        screenCenter = Camera.main.ScreenToWorldPoint(screenCenter);

        if (CheckNeedFollow() && !isMoving)
        {
            Invoke("MoveToTarget", 0.3f);
        }
    }

    private bool CheckNeedFollow()
    {
        return Vector3.Distance(screenCenter, transform.position) > distanceThreshold;
    }

    private void MoveToTarget()
    {
        isMoving = true;
     Tweener tween =  transform.DOMove(screenCenter, 0.4f).OnComplete(() =>
        {
            isMoving = false;
            transform.DORotate(cameraRoot.rotation.eulerAngles, 0.4f);
        });
        tween.SetEase(Ease.InQuart);
    }
}