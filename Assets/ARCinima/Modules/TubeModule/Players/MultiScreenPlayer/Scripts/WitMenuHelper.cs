using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using NREAL.AR;

public class WitMenuHelper : MonoBehaviour {
    public Transform cameraRoot;
    public Transform menuTrans;
    public Transform helperIcon;
    public ARInteractiveItem interactiveItem;
    public GameObject hoverFX;

    private float distance;
    private bool isHovering;
    private bool isHiding;
    private float followRadius;
    private float followCheckTimer;

    const float Normal_Follow_Radius = 0.3f;  //正常的跟随范围
    const float Max_Follow_Radius = 0.7f;  //放大范围的跟随范围
    const float Follow_Check_Interval = 1.2f;  //检测跟随的时间间隔
    const float Follow_Move_Time = 1.5f;  //跟随目标时，到达目标的时间

	// Use this for initialization
	void Start () {
        menuTrans.gameObject.SetActive(false);
        distance = Vector3.Distance(transform.position, cameraRoot.position);
        followRadius = Normal_Follow_Radius;
        helperIcon.parent = null;
    }

    void OnEnable()
    {
        interactiveItem.OnClick += OnClick;
        interactiveItem.OnHover += OnHover;
        interactiveItem.OnOut += OnOut;
    }

    void OnDisable()
    {
        interactiveItem.OnClick -= OnClick;
        interactiveItem.OnHover -= OnHover;
        interactiveItem.OnOut -= OnOut;
    }

    void OnHover()
    {
        isHovering = true;
        if(hoverFX)
            hoverFX.SetActive(true);
    }

    void OnOut()
    {
        isHovering = false;
        if (hoverFX)
            hoverFX.SetActive(false);
    }
	
	void Update () {
        if (cameraRoot)
        {
            transform.position = cameraRoot.position + cameraRoot.forward * distance;
            transform.LookAt(cameraRoot, cameraRoot.up);
        }

        if (!isHiding && followCheckTimer < Follow_Check_Interval)
        {
            followCheckTimer += Time.deltaTime;
            if(followCheckTimer >= Follow_Check_Interval)
            {
                followCheckTimer = 0f;
                if(CheckNeedFollow())
                    RandomFollow();
            }
        }
	}

    private void ShowMenu()
    {
        menuTrans.gameObject.SetActive(true);
        followRadius = Max_Follow_Radius;
    }

    public void HideMenu()
    {
        menuTrans.gameObject.SetActive(false);
        followRadius = Normal_Follow_Radius;
    }

    private bool CheckNeedFollow()
    {
        if (isHovering)
            return false;
        //假设X，Y方向各偏差1，则在位移上偏差为2的开平方，约等于1.5
        return Vector3.Distance(transform.position, helperIcon.position) > followRadius * 1.5f;
    }

    private void RandomFollow()
    {
        helperIcon.DOKill();
        float randomX = Random.Range(followRadius / 2f, followRadius) * Mathf.Sign(Random.Range(-1f, 1f));
        float randomY = Random.Range(followRadius / 2f, followRadius) * Mathf.Sign(Random.Range(-1f, 1f));
        Vector3 targetPos = transform.position + transform.right * randomX + transform.up * randomY;
        helperIcon.DOMove(targetPos, Follow_Move_Time).SetEase(Ease.OutExpo);
        helperIcon.DORotate(transform.eulerAngles, Follow_Move_Time);
    }

    private void OnClick()
    {
        ShowMenu();
    }


    public void Show()
    {
        helperIcon.gameObject.SetActive(true);
        isHiding = false;
    }

    public void Hide()
    {
        HideMenu();
        helperIcon.gameObject.SetActive(false);
        isHiding = true;
    }
}
