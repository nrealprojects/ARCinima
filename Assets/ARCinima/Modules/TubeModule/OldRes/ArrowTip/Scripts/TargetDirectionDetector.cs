using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NREAL.AR;
using NRToolkit.Sharing.Tools;

public class TargetDirectionDetector : NREAL.AR.Singleton<TargetDirectionDetector>
{
    public Transform mTarget; //要用箭头追踪的物体
    public Transform mArrow; //指示箭头

    private Transform mARCam;  //主摄像机的Transform
    private const float SENSITIVITY_X = 0.8f; //横向敏感值，越小越容易出现箭头
    private const float SENSITIVITY_Y = 1.2f; //纵向敏感值，越小越容易出现箭头

    private float fov = 25f;
    private float aspecRatio = 3840f / 1080f;

    // Use this for initialization
    void Start()
    {
        Init();
    }

    // Update is called once per frame
    void Update()
    {
        if (NeedShowArrow()) ShowArrow();
        else HideArrow();
    }

    private void Init()
    {
        if (Camera.main)
            mARCam = Camera.main.transform;
        if (mARCam && mARCam.parent)
            mARCam = mARCam.parent;
        if (mARCam == null)
            Debug.LogError("Can not find main cam in Hierachy!!");
        mArrow.gameObject.SetActive(false);
    }

    //判定是否显示箭头
    private bool NeedShowArrow()
    {
        if (mARCam && mTarget && mTarget.gameObject.activeInHierarchy)
        {
            //获得从摄像机到target的向量
            Vector3 targetDirection = mTarget.position - mARCam.position;
            //判断朝向是否相背，如果相背则显示箭头
            if (Vector3.Angle(targetDirection, mARCam.forward) >= 90f) return true;
            //计算该向量在摄像机foward方向上的投影长度
            float fowardLength = Vector3.Project(targetDirection, mARCam.forward).magnitude;
            //计算在Y轴方向的最大范围
            float max_y = fowardLength * Mathf.Tan(Mathf.Deg2Rad * fov / 2f);
            //计算在x轴方向的最大范围
            float max_x = fowardLength * Mathf.Tan(Mathf.Deg2Rad * fov * aspecRatio / 2f);
            //计算实际在y轴方向的偏移距离
            float real_y = Vector3.Project(targetDirection, mARCam.up).magnitude;
            //计算实际在x轴方向的偏移距离
            float real_x = Vector3.Project(targetDirection, mARCam.right).magnitude;
            //两个方向有任一方向超出最大范围，则显示箭头
            if (real_x > max_x * SENSITIVITY_X || real_y > max_y * SENSITIVITY_Y) return true;
        }
        return false;
    }

    private void ShowArrow()
    {
        transform.position = mARCam.position;
        transform.rotation = mARCam.rotation;
        mArrow.gameObject.SetActive(true);
        mArrow.localPosition = Vector3.zero;
        mArrow.rotation = Quaternion.LookRotation(GetCurrentArrowLookDirection(), mArrow.up);
    }

    private void HideArrow()
    {
        mArrow.gameObject.SetActive(false);
    }

    private Vector3 GetCurrentArrowLookDirection()
    {
        Vector3 targetDirection = mTarget.position - mARCam.position;
        Vector3 arrowDirection = targetDirection - Vector3.Project(targetDirection, mARCam.forward);
        return arrowDirection;
    }

    //绑定新目标
    public void BindTarget(Transform newTarget)
    {
        if (newTarget) mTarget = newTarget;
        else Debug.LogError("Bind target failed, newTarget is null !!");
    }

    //解绑当前目标
    public void UnBindTarget()
    {
        mTarget = null;
    }
}
