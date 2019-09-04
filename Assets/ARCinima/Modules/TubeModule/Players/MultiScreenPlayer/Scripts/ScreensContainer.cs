using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NREAL.AR;
using NRKernal;

public class ScreensContainer : MonoBehaviour
{
    /// <summary>
    /// Max Rotate Speed When Swipe
    /// </summary>
    private float maxVelocity = 25f;  //滑动获得的最大旋转速度

    /// <summary>
    /// The Touch Sensitivity
    /// </summary>
    private float rotateSensitivity = 15f;  //旋转系数，越大旋转越快

    private Transform arCam;
    private float xAxis = 0f;
    private float yAxis = 0f;
    private Vector2 velocity = Vector2.zero;
    private Vector2 currentSpeed = Vector2.zero;
    private float smoothingTime = 0.2f;
    private bool isLocked = false;
    private bool inited = false;

    public bool IsRotating { get; private set; }
    public float DistanceFromCamera { get; private set; }

    private NRPointerRaycaster m_NRPointerRaycaster = null;
    public NRPointerRaycaster Raycaster
    {
        get
        {
            if (m_NRPointerRaycaster == null)
            {
                m_NRPointerRaycaster = GameObject.FindObjectOfType<NRKernal.NRPointerRaycaster>();
            }

            return m_NRPointerRaycaster;
        }
    }

    public void Init(Transform arCam)
    {
        this.arCam = arCam;
        inited = true;
    }

    public void Unlock()
    {
        isLocked = false;
    }

    public void Lock()
    {
        isLocked = true;
    }

    private void Update()
    {
        if (inited)
            DistanceFromCamera = (arCam.position - transform.position).magnitude;
        if (inited && !isLocked && Raycaster)
        {
            if (!Raycaster.FirstRaycastResult().isValid)
                return;
            UpdateTouchPad();
        }
    }

    private float rotateSpeed = 100f;

    private float accelerValue = 6f;  //减速时的阻尼加速度
    private Vector2 lastTouchPos;
    private Vector2 deltaTouch;
    private float acceler_x;
    private float acceler_y;
    private float velocity_x;
    private float velocity_y;
    private Vector2 touchDownPos;
    private float touchDownTime;
    private Vector2 touchDeltaMove;
    private float touchDeltaTime;

    //使用touchpad输入
    private void UpdateTouchPad()
    {
        var touchpos = NRInput.GetTouch(ControllerHandEnum.Right);
        xAxis = touchpos.x;
        yAxis = touchpos.y;
        if (xAxis != 0f || yAxis != 0f)
        {
            if (lastTouchPos == Vector2.zero)
                OnTouchDown(xAxis, yAxis);
            deltaTouch = (lastTouchPos == Vector2.zero) ? Vector2.zero : new Vector2(xAxis - lastTouchPos.x, yAxis - lastTouchPos.y);
            lastTouchPos.x = xAxis;
            lastTouchPos.y = yAxis;
            if (IsRotating)
            {
                transform.Rotate(-arCam.up, deltaTouch.x * rotateSensitivity, Space.World);
                transform.Rotate(arCam.right, deltaTouch.y * rotateSensitivity, Space.World);
            }
        }
        else
        {
            if (lastTouchPos != Vector2.zero)
                OnTouchUp();
            velocity_x += acceler_x * Time.deltaTime;
            velocity_y += acceler_y * Time.deltaTime;
            if (Mathf.Sign(velocity_x) == Mathf.Sign(acceler_x))
                velocity_x = 0f;
            if (Mathf.Sign(velocity_y) == Mathf.Sign(acceler_y))
                velocity_y = 0f;
            IsRotating = (velocity_x != 0f || velocity_y != 0f);
            if (IsRotating)
            {
                transform.Rotate(-arCam.up, velocity_x * rotateSensitivity * Time.deltaTime, Space.World);
                transform.Rotate(arCam.right, velocity_y * rotateSensitivity * Time.deltaTime, Space.World);
            }
        }
    }

    private void OnTouchDown(float x, float y)
    {
        IsRotating = true;
        touchDownPos = new Vector2(x, y);
        touchDownTime = Time.time;
        acceler_x = 0f;
        acceler_y = 0f;
        velocity_x = 0f;
        velocity_y = 0f;
    }

    private void OnTouchUp()
    {
        touchDeltaMove = lastTouchPos - touchDownPos;
        touchDeltaTime = Time.time - touchDownTime;
        velocity_x = touchDeltaMove.x / touchDeltaTime;
        velocity_y = touchDeltaMove.y / touchDeltaTime;
        velocity_x *= Mathf.Abs(velocity_x);
        velocity_y *= Mathf.Abs(velocity_y);
        velocity_x = Mathf.Clamp(velocity_x, -maxVelocity, maxVelocity);
        acceler_x = -Mathf.Sign(velocity_x) * accelerValue;
        velocity_y = Mathf.Clamp(velocity_y, -maxVelocity, maxVelocity);
        acceler_y = -Mathf.Sign(velocity_y) * accelerValue;
        touchDownPos = Vector2.zero;
        lastTouchPos = Vector2.zero;
    }
}
