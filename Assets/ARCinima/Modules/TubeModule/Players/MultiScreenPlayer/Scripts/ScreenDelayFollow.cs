using UnityEngine;
using DG.Tweening;
using NRKernal;

public class ScreenDelayFollow : MonoBehaviour
{
    private Transform cameraRoot;
    private Vector3 screenCenter = Vector3.zero;
    private float distance;
    private bool isMoving = false;
    private float distanceThreshold = 1f;              //距离阈值
    private Tweener tw_1;
    private Tweener tw_2;

    //private Transform leftCam;
    //private Transform rightCam;

    public bool followCameraOnStart = false;
    public bool useReverseFoward = false;
    public bool trackRotation = true;

    public void Init(Transform target = null)
    {
        if(target == null)
        {
            NRSessionBehaviour arCam = FindObjectOfType<NRSessionBehaviour>();
            if (arCam)
                target = arCam.transform;
            else
            {
                Debug.LogError("No ARCameraManager !");
                return;
            }
        }
        cameraRoot = target;
        //leftCam = cameraRoot.Find("LeftCamera");
        //rightCam = cameraRoot.Find("RightCamera");
        distance = Vector3.Distance(transform.position, cameraRoot.position);
    }

    void Start()
    {
        if (followCameraOnStart)
            Init();
    }

    void Update()
    {
        if (!cameraRoot)
            return;
        //UpdateCameraCenterRotation();
        if (trackRotation)
        {
            if (useReverseFoward)
            {
                transform.LookAt(transform.position * 2f - cameraRoot.position, cameraRoot.up);
            }
            else
            {
                transform.LookAt(cameraRoot, cameraRoot.up);
            }
        }
        screenCenter = cameraRoot.position + cameraRoot.forward * distance;
        if (CheckNeedFollow() && !isMoving)
        {
            Invoke("MoveToTarget", 0.3f);
        }
    }

    //private void UpdateCameraCenterRotation()
    //{
    //    Vector3 farPos = (leftCam.forward + rightCam.forward) / 2f * 10000f;
    //    transform.LookAt(farPos);
    //}

    private bool CheckNeedFollow()
    {
        return Vector3.Distance(screenCenter, transform.position) > distanceThreshold;
    }

    private void MoveToTarget()
    {
        transform.DOKill();
        isMoving = true;
        tw_1 = transform.DOMove(screenCenter, 0.4f).OnComplete(() =>
        {
            isMoving = false;
        });
        //tw_2 = transform.DORotate(camCenter.rotation.eulerAngles, 0.4f);
    }
}